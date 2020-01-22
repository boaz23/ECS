using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class Compiler
    {
        private const string COMMENT_START = "//";
        private static readonly char[] WhitespaceSeparators = new char[] { ' ', '\t' };
        private static readonly char[] SplitSeparators;

        private static readonly Regex IdentifierRegex = new Regex(@"^(_|[a-z]|[A-Z])\w*$");

        private Dictionary<string, int> m_dSymbolTable;
        private int m_cLocals;

        static Compiler()
        {
            List<char> splitSeparators = new List<char>();
            splitSeparators.AddRange(Token.Parentheses);
            splitSeparators.AddRange(Token.Operators);
            SplitSeparators = splitSeparators.ToArray();
        }

        public Compiler()
        {
            m_dSymbolTable = new Dictionary<string, int>();
            m_cLocals = 0;
        }

        public List<VarDeclaration> ParseVarDeclarations(List<string> lVarLines)
        {
            List<VarDeclaration> lVars = new List<VarDeclaration>();
            for(int i = 0; i < lVarLines.Count; i++)
            {
                List<Token> lTokens = Tokenize(lVarLines[i], i);
                TokensStack stack = new TokensStack(lTokens);
                VarDeclaration var = new VarDeclaration();
                var.Parse(stack);
                lVars.Add(var);
            }
            return lVars;
        }


        public List<LetStatement> ParseAssignments(List<string> lLines)
        {
            List<LetStatement> lParsed = new List<LetStatement>();
            List<Token> lTokens = Tokenize(lLines);
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            while(sTokens.Count > 0)
            {
                LetStatement ls = new LetStatement();
                ls.Parse(sTokens);
                lParsed.Add(ls);

            }
            return lParsed;
        }

 

        public List<string> GenerateCode(LetStatement aSimple, Dictionary<string, int> dSymbolTable)
        {
            List<string> lAssembly = new List<string>();
            //add here code for computing a single let statement containing only a simple expression
            return lAssembly;
        }


        public Dictionary<string, int> ComputeSymbolTable(List<VarDeclaration> lDeclerations)
        {
            Dictionary<string, int> dTable = new Dictionary<string, int>();
            //add here code to comptue a symbol table for the given var declarations
            //real vars should come before (lower indexes) than artificial vars (starting with _), and their indexes must be by order of appearance.
            //for example, given the declarations:
            //var int x;
            //var int _1;
            //var int y;
            //the resulting table should be x=0,y=1,_1=2
            //throw an exception if a var with the same name is defined more than once
            int offset = 0;
            for (int i = 0; i < lDeclerations.Count; i++)
            {
                string varName = lDeclerations[i].Name;
                if (varName[0] != '_')
                {
                    dTable[varName] = offset++;
                }
            }
            for (int i = 0; i < lDeclerations.Count; i++)
            {
                string varName = lDeclerations[i].Name;
                if (varName[0] == '_')
                {
                    dTable[varName] = offset++;
                }
            }
            return dTable;
        }


        public List<string> GenerateCode(List<LetStatement> lSimpleAssignments, List<VarDeclaration> lVars)
        {
            List<string> lAssembly = new List<string>();
            Dictionary<string, int> dSymbolTable = ComputeSymbolTable(lVars);
            foreach (LetStatement aSimple in lSimpleAssignments)
                lAssembly.AddRange(GenerateCode(aSimple, dSymbolTable));
            return lAssembly;
        }

        public List<LetStatement> SimplifyExpressions(LetStatement s, List<VarDeclaration> lVars)
        {
            //add here code to simply expressins in a statement. 
            //add var declarations for artificial variables.
            return null;
        }
        public List<LetStatement> SimplifyExpressions(List<LetStatement> ls, List<VarDeclaration> lVars)
        {
            List<LetStatement> lSimplified = new List<LetStatement>();
            foreach (LetStatement s in ls)
                lSimplified.AddRange(SimplifyExpressions(s, lVars));
            return lSimplified;
        }

 
        public LetStatement ParseStatement(List<Token> lTokens)
        {
            TokensStack sTokens = new TokensStack();
            for (int i = lTokens.Count - 1; i >= 0; i--)
                sTokens.Push(lTokens[i]);
            LetStatement s = new LetStatement();
            s.Parse(sTokens);
            return s;
        }

 
        public List<Token> Tokenize(string sLine, int iLine)
        {
            List<Token> lTokens = new List<Token>();
            int linePos = 0;
            List<string> separatorSplits = Split(sLine, Token.Separators);
            for (int j = 0; j < separatorSplits.Count; j++)
            {
                string linePart = separatorSplits[j];
                while (!string.IsNullOrEmpty(linePart))
                {
                    if (linePart.StartsWith(COMMENT_START))
                    {
                        goto NEXT_LINE;
                    }

                    string word;
                    int charsCount;
                    linePart = Next(linePart, WhitespaceSeparators, out word, out charsCount);
                    List<string> lineParts = Split(word, SplitSeparators);
                    for (int k = 0; k < lineParts.Count; k++)
                    {
                        string sToken = lineParts[k];
                        Token token = Tokenize(sToken, iLine, linePos);
                        if (token != null)
                        {
                            lTokens.Add(token);
                        }
                        linePos += sToken.Length;
                    }
                }
            }

        NEXT_LINE:;
            return lTokens;
        }

        public List<Token> Tokenize(List<string> lCodeLines)
        {
            List<Token> lTokens = new List<Token>();
            for (int i = 0; i < lCodeLines.Count; i++)
            {
                string sLine = lCodeLines[i];
                List<Token> lLineTokens = Tokenize(sLine, i);
                lTokens.AddRange(lLineTokens);
            }
            return lTokens;
        }

        private string Next(string s, char[] aDelimiters, out string sToken)
        {
            string sReturn = string.Empty;
            var sTokenBuilder = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (aDelimiters.Contains(s[i]))
                {
                    sReturn = s.Substring(i + 1);
                    break;
                }

                sTokenBuilder.Append(s[i]);
            }

            sToken = sTokenBuilder.ToString();
            return sReturn;
        }

        //Computes the next token in the string s, from the begining of s until a delimiter has been reached. 
        //Returns the string without the token.
        private string Next(string s, char[] aDelimiters, out string sToken, out int cChars)
        {
            cChars = 1;
            sToken = s[0] + "";
            if (aDelimiters.Contains(s[0]))
                return s.Substring(1);
            int i = 0;
            for (i = 1; i < s.Length; i++)
            {
                if (aDelimiters.Contains(s[i]))
                    return s.Substring(i);
                else
                    sToken += s[i];
                cChars++;
            }
            return null;
        }

        //Splits a string into a list of tokens, separated by delimiters
        private List<string> Split(string s, char[] aDelimiters)
        {
            List<string> lTokens = new List<string>();
            while (s.Length > 0)
            {
                string sToken = "";
                int i = 0;
                for (i = 0; i < s.Length; i++)
                {
                    if (aDelimiters.Contains(s[i]))
                    {
                        if (sToken.Length > 0)
                            lTokens.Add(sToken);
                        lTokens.Add(s[i] + "");
                        break;
                    }
                    else
                        sToken += s[i];
                }
                if (i == s.Length)
                {
                    lTokens.Add(sToken);
                    s = "";
                }
                else
                    s = s.Substring(i + 1);
            }
            return lTokens;
        }

        private static Token Tokenize(string word, int line, int linePos)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return null;
            }
            else if (Token.Statements.Contains(word))
            {
                return new Statement(word, line, linePos);
            }
            else if (Token.VarTypes.Contains(word))
            {
                return new VarType(word, line, linePos);
            }
            else if (Token.Constants.Contains(word))
            {
                return new Constant(word, line, linePos);
            }
            else
            {
                if (word.Length == 1)
                {
                    char symbol = word[0];
                    if (Token.Operators.Contains(symbol))
                    {
                        return new Operator(symbol, line, linePos);
                    }
                    else if (Token.Separators.Contains(symbol))
                    {
                        return new Separator(symbol, line, linePos);
                    }
                    else if (Token.Parentheses.Contains(symbol))
                    {
                        return new Parentheses(symbol, line, linePos);
                    }
                }

                int n;
                if (int.TryParse(word, out n))
                {
                    // number
                    return new Number(word, line, linePos);
                }
                else
                {
                    // identifier
                    var token = new Identifier(word, line, linePos);
                    if (!IsValidIdentifier(token.Name))
                    {
                        throw new SyntaxErrorException($"'{token.Name}' is not a valid identifier name.", token);
                    }

                    return token;
                }
            }
        }

        private static bool IsValidIdentifier(string identifier)
        {
            return IdentifierRegex.IsMatch(identifier);
        }
    }
}
