using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class LetStatement : StatetmentBase
    {
        public string Variable { get; set; }
        public Expression Value { get; set; }

        public override string ToString()
        {
            return "let " + Variable + " = " + Value + ";";
        }

        public override void Parse(TokensStack sTokens)
        {
            Token token;

            token = sTokens.Pop(); // 'let'
            if (!(token is Statement) && ((Statement)token).Name != "let")
            {
                throw new SyntaxErrorException($"Expected 'let' but got {token}", token);
            }

            token = sTokens.Pop();
            if (!(token is Identifier))
            {
                throw new SyntaxErrorException($"Expected an identifier but got {token}", token);
            }
            Variable = ((Identifier)token).Name;

            token = sTokens.Pop(); // '='
            if (!(token is Operator) || ((Operator)token).Name != '=')
            {
                throw new SyntaxErrorException($"Expected a '=' but saw '{token}'", token);
            }

            Expression value = Expression.Create(sTokens);
            value.Parse(sTokens);
            Value = value;

            token = sTokens.Pop(); // ';'
            if (!(token is Separator) || ((Separator)token).Name != ';')
            {
                throw new SyntaxErrorException($"Expected a ';' but saw '{token}'", token);
            }
        }
    }
}
