using System.Text;
using System.Collections;
using UnityEngine;
using Polymorph.Serialization;

namespace Polymorph.Unity.Serializers {

    public class Vector2Serializer : Serializer, Deserializer {

        public System.Type intendedType { get { return typeof(Vector2); } }

        public string Serialize(object obj) {
            var vector = (Vector2) obj;
            var builder = new StringBuilder();
            builder.Append("{ \"x\": ");
            builder.Append(vector.x);
            builder.Append(", \"y\": ");
            builder.Append(vector.y);
            builder.Append("}");
            return builder.ToString();
        }

        public object Deserialize(System.Type type, object json, object proto = null) {
            Vector2 retVal;
            if(proto != null) {
                retVal = (Vector2) proto;
            } else {
                retVal = new Vector2();
            }
            var dict = json as IDictionary;
            if(dict != null) {
                if(dict.Contains("x")) {
                    retVal.x = JsonSerializer.Deserialize<float>(dict["x"]);
                }
                if(dict.Contains("y")) {
                    retVal.y = JsonSerializer.Deserialize<float>(dict["y"]);
                }
            }
            return retVal;
        }
    }

    public class Vector3Serializer : Serializer, Deserializer {

        public System.Type intendedType { get { return typeof(Vector3); } }

        public string Serialize(object obj) {
            var vector = (Vector3) obj;
            var builder = new StringBuilder();
            builder.Append("{ \"x\": ");
            builder.Append(vector.x);
            builder.Append(", \"y\": ");
            builder.Append(vector.y);
            builder.Append(", \"z\": ");
            builder.Append(vector.z);
            builder.Append("}");
            return builder.ToString();
        }

        public object Deserialize(System.Type type, object json, object proto = null) {
            Vector3 retVal;
            if(proto != null) {
                retVal = (Vector3) proto;
            } else {
                retVal = new Vector3();
            }
            var dict = json as IDictionary;
            if(dict != null) {
                if(dict.Contains("x")) {
                    retVal.x = JsonSerializer.Deserialize<float>(dict["x"]);
                }
                if(dict.Contains("y")) {
                    retVal.y = JsonSerializer.Deserialize<float>(dict["y"]);
                }
                if(dict.Contains("z")) {
                    retVal.z = JsonSerializer.Deserialize<float>(dict["z"]);
                }
            }
            return retVal;
        }
    }

    public class Vector4Serializer : Serializer, Deserializer {

        public System.Type intendedType { get { return typeof(Vector4); } }

        public string Serialize(object obj) {
            var vector = (Vector4) obj;
            var builder = new StringBuilder();
            builder.Append("{ \"x\": ");
            builder.Append(vector.x);
            builder.Append(", \"y\": ");
            builder.Append(vector.y);
            builder.Append(", \"z\": ");
            builder.Append(vector.z);
            builder.Append(", \"w\": ");
            builder.Append(vector.w);
            builder.Append("}");
            return builder.ToString();
        }

        public object Deserialize(System.Type type, object json, object proto = null) {
            Vector4 retVal;
            if(proto != null) {
                retVal = (Vector4) proto;
            } else {
                retVal = new Vector4();
            }
            var dict = json as IDictionary;
            if(dict != null) {
                if(dict.Contains("x")) {
                    retVal.x = JsonSerializer.Deserialize<float>(dict["x"]);
                }
                if(dict.Contains("y")) {
                    retVal.y = JsonSerializer.Deserialize<float>(dict["y"]);
                }
                if(dict.Contains("z")) {
                    retVal.z = JsonSerializer.Deserialize<float>(dict["z"]);
                }
                if(dict.Contains("w")) {
                    retVal.w = JsonSerializer.Deserialize<float>(dict["w"]);
                }
            }
            return retVal;
        }
    }

    public class QuaternionSerializer : Serializer, Deserializer {

        public System.Type intendedType { get { return typeof(Quaternion); } }

        public string Serialize(object obj) {
            var vector = (Quaternion) obj;
            var builder = new StringBuilder();
            builder.Append("{ \"x\": ");
            builder.Append(vector.x);
            builder.Append(", \"y\": ");
            builder.Append(vector.y);
            builder.Append(", \"z\": ");
            builder.Append(vector.z);
            builder.Append(", \"w\": ");
            builder.Append(vector.w);
            builder.Append("}");
            return builder.ToString();
        }

        public object Deserialize(System.Type type, object json, object proto = null) {
            Quaternion retVal;
            if(proto != null) {
                retVal = (Quaternion) proto;
            } else {
                retVal = new Quaternion();
            }
            var dict = json as IDictionary;
            if(dict != null) {
                if(dict.Contains("x")) {
                    retVal.x = JsonSerializer.Deserialize<float>(dict["x"]);
                }
                if(dict.Contains("y")) {
                    retVal.y = JsonSerializer.Deserialize<float>(dict["y"]);
                }
                if(dict.Contains("z")) {
                    retVal.z = JsonSerializer.Deserialize<float>(dict["z"]);
                }
                if(dict.Contains("w")) {
                    retVal.w = JsonSerializer.Deserialize<float>(dict["w"]);
                }
            }
            return retVal;
        }
    }
}
