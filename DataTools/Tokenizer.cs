using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Polymorph.DataTools {

    public static class StandardRules {

        public static string FloatRule = @"[-+]?[0-9]*\.?[0-9]*[eE]?[-+]?[0-9]+";
        public static string AnythingRule = @"[.\s\S]*?";
        public static string SingleLineAnything = @".*?";
    }

    public class Rule<T> {
        public string rule;
        public T logic;
        public Rule(string r, T l) {
            rule = r;
            logic = l;
        }
        public override string ToString() {
            return "(" + rule + ")";
        }
    }

    public class ComplexRule<T> : Rule<T> {
        public string prefix;
        public string suffix;
        public ComplexRule(string p, string r, string s, T l)
            : base(r, l) {
            prefix = p;
            suffix = s;
        }
        public override string ToString() {
            return prefix + base.ToString() + suffix;
        }
    }

    public struct Token<InnerT> {

        public string value;
        public InnerT rule;
        public int pos;

        public Token(string v, InnerT r, int p) {
            value = v;
            rule = r;
            pos = p;
        }

        public override string ToString() {
            return "Token<" + typeof(InnerT).Name + ">:(\"" + value + "\", " + rule + ", " + pos + ")";
        }

    } // end class Token<T>

    public static class SimpleTokenizer {

        public static string[] Tokenize(string rule, string stream, RegexOptions options = RegexOptions.None) {

            Regex reg = new Regex("(" + rule + ")", options);
            var col = reg.Matches(stream);
            var l = new List<string>();
            for(int i = 0; i < col.Count; ++i) {
                if(!string.IsNullOrEmpty(col[i].Groups[1].Value)) {
                    l.Add(col[i].Value);
                }
            }

            var retVal = l.ToArray();

            return retVal;

        }

        public static T[] Tokenize<T>(string rule, string stream, System.Converter<string, T> converter, RegexOptions options = RegexOptions.None) {

            var retVal = new List<T>();
            Regex reg = new Regex("(" + rule + ")", options);
            var col = reg.Matches(stream);
            for(int i = 0; i < col.Count; ++i) {
                if(!string.IsNullOrEmpty(col[i].Groups[1].Value)) {
                    retVal.Add(converter(col[i].Value));
                }
            }
            return retVal.ToArray();

        }

    }

    /// <summary>
    /// A simple regex based tokenizer
    /// </summary>
    /// <typeparam name="T">The enum type to be used as the returned logic on matches</typeparam>
    /// <author>Victor Belski</author>
    public class Tokenizer<T> {

        string _stream;
        public string stream {
            get { return _stream; }
            set {
                _stream = value;
                _currentPosition = 0;
                reloadStream = true;
            }
        }
        List<Rule<T>> rules;
        Regex regRules;
        Match currentMatch;
        bool rulesChanged = false;
        bool reloadStream = false;
        int lastTokenPos = -1;
        RegexOptions options;
        int _currentPosition = 0;
        public int currentPosition {
            get { return _currentPosition; }
            set {
                reloadStream = true;
                if(_stream != null) {
                    if(value > _stream.Length) {
                        _currentPosition = _stream.Length;
                        return;
                    }
                }
                _currentPosition = value;
            }
        }

        public Token<T> currentToken;

        /// <summary>
        /// A simple constructor, use AddRule(string, T) to add all rules then Load(string) to load the stream
        /// </summary>
        public Tokenizer(RegexOptions options = RegexOptions.None) {
            rules = new List<Rule<T>>();
            this.options = options;
        }

        public Tokenizer(string stream, RegexOptions options = RegexOptions.None) {
            rules = new List<Rule<T>>();
            this.options = options;
            this.stream = stream;
        }

        /// <summary>
        /// Add a rule derived from Rule class
        /// </summary>
        /// <param name="rule">The rule to be added</param>
        public Tokenizer<T> AddRule(Rule<T> rule) {
            rules.Add(rule);
            rulesChanged = true;
            return this;
        }

        /// <summary>
        /// Adds a simple rule to be used while tokenizing.
        /// </summary>
        /// <param name="r">Regex form of the rule to be added</param>
        /// <param name="l">Logic to be returned on a match of this rule</param>
        public Rule<T> AddSimpleRule(string r, T l) {
            var retVal = new Rule<T>(r, l);
            rules.Add(retVal);
            rulesChanged = true;
            return retVal;
        }

        /// <summary>
        /// Adds a complex rule to used while tokenizing, the captured value will only include the characters
        /// matching the rule, without the prefix and the suffix
        /// </summary>
        /// <param name="p">Regex form of the prefix</param>
        /// <param name="r">Regex form of the rule to be captured</param>
        /// <param name="s">Regex form of the suffix</param>
        /// <param name="l">Logic to be returned on a match of this rule</param>
        public ComplexRule<T> AddComplexRule(string p, string r, string s, T l) {
            var retVal = new ComplexRule<T>(p, r, s, l);
            rules.Add(retVal);
            rulesChanged = true;
            return retVal;
        }

        public Rule<T> RemoveRule(int index) {
            var retVal = rules[index];
            rules.RemoveAt(index);
            rulesChanged = true;
            return retVal;
        }

        /// <summary>
        /// Reads the next token in the stream, placing the token value in currentToken, a null token implies\n
        /// the end has been reached
        /// </summary>
        /// <returns>true is a match is found, false is no matches were found</returns>
        public bool NextToken() {
            if(_stream == null) {
                throw new System.Exception("No stream loaded, but you are trying to read tokens");
            }
            if(rulesChanged) {
                return CreateRegex();
            } else {
                if(reloadStream) {
                    return Restart();
                } else {
                    currentMatch = currentMatch.NextMatch();
                    return ReadCurrent();
                }
            }
        }

        /// <summary>
        /// Reads the contents of the current match to currentToken
        /// </summary>
        /// <returns>true is a match is found, false is no matches were found</returns>
        bool ReadCurrent() {

            if(currentMatch.Success) {

                for(int i = 0; i < rules.Count; ++i) {
                    string groupMatch = currentMatch.Groups[i + 1].Value;
                    if(!string.IsNullOrEmpty(groupMatch)) {
                        currentToken.value = groupMatch;
                        currentToken.rule = rules[i].logic;
                        currentToken.pos = currentMatch.Index;
                        // TODO: Add Complex rule prefix removal from index
                        _currentPosition = currentMatch.Index + groupMatch.Length;
                        if(lastTokenPos == currentToken.pos) {
                            return false;
                        } else {
                            lastTokenPos = currentToken.pos;
                            return true;
                        }
                    }
                }

                //Empty case, ignore and continue
                return NextToken();

            } else {
                currentToken = new Token<T>();
            } // fi(currentMatch.Success)

            return false;
        } // end ReadCurrent()

        /// <summary>
        /// Intializes the Regex class to for work with the current rules
        /// </summary>
        /// <returns>true is a match is found, false is no matches were found</returns>
        bool CreateRegex() {

            bool first = true;
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < rules.Count; ++i) {
                if(first) {
                    builder.Append(rules[i].ToString());
                    first = false;
                } else {
                    builder.Append("|" + rules[i].ToString());
                }
            }
            regRules = new Regex(builder.ToString(), options);
            rulesChanged = false;
            return Restart();
        } // end ConvertRegex()

        /// <summary>
        /// Start reading the stream from the current position
        /// </summary>
        /// <returns>true is a match is found, false is no matches were found</returns>
        bool Restart() {
            currentMatch = regRules.Match(_stream, _currentPosition);
            lastTokenPos = -1;
            reloadStream = false;
            currentToken = new Token<T>();
            return ReadCurrent();
        }

    } // end class Tokenizer<T>

}