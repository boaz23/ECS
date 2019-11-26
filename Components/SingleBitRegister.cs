using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a register that can maintain 1 bit.
    class SingleBitRegister : Gate
    {
        public Wire Input { get; private set; }
        public Wire Output { get; private set; }
        //A bit setting the register operation to read or write
        public Wire Load { get; private set; }

        private DFlipFlopGate m_flipFlop;
        private MuxGate m_mux;

        public SingleBitRegister()
        {
            
            Input = new Wire();
            Load = new Wire();
            //your code here 

            Output = new Wire();
            m_flipFlop = new DFlipFlopGate();
            m_mux = new MuxGate();
            m_mux.ConnectInput1(m_flipFlop.Output);
            m_mux.ConnectInput2(Input);
            m_mux.ConnectControl(Load);
            m_flipFlop.ConnectInput(m_mux.Output);
            Output.ConnectInput(m_flipFlop.Output);
        }

        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }

      

        public void ConnectLoad(Wire wLoad)
        {
            Load.ConnectInput(wLoad);
        }


        public override bool TestGate()
        {
            Load.Value = 1;
            Input.Value = 1;
            Clock.ClockDown();
            Clock.ClockUp();
            Input.Value = 0;
            if (Output.Value != 1)
            {
                return false;
            }
            Clock.ClockDown();
            Clock.ClockUp();
            Load.Value = 0;
            Input.Value = 1;
            if (Output.Value != 0)
            {
                return false;
            }
            Clock.ClockDown();
            Clock.ClockUp();
            Input.Value = 0;
            if (Output.Value != 0)
            {
                return false;
            }
            Load.Value = 1;
            Input.Value = 1;
            if (Output.Value != 0)
            {
                return false;
            }

            return true;
        }
    }
}
