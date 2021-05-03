using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.DataTools.Exceptions {

    public class InvalidOperation : Exception {

        public string path;
        public string operation;
        public string type;

        public InvalidOperation(string o, string p, string t, string a = null)
            : base(FormMessage(o, p, t, a)) {
            path = p;
            operation = o;
            type = t;
        }

        static string FormMessage(string o, string p, string t, string additional = null) {
            var msg = new StringBuilder();
            msg.Append("Invalid operation ");
            msg.Append(o);
            msg.Append(" on path ");
            msg.Append(p);
            if(t != null) {
                msg.Append(" with object of type ");
                msg.Append(t);
            }
            if(additional != null) {
                msg.Append("\n");
                msg.Append(additional);
            }
            return msg.ToString();
        }
    }

    public class NameNotFoundException : Exception {

        public string name;
        public object obj;
        public Type type;

        public NameNotFoundException(string name, object obj, string additional = null)
            : base(FormMessage(name, obj, additional)) {
            this.name = name;
            this.obj = obj;
            this.type = obj.GetType();
        }

        static string FormMessage(string name, object obj, string additional) {
            var retVal = new StringBuilder();
            retVal.Append("Could not find name ");
            retVal.Append(name);
            retVal.Append(" int type ");
            retVal.Append(obj.GetType());
            if(additional != null) {
                retVal.Append("\n");
                retVal.Append(additional);
            }
            return retVal.ToString();
        }
    }

    public class InvalidPathException : Exception {

        public string path;
        public int position;
        public int length;

        public InvalidPathException(string msg, int position, int length, string path = null)
            : base(FormErrorMessage(msg, path, position, length)) {
            this.path = path;
            this.position = position;
            this.length = length;
        }

        public InvalidPathException(string message) : base(message) { }

        static string FormErrorMessage(string msg, string path, int position, int length) {
            StringBuilder builder = new StringBuilder();

            builder.Append(msg);
            builder.Append(":\n\"").Append(path.Substring(position, length)).Append("\" in ");
            builder.Append(path);
            builder.Append("\n");

            return builder.ToString();
        }
    }
}
