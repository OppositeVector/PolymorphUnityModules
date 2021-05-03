using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Polymorph.DataTools.Exceptions;
using Polymorph.Serialization;

namespace Polymorph.DataTools.Dynamics {

    /// <summary>
    /// An interface for objects that will receive their path when a PathFollower follows a path that traverses them
    /// </summary>
    public interface IPathedNode {
        /// <summary>
        /// Called by PathFollower when the object is reached using some path
        /// </summary>
        /// <param name="path">The path to this object from the last root traversed</param>
        void SetPath(string path);
    }

    enum ObjectType { InDictionary, NotInDictionary, NotInObject, Field, Property, PropertyNotSettable, ImplementationError }

    /// <summary>
    /// Specialises the generic PathFollower to use a Json dictionary (string, object) as the base object to follow
    /// </summary>
    public class PathFollower : PathFollower<Dictionary<string, object>> {
        /// <summary>
        /// Creates a PathFollower ready for following a path
        /// </summary>
        public PathFollower() { }
        /// <summary>
        /// Creates a PathFollower and follows a path on an object, equivilant to creating a PathFollower() and calling Follow(string, object, bool)
        /// </summary>
        /// <param name="path">The path to follow, Json type path i.e. house.kitchen.fridge.id</param>
        /// <param name="obj">The root object on which the path is to be followed</param>
        /// <param name="create">Should the path be created if it does not exist</param>
        public PathFollower(string path, object obj) : base(path, obj) { }
    }

