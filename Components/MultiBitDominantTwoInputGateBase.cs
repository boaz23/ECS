using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    abstract class MultiBitDominantTwoInputGateBase : MultiBitGate
    {
        private TwoInputGate[] m_gates;

        protected int DominantValue { get; }

        public MultiBitDominantTwoInputGateBase(int iInputCount, int dominantValue)
            : base(iInputCount)
        {
            //your code here
            int n = iInputCount;
            DominantValue = dominantValue;
            m_gates = new TwoInputGate[n - 1];

            TwoInputGate gate = CreateGate();
            TwoInputGate prevGate = gate;

            m_gates[0] = gate;
            gate.ConnectInput1(m_wsInput[0]);
            gate.ConnectInput2(m_wsInput[1]);

            for (int i = 2; i < n; i++)
            {
                gate = CreateGate();
                gate.ConnectInput1(prevGate.Output);
                gate.ConnectInput2(m_wsInput[i]);

                m_gates[i - 1] = gate;
                prevGate = gate;
            }

            Output = prevGate.Output;
        }

        public override bool TestGate()
        {
            return DoPermutations(new int[m_wsInput.Size], 0);
        }

        protected abstract TwoInputGate CreateGate();

        private bool DoPermutations(int[] inputs, int wireIndex)
        {
            if (wireIndex == inputs.Length)
            {
                SetWireValues(inputs);
                if (!TestOutput(inputs))
                {
                    return false;
                }

                return true;
            }
            else
            {
                return DoWire(inputs, wireIndex);
            }
        }

        private void SetWireValues(int[] inputs)
        {
            for (int i = 0; i < m_wsInput.Size; i++)
            {
                m_wsInput[i].Value = inputs[i];
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

        private bool TestOutput(int[] inputs)
        {
            return Output.Value == MultiAnd(inputs);
        }

        private int MultiAnd(int[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == DominantValue)
                {
                    return DominantValue;
                }
            }

            return 1 - DominantValue;
        }
    }
}
