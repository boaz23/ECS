using System.Collections.Generic;
using System.Linq;

namespace SimpleCompiler
{
    internal class TokenDebugInfo
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Position { get; set; }

        public static TokenDebugInfo[] FromTokens(IEnumerable<Token> tokens)
        {
            return tokens.Select(x => new TokenDebugInfo
            {
                Type = x.GetType().Name,
                Value = x.ToString(),
                Line = x.Line + 1,
                Position = x.Position + 1
            })
            .ToArray();
        }

        public override string ToString()
        {
            return $"{Line}, {Position}, '{Value}', {Type}";
        }
    }
}