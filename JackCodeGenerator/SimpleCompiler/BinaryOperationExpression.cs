using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class BinaryOperationExpression : Expression
    {
        public string Operator { get;  set; }
        public Expression Operand1 { get;  set; }
        public Expression Operand2 { get;  set; }

        public override string ToString()
        {
            return "(" + Operand1 + " " + Operator + " " + Operand2 + ")";
        }

        public override void Parse(TokensStack sTokens)
        {
            Token token;

            token = sTokens.Pop(); // '('
            if (!(token is Parentheses) || ((Parentheses)token).Name != '(')
            {
                throw new SyntaxErrorException($"Expected a '(' but saw '{token}'", token);
            }

            Expression op1 = Create(sTokens);
            op1.Parse(sTokens);
            Operand1 = op1;

            token = sTokens.Pop(); // binary operator
            if (!(token is Operator) || !Token.BinaryOperators.Contains(((Operator)token).Name))
            {
                throw new SyntaxErrorException($"Expected a binary operator but saw '{token}'", token);
            }
            Operator = ((Operator)token).Name.ToString();

            Expression op2 = Create(sTokens);
            op2.Parse(sTokens);
            Operand2 = op2;

            token = sTokens.Pop(); // ')'
            if (!(token is Parentheses) || ((Parentheses)token).Name != ')')
            {
                throw new SyntaxErrorException($"Expected a ')' but saw '{token}'", token);
            }
        }
    }
}
