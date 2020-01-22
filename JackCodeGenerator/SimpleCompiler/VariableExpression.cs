using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class VariableExpression : Expression
    {
        public string Name;

        public override void Parse(TokensStack sTokens)
        {
            Token token;

            token = sTokens.Pop();
            if (!(token is Identifier))
            {
                throw new SyntaxErrorException($"Expected an identifier but got {token}", token);
            }

            Name = ((Identifier)token).Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
