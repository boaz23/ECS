using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    class UnaryOperatorExpression : Expression
    {
        public string Operator { get; set; }
        public Expression Operand { get; set; }

        public override string ToString()
        {
            return Operator + Operand;
        }

        public override void Parse(TokensStack sTokens)
        {
            Token token;

            token = sTokens.Pop(); // unary operator
            if (!(token is Operator) || !Token.UnaryOperators.Contains(((Operator)token).Name))
            {
                throw new SyntaxErrorException($"Expected a unary operator but saw '{token}'", token);
            }
            Operator = ((Operator)token).Name.ToString();

            Expression op = Create(sTokens);
            op.Parse(sTokens);
            Operand = op;
        }
    }
}
