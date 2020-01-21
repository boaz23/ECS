using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public abstract class StatementWithBody : StatetmentBase
    {
        protected static void ParseStatements(TokensStack sTokens, List<StatetmentBase> body)
        {
            Token token = sTokens.Pop(); // '{'
            if (!(token is Parentheses) || ((Parentheses)token).Name != '{')
            {
                throw new SyntaxErrorException($"Expected a '{{' but saw '{token}'", token);
            }

            while (sTokens.Count > 0 && !(sTokens.Peek() is Parentheses))
            {
                token = sTokens.Pop();
                StatetmentBase statetment = Create(token);
                statetment.Parse(sTokens);
                body.Add(statetment);
            }

            token = sTokens.Pop(); // '}'
            if (!(token is Parentheses) || ((Parentheses)token).Name != '}')
            {
                throw new SyntaxErrorException($"Expected a '}}' but saw '{token}'", token);
            }
        }
    }
}