    /// <summary>
    /// Follows a path given to it on an object that derives from IDictionary and spits out the object at the end of the path
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PathFollower<T> where T : IDictionary, new() {

        public static bool debug = false;

        class Node {

            static Type pathedNodeType;
            static Node() {
                pathedNodeType = typeof(IPathedNode);
            }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

            PathFollower<T> parent;
            public Token<Tokens> token;
            public string name;
            public object container;
            public ObjectType type;
            public Tokens rule;
            [NonSerialized]
            public MemberInfo memberInfo;
            public object value;
            public Node next;
            public bool creatable = false;
            int pathLength;

            public Node(PathFollower<T> parent, Token<Tokens> token, object container) {
                this.parent = parent;
                this.token = token;
                this.container = container;
                DoStep();
            }

            public void DoStep() {
                name = token.value;
                rule = token.rule;
                pathLength = parent.builder.Length;
                if(container == null) {
                    type = ObjectType.NotInDictionary;
                } else {
                    switch(token.rule) {
                        case Tokens.WORD:
                            WordStep();
                            break;
                        default:
                            type = ObjectType.ImplementationError;
                            break;
                    }
                }
            }

            void WordStep() {

                value = null;
                if(container is T) {

                    var dict = (T) container;
                    if(dict.Contains(name)) {
                        type = ObjectType.InDictionary;
                        value = dict[name];
                    } else {
                        type = ObjectType.NotInDictionary;
                        creatable = true;
                    }
                } else {

                    var objType = container.GetType();
                    var prop = objType.GetProperty(name, flags);
                    if(prop != null) {
                        memberInfo = prop;
                        value = prop.GetValue(container, null);
                        if(prop.GetSetMethod() == null) {
                            type = ObjectType.PropertyNotSettable;
                        } else {
                            type = ObjectType.Property;
                        }
                        if((value == null) && typeof(T).IsAssignableFrom(prop.PropertyType)) {
                            creatable = true;
                        }
                    } else {
                        var field = objType.GetField(name, flags);
                        if(field != null) {
                            type = ObjectType.Field;
                            memberInfo = field;
                            value = field.GetValue(container);
                            if((value == null) && typeof(T).IsAssignableFrom(field.FieldType)) {
                                creatable = true;
                            }
                        } else {
                            type = ObjectType.NotInObject;
                        }
                    }
                }
            } 

            public void SetValue(object newValue) {
                switch(type) {
                    case ObjectType.InDictionary:
                        ((T)container)[name] = newValue;
                        break;
                    case ObjectType.NotInDictionary:
                        ((T)container).Add(name, newValue);
                        break;
                    case ObjectType.Field:
                        var field = memberInfo as FieldInfo;
                        field.SetValue(container, newValue);
                        break;
                    case ObjectType.Property:
                        var prop = memberInfo as PropertyInfo;
                        prop.SetValue(container, newValue, null);
                        break;
                    default:
                        var type = newValue != null ? newValue.GetType().Name : "null";
                        throw new InvalidOperation("Set", parent.builder.ToString(0, pathLength), type);
                }
                value = newValue;
                if(pathedNodeType.IsAssignableFrom(newValue.GetType())) {
                    var pathedNode = (IPathedNode) newValue;
                    pathedNode.SetPath(parent.builder.ToString(0, pathLength));
                }
            }

            public void Create() {
                var path = parent.builder.ToString(0, pathLength);
#if DEBUG
                Console.WriteLine("Creating " + path);
#endif
                var newValue = new T();
                if(pathedNodeType.IsAssignableFrom(typeof(T))) {
                    var pathedNode = (IPathedNode) newValue;
                    pathedNode.SetPath(parent.builder.ToString(0, pathLength));
                }
                SetValue(newValue);
                if(next != null) {
                    next.container = newValue;
                    next.creatable = true;
                }
            }
        }

        /// <summary>
        /// The current path that was followed
        /// </summary>
        public string currentPath;
        /// <summary>
        /// The root object on which the current path was followed
        /// </summary>
        public object rootObject;
        /// <summary>
        /// Denotes if this path can be assigned to
        /// </summary>
        public bool isAssignable = false;
        /// <summary>
        /// Denotes if the path to the object exists
        /// </summary>
        public bool pathExists { get; private set; }
        /// <summary>
        /// Denotes if an object at the end of the path exists
        /// </summary>
        public bool exists { get; private set; }
        /// <summary>
        /// Node count in this path
        /// </summary>
        public int count { get; private set; }

        Node lastMember;
        Node firstMemeber;

        [NonSerialized]
        PathTokenizer tokenizer;
        [NonSerialized]
        StringBuilder builder;
        [NonSerialized]
        bool first;

        /// <summary>
        /// Creates a PathFollower, call Follow(string, object) to follow a path
        /// </summary>
        public PathFollower() {
            Init();
            exists = false;
        }

        /// <summary>
        /// Creates a PathFollower and follows a path on an object, equivilant to creating a PathFollower() and calling Follow(string, object, bool)
        /// </summary>
        /// <param name="path">The path to follow, Json type path i.e. house.kitchen.fridge.id</param>
        /// <param name="obj">The root object on which the path is to be followed</param>
        public PathFollower(string path, object obj) {
            Init();
            Follow(path, obj);
        }

        void Init() {
            builder = new StringBuilder();
            tokenizer = new PathTokenizer();
        }

        /// <summary>
        /// Follows a path on an object
        /// </summary>
        /// <param name="path">The path to follow, Json type path i.e. house.kitchen.fridge.id</param>
        /// <param name="obj">The root object on which the path is to be followed</param>
        public void Follow(string path, object obj) {

            currentPath = path;
            rootObject = obj;
            builder.Remove(0, builder.Length);

            first = true;

            tokenizer.stream = path;
            tokenizer.currentPosition = 0;
            Token<Tokens> current;
            Node currentMemeber;
            pathExists = true;
            isAssignable = false;
            exists = false;
            firstMemeber = null;
            lastMember = null;
            count = 0;
            if(obj == null) {
                pathExists = false;
                return;
            }
            while(tokenizer.NextToken()) {

                ++count;
                current = tokenizer.currentToken;
                if(current.rule == Tokens.UNACCEPTABLE) {
                    pathExists = false;
                    isAssignable = false;
                    throw new InvalidPathException("Could not parse path", current.pos + (first ? 0 : 1), current.value.Length, path);
                }
                AddToBuilder(current);
                currentMemeber = new Node(this, current, obj);
                if(lastMember == null) {
                    firstMemeber = currentMemeber;
                } else {
                    lastMember.next = currentMemeber;
                }

                switch(currentMemeber.type) {
                    case ObjectType.ImplementationError:
                        pathExists = false;
                        isAssignable = false;
                        throw new NotImplementedException("Following through arrays and indexers not implemented yet, path: " + tokenizer.stream);
                    case ObjectType.NotInObject:
                        pathExists = false;
                        isAssignable = false;
                        throw new InvalidPathException("Could not traverse path, token does not exist", current.pos + (first ? 0 : 1), current.value.Length, path);
                    case ObjectType.NotInDictionary:
                        pathExists = false;
                        break;
                    case ObjectType.PropertyNotSettable:
                        isAssignable = false;
                        break;
                    default:
                        isAssignable = true;
                        break;
                }
                obj = currentMemeber.value;
                lastMember = currentMemeber;
            }
            if(lastMember != null) {
                if(lastMember.value != null) {
                    exists = true;
                }
            }
        }

        /// <summary>
        /// Get the value at the end of the current path
        /// </summary>
        /// <returns>Object at the end of the current path</returns>
        public object GetValue() {
            return lastMember.value;
        }

        /// <summary>
        /// Get a reference at the end of the current path
        /// </summary>
        /// <typeparam name="RetT">The type of the reference</typeparam>
        /// <returns>Refernce of type RetT to an object at the end of the current path</returns>
        public RetT GetValue<RetT>() where RetT : class {
            return lastMember.value as RetT;
        }

        /// <summary>
        /// Set the value at the end of the current path
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(object value) {
            if(!pathExists) {
                Create(lastMember);
            }
            lastMember.SetValue(value);
            exists = true;
        }

        /// <summary>
        /// Returns the name of a node at a specific index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetName(int index) {
            var current = firstMemeber;
            while(index > 0) {
                current = current.next;
                --index;
            }
            return current.name;
        }

