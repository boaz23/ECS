using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //Multibit gates take as input k bits, and compute a function over all bits - z=f(x_0,x_1,...,x_k)

    class MultiBitOrGate : MultiBitGate
    {
        //your code here
        private OrGate[] m_orGates;

        public MultiBitOrGate(int iInputCount)
            : base(iInputCount)
        {
            //your code here

            int n = iInputCount;
            m_orGates = new OrGate[n - 1];
            OrGate orGate = new OrGate();
            OrGate prevOrGate = orGate;

            m_orGates[0] = orGate;
            orGate.ConnectInput1(m_wsInput[0]);
            orGate.ConnectInput2(m_wsInput[1]);

            for (int i = 2; i < n; i++)
            {
                orGate = new OrGate();
                orGate.ConnectInput1(prevOrGate.Output);
                orGate.ConnectInput2(m_wsInput[i]);

                m_orGates[i - 1] = orGate;
                prevOrGate = orGate;
            }

            Output = prevOrGate.Output;
        }

        public override bool TestGate()
        {
            return DoPermutations(new int[m_wsInput.Size], 0);
        }

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
            inputs[wireIndex] = 0;
        }

        private bool TestOutput(int[] inputs)
        {
            return Output.Value == MultiAnd(inputs);
        }

        private int MultiAnd(int[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == 1)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}
