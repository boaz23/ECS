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
            Identifier t = (Identifier)sTokens.Pop();
            Name = t.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
