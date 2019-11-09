using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class MultiwayMuxGate : Gate
    {
        public int ControlBits { get; }
        public Wire Output { get; }
        public WireSet Control { get; }
        public Wire[] Inputs { get; }

        private readonly MuxGate[] m_muxGates;

        public MultiwayMuxGate(int cControlBits)
        {
            ControlBits = cControlBits;
            Control = new WireSet(ControlBits);
            Inputs = new Wire[(int)Math.Pow(2, ControlBits)];

            m_muxGates = new MuxGate[Inputs.Length - 1];
            int muxGatesAtDepth = Inputs.Length / 2;
            for (int i = 0; i < m_muxGates.Length; i++)
            {
                m_muxGates[i] = new MuxGate();
            }

            //Output = 
        }
        public void ConnectInput(int i, Wire wInput)
        {
            Inputs[i].ConnectInput(wInput);
        }
        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }

        public override bool TestGate()
        {
            throw new NotImplementedException();
        }
    }
}
