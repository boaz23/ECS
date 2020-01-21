using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCompiler
{
    public class FunctionCallExpression : Expression
    {
        public string FunctionName { get; private set; }
        public List<Expression> Args { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            Token token;

            token = sTokens.Pop();
            if (!(token is Identifier))
            {
                throw new SyntaxErrorException($"Expected an identifier but got {token}", token);
            }
            FunctionName = ((Identifier)token).Name;

            token = sTokens.Pop(); // '('
            if (!(token is Parentheses) || ((Parentheses)token).Name != '(')
            {
                throw new SyntaxErrorException($"Expected a '(' but saw '{token}'", token);
            }

            while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                Expression expression = Create(sTokens);
                expression.Parse(sTokens);
                Args.Add(expression);

                //If there is a comma, then there is another argument
                if (sTokens.Count > 0 && sTokens.Peek() is Separator) //,
                {
                    token = sTokens.Pop(); // ','
                    if (!(token is Separator) || ((Separator)token).Name != ',')
                    {
                        throw new SyntaxErrorException($"Expected a ',' but saw '{token}'", token);
                    }
                }
            }

            token = sTokens.Pop(); // ')'
            if (!(token is Parentheses) || ((Parentheses)token).Name != ')')
            {
                throw new SyntaxErrorException($"Expected a ')' but saw '{token}'", token);
            }
        }

        public override string ToString()
        {
            string sFunction = FunctionName + "(";
            for (int i = 0; i < Args.Count - 1; i++)
                sFunction += Args[i] + ",";
            if (Args.Count > 0)
                sFunction += Args[Args.Count - 1];
            sFunction += ")";
            return sFunction;
        }
    }
}