using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorph.DataTools.Dynamics {

    enum Tokens { STRINGINDEX, INDEX, MULTIINDEX, WORD, UNACCEPTABLE }

    class PathTokenizer : Tokenizer<Tokens> {

        public PathTokenizer() {
            AddRules();
        }

        public PathTokenizer(string stream) : base(stream) {
            AddRules();
        }

        void AddRules() {
            AddSimpleRule(@"^\w*", Tokens.WORD);
            AddComplexRule(@"\.", @"\w*", "", Tokens.WORD);
            AddComplexRule(@"\[", @"\w*", @"\]", Tokens.INDEX);
            AddComplexRule(@"\[", @"[\w,]*", @"\]", Tokens.MULTIINDEX);
            AddSimpleRule(@".+", Tokens.UNACCEPTABLE);
        }
    }
}
