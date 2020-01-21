using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class IfStatement : StatementWithBody
    {
        public Expression Term { get; private set; }
        public List<StatetmentBase> DoIfTrue { get; private set; }
        public List<StatetmentBase> DoIfFalse { get; private set; }

        public override void Parse(TokensStack sTokens)
        {
            Token token;

            token = sTokens.Pop(); // 'if'
            if (!(token is Statement) && ((Statement)token).Name != "if")
            {
                throw new SyntaxErrorException($"Expected 'if' but got {token}", token);
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

            ParseStatements(sTokens, DoIfTrue);

            token = sTokens.Peek(); // maybe "else"
            if (token is Statement && ((Statement)token).Name == "else")
            {
                sTokens.Pop();
                DoIfFalse = new List<StatetmentBase>();
                ParseStatements(sTokens, DoIfFalse);
            }
        }

        public override string ToString()
        {
            string sIf = "if(" + Term + "){\n";
            foreach (StatetmentBase s in DoIfTrue)
                sIf += "\t\t\t" + s + "\n";
            sIf += "\t\t}";
            if (DoIfFalse != null)
            {
                sIf += "else{";
                foreach (StatetmentBase s in DoIfFalse)
                    sIf += "\t\t\t" + s + "\n";
                sIf += "\t\t}";
            }
            return sIf;
        }
    }
}
