using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    abstract class BitwiseDominantTwoInputGateBase : BitwiseTwoInputGate
    {
        private readonly TwoInputGate[] m_gates;

        protected int DominantValue { get; }

        public BitwiseDominantTwoInputGateBase(int iSize, int dominantValue)
            : base(iSize)
        {
            DominantValue = dominantValue;

            m_gates = new TwoInputGate[iSize];
            for (int i = 0; i < iSize; i++)
            {
                var gate = CreateGate();
                m_gates[i] = gate;
                gate.ConnectInput1(Input1[i]);
                gate.ConnectInput2(Input2[i]);

                Output[i].ConnectInput(gate.Output);
            }
        }

        protected abstract TwoInputGate CreateGate();

        public override bool TestGate()
        {
            return DoPermutations(new int[Size], 0, new int[Size], 0);
        }

        private bool DoPermutations(int[] input1, int iWire1, int[] input2, int iWire2)
        {
            if (iWire1 == Size && iWire2 == Size)
            {
                SetWireValues(input1, input2);
                int[] expected = BitwiseOp(input1, input2);
                return TestOutput(expected);
            }
            else if (iWire1 < Size)
            {
                input1[iWire1] = 0;
                if (!DoPermutations(input1, iWire1 + 1, input2, iWire2))
                {
                    return false;
                }

                input1[iWire1] = 1;
                if (!DoPermutations(input1, iWire1 + 1, input2, iWire2))
                {
                    return false;
                }

                ResetWire(input1, iWire1);
                return true;
            }
            else // iWire2 < Size
            {
                input2[iWire2] = 0;
                if (!DoPermutations(input1, iWire1, input2, iWire2 + 1))
                {
                    return false;
                }

                input2[iWire2] = 1;
                if (!DoPermutations(input1, iWire1, input2, iWire2 + 1))
                {
                    return false;
                }

                ResetWire(input2, iWire2);
                return true;
            }
        }

        private static void ResetWire(int[] inputs, int wireIndex)
        {
            inputs[wireIndex] = default(int);
        }

        private void SetWireValues(int[] input1, int[] input2)
        {
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = input1[i];
                Input2[i].Value = input2[i];
            }
        }

        private int[] BitwiseOp(int[] input1, int[] input2)
        {
            int[] result = new int[Size];
            for (int i = 0; i < Size; i++)
            {
                result[i] = Op(input1[i], input2[i]);
            }

            return result;
        }

        private int Op(int x, int y)
        {
            return x == DominantValue || y == DominantValue ? DominantValue : (1 - DominantValue);
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
