using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A bitwise gate takes as input WireSets containing n wires, and computes a bitwise function - z_i=f(x_i)
    class BitwiseMux : BitwiseTwoInputGate
    {
        public Wire ControlInput { get; private set; }

        //your code here
        private readonly MuxGate[] m_gates;

        public BitwiseMux(int iSize)
            : base(iSize)
        {

            ControlInput = new Wire();
            //your code here

            m_gates = new MuxGate[Size];
            for (int i = 0; i < Size; i++)
            {
                var gate = new MuxGate();
                m_gates[i] = gate;

                gate.ConnectControl(ControlInput);
                gate.ConnectInput1(Input1[i]);
                gate.ConnectInput2(Input2[i]);
                Output[i].ConnectInput(gate.Output);
            }
        }

        public void ConnectControl(Wire wControl)
        {
            ControlInput.ConnectInput(wControl);
        }



        public override string ToString()
        {
            return "Mux " + Input1 + "," + Input2 + ",C" + ControlInput.Value + " -> " + Output;
        }





        public override bool TestGate()
        {
            return DoPermutations(0, new int[Size], 0, new int[Size], 0) &&
                   DoPermutations(1, new int[Size], 0, new int[Size], 0);
        }

        private bool DoPermutations(int control, int[] input1, int iWire1, int[] input2, int iWire2)
        {
            if (iWire1 == Size && iWire2 == Size)
            {
                SetWiresValues(control, input1, input2);
                int[] expected = BitwiseOp(control, input1, input2);
                return TestOutput(expected);
            }
            else if (iWire1 < Size)
            {
                input1[iWire1] = 0;
                if (!DoPermutations(control, input1, iWire1 + 1, input2, iWire2))
                {
                    return false;
                }

                input1[iWire1] = 1;
                if (!DoPermutations(control, input1, iWire1 + 1, input2, iWire2))
                {
                    return false;
                }

                ResetWire(input1, iWire1);
                return true;
            }
            else // iWire2 < Size
            {
                input2[iWire2] = 0;
                if (!DoPermutations(control, input1, iWire1, input2, iWire2 + 1))
                {
                    return false;
                }

                input2[iWire2] = 1;
                if (!DoPermutations(control, input1, iWire1, input2, iWire2 + 1))
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

        private void SetWiresValues(int control, int[] input1, int[] input2)
        {
            ControlInput.Value = control;
            for (int i = 0; i < Size; i++)
            {
                Input1[i].Value = input1[i];
                Input2[i].Value = input2[i];
            }
        }

        private int[] BitwiseOp(int control, int[] input1, int[] input2)
        {
            int[] result = new int[Size];
            for (int i = 0; i < Size; i++)
            {
                result[i] = Op(control, input1[i], input2[i]);
            }

            return result;
        }

        private int Op(int control, int x, int y)
        {
            return control == 0 ? x : y;
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
