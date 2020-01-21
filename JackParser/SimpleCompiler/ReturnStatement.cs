using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    public class ReturnStatement : StatetmentBase
    {
        public Expression Expression { get; private set; }

        //This is an example of the implementation of the Parse method
        //You need to add here correctness checks. 
        public override void Parse(TokensStack sTokens)
        {
            //First, we remove the "return" token
            Token tRet = sTokens.Pop();//return
            //Now, we create the correct Expression type based on the top token in the stack
            Expression = Expression.Create(sTokens);
            //We transfer responsibility of the parsing to the created expression
            Expression.Parse(sTokens);
            //After the expression was parsed, we expect to see ;
            Token tEnd = sTokens.Pop();//;
        }

        public override string ToString()
        {
            return "return " + Expression + ";";
        }
    }
}
