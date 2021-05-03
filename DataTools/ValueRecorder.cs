using System;
using System.Collections.Generic;
using Polymorph.Serialization;

namespace Polymorph.DataTools {

    /// <summary>
    /// Saves a set of value based on who is bigger
    /// </summary>
    /// <typeparam name="T">The type of value recorded</typeparam>
    public abstract class ValueRecorder<T> {

        Dictionary<string, T> state;
        T defaultValue;

        /// <summary>
        /// Get a value from the recorder
        /// </summary>
        /// <param name="name">The name of the value</param>
        /// <returns>The value if exists, defaultValue if it doesnt</returns>
        public T this[string name] {
            get {
                if(state.ContainsKey(name)) {
                    return state[name];
                } else {
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// Constructs a value recorder
        /// </summary>
        /// <param name="defaultValue">The value returned if a name doesnt exist in the recorder</param>
        public ValueRecorder(T defaultValue) {
            state = new Dictionary<string, T>();
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// Remembers the value if it is bigger then the known value
        /// </summary>
        /// <param name="name">Name of the value</param>
        /// <param name="val">The value to be recorded</param>
        /// <returns></returns>
        public bool Record(string name, T val) {
            if(state.ContainsKey(name)) {
                if(Compare(state[name], val)) {
                    state[name] = val;
                    return true;
                }
            } else {
                state.Add(name, val);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determins how to compare which value is bigger
        /// </summary>
        /// <param name="oldV">The value in the recorder</param>
        /// <param name="newV">The new value to record</param>
        /// <returns></returns>
        public abstract bool Compare(T oldV, T newV);

        /// <summary>
        /// Standard ToString override
        /// </summary>
        /// <returns>A String</returns>
        public override string ToString() {
            return JsonSerializer.SerializeAndFormat(this);
        }
    }
}
