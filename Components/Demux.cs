using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A demux has 2 outputs. There is a single input and a control bit, selecting whether the input should be directed to the first or second output.
    class Demux : Gate
    {
        public Wire Output1 { get; private set; }
        public Wire Output2 { get; private set; }
        public Wire Input { get; private set; }
        public Wire Control { get; private set; }

        //your code here
        private NotGate m_gNotC;
        private AndGate m_gAnd1;
        private AndGate m_gAnd2;

        public Demux()
        {
            Input = new Wire();
            //your code here

            Control = new Wire();

            m_gNotC = new NotGate();
            m_gAnd1 = new AndGate();
            m_gAnd2 = new AndGate();

            m_gNotC.ConnectInput(Control);
            m_gAnd1.ConnectInput1(Input);
            m_gAnd1.ConnectInput2(m_gNotC.Output);
            m_gAnd2.ConnectInput1(Input);
            m_gAnd2.ConnectInput2(Control);

            Output1 = m_gAnd1.Output;
            Output2 = m_gAnd2.Output;
        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }
        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }



        public override bool TestGate()
        {
            return TestGate(c: 0, x: 0, expected1: 0, expected2: 0) &&
                   TestGate(c: 0, x: 1, expected1: 1, expected2: 0) &&
                   TestGate(c: 1, x: 0, expected1: 0, expected2: 0) &&
                   TestGate(c: 1, x: 1, expected1: 0, expected2: 1);
        }

        private bool TestGate(int c, int x, int expected1, int expected2)
        {
            Input.Value = x;
            Control.Value = c;

            return Output1.Value == expected1 && Output2.Value == expected2;
        }
    }
}
