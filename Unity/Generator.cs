using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polymorph.Unity {
    /// <summary>
    /// A generator class that will produce elements based on the 
    /// given prototype, and places them under a single content parent
    /// </summary>
    [System.Serializable]
    public class Generator : IGenerator {
        /// <summary>
        /// The parent of the active elements
        /// </summary>
        [SerializeField]
        [Tooltip("The parent of the active elements")]
        protected Transform ContentParent;
        /// <summary>
        /// Should the parented element keep its position, or should it go to its current position in the parent
        /// </summary>
        [SerializeField]
        [Tooltip("Should the parented element keep its position, or should it go to its current position in the parent")]
        protected bool WorldPositionStays = false;
        /// <summary>
        /// All the active elemets in the pool
        /// </summary>
        protected List<Object> Active = new List<Object>();
        /// <summary>
        /// Returns an active element at the given index
        /// </summary>
        /// <param name="i">The index to retrieve</param>
        /// <returns>The requested element</returns>
        public Object this[int i] {
            get { return Active[i]; }
        }
        /// <summary>
        /// The amount of active elements in the pool
        /// </summary>
        public int Length {
            get { return Active.Count; }
        }
        public void Setup(Transform contentParent, bool worldPositionStays) {
            SetupContentParent(contentParent);
            SetupWorldPositionStays(worldPositionStays);
        }
        public void SetupContentParent(Transform contentParent) => ContentParent = contentParent;
        public void SetupWorldPositionStays(bool worldPositionStays) => WorldPositionStays = worldPositionStays;
        /// <summary>
        /// Create a new element
        /// </summary>
        /// <param name="proto">The prototype of the required element</param>
        /// <param name="index">Optional. place the new element in a specific index. in the pool, and in the content parent</param>
        /// <returns>Created element</returns>
        public virtual Object Generate(Object proto, int index = -1) {
            var retVal = HandleCreation(proto);
            Activate(retVal, index);
            return retVal;
        }
        /// <summary>
        /// Places an element in the actives and in the content parent
        /// </summary>
        /// <param name="obj">The element to be placed</param>
        /// <param name="index">Index where to place the new elemnt, -1 means the index is irrelevant</param>
        protected void Activate(Object obj, int index) {
            if(index > -1) {
                Active.Insert(index, obj);
            } else {
                Active.Add(obj);
            }
            HandleParenting(obj, ContentParent, index);
        }
        /// <summary>
        /// Called when a creation of a element is required
        /// </summary>
        /// <param name="proto">Prototype of the element to create</param>
        /// <returns>A new element inside the parent</returns>
        protected Object HandleCreation(Object proto) {
            if(proto is GameObject) {
                return Object.Instantiate(proto);
            } else {
                var cProto = proto as Component;
                var retVal = Object.Instantiate(cProto.gameObject).GetComponent(proto.GetType());
                if(retVal is IGeneratedItem)
                    ((IGeneratedItem) retVal).SetGenerator(this);
                return retVal;
            }
        }
        /// <summary>
        /// Called when an element needs to be parented to another.
        /// </summary>
        /// <param name="ele">The element to be parented</param>
        /// <param name="parent">To which parent</param>
        /// <param name="index">Optional. Sibling index inside parent</param>
        protected void HandleParenting(Object ele, Transform parent, int index = -1) {
            Transform transform;
            if(ele is Transform) { // Maybe im assuming too much, and should let a higher level make the decision
                transform = (Transform) ele;
            } else if(ele is Component) {
                transform = ((Component) ele).transform;
            } else {
                transform = ((GameObject) ele).transform;
            }
            transform.SetParent(parent, WorldPositionStays);
            if(index > -1) {
                transform.SetSiblingIndex(index);
            }
        }
        /// <summary>
        /// Called when an element needs to be destroyed.
        /// </summary>
        /// <param name="ele">The element to be destroyed</param>
        protected void HandleDestruction(Object ele) {
            GameObject go;
            if(ele is GameObject) {
                go = ele as GameObject;
            } else {
                go = (ele as Component).gameObject;
            }
            if(Application.isPlaying) {
                Object.Destroy(go);
            } else {
                Object.DestroyImmediate(go);
            }
        }
        /// <summary>
        /// Used for object deconstruction.
        /// </summary>
        /// <param name="item">The item to be degenerated</param>
        public virtual void Degenerate(Object ele) {
            Active.Remove(ele);
            HandleDestruction(ele);
        }
        /// <summary>
        /// Removes all actives.
        /// </summary>
        public void ClearActives() {
            while(Active.Count > 0)
            {
                Degenerate(Active[0]);
            }
        }
        /// <summary>
        /// Used by the IGenerator interface to route IGeneratedItem to degeneration
        /// </summary>
        /// <param name="item"></param>
        public void Degenerate(IGeneratedItem item) {
            if(item is Object)
                Degenerate(item as Object);
        }
    }

    [System.Serializable]
    public abstract class Generator<T> : Generator where T : Object {
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
        /// Create a new element based on "proto" field
        /// </summary>
        /// <param name="index">Optional. place the new element in a specific index. in the pool, and in the content parent</param>
        /// <returns>Created element</returns>
        public T Generate(int index = -1) {
            return (T) Generate(Proto, index);
        }

        public T GetProto() => Proto;
    }
}
