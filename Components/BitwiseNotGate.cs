using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This bitwise gate takes as input one WireSet containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseNotGate : Gate
    {
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        public int Size { get; private set; }

        //your code here
        private NotGate[] m_gates;

        public BitwiseNotGate(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);
            //your code here

            m_gates = new NotGate[iSize];
            for (int i = 0; i < iSize; i++)
            {
                var gate = new NotGate();
                m_gates[i] = gate;

                gate.ConnectInput(Input[i]);
                Output[i].ConnectInput(gate.Output);
            }
        }

        public void ConnectInput(WireSet ws)
        {
            Input.ConnectInput(ws);
        }

        //an implementation of the ToString method is called, e.g. when we use Console.WriteLine(not)
        //this is very helpful during debugging
        public override string ToString()
        {
            return "Not " + Input + " -> " + Output;
        }

        public override bool TestGate()
        {
            return DoPermutations(new int[Size], 0);
        }

        private bool DoPermutations(int[] inputs, int wireIndex)
        {
            if (wireIndex == inputs.Length)
            {
                SetWireValues(inputs);
                int[] expected = BitwiseNot(inputs);
                bool result = TestOutput(expected);
                return result;
            }
            else
            {
                return DoWire(inputs, wireIndex);
            }
        }

        private void SetWireValues(int[] inputs)
        {
            for (int i = 0; i < Size; i++)
            {
                Input[i].Value = inputs[i];
            }
        }

        private bool DoWire(int[] inputs, int wireIndex)
        {
            if (!DoWire(inputs, wireIndex, 0))
            {
                return false;
            }
            if (!DoWire(inputs, wireIndex, 1))
            {
                return false;
            }

            ResetWire(inputs, wireIndex);
            return true;
        }

        private bool DoWire(int[] inputs, int wireIndex, int value)
        {
            inputs[wireIndex] = value;
            return DoPermutations(inputs, wireIndex + 1);
        }

        private static void ResetWire(int[] inputs, int wireIndex)
        {
            inputs[wireIndex] = default(int);
        }

        private int[] BitwiseNot(int[] input)
        {
            int[] result = new int[Size];
            for (int i = 0; i < Size; i++)
            {
                result[i] = Not(input[i]);
            }

            return result;
        }

        private int Not(int x)
        {
            return x == 0 ? 1 : 0;
        }

        private bool TestOutput(int[] expected)
        {
            for (int i = 0; i < Size; i++)
            {
                if (Output[i].Value != expected[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
