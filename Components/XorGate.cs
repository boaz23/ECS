using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This gate implements the xor operation. To implement it, follow the example in the And gate.
    class XorGate : TwoInputGate
    {
        //your code here
        private AAndNotBGate m_gAAndNotB;
        private AAndNotBGate m_gNotAAndB;
        private OrGate m_gOr;

        public XorGate()
        {
            //your code here
            m_gAAndNotB = new AAndNotBGate();
            m_gNotAAndB = new AAndNotBGate();
            m_gOr = new OrGate();
            m_gAAndNotB.ConnectInput1(Input1);
            m_gAAndNotB.ConnectInput2(Input2);
            m_gNotAAndB.ConnectInput1(Input2);
            m_gNotAAndB.ConnectInput2(Input1);
            m_gOr.ConnectInput1(m_gAAndNotB.Output);
            m_gOr.ConnectInput2(m_gNotAAndB.Output);
            Output = m_gOr.Output;
        }

        //an implementation of the ToString method is called, e.g. when we use Console.WriteLine(xor)
        //this is very helpful during debugging
        public override string ToString()
        {
            return "Xor " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }


        //this method is used to test the gate. 
        //we simply check whether the truth table is properly implemented.
        public override bool TestGate()
        {
            Input1.Value = 0;
            Input2.Value = 0;
            if (Output.Value != 0)
                return false;
            Input1.Value = 0;
            Input2.Value = 1;
            if (Output.Value != 1)
                return false;
            Input1.Value = 1;
            Input2.Value = 0;
            if (Output.Value != 1)
                return false;
            Input1.Value = 1;
            Input2.Value = 1;
            if (Output.Value != 0)
                return false;
            return true;
        }

        internal class AAndNotBGate : TwoInputGate
        {
            private NotGate m_gNot;
            private AndGate m_gAnd;

            public AAndNotBGate()
            {
                m_gNot = new NotGate();
                m_gAnd = new AndGate();
                m_gNot.ConnectInput(Input2);
                m_gAnd.ConnectInput1(Input1);
                m_gAnd.ConnectInput2(m_gNot.Output);
                Output = m_gAnd.Output;
            }

            public override string ToString()
            {
                return "PAndNotQ " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
            }

            public override bool TestGate()
            {
                Input1.Value = 0;
                Input2.Value = 0;
                if (Output.Value != 0)
                    return false;
                Input1.Value = 0;
                Input2.Value = 1;
                if (Output.Value != 0)
                    return false;
                Input1.Value = 1;
                Input2.Value = 0;
                if (Output.Value != 1)
                    return false;
                Input1.Value = 1;
                Input2.Value = 1;
                if (Output.Value != 0)
                    return false;
                return true;
            }
        }
    }
}
