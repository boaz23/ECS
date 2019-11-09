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

            var muxGates = new MuxGate[Inputs.Length - 1];
            InitializeMuxGates(muxGates);
            ConnectInputs(muxGates);
            BuildMuxGatesTree(muxGates);
            //for (int i = 0, exponent = 1; i < muxGates.Length; exponent *= 2, i += exponent)
            //{
            //    for (int j = 0; j < exponent; j++)
            //    {

            //    }
            //}

            Output = muxGates[0].Output;

            m_muxGates = muxGates;
        }

        private void BuildMuxGatesTree(MuxGate[] muxGates)
        {
            int depthGatesCount = Inputs.Length / 4;
            int iGate = depthGatesCount - 1;
            int iControl = 1;
            while (iGate >= 0)
            {
                for (int i = 0; i < depthGatesCount; i++)
                {
                    MuxGate gate = muxGates[iGate];
                    gate.ConnectInput1(muxGates[2 * iGate].Output);
                    gate.ConnectInput2(muxGates[(2 * iGate) + 1].Output);
                    gate.ConnectControl(Control[iControl]);

                    iGate++;
                }

                iGate -= depthGatesCount;
                depthGatesCount /= 2;
                iGate -= depthGatesCount;
                iControl++;
            }
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

        private static void InitializeMuxGates(MuxGate[] muxGates)
        {
            for (int i = 0; i < muxGates.Length; i++)
            {
                muxGates[i] = new MuxGate();
            }
        }

        private void ConnectInputs(MuxGate[] muxGates)
        {
            int iGate = (Inputs.Length / 2) - 1;
            int iInput = 0;
            while (iInput < Inputs.Length)
            {
                MuxGate muxGate = muxGates[iGate];
                muxGate.ConnectInput1(Inputs[iInput]);
                muxGate.ConnectInput2(Inputs[iInput + 1]);
                muxGate.ConnectControl(Control[0]);

                iGate++;
                iInput += 2;
            }
        }
    }
}
