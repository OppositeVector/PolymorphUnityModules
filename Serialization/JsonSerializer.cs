using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using MiniJSON;

namespace Polymorph.Serialization {

    public static class JsonSerializer {

        static Regex regex = new Regex("\"|\\\\");
        static string EvaluateMatch(Match m) {
            if(m.Value == "\"") {
                return "\\\"";
            } else {
                return "\\\\";
            }
        }

        static Dictionary<Type, Serializer> serializers;
        static Dictionary<Type, Deserializer> deserializers;

        static JsonSerializer() {

            // Find all serializers created outside known code
            serializers = new Dictionary<Type, Serializer>();
            var serializerType = typeof(Serializer);
            deserializers = new Dictionary<Type, Deserializer>();
            var deserializerType = typeof(Deserializer);
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(Type type in assembly.GetTypes()) {
                    if((type != deserializerType) && (deserializerType.IsAssignableFrom(type))) {
                        var deserializer = Activator.CreateInstance(type) as Deserializer;
                        if(deserializers.ContainsKey(deserializer.intendedType)) {
                            Console.WriteLine("Multiple Deserializers for " + deserializer.intendedType);
                            deserializers[deserializer.intendedType] = deserializer;
                        } else {
                            deserializers.Add(deserializer.intendedType, deserializer);
                        }
                    }
                    if((type != serializerType) && (serializerType.IsAssignableFrom(type))) {
                        var serializer = Activator.CreateInstance(type) as Serializer;
                        if(serializers.ContainsKey(serializer.intendedType)) {
                            Console.WriteLine("Multiple Serializers for " + serializer.intendedType);
                            serializers[serializer.intendedType] = serializer;
                        } else {
                            serializers.Add(serializer.intendedType, serializer);
                        }
                    }
                }
            }
        }

        public static object Deserialize(Type type, string json, object proto = null) {

            var jsonObj = MiniJSON.Json.Deserialize(json);
            return RecursiveDeserialize(type, jsonObj, proto);
        }

        public static object Deserialize(Type type, object jsonObj, object proto = null) {
            return RecursiveDeserialize(type, jsonObj, proto);
        }

        static object RecursiveDeserialize(Type type, object jsonObj, object proto = null) {

