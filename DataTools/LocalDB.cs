using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;
using System.Text;
using MiniJSON;
using Polymorph.Serialization;
using Polymorph.DataTools.Exceptions;
using Polymorph.DataTools.Dynamics;

namespace Polymorph.DataTools {

	/// <summary>
    /// A base class for Local Databases that handles reading and writing data to a database file
    /// </summary>
	public class LocalDB {
	
	    enum Tokens { NUM, WORD, UNACCEPTABLE }
	
	    Dictionary<string, object> tables;
        string dbPath;

        /// <summary>
        /// Create a database access instance.
        /// </summary>
        /// <param name="path">The path to the database file</param>
        public LocalDB(string path) {

            dbPath = path;

            if(File.Exists(dbPath)) {
                var dataString = File.ReadAllText(dbPath);
                try {
                    tables = Json.Deserialize(dataString) as Dictionary<string, object>;
                } catch { }
            }
            if(tables == null) {
                tables = new Dictionary<string, object>();
            }
        }
	    
        /// <summary>
        /// Commit the current data in this instance to the file
        /// </summary>
	    public virtual void Commit() {
	        File.WriteAllText(dbPath, JsonSerializer.SerializeAndFormat(tables));
	    }
	
        /// <summary>
        /// Set a value on an arbitrary path in this instance, call Commit() to save the changes to the file
        /// </summary>
        /// <param name="path">Path to the object to set</param>
        /// <param name="val">The object to set</param>
	    public virtual void Set(string path, object val) {
            var follower = new PathFollower(path, tables);
            follower.SetValue(JsonSerializer.Dictionarize(val));
	    }
	    
        /// <summary>
        /// Add to a value, implementation removed for now.
        /// </summary>
        /// <param name="path">Path to the object to add to</param>
        /// <param name="val">The value to add</param>
	    public virtual void Add(string path, double val) {
            throw new NotImplementedException("Add has been removed untill full implementation of PathFollower");
	    }
	    
        /// <summary>
        /// Get an object on an arbitrary path on this instance
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
	    public virtual object Get(string path) {
            var follower = new PathFollower(path, tables);
            return follower.GetValue();
	    }
	
        /// <summary>
        /// Get an object of a specific type and cast it to that type
        /// </summary>
        /// <param name="path">Path to the object</param>
        /// <param name="type">The type that should be returned</param>
        /// <param name="proto">A proto object to deserialize to, if no proto is given, a new object will be created</param>
        /// <returns>Deserialized object at the path of the given type</returns>
	    public object Get(string path, Type type, object proto = null) {
	        var retVal = Get(path);
	        return JsonSerializer.Deserialize(type, retVal, proto);
	    }
	
        /// <summary>
        /// Generic version of Get(string, Type, object), creates a new object of type T
        /// </summary>
        /// <typeparam name="T">The type of the object at the path</typeparam>
        /// <param name="path">Path to the object</param>
        /// <returns>Deserialized object at the path</returns>
	    public T Get<T>(string path) {
	        return (T) Get(path, typeof(T), null);
	    }

        /// <summary>
        /// Generic version of Get(string, Type, object), does not create a object
        /// </summary>
        /// <typeparam name="T">The type of the object at the path</typeparam>
        /// <param name="path">Path to the object</param>
        /// <param name="proto">Prototype object into which the data should be inserted</param>
        /// <returns>Deserialized object at the path</returns>
        public T Get<T>(string path, T proto) {
            return (T) Get(path, typeof(T), proto);
        }
	}
}
