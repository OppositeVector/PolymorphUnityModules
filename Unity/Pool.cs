using System.Collections.Generic;
using UnityEngine;

namespace Polymorph.Unity {

    /// <summary>
    /// A specialized pool for GameObjects.
    /// </summary>
    [System.Serializable]
    public class Pool : Generator {
        /// <summary>
        /// The parent of the inactive elements
        /// </summary>
        [SerializeField]
        [Tooltip("The parent of the inactive elements")]
        protected Transform PoolParent;
        /// <summary>
        /// All the inactive elements in the pool
        /// </summary>
        protected Dictionary<System.Type, Stack<Object>> Inactive = new Dictionary<System.Type, Stack<Object>>();
        /// <summary>
        /// Find an element in the pool or create a new element if the pool is empty
        /// </summary>
        /// <param name="proto">The prototype of the required element</param>
        /// <param name="index">Optional. place the new element in a specific index. in the pool, and in the content parent</param>
        /// <returns>Created element</returns>
        public override Object Generate(Object proto, int index = -1) {
            var type = proto.GetType();
            if(Inactive.ContainsKey(type) && (Inactive[type].Count > 0)) {
                var obj = Inactive[type].Pop();
                Activate(obj, index);
                return obj;
            } else {
                return base.Generate(proto, index);
            }
        }
        /// <summary>
        /// Move an element from active to inactive.
        /// </summary>
        /// <param name="item">The item to be degenerated</param>
        public override void Degenerate(Object ele) {
            Active.Remove(ele);
            var type = ele.GetType();
            if(!Inactive.ContainsKey(type)) {
                var stack = new Stack<Object>();
                stack.Push(ele);
                Inactive.Add(type, stack);
            } else {
                Inactive[type].Push(ele);
            }
            HandleParenting(ele, PoolParent);
        }
        /// <summary>
        /// Clears all the active and inactive elements from the pool.
        /// </summary>
        public void ResetPool() {
            foreach(var ele in Active) {
                HandleDestruction(ele);
            }
            Active.Clear();
            foreach(var pair in Inactive) {
                foreach(var ele in pair.Value) {
                    HandleDestruction(ele);
                }
            }
            Inactive.Clear();
        }
    }
    /// <summary>
    /// A generic pool for MonoBehaviour elements
    /// </summary>
    /// <typeparam name="T">The type of the pototype object of the pool</typeparam>
    [System.Serializable]
    public abstract class Pool<T> : Pool where T : Object {
        /// <summary>
        /// The prototype object that will be instantiated when an element is required
        /// </summary>
        [SerializeField]
        [Tooltip("The prototype object that will be instantiated when an element is required")]
        protected T Proto;
        /// <summary>
        /// Returns an active element at the given index
        /// </summary>
        /// <param name="i">The index to retrieve</param>
        /// <returns>The requested element</returns>
        new public T this[int i] {
            get { return (T) Active[i]; }
        }
        public void Setup(T proto, Transform contentParent, bool worldPositionStays) {
            Setup(contentParent, worldPositionStays);
            SetupProto(proto);
        }
        public void SetupProto(T proto) {
            Proto = proto;
        }
        /// <summary>
        /// Create a new element based on the known prototype
        /// </summary>
        /// <returns>Created Object</returns>
        public T Generate(int index = -1) {
            return (T) Generate(Proto, index);
        }
        public T GetProto() => Proto;
    }
}