            if(jsonObj != null) {

                var nullable = Nullable.GetUnderlyingType(type);
                if(nullable != null)
                {
                    type = nullable;
                }

                if(deserializers.ContainsKey(type)) {
                    return deserializers[type].Deserialize(type, jsonObj, proto);
                }

                var jsonType = jsonObj.GetType();

                if(type.IsEnum) {
                    if(jsonObj is string) {
                        var vals = (int[]) Enum.GetValues(type);
                        int finalVal = 0;
                        var enumVals = (jsonObj as string).Split('|');
                        foreach(var enumVal in enumVals) {
                            for(int i = 0; i < vals.Length; ++i) {
                                if(enumVal == Enum.GetName(type, vals[i])) {
                                    finalVal += vals[i];
                                }
                            }
                        }
                        return Enum.ToObject(type, finalVal);
                    } else if(jsonType.IsPrimitive) {
                        return Enum.ToObject(type, jsonObj);
                    }
                } else if(type.IsPrimitive) {
                    if(jsonType.IsPrimitive) {
                        return Convert.ChangeType(jsonObj, type);
                    }
                } else if(type == typeof(string)) {
                    if(jsonType == typeof(string)) {
                        return jsonObj.ToString();
                    }
                } else if(type == typeof(Guid)) {
                    if(jsonType == typeof(string)) {
                        return new Guid(jsonObj.ToString());
                    }
                } else if(type.IsArray) {
                    if(jsonObj is IList) {
                        var jsonList = jsonObj as IList;
                        var rank = type.GetArrayRank();
                        var traverser = new ListToArray(jsonList, rank);
                        Array retVal;
                        retVal = Array.CreateInstance(type.GetElementType(), traverser.lengths);
                        foreach(var ele in traverser.elements) {
                            retVal.SetValue(RecursiveDeserialize(type.GetElementType(), ele.obj), ele.indicies);
                        }
                        return retVal;
                    }
                } else if(typeof(IList).IsAssignableFrom(type)) {
                    if(jsonObj is IList) {
                        if(type == typeof(IList)) {
                            return jsonObj;
                        } else {
                            var jsonList = jsonObj as IList;
                            IList retVal;
                            if(proto == null) {
                                retVal = Activator.CreateInstance(type) as IList;
                            } else {
                                retVal = proto as IList;
                                retVal.Clear();
                            }
                            for(int i = 0; i < jsonList.Count; ++i) {
                                retVal.Add(RecursiveDeserialize(type.GetGenericArguments()[0], jsonList[i]));
                            }
                            return retVal;
                        }
                    }
                } else if(typeof(IDictionary).IsAssignableFrom(type)) {
                    if(jsonObj is IDictionary) {
                        if(type == typeof(IDictionary)) {
                            return jsonObj;
                        } else {
                            var keyType = type.GetGenericArguments()[0];
                            if(keyType.IsEnum || keyType.IsPrimitive || (keyType == typeof(string))) {
                                var dict = jsonObj as IDictionary;
                                IDictionary retVal;
                                if(proto == null) {
                                    retVal = Activator.CreateInstance(type) as IDictionary;
                                } else {
                                    retVal = proto as IDictionary;
                                    retVal.Clear();
                                }
                                foreach(var key in dict.Keys) {
                                    var dKey = RecursiveDeserialize(keyType, key);
                                    var dValue = RecursiveDeserialize(type.GetGenericArguments()[1], dict[key]);
                                    if(retVal.Contains(dKey)) {
                                        retVal[dKey] = dValue;
                                    } else {
                                        retVal.Add(dKey, dValue);
                                    }
                                }
                                return retVal;
                            }
                        }
                    }
                } else {
                    if(type == typeof(object)) {
                        return jsonObj;
                    }
                    if(jsonObj is IDictionary) {
                        object retVal;
                        if(proto != null) {
                            retVal = proto;
                        } else {
                            retVal = Activator.CreateInstance(type);
                        }
                        var dict = jsonObj as IDictionary;
                        var fields = CollectFields(type);
                        object internalProto;
                        foreach(var field in fields) {
                            if(IsDelegate(field.FieldType)) { continue; }
                            if(dict.Contains(field.Name)) {
                                object val;
                                internalProto = field.GetValue(retVal);
                                val = RecursiveDeserialize(field.FieldType, dict[field.Name], internalProto);
                                field.SetValue(retVal, val);
                            }
                        }
                        var props = CollectProperties(type);
                        foreach(var prop in props) {
                            if(IsDelegate(prop.PropertyType)) { continue; }
                            if(dict.Contains(prop.Name)) {
                                internalProto = null;
                                if(prop.GetGetMethod() != null) {
                                    internalProto = prop.GetValue(retVal, null);
                                }
                                if(prop.GetSetMethod() != null) {
                                    object val;
                                    val = RecursiveDeserialize(prop.PropertyType, dict[prop.Name], internalProto);
                                    prop.SetValue(retVal, val, null);
                                } else if(internalProto != null) {
                                    RecursiveDeserialize(prop.PropertyType, dict[prop.Name], internalProto);
                                }
                            }
                        }
                        return retVal;
                    }
                }
            }
            if(proto != null) {
                return proto;
            } else {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }
        }

        public static T Deserialize<T>(string json, T proto = default(T)) {
            var jsonObj = MiniJSON.Json.Deserialize(json);
            if(typeof(T) == typeof(IDictionary))
                return (T) jsonObj;
            return (T) RecursiveDeserialize(typeof(T), jsonObj, proto);
        }

        public static T Deserialize<T>(object jsonObj, T proto = default(T)) {
            return (T) RecursiveDeserialize(typeof(T), jsonObj, proto);
        }

        public static string Serialize(object obj) {
            var origCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var retVal = RecursiveSerialize(obj, new List<object>());
            CultureInfo.CurrentCulture = origCulture;
            return retVal;
        }

