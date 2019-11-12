using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements an adder, receving as input two n bit numbers, and outputing the sum of the two numbers
    class MultiBitAdder : Gate
    {
        //Word size - number of bits in each input
        public int Size { get; private set; }

        public WireSet Input1 { get; private set; }
        public WireSet Input2 { get; private set; }
        public WireSet Output { get; private set; }
        //An overflow bit for the summation computation
        public Wire Overflow { get; private set; }

        private HalfAdder m_firstAdder;
        private FullAdder[] m_fullAdders;

        public MultiBitAdder(int iSize)
        {
            Size = iSize;
            Input1 = new WireSet(Size);
            Input2 = new WireSet(Size);
            Output = new WireSet(Size);
            //your code here

            Overflow = new Wire();

            var firstAdder = new HalfAdder();
            m_firstAdder = firstAdder;
            firstAdder.ConnectInput1(Input1[0]);
            firstAdder.ConnectInput2(Input2[0]);
            Output[0].ConnectInput(firstAdder.Output);

            var prevCout = firstAdder.CarryOutput;
            m_fullAdders = new FullAdder[iSize - 1];
            for (int i = 1; i < iSize; i++)
            {
                var adder = new FullAdder();
                m_fullAdders[i - 1] = adder;
                adder.ConnectInput1(Input1[i]);
                adder.ConnectInput2(Input2[i]);
                adder.CarryInput.ConnectInput(prevCout);
                Output[i].ConnectInput(adder.Output);
                prevCout = adder.CarryOutput;
            }

            Overflow.ConnectInput(prevCout);
        }

        public override string ToString()
        {
            return Input1 + "(" + Input1.Get2sComplement() + ")" + " + " + Input2 + "(" + Input2.Get2sComplement() + ")" + " = " + Output + "(" + Output.Get2sComplement() + ")";
        }

        public void ConnectInput1(WireSet wInput)
        {
            Input1.ConnectInput(wInput);
        }
        public void ConnectInput2(WireSet wInput)
        {
            Input2.ConnectInput(wInput);
        }


        public override bool TestGate()
        {
            return TestGate(new int[Size], new int[Size]);
        }

        private bool TestGate(int[] a, int[] b)
        {
            Func<bool> test = () =>
            {
                return SetValuesAndTest(a, b);
            };
            Func<bool> doPermutationsB = () => DoPermutations(b, 0, test);
            return DoPermutations(a, 0, doPermutationsB);
        }

        private bool SetValuesAndTest(int[] a, int[] b)
        {
            SetInputValues(a, Input1);
            SetInputValues(b, Input2);
            int cout;
            int[] sum = Add(a, b, out cout);
            return TestOutput(sum, cout);
        }

        private bool DoPermutations(int[] input, int i, Func<bool> onFinish)
        {
            if (i == input.Length)
            {
                return onFinish();
            }
            else
            {
                input[i] = 0;
                if (!DoPermutations(input, i + 1, onFinish))
                {
                    return false;
                }

                input[i] = 1;
                if (!DoPermutations(input, i + 1, onFinish))
                {
                    return false;
                }

                return true;
            }
        }

        private void SetInputValues(int[] input, WireSet inputWs)
        {
            for (int i = 0; i < Size; i++)
            {
                inputWs[i].Value = input[i];
            }
        }

        private bool TestOutput(int[] sum, int overflow)
        {
            if (Overflow.Value != overflow)
            {
                return false;
            }

            for (int i = 0; i < Size; i++)
            {
                if (Output[i].Value != sum[i])
                {
                    return false;
                }
            }

            return true;
        }

        private int[] Add(int[] a, int[] b, out int cout)
        {
            int[] sum = new int[Size];
            int c = 0;

            sum[0] = Add(a[0], b[0], ref c);
            for (int i = 1; i < Size; i++)
            {
                sum[i] = Add(a[i], b[i], ref c);
            }

            cout = c;
            return sum;
        }

        private static int Add(int a, int b, ref int c)
        {
            int s = a + b + c;
            if (s <= 1)
            {
                c = 0;
            }
            else
            {
                c = 1;
                s -= 2;
            }

            return s;
        }
    }
}
