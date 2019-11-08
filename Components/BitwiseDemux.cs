using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A bitwise gate takes as input WireSets containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseDemux : Gate
    {
        public int Size { get; private set; }
        public WireSet Output1 { get; private set; }
        public WireSet Output2 { get; private set; }
        public WireSet Input { get; private set; }
        public Wire Control { get; private set; }

        //your code here
        private readonly Demux[] m_gates;

        public BitwiseDemux(int iSize)
        {
            Size = iSize;
            Control = new Wire();
            Input = new WireSet(Size);

            //your code here
            Output1 = new WireSet(Size);
            Output2 = new WireSet(Size);

            m_gates = new Demux[Size];
            for (int i = 0; i < Size; i++)
            {
                var gate = new Demux();
                m_gates[i] = gate;

                gate.ConnectControl(Control);
                gate.ConnectInput(Input[i]);
                Output1[i].ConnectInput(gate.Output1);
                Output2[i].ConnectInput(gate.Output2);
            }
        }

        public void ConnectControl(Wire wControl)
        {
            Control.ConnectInput(wControl);
        }
        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        public override bool TestGate()
        {
            return DoPermutations(0, new int[Size], 0) &&
                   DoPermutations(1, new int[Size], 0);
        }

        private bool DoPermutations(int control, int[] inputs, int wireIndex)
        {
            if (wireIndex == inputs.Length)
            {
                SetWireValues(control, inputs);
                var expected = BitwiseOp(control, inputs);
                bool result = TestOutput(expected);
                return result;
            }
            else
            {
                return DoWire(control, inputs, wireIndex);
            }
        }

        private void SetWireValues(int control, int[] inputs)
        {
            Control.Value = control;
            for (int i = 0; i < Size; i++)
            {
                Input[i].Value = inputs[i];
            }
        }

        private bool DoWire(int control, int[] inputs, int wireIndex)
        {
            if (!DoWire(control, inputs, wireIndex, 0))
            {
                return false;
            }
            if (!DoWire(control, inputs, wireIndex, 1))
            {
                return false;
            }

            ResetWire(inputs, wireIndex);
            return true;
        }

        private bool DoWire(int control, int[] inputs, int wireIndex, int value)
        {
            inputs[wireIndex] = value;
            return DoPermutations(control, inputs, wireIndex + 1);
        }

        private static void ResetWire(int[] inputs, int wireIndex)
        {
            inputs[wireIndex] = default(int);
        }

        private Expected[] BitwiseOp(int control, int[] input)
        {
            Expected[] result = new Expected[Size];
            for (int i = 0; i < Size; i++)
            {
                result[i] = Op(control, input[i]);
            }

            return result;
        }

        private static Expected Op(int control, int x)
        {
            if (control == 0)
            {
                return new Expected
                {
                    expected1 = x,
                    expected2 = 0
                };
            }
            else
            {
                return new Expected
                {
                    expected1 = 0,
                    expected2 = x
                };
            }
        }

        private bool TestOutput(Expected[] expected)
        {
            for (int i = 0; i < Size; i++)
            {
                if (Output1[i].Value != expected[i].expected1 ||
                    Output2[i].Value != expected[i].expected2)
                {
                    return false;
                }
            }

            return true;
        }

        struct Expected
        {
            public int expected1;
            public int expected2;

            public override string ToString()
            {
                return $"({expected1}, {expected2})";
            }
        }
    }
}
