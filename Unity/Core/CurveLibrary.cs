using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polymorph.Unity.Core {

    [CreateAssetMenu(fileName = "Curve Library", menuName = "Polymorph/Create Curve Library", order = 100)]
    public class CurveLibrary : ScriptableObject, ISerializationCallbackReceiver {

        public class NameNotFoundException : System.Exception {
            public NameNotFoundException(string name) : base(FormatMessage(name)) { }
            static string FormatMessage(string name) {
                return name + " Could not be found in the library";
            }
        }

        public class NameInLibraryException : System.Exception {
            public NameInLibraryException(string name) : base(FormatMessage(name)) { }
            static string FormatMessage(string name) {
                return name + " is already in this curve library";
            }
        }

        public class IndexOutOfBoundsException : System.Exception {
            public IndexOutOfBoundsException(int index) : base(FormatString(index)) { }
            static string FormatString(int index) {
                return "Index '" + index + "' is out of library bounds";
            }
        }

        [System.Serializable]
        public class Curve {
            public string name;
            public AnimationCurve curve;
        }

        [SerializeField]
        List<Curve> curves = new List<Curve>();

        Dictionary<string, AnimationCurve> quickRefs;

        public AnimationCurve this[string name] {
            get {
                if(!quickRefs.ContainsKey(name)) {
                    throw new NameNotFoundException(name);
                }
                return new AnimationCurve(quickRefs[name].keys); }
            set {
                if(!quickRefs.ContainsKey(name)) {
                    throw new NameNotFoundException(name);
                }
                var curve = FindCurve(name);
                curve.curve = new AnimationCurve(value.keys);
                quickRefs[name] = curve.curve;
            }
        }

        public int count { get { return curves.Count; } }

        public AnimationCurve this[int index] {
            get { 
                if((index < 0) || (index >= curves.Count)) {
                    throw new IndexOutOfBoundsException(index);
                }
                return new AnimationCurve(curves[index].curve.keys); }
            set {
                if((index < 0) || (index >= curves.Count)) {
                    throw new IndexOutOfBoundsException(index);
                }
                var curve = curves[index];
                curve.curve = new AnimationCurve(value.keys);
                quickRefs[curve.name] = curve.curve;
            }
        }

        public string[] GetNames() {
            var retVal = new string[curves.Count];
            for(int i = 0; i < curves.Count; ++i) {
                retVal[i] = curves[i].name;
            }
            return retVal;
        }

        public bool ContainsName(string name) {
            return quickRefs.ContainsKey(name);
        }

        public void AddCurve(string name, AnimationCurve curve) {
            if(quickRefs.ContainsKey(name)) {
                throw new NameInLibraryException(name);
            }
            var newCurve = new Curve();
            newCurve.name = name;
            newCurve.curve = curve;
            curves.Add(newCurve);
            quickRefs.Add(newCurve.name, newCurve.curve);
        }

        public void RemoveCurve(string name) {
            if(!quickRefs.ContainsKey(name)) {
                throw new NameNotFoundException(name);
            }
            var curve = FindCurve(name);
            curves.Remove(curve);
            quickRefs.Remove(curve.name);
        }

        public void OnAfterDeserialize() {
            quickRefs = new Dictionary<string, AnimationCurve>();
            for(int i = 0; i < curves.Count; ++i) {
                if(!quickRefs.ContainsKey(curves[i].name)) {
                    quickRefs.Add(curves[i].name, curves[i].curve);
                }
            }
        }

        public void OnBeforeSerialize() { }

        Curve FindCurve(string name) {
            Curve retVal = null;
            for(int i = 0; i < curves.Count; ++i) {
                if(name == curves[i].name) {
                    retVal = curves[i];
                    break;
                }
            }
            return retVal;
        }
    }
}
