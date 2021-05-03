using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Polymorph.DataTools.Dynamics;

namespace Polymorph.Unity.MVVM {

    internal class PathNode : IDictionary, IPathedNode {

        Dictionary<string, PathNode> children;
        List<Action<object>> subscribers;

        public string path { get; private set; }
        public string name { get; private set; }
        public object provider { get; private set; }

        public PathNode() {
            subscribers = new List<Action<object>>();
            children = new Dictionary<string, PathNode>();
        }

        public PathNode(ViewModel vm) {
            children = new Dictionary<string, PathNode>();
            subscribers = new List<Action<object>>();
            SetProvider(vm);
        }

        public void Subscribe(Action<object> subscriber) {
            subscribers.Add(subscriber);
            try {
                subscriber(provider);
            } catch(Exception e) {
                Debug.LogException(e);
            }
        }

        public void Subscribe(ICollection<Action<object>> subscriberSet) {
            subscribers.AddRange(subscriberSet);
            foreach(var subscriber in subscriberSet) {
                try {
                    subscriber(provider);
                } catch(Exception e) {
                    Debug.LogException(e);
                }
            }
        }

        public void Unsubscribe(Action<object> subscriber) {
            subscribers.Remove(subscriber);
            try {
                subscriber(null);
            } catch(Exception e) {
                Debug.LogException(e);
            }
        }

        public void Unsubscribe(ICollection<Action<object>> subscribeerSet) {
            foreach(var subscriber in subscribeerSet) {
                Unsubscribe(subscriber);
            }
        }

        public void SetProvider(object model) {

            var vm = provider as ViewModel;
            if(vm != null) {
                vm.RemoveNode(this);
            }
            provider = vm;
            vm = provider as ViewModel;
            if(vm != null) {
                vm.AddPathNode(this);
                var modelChildren = vm.GetChildren();
                var strings = new List<string>(children.Keys);
                foreach(var child in modelChildren) {

                    if(strings.Contains(child.Key)) {
                        strings.Remove(child.Key);
                        children[child.Key].SetProvider(child.Value);
                    } else {
                        var newNode = new PathNode(child.Value);
                        var pathed = newNode as IPathedNode;
                        pathed.SetPath(path + "." + child.Key);
                        children.Add(child.Key, newNode);
                    }
                }

            }

            UpdateSubscribers();
        }

        internal void ClearProvider() {
            var vm = provider as ViewModel;
            if(vm != null) {
                vm.RemoveNode(this);
            }
            provider = null;
            foreach(var child in children) {
                child.Value.ClearProvider();
            }
            UpdateSubscribers();
        }

        internal void CleanNode() {
            ClearProvider();
            children = new Dictionary<string, PathNode>();
            subscribers = new List<Action<object>>();
        }

        void UpdateSubscribers() {
            foreach(var subscriber in subscribers) {
                try {
                    subscriber(provider);
                } catch(Exception e) {
                    Debug.LogException(e);
                }
            }
        }

        public void ChildChanged(string child, ViewModel vm) {
            if(children.ContainsKey(child)) {
                children[child].SetProvider(vm);
            } else {
                children.Add(child, new PathNode(vm));
            }
        }

        public void ViewModelInternalsChagned() {
            UpdateSubscribers();
        }

        public PathNode this[string key] {
            get { return children[key]; }
            set { children[key] = value; }
        }

        #region IPathedNode implementation
        void IPathedNode.SetPath(string path) {
            this.path = path;
            this.name = path.Substring(path.LastIndexOf(".") + 1);
        }
        #endregion

        #region IDictionary implementation
        public bool IsFixedSize { get { return (children as IDictionary).IsFixedSize; } }
        public bool IsReadOnly { get { return (children as IDictionary).IsReadOnly; } }
        public ICollection Keys { get { return children.Keys; } }
        public ICollection Values { get { return children.Values; } }
        public int Count { get { return children.Count; } }
        public bool IsSynchronized { get { return (children as ICollection).IsSynchronized; } }
        public object SyncRoot { get { return (children as ICollection).SyncRoot; } }
        public object this[object key] {
            get { return children[Convert.ToString(key)]; }
            set { children[Convert.ToString(key)] = value as PathNode; }
        }

        public void Add(object key, object value) {
            children.Add(Convert.ToString(key), value as PathNode);
        }
        public void Clear() {
            children.Clear();
        }
        public bool Contains(object key) {
            return children.ContainsKey(Convert.ToString(key));
        }
        public IDictionaryEnumerator GetEnumerator() {
            return children.GetEnumerator();
        }
        public void Remove(object key) {
            children.Remove(Convert.ToString(key));
        }
        public void CopyTo(Array array, int index) {
            (children as ICollection).CopyTo(array, index);
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}
