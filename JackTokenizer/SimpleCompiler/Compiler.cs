using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleCompiler
{
    class Compiler
    {
        private const string COMMENT_START = "//";
        private static readonly char[] WhitespaceSeparators = new char[] { ' ', '\t' };
        private static readonly char[] SplitSeparators;

        private static readonly Regex IdentifierRegex = new Regex(@"^(_|[a-z]|[A-Z])\w*$");

        static Compiler()
        {
            List<char> splitSeparators = new List<char>();
            splitSeparators.AddRange(Token.Parentheses);
            splitSeparators.AddRange(Token.Operators);
            SplitSeparators = splitSeparators.ToArray();
        }

        public Compiler()
        {
        }

        //reads a file into a list of strings, each string represents one line of code
        public List<string> ReadFile(string sFileName)
        {
            StreamReader sr = new StreamReader(sFileName);
            List<string> lCodeLines = new List<string>();
            while (!sr.EndOfStream)
            {
                lCodeLines.Add(sr.ReadLine());
            }
            sr.Close();
            return lCodeLines;
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

        //This is the main method for the Tokenizing assignment. 
        //Takes a list of code lines, and returns a list of tokens.
        //For each token you must identify its type, and instantiate the correct subclass accordingly.
        //You need to identify the token position in the file (line, index within the line).
        //You also need to identify errors, in this assignement - illegal identifier names.
        public List<Token> Tokenize(List<string> lCodeLines)
        {
            List<Token> lTokens = new List<Token>();

            for (int iLine = 0; iLine < lCodeLines.Count; iLine++)
            {
                int linePos = 0;
                string sLine = lCodeLines[iLine];
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

