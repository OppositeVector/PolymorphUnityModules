using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Polymorph.Serialization;
using Polymorph.DataTools.Dynamics;

namespace Polymorph.Unity.MVVM {

    /// <summary>
    /// A central point where all view models should be registered
    /// </summary>
    public static class ViewModelRegistry {

        class Follower : PathFollower<PathNode> {
            public Follower(string path, object obj) : base(path, obj) {
                if(!exists) {
                    SetValue(new PathNode());
                }
            }
        }

        // Might need to add a dispatcher to make sure events rise from the unity main thread

        static PathNode root;

        static ViewModelRegistry() {
            root = new PathNode();
        }

        static Follower GetElement(string path) {
            return new Follower(path, root);
        }

        /// <summary>
        /// <para>Subscribe to a certain path on the registry, upon subscription and every time that</para>
        /// <para>path changes viewmodels, the subscription action will be raised</para>
        /// </summary>
        /// <param name="path">Path to subscribe to</param>
        /// <param name="subscriber">Action that will be called upon changes</param>
        public static void Subscribe(string path, Action<object> subscriber) {
            var ele = GetElement(path).GetValue<PathNode>();
            ele.Subscribe(subscriber);
        }

        /// <summary>
        /// <para>Subscribe to a certain path on the registry, upon subscription and every time that</para>
        /// <para>path changes viewmodels, the subscription action will be raised</para>
        /// </summary>
        /// <param name="path">Path to subscribe to</param>
        /// <param name="subscribers">Actions that will be called upon changes</param>
        public static void Subscribe(string path, ICollection<Action<object>> subscribers) {
            var ele = GetElement(path).GetValue<PathNode>();
            ele.Subscribe(subscribers);
        }

        /// <summary>
        /// <para>Unsubscribe from a path to stop receiving viewmodel updates</para>
        /// </summary>
        /// <param name="path">Path to unsubscribe from</param>
        /// <param name="subscriber">Subscriber that should be removed</param>
        public static void Unsubscribe(string path, Action<object> subscriber) {
            var ele = GetElement(path).GetValue<PathNode>();
            ele.Unsubscribe(subscriber);
        }

        /// <summary>
        /// <para>Unsubscribe from a path to stop receiving viewmodel updates</para>
        /// </summary>
        /// <param name="path">Path to unsubscribe from</param>
        /// <param name="subscribers">Subscribers that should be removed</param>
        public static void Unsubscribe(string path, ICollection<Action<object>> subscribers) {
            var ele = GetElement(path).GetValue<PathNode>();
            ele.Unsubscribe(subscribers);
        }

        /// <summary>
        /// Register a provider, everyone subscribed to the path will receive a notification of the change
        /// </summary>
        /// <param name="path">Path to register on</param>
        /// <param name="vm">The provider to register</param>
        public static void DeclareProvider(string path, object vm) {
            var follower = GetElement(path);
            var node = follower.GetValue<PathNode>();
            node.SetProvider(vm);
        }

        /// <summary>
        /// Clear the ViewModel at the path and all children ViewModels of this path
        /// </summary>
        /// <param name="path">The path to clear</param>
        /// <param name="vm">Optional. if given, will clear only if the ViewModel on the path matches the given ViewModel</param>
        public static void ClearProvider(string path, ViewModel vm = null) {
            var follower = GetElement(path);
            var node = follower.GetValue<PathNode>();
            if(vm != null) {
                if(vm != node.provider) {
                    return;
                }
            }
            node.ClearProvider();
        }

        /// <summary>
        /// A one time pull of a specific provider
        /// </summary>
        /// <param name="path">Path of the provider</param>
        /// <returns>That provider, if there is one, null in other cases</returns>
        public static object GetProvider(string path) {
            var follower = GetElement(path);
            var node = follower.GetValue<PathNode>();
            return node.provider;
        }

        /// <summary>
        /// Clears registry of all the models in it
        /// </summary>
        public static void ClearRegistry() {
            root.CleanNode();
        }

        /// <summary>
        /// Prints out the registry structure
        /// </summary>
        /// <returns></returns>
        public static string DebugToString() {
            return JsonSerializer.SerializeAndFormat(root);
        }

        /// <summary>
        /// Prints out a readable snapshot of the registry
        /// </summary>
        /// <returns></returns>
        public static string GetPrintable() {
            var builder = new StringBuilder();
            builder.Append("ViewModelRegistry:\n\n");
            foreach(var key in root.Keys) {
                GetPrintableRecursive(builder, (PathNode) root[key], 0);
            }
            return builder.ToString();
        }

        static void GetPrintableRecursive(StringBuilder builder, PathNode current, int depth) {
            for(int i = 0; i < depth; ++i) {
                builder.Append("\t");
            }
            builder.Append(current.name);
            builder.Append(": ");
            builder.Append((current.provider == null) ? "null" : current.provider.GetType().ToString());
            builder.Append("\n");
            foreach(var key in current.Keys) {
                GetPrintableRecursive(builder, (PathNode) current[key], depth + 1);
            }
        }
    }
}
