using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    class Program
    {




        static void Main(string[] args)
        {
            TestParse();
        }



        public static string GetName(Token t)
        {
            if (t is Identifier)
            {
                return ((Identifier)t).Name;
            }
            if (t is Keyword)
            {
                return ((Keyword)t).Name;
            }
            if (t is Symbol)
            {
                return ((Symbol)t).Name + "";
            }
            if (t is Number)
            {
                return ((Number)t).Value +"";
            }
            return "";
        }



        private static bool TestParse()
        {
            try
            {
                Compiler sc = new Compiler();
                List<string> lLines = sc.ReadFile(@"Program.Jack");
                List<Token> lTokens = sc.Tokenize(lLines);
                TokensStack sTokens = new TokensStack();
                for (int i = lTokens.Count - 1; i >= 0; i--)
                    sTokens.Push(lTokens[i]);
                JackProgram prog = new JackProgram();
                prog.Parse(sTokens);
                string sAfterParsing = prog.ToString();
                sAfterParsing = sAfterParsing.Replace(" ", "");
                sAfterParsing = sAfterParsing.Replace("\t", "");
                sAfterParsing = sAfterParsing.Replace("\n", "");
                sAfterParsing = sAfterParsing.ToLower();

                string sAllTokens = "";
                foreach (Token t in lTokens)
                    sAllTokens += GetName(t).ToLower();


                for (int i = 0; i < sAllTokens.Length; i++)
                    if(sAllTokens[i] != sAfterParsing[i])
                        return false;
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }
    }
}
