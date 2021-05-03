using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.Unity.MVVM {

    public class ViewModel : IDisposable {

        [NonSerialized]
        Dictionary<string, ViewModel> childModels;
        [NonSerialized]
        List<PathNode> nodes;

        protected ViewModel this[string key] {
            get { return childModels[key]; }
            set {
                if(childModels.ContainsKey(key)) {
                    if(value != null) {
                        if(childModels[key] != value) {
                            childModels[key] = value;
                            ChildChanged(key, value);
                        }
                    } else {
                        childModels.Remove(key);
                        ChildChanged(key, value);
                    }
                } else {
                    if(value != null) {
                        childModels.Add(key, value);
                        ChildChanged(key, value);
                    }
                }
            }
        }

        public ViewModel() {
            childModels = new Dictionary<string, ViewModel>();
            nodes = new List<PathNode>();
        }

        internal void AddPathNode(PathNode node) {
            nodes.Add(node);
        }

        internal void RemoveNode(PathNode node) {
            nodes.Remove(node);
            if(nodes.Count == 0) {
                Dispose();
            }
        }

        internal Dictionary<string, ViewModel> GetChildren() {
            return childModels;
        }

        void ChildChanged(string name, ViewModel changedTo) {
            foreach(var node in nodes) {
                node.ChildChanged(name, changedTo);
            }
        }

        public virtual void Dispose() { }

        protected void InformChange() {
            foreach(var node in nodes) {
                node.ViewModelInternalsChagned();
            }
        }

        public override string ToString() {
            var builder = new StringBuilder();
            builder.Append("VM(").Append(GetType()).Append(", ");
            for(int i = 0; i < nodes.Count; ++i) {
                if(i > 0) {
                    builder.Append(",");
                }
                builder.Append(nodes[i].path);
            }
            builder.Append(")");
            return builder.ToString();
        }

        public string Children() {
            return Serialization.JsonSerializer.Serialize(childModels);
        }

        public void AttachViewModel(string name, ViewModel vm) {
            this[name] = vm;
        }

        public void DetachViewModel(string name, ViewModel vm) {
            if(childModels.ContainsKey(name)) {
                if(childModels[name] == vm) {
                    childModels.Remove(name);
                    ChildChanged(name, vm);
                }
            }
        }
    }
}
