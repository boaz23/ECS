using System.Diagnostics;
using static System.Linq.Enumerable;

namespace SimpleCompiler
{
    [DebuggerDisplay("{Line}, {Position}, '{ToString()}', {GetType().Name}")]
    public class Token
    {
        public static string[] Statements = { "function", "var", "let", "while", "if", "else", "return" };
        public static string[] VarTypes = { "int", "char", "boolean", "array" };
        public static string[] Constants = { "true", "false", "null" };
        public static char[] BinaryOperators = new char[] { '*', '+', '-', '/', '<', '>', '&', '=', '|' };
        public static char[] UnaryOperators = new char[] { '-', '!' };
        public static char[] Operators = new char[] { '*', '+', '-', '/', '<', '>', '&', '=', '|', '!' };
        public static char[] Parentheses = new char[] { '(', ')', '[', ']', '{', '}' };
        public static char[] Separators = new char[] { ',', ';' };

        static Token()
        {
            Operators = BinaryOperators
                .Union(UnaryOperators)
                .ToArray();
        }

        public int Line { get; set; }
        public int Position { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is Token)
            {
                Token t = (Token)obj;
                return t.Line == Line && t.Position == Position;
            }
            return false;
        }
    }
}
