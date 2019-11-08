using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A mux has 2 inputs. There is a single output and a control bit, selecting which of the 2 inpust should be directed to the output.
    class MuxGate : TwoInputGate
    {
        public Wire ControlInput { get; private set; }
        //your code here

        private NotGate m_gNotC;
        private AndGate m_gAndX;
        private AndGate m_gAndY;
        private OrGate m_gOr;

        public MuxGate()
        {
            ControlInput = new Wire();

            //your code here
             m_gNotC = new NotGate();
            m_gAndX = new AndGate();
            m_gAndY = new AndGate();
            m_gOr = new OrGate();

            m_gNotC.ConnectInput(ControlInput);
            m_gAndX.ConnectInput1(Input1);
            m_gAndX.ConnectInput2(m_gNotC.Output);
            m_gAndY.ConnectInput1(Input2);
            m_gAndY.ConnectInput2(ControlInput);
            m_gOr.ConnectInput1(m_gAndX.Output);
            m_gOr.ConnectInput2(m_gAndY.Output);

            Output = m_gOr.Output;
        }

        public void ConnectControl(Wire wControl)
        {
            ControlInput.ConnectInput(wControl);
        }


        public override string ToString()
        {
            return "Mux " + Input1.Value + "," + Input2.Value + ",C" + ControlInput.Value + " -> " + Output.Value;
        }



        public override bool TestGate()
        {
            return TestGate(c: 0, x: 0, y: 0, expected: 0) &&
                   TestGate(c: 0, x: 0, y: 1, expected: 0) &&
                   TestGate(c: 0, x: 1, y: 0, expected: 1) &&
                   TestGate(c: 0, x: 1, y: 1, expected: 1) &&
                   TestGate(c: 1, x: 0, y: 0, expected: 0) &&
                   TestGate(c: 1, x: 0, y: 1, expected: 1) &&
                   TestGate(c: 1, x: 1, y: 0, expected: 0) &&
                   TestGate(c: 1, x: 1, y: 1, expected: 1);
        }

        private bool TestGate(int c, int x, int y, int expected)
        {
            Input1.Value = x;
            Input2.Value = y;
            ControlInput.Value = c;

            return Output.Value == expected;
        }
    }
}
