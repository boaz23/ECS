using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //Multibit gates take as input k bits, and compute a function over all bits - z=f(x_0,x_1,...,x_k)

    class MultiBitOrGate : MultiBitTwoInputGateBase
    {
        //your code here

        public MultiBitOrGate(int iInputCount)
            : base(iInputCount, 1)
        {
        }

        protected override TwoInputGate CreateGate()
        {
            return new OrGate();
        }
    }
}
