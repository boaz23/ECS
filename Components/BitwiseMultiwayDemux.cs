using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a demux with k outputs, each output with n wires. The input also has n wires.

    class BitwiseMultiwayDemux : Gate
    {
        //Word size - number of bits in each output
        public int Size { get; private set; }

        //The number of control bits needed for k outputs
        public int ControlBits { get; private set; }

        public WireSet Input { get; private set; }
        public WireSet Control { get; private set; }
        public WireSet[] Outputs { get; private set; }

        //your code here
        private readonly MultiwayDemuxGate[] m_multiwayDemuxGates;

        public BitwiseMultiwayDemux(int iSize, int cControlBits)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Control = new WireSet(cControlBits);
            Outputs = new WireSet[(int)Math.Pow(2, cControlBits)];
            for (int i = 0; i < Outputs.Length; i++)
            {
                Outputs[i] = new WireSet(Size);
            }
            //your code here
            ControlBits = cControlBits;
            m_multiwayDemuxGates = new MultiwayDemuxGate[Size];
            for (int i = 0; i < Size; i++)
            {
                var multiwayDemuxGate = new MultiwayDemuxGate(ControlBits);
                m_multiwayDemuxGates[i] = multiwayDemuxGate;

                multiwayDemuxGate.ConnectInput(Input[i]);
                multiwayDemuxGate.ConnectControl(Control);
                for (int j = 0; j < Outputs.Length; j++)
                {
                    Outputs[j][i].ConnectInput(multiwayDemuxGate.Outputs[j]);
                }
            }
        }


        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }
        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }


        public override bool TestGate()
        {
            int inputPermutationsCount = (int)Math.Pow(2, Size);
            for (int input = 0; input < inputPermutationsCount; input++)
            {
                if (!TestGate(input))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TestGate(int input)
        {
            Input.SetValue(input);
            for (int control = 0; control < Outputs.Length; control++)
            {
                Control.SetValue(control);
                int[,] expected = DoMultiwayDemux();
                if (!TestOutput(expected))
                {
                    return false;
                }
            }

            return true;
        }

        private int[,] DoMultiwayDemux()
        {
            int iOutput = Control.GetValue();
            int[,] expected = new int[Outputs.Length, Size];
            for (int i = 0; i < Input.Size; i++)
            {
                expected[iOutput, i] = Input[i].Value;
            }

            return expected;
        }

        private bool TestOutput(int[,] expected)
        {
            for (int i = 0; i < Outputs.Length; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Outputs[i][j].Value != expected[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
