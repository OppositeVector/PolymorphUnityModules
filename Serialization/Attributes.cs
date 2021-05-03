using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.Serialization {
    /// <summary>
    /// Defined on a field or a property so that the serialization system would ignore it
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class DoNotSerilizeAttribute : Attribute {

    }

    /// <summary>
    /// Defined on a class so that the serialization system would also serialize the properties in it
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SerializeProperties : Attribute {

    }
}
