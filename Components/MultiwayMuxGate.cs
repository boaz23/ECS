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
            InitializeInputs();

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
            while (depthGatesCount > 0)
            {
                for (int i = 0; i < depthGatesCount; i++)
                {
                    MuxGate gate = muxGates[iGate];
                    gate.ConnectInput1(muxGates[(2 * iGate) + 1].Output);
                    gate.ConnectInput2(muxGates[(2 * iGate) + 2].Output);
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
            return DoPermutations(new int[ControlBits], 0, new int[Inputs.Length], 0);
        }

        private void InitializeInputs()
        {
            for (int i = 0; i < Inputs.Length; i++)
            {
                Inputs[i] = new Wire();
            }
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

        private bool DoPermutations(int[] control, int iControlWire, int[] input, int iInputWire)
        {
            if (iControlWire == ControlBits && iInputWire == Inputs.Length)
            {
                SetWireValues(control, input);
                int expected = MultiwayMux(control, input);
                return TestOutput(expected);
            }
            else if (iControlWire < ControlBits)
            {
                control[iControlWire] = 0;
                if (!DoPermutations(control, iControlWire + 1, input, iInputWire))
                {
                    return false;
                }

                control[iControlWire] = 1;
                if (!DoPermutations(control, iControlWire + 1, input, iInputWire))
                {
                    return false;
                }

                ResetWire(control, iControlWire);
                return true;
            }
            else // iWire2 < Size
            {
                input[iInputWire] = 0;
                if (!DoPermutations(control, iControlWire, input, iInputWire + 1))
                {
                    return false;
                }

                input[iInputWire] = 1;
                if (!DoPermutations(control, iControlWire, input, iInputWire + 1))
                {
                    return false;
                }

                ResetWire(input, iInputWire);
                return true;
            }
        }

        private static void ResetWire(int[] inputs, int wireIndex)
        {
            inputs[wireIndex] = default(int);
        }

        private void SetWireValues(int[] control, int[] input)
        {
            for (int i = 0; i < ControlBits; i++)
            {
                Control[i].Value = control[i];
            }
            for (int i = 0; i < Inputs.Length; i++)
            {
                Inputs[i].Value = input[i];
            }
        }

        private int MultiwayMux(int[] control, int[] input)
        {
            int exp = 1;
            int iInput = 0;
            for (int i = 0; i < control.Length; i++)
            {
                iInput += control[i] * exp;
                exp *= 2;
            }

            return input[iInput];
        }

        private bool TestOutput(int expected)
        {
            return Output.Value == expected;
        }
    }
}
