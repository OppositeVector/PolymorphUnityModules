using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Polymorph.Unity {
    public interface IGenerator {
        void Degenerate(IGeneratedItem item);
    }
}
