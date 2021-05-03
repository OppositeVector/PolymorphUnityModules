using System;

namespace Polymorph.Serialization {
    public interface Serializer {
        Type intendedType { get; }
        string Serialize(object obj);
    }

    public interface Deserializer {
        Type intendedType { get; }
        object Deserialize(Type type, object json, object proto = null);
    }

}
