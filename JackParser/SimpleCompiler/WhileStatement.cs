using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class WhileStatement : StatementWithBody
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> Body { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            Token token;

            token = sTokens.Pop(); // 'while'
            if (!(token is Statement) && ((Statement)token).Name != "while")
            {
                throw new SyntaxErrorException($"Expected 'while' but got {token}", token);
            }

            token = sTokens.Pop(); // '('
            if (!(token is Parentheses) || ((Parentheses)token).Name != '(')
            {
                throw new SyntaxErrorException($"Expected a '(' but saw '{token}'", token);
            }

            Expression term = Expression.Create(sTokens);
            term.Parse(sTokens);
            Term = term;

            token = sTokens.Pop(); // ')'
            if (!(token is Parentheses) || ((Parentheses)token).Name != ')')
            {
                throw new SyntaxErrorException($"Expected a ')' but saw '{token}'", token);
            }

            ParseStatements(sTokens, Body);
        }

        public override string ToString()
        {
            string sWhile = "while(" + Term + "){\n";
            foreach (StatetmentBase s in Body)
                sWhile += "\t\t\t" + s + "\n";
            sWhile += "\t\t}";
            return sWhile;
        }
    }
}
