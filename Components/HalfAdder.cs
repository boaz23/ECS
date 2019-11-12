using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a HalfAdder, taking as input 2 bits - 2 numbers and computing the result in the output, and the carry out.

    class HalfAdder : TwoInputGate
    {
        public Wire CarryOutput { get; private set; }

        //your code here
        private XorGate m_xorGate;
        private AndGate m_andGate;

        public HalfAdder()
        {
            //your code here
            m_xorGate = new XorGate();
            m_andGate = new AndGate();

            m_xorGate.ConnectInput1(Input1);
            m_xorGate.ConnectInput2(Input2);
            Output.ConnectInput(m_xorGate.Output);

            m_andGate.ConnectInput1(Input1);
            m_andGate.ConnectInput2(Input2);
            CarryOutput = new Wire();
            CarryOutput.ConnectInput(m_andGate.Output);
        }


        public override string ToString()
        {
            return "HA " + Input1.Value + "," + Input2.Value + " -> " + Output.Value + " (C" + CarryOutput + ")";
        }

        public override bool TestGate()
        {
            return Test(0, 0, 0, 0) &&
                   Test(0, 1, 1, 0) &&
                   Test(1, 0, 1, 0) &&
                   Test(1, 1, 0, 1);
        }

        private bool Test(int a, int b, int expectedS, int expectedC)
        {
            Input1.Value = a;
            Input2.Value = b;

            return Output.Value == expectedS && CarryOutput.Value == expectedC;
        }
    }
}
