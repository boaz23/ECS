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
        private XorGate m_gXor;
        private AndGate m_gAnd;

        public HalfAdder()
        {
            //your code here
            m_gXor = new XorGate();
            m_gAnd = new AndGate();

            m_gXor.ConnectInput1(Input1);
            m_gXor.ConnectInput2(Input2);
            Output.ConnectInput(m_gXor.Output);

            m_gAnd.ConnectInput1(Input1);
            m_gAnd.ConnectInput2(Input2);
            CarryOutput = new Wire();
            CarryOutput.ConnectInput(m_gAnd.Output);
        }


        public override string ToString()
        {
            return "HA " + Input1.Value + "," + Input2.Value + " -> " + Output.Value + " (C" + CarryOutput + ")";
        }

        public override bool TestGate()
        {
            return Test(a: 0, b: 0, expectedS: 0, expectedC: 0) &&
                   Test(a: 0, b: 1, expectedS: 1, expectedC: 0) &&
                   Test(a: 1, b: 0, expectedS: 1, expectedC: 0) &&
                   Test(a: 1, b: 1, expectedS: 0, expectedC: 1);
        }

        private bool Test(int a, int b, int expectedS, int expectedC)
        {
            Input1.Value = a;
            Input2.Value = b;

            return Output.Value == expectedS && CarryOutput.Value == expectedC;
        }
    }
}
