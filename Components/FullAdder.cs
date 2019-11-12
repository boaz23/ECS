using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a FullAdder, taking as input 3 bits - 2 numbers and a carry, and computing the result in the output, and the carry out.
    class FullAdder : TwoInputGate
    {
        public Wire CarryInput { get; private set; }
        public Wire CarryOutput { get; private set; }

        //your code here
        private HalfAdder m_gHf;
        private HalfAdder m_gHfCin;
        private OrGate m_gOrCout;

        public FullAdder()
        {
            CarryInput = new Wire();
            //your code here

            CarryOutput = new Wire();

            m_gHf = new HalfAdder();
            m_gHf.ConnectInput1(Input1);
            m_gHf.ConnectInput2(Input2);

            m_gHfCin = new HalfAdder();
            m_gHfCin.ConnectInput1(m_gHf.Output);
            m_gHfCin.ConnectInput2(CarryInput);
            Output.ConnectInput(m_gHfCin.Output);

            m_gOrCout = new OrGate();
            m_gOrCout.ConnectInput1(m_gHf.CarryOutput);
            m_gOrCout.ConnectInput2(m_gHfCin.CarryOutput);
            CarryOutput.ConnectInput(m_gOrCout.Output);
        }


        public override string ToString()
        {
            return Input1.Value + "+" + Input2.Value + " (C" + CarryInput.Value + ") = " + Output.Value + " (C" + CarryOutput.Value + ")";
        }

        public override bool TestGate()
        {
            return Test(a: 0, b: 0, cin: 0, expectedS: 0, expectedCout: 0) &&
                   Test(a: 0, b: 0, cin: 1, expectedS: 1, expectedCout: 0) &&
                   Test(a: 0, b: 1, cin: 0, expectedS: 1, expectedCout: 0) &&
                   Test(a: 0, b: 1, cin: 1, expectedS: 0, expectedCout: 1) &&
                   Test(a: 1, b: 0, cin: 0, expectedS: 1, expectedCout: 0) &&
                   Test(a: 1, b: 0, cin: 1, expectedS: 0, expectedCout: 1) &&
                   Test(a: 1, b: 1, cin: 0, expectedS: 0, expectedCout: 1) &&
                   Test(a: 1, b: 1, cin: 1, expectedS: 1, expectedCout: 1);
        }

        private bool Test(int a, int b, int cin, int expectedS, int expectedCout)
        {
            Input1.Value = a;
            Input2.Value = b;
            CarryInput.Value = cin;

            return Output.Value == expectedS && CarryOutput.Value == expectedCout;
        }
    }
}