        static string RecursiveSerialize(object obj, List<object> stack) {

            if(obj == null) {
                return "null";
            }

            var type = obj.GetType();

            if(stack.Contains(obj)) {
                // TODO: Show loop
                return "Object Loop to type " + type.Name;
            } else {
                stack.Add(obj);
            }
            
            string retVal;

            if(serializers.ContainsKey(type)) {
                retVal = serializers[type].Serialize(obj);
            } else {
                if(type.IsPrimitive) {
                    if(type == typeof(char)) {
                        retVal = obj.ToString();
                    } else {
                        retVal = obj.ToString().ToLower();
                    }
                } else if((obj is string)) {
                    var str = obj as string;
                    str = regex.Replace(str, EvaluateMatch);
                    retVal = "\"" + str + "\"";
                } else if(type.IsEnum) {
                    var flags = type.GetCustomAttributes(typeof(FlagsAttribute), false);
                    if(flags.Length == 0) {
                        retVal = "\"" + obj + "\"";
                    } else {
                        var enumList = GetEnumList(type, obj);
                        var builder = new StringBuilder();
                        builder.Append("\"");
                        var first = true;
                        for(int i = 0; i < enumList.Count; ++i) {
                            if(first) {
                                first = false;
                            } else {
                                builder.Append('|');
                            }
                            builder.Append(enumList[i]);
                        }
                        builder.Append("\"");
                        retVal = builder.ToString();
                    }
                } else if(type == typeof(Guid)) {
                    retVal = "\"" + obj.ToString() + "\"";
                } else if(type.IsArray) {
                    var arr = obj as Array;
                    var traverser = new ArrayToList(arr);
                    var sb = new StringBuilder();
                    sb.Append("[");
                    bool first = true;
                    foreach(var item in traverser.elements) {
                        if(first) {
                            first = false;
                        } else {
                            sb.Append(",");
                        }
                        sb.Append(RecursiveSerialize(item, stack));
                    }
                    sb.Append("]");
                    retVal = sb.ToString();
                } else if(obj is IList) {
                    var list = obj as IList;
                    var sb = new StringBuilder();
                    sb.Append("[");
                    bool first = true;
                    foreach(var item in list) {
                        if(first) {
                            first = false;
                        } else {
                            sb.Append(",");
                        }
                        sb.Append(RecursiveSerialize(item, stack));
                    }
                    sb.Append("]");
                    retVal = sb.ToString();
                } else if(type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))) {
                    var keyType = type.GetGenericArguments()[0];
                    if(keyType.IsEnum || keyType.IsPrimitive || (keyType == typeof(string))) {
                        var sb = new StringBuilder();
                        sb.Append("{");
                        var dict = obj as IDictionary;
                        bool first = true;
                        foreach(var key in dict.Keys) {
                            if(first) {
                                first = false;
                            } else {
                                sb.Append(",");
                            }
                            sb.Append(RecursiveSerialize(key, stack)).Append(": ").Append(RecursiveSerialize(dict[key], stack));
                        }
                        sb.Append("}");
                        retVal = sb.ToString();
                    } else {
                        retVal = null;
                    }
                } else {
                    var sb = new StringBuilder();
                    sb.Append("{");
                    bool first = true;
                    List<FieldInfo> fields = CollectFields(type);
                    foreach(var field in fields) {
                        if(IgnoreField(field)) { continue; }
                        if(first) {
                            first = false;
                        } else {
                            sb.Append(",");
                        }
                        var name = field.Name;
                        if(name.Contains("<")) {
                            name = name.Replace("<", "");
                            name = name.Replace(">", "");
                        }
                        if(name.EndsWith("i__Field")) {
                            name = name.Substring(0, name.Length - 8);
                        }
                        sb.Append("\"").Append(name).Append("\": ").Append(RecursiveSerialize(field.GetValue(obj), stack));
                    }
                    if(type.GetCustomAttributes(typeof(SerializeProperties), false).Length > 0) {
                        List<PropertyInfo> props = CollectProperties(type);
                        foreach(var prop in props) {
                            if(IgnoreProperty(prop)) { continue; }
                            if(first) {
                                first = false;
                            } else {
                                sb.Append(",");
                            }
                            sb.Append("\"").Append(prop.Name).Append("\": ").Append(RecursiveSerialize(prop.GetValue(obj, null), stack));
                        }
                    }
                    sb.Append("}");
                    retVal = sb.ToString();
                }
            }
            stack.Remove(obj);
            return retVal;
        }

        public static string SerializeAndFormat(object obj) {
            return Formatter.Json(Serialize(obj));
        }

        public static object Dictionarize(object obj) {
            return RecursiveDictionarize(obj, new List<object>());
        }

        static object RecursiveDictionarize(object obj, List<object> stack) {

            if(obj == null) {
                return null;
            }

            var type = obj.GetType();

            if(stack.Contains(obj)) {
                return "Object Loop to type " + type.Name;
            } else {
                stack.Add(obj);
            }

            object retVal;

            if(type.IsPrimitive || type.IsEnum || (obj is string)) {
                retVal = obj;
            } else if(type == typeof(Guid)) {
                retVal = obj.ToString();
            } else if(type.IsArray) {
                var arr = obj as Array;
                var traverser = new ArrayToList(arr);
                var list = traverser.elements;
                for(int i = 0; i < list.Count; ++i) {
                    list[i] = RecursiveDictionarize(list[i], stack);
                }
                retVal = list;
            } else if(obj is IList) {
                var list = obj as IList;
                var listRetVal = new List<object>();
                foreach(var item in list) {
                    listRetVal.Add(RecursiveDictionarize(item, stack));
                }
                retVal = listRetVal;
            } else if(type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))) {
                var keyType = type.GetGenericArguments()[0];
                if(keyType.IsEnum || keyType.IsPrimitive || (keyType == typeof(string))) {
                    var dictRetVal = new Dictionary<string, object>();
                    var dict = obj as IDictionary;
                    foreach(var key in dict.Keys) {
                        dictRetVal.Add(ConvertKey(key), RecursiveDictionarize(dict[key], stack));
                    }
                    retVal = dictRetVal;
                } else {
                    retVal = null;
                }
            } else {
                var dictRetVal = new Dictionary<string, object>();
                var fields = CollectFields(type);
                foreach(var field in fields) {
                    if(IgnoreField(field)) { continue; }
                    var name = field.Name;
                    if(name.Contains("<")) {
                        name = name.Replace("<", "");
                        name = name.Replace(">", "");
                    }
                    if(name.EndsWith("i__Field")) {
                        name = name.Substring(0, name.Length - 8);
                    }
                    dictRetVal.Add(name, RecursiveDictionarize(field.GetValue(obj), stack));
                }
                if(type.GetCustomAttributes(typeof(SerializeProperties), false).Length > 0) {
                    List<PropertyInfo> props = CollectProperties(type);
                    foreach(var prop in props) {
                        if(IgnoreProperty(prop)) { continue; }
                        dictRetVal.Add(prop.Name, RecursiveDictionarize(prop.GetValue(obj, null), stack));
                    }
                }
                retVal = dictRetVal;
            }
            stack.Remove(obj);
            return retVal;
        }

        static List<FieldInfo> CollectFields(Type t) {
            List<FieldInfo> fields = new List<FieldInfo>();
            while(t != typeof(object)) {
                fields.AddRange(t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                t = t.BaseType;
            }
            return fields;
        }

        static List<PropertyInfo> CollectProperties(Type t) {
            List<PropertyInfo> props = new List<PropertyInfo>();
            while(t != typeof(object)) {
                props.AddRange(t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                t = t.BaseType;
            }
            return props;
        }

        static bool IsDelegate(Type type) {
            return typeof(Delegate).IsAssignableFrom(type);
        }

        static bool DoNotSerialize(FieldInfo field) {
            return (field.GetCustomAttributes(typeof(DoNotSerilizeAttribute), false).Length > 0) ||
                    (field.GetCustomAttributes(typeof(NonSerializedAttribute), false).Length > 0);
        }

        static bool DoNotSerialize(PropertyInfo field) {
            return field.GetCustomAttributes(typeof(DoNotSerilizeAttribute), false).Length > 0;
        }

        static bool IsBackikngField(FieldInfo field) {
            return field.Name.Contains("__BackingField");
        }

        static bool IgnoreField(FieldInfo field) {
            return IsDelegate(field.FieldType) || DoNotSerialize(field) || IsBackikngField(field);
        }

        static bool IgnoreProperty(PropertyInfo prop) {
            return  IsDelegate(prop.PropertyType) ||
                    DoNotSerialize(prop) ||
                    (prop.Name == "Item") ||
                    (prop.GetGetMethod() == null);
        }

        public static string ConvertKey(object key) {
            if(key is string) {
                return key as string;
            } else {
                var type = key.GetType();
                if(type.IsPrimitive) {
                    return key.ToString();
                } else {
                    var flags = type.GetCustomAttributes(typeof(FlagsAttribute), false);
                    if(flags.Length == 0) {
                        return key.ToString();
                    } else {
                        var enumList = GetEnumList(type, key);
                        var retVal = new StringBuilder();
                        var first = true;
                        for(int i = 0; i < enumList.Count; ++i) {
                            if(first) {
                                first = false;
                            } else {
                                retVal.Append('|');
                            }
                            retVal.Append(enumList[i]);
                        }
                        return retVal.ToString();
                    }
                }
            }
        }

        static List<string> GetEnumList(Type type, object obj) {
            var retVal = new List<string>();
            var vals = (int[]) Enum.GetValues(type);
            var val = (int) obj;
            if(val == 0) {
                retVal.Add(obj.ToString());
            } else {
                for(int i = 0; i < vals.Length; ++i) {
                    if((val & vals[i]) != 0) {
                        retVal.Add(Enum.GetName(type, vals[i]));
                    }
                }
            }
            return retVal;
        }
    }
}
