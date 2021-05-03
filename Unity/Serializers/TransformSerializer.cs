using System.Text;
using System.Collections;
using UnityEngine;
using Polymorph.Serialization;

namespace Polymorph.Unity.Serializers {

    public class TransformSerializer : Serializer, Deserializer {

        public System.Type intendedType {
            get { return typeof(Transform); }
        }

        public string Serialize(object obj) {
            var builder = new StringBuilder();
            var transform = obj as Transform;
            if(transform == null) {
                return "null";
            }
            builder.Append("{ \"p\": ");
            builder.Append(JsonSerializer.Serialize(transform.localPosition));
            builder.Append(", \"r\": ");
            builder.Append(JsonSerializer.Serialize(transform.localRotation));
            builder.Append(", \"s\": ");
            builder.Append(JsonSerializer.Serialize(transform.localScale));
            builder.Append("}");
            return builder.ToString();
        }

        public object Deserialize(System.Type type, object json, object proto = null) {
            if(!(proto is Transform)) { return proto; }
            var transform = (Transform)proto;
            var dict = json as IDictionary;
            if(dict != null) {
                if(dict.Contains("p")) {
                    transform.localPosition = JsonSerializer.Deserialize<Vector3>(dict["p"]);
                }
                if(dict.Contains("r")) {
                    transform.localRotation = JsonSerializer.Deserialize<Quaternion>(dict["r"]);
                }
                if(dict.Contains("s")) {
                    transform.localScale = JsonSerializer.Deserialize<Vector3>(dict["s"]);
                }
            }
            return transform;
        }
    }
}
