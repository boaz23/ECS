using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This gate implements the or operation. To implement it, follow the example in the And gate.
    class OrGate : TwoInputGate
    {
        //your code here 
        private NotGate m_gNotA;
        private NotGate m_gNotB;
        private NAndGate m_gNand;

        public OrGate()
        {
            //your code here 
            m_gNotA = new NotGate();
            m_gNotB = new NotGate();
            m_gNand = new NAndGate();
            m_gNand.ConnectInput1(m_gNotA.Output);
            m_gNand.ConnectInput2(m_gNotB.Output);
            Input1 = m_gNotA.Input;
            Input2 = m_gNotB.Input;
            Output = m_gNand.Output;
        }


        public override string ToString()
        {
            return "Or " + Input1.Value + "," + Input2.Value + " -> " + Output.Value;
        }

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
            if (Output.Value != 1)
                return false;
            return true;
        }
    }

}