        void Create(Node endNode) {
            if(firstMemeber == null) { return; }
            var current = firstMemeber;
            // Find first creatable
            while((current != endNode) && (!current.creatable)) {
                current = current.next;
            }
            // Create path
            while((current != null) && (current != endNode)) {
                if(!current.creatable) {
                    throw new InvalidPathException("Could not create path", current.token.pos, current.token.value.Length, currentPath);
                }
                current.Create();
                current = current.next;
            }
            pathExists = true;
            isAssignable = true;
        }

        /// <summary>
        /// Standard ToString, prints all the data in the follower 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append("PathFollower {");
            builder.Append("\n\tcurrentPath: ").Append(currentPath);
            builder.Append(",\n\trootObject: ").Append(rootObject);
            builder.Append(",\n\tisAssignable: ").Append(isAssignable);
            builder.Append(",\n\tpathExist: ").Append(pathExists);
            builder.Append(",\n\texists: ").Append(exists);
            builder.Append(",\n}");
            return builder.ToString();
            // return JsonSerializer.SerializeAndFormat(this);
        }

        void AddToBuilder(Token<Tokens> token) {
            switch(token.rule) {
                case Tokens.WORD:
                    if(first) {
                        first = false;
                    } else {
                        builder.Append(".");
                    }
                    builder.Append(token.value);
                    break;
                case Tokens.INDEX:
                case Tokens.MULTIINDEX:
                case Tokens.STRINGINDEX:
                    builder.Append("[");
                    builder.Append(token.value);
                    builder.Append("]");
                    break;
                default:
                    if(first) {
                        first = false;
                    } else {
                        builder.Append(".");
                    }
                    builder.Append(Tokens.UNACCEPTABLE);
                    break;

            }
        }
    }
}
