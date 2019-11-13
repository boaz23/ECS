using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class is used to implement the ALU
    //The specification can be found at https://b1391bd6-da3d-477d-8c01-38cdf774495a.filesusr.com/ugd/56440f_2e6113c60ec34ed0bc2035c9d1313066.pdf slides 48,49
    class ALU : Gate
    {
        //The word size = number of bit in the input and output
        public int Size { get; private set; }

        //Input and output n bit numbers
        public WireSet InputX { get; private set; }
        public WireSet InputY { get; private set; }
        public WireSet Output { get; private set; }

        //Control bit 
        public Wire ZeroX { get; private set; }
        public Wire ZeroY { get; private set; }
        public Wire NotX { get; private set; }
        public Wire NotY { get; private set; }
        public Wire F { get; private set; }
        public Wire NotOutput { get; private set; }

        //Bit outputs
        public Wire Zero { get; private set; }
        public Wire Negative { get; private set; }


        //your code here
        private WireSet m_wsZero;

        private BitwiseMultiwayMux m_gZxMux;
        private BitwiseNotGate m_gNxNot;
        private BitwiseMultiwayMux m_gNxMux;

        private BitwiseMultiwayMux m_gZyMux;
        private BitwiseNotGate m_gNyNot;
        private BitwiseMultiwayMux m_gNyMux;

        private BitwiseMultiwayMux m_gFMux;
        private MultiBitAdder m_gFAdder;
        private BitwiseAndGate m_gFAnd;

        private BitwiseMultiwayMux m_gNoMux;
        private BitwiseNotGate m_gNoNot;

        private MultiBitOrGate m_gZrOr;
        private NotGate m_gZrNot;

        public ALU(int iSize)
        {
            Size = iSize;
            InputX = new WireSet(Size);
            InputY = new WireSet(Size);
            ZeroX = new Wire();
            ZeroY = new Wire();
            NotX = new Wire();
            NotY = new Wire();
            F = new Wire();
            NotOutput = new Wire();
            Negative = new Wire();
            Zero = new Wire();


            //Create and connect all the internal components
            Output = new WireSet(Size);

            m_wsZero = new WireSet(Size);
            for (int i = 0; i < Size; i++)
            {
                m_wsZero[i].Value = 0;
            }

            m_gZxMux = new BitwiseMultiwayMux(Size, 1);
            m_gZxMux.ConnectInput(0, InputX);
            m_gZxMux.ConnectInput(1, m_wsZero);
            m_gZxMux.Control[0].ConnectInput(ZeroX);

            m_gNxNot = new BitwiseNotGate(Size);
            m_gNxNot.ConnectInput(m_gZxMux.Output);
            m_gNxMux = new BitwiseMultiwayMux(Size, 1);
            m_gNxMux.ConnectInput(0, m_gZxMux.Output);
            m_gNxMux.ConnectInput(1, m_gNxNot.Output);
            m_gNxMux.Control[0].ConnectInput(NotX);

            m_gZyMux = new BitwiseMultiwayMux(Size, 1);
            m_gZyMux.ConnectInput(0, InputY);
            m_gZyMux.ConnectInput(1, m_wsZero);
            m_gZyMux.Control[0].ConnectInput(ZeroY);

            m_gNyNot = new BitwiseNotGate(Size);
            m_gNyNot.ConnectInput(m_gZyMux.Output);
            m_gNyMux = new BitwiseMultiwayMux(Size, 1);
            m_gNyMux.ConnectInput(0, m_gZyMux.Output);
            m_gNyMux.ConnectInput(1, m_gNyNot.Output);
            m_gNyMux.Control[0].ConnectInput(NotY);

            m_gFAdder = new MultiBitAdder(Size);
            m_gFAdder.ConnectInput1(m_gNxMux.Output);
            m_gFAdder.ConnectInput2(m_gNyMux.Output);
            m_gFAnd = new BitwiseAndGate(Size);
            m_gFAnd.ConnectInput1(m_gNxMux.Output);
            m_gFAnd.ConnectInput2(m_gNyMux.Output);
            m_gFMux = new BitwiseMultiwayMux(Size, 1);
            m_gFMux.ConnectInput(0, m_gFAnd.Output);
            m_gFMux.ConnectInput(1, m_gFAdder.Output);
            m_gFMux.Control[0].ConnectInput(F);

            m_gNoNot = new BitwiseNotGate(Size);
            m_gNoNot.ConnectInput(m_gFMux.Output);
            m_gNoMux = new BitwiseMultiwayMux(Size, 1);
            m_gNoMux.ConnectInput(0, m_gFMux.Output);
            m_gNoMux.ConnectInput(1, m_gNoNot.Output);
            m_gNoMux.Control[0].ConnectInput(NotOutput);

            Output.ConnectInput(m_gNoMux.Output);

            m_gZrOr = new MultiBitOrGate(Size);
            m_gZrOr.ConnectInput(Output);
            m_gZrNot = new NotGate();
            m_gZrNot.ConnectInput(m_gZrOr.Output);

            Zero.ConnectInput(m_gZrNot.Output);
            Negative.ConnectInput(Output[Size - 1]);
        }

        public override bool TestGate()
        {
            int wordBitsFilter = WordBitsFilter();
            var wsFlags = new WireSet(6);
            int inputPermutationsCount = (int)Math.Pow(2, Size);
            int flagsPermutationsCount = (int)Math.Pow(2, wsFlags.Size);
            for (int i = 0; i < inputPermutationsCount; i++)
            {
                InputX.SetValue(i);
                for (int j = 0; j < inputPermutationsCount; j++)
                {
                    InputY.SetValue(j);
                    for (int k = 0; k < flagsPermutationsCount; k++)
                    {
                        wsFlags.SetValue(k);
                        SetFlags(wsFlags);
                        bool test = Test(
                            wordBitsFilter,
                            InputX.Get2sComplement(),
                            InputY.Get2sComplement(),
                            ZeroX.Value,
                            NotX.Value,
                            ZeroY.Value,
                            NotY.Value,
                            F.Value,
                            NotOutput.Value
                        );
                        if (!test)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private int WordBitsFilter()
        {
            return (int)Math.Pow(2, Size) - 1;
        }

        private void SetFlags(WireSet wsFlags)
        {
            ZeroX.Value = wsFlags[0].Value;
            NotX.Value = wsFlags[1].Value;
            ZeroY.Value = wsFlags[2].Value;
            NotY.Value = wsFlags[3].Value;
            F.Value = wsFlags[4].Value;
            NotOutput.Value = wsFlags[5].Value;
        }

        private bool Test(int wordBitsFilter, int x, int y, int zx, int nx, int zy, int ny, int f, int no)
        {
            if (zx != 0)
            {
                x = 0;
            }
            if (nx != 0)
            {
                x = ~x;
            }
            if (zy != 0)
            {
                y = 0;
            }
            if (ny != 0)
            {
                y = ~y;
            }

            int output;
            if (f == 0)
            {
                output = x & y;
            }
            else
            {
                output = x + y;
            }

            if (no != 0)
            {
                output = ~output;
            }

            output = output & wordBitsFilter;
            var wsExpectedOutput = new WireSet(Size);
            wsExpectedOutput.Set2sComplement(output);

            int expectedZr = output == 0 ? 1 : 0;
            int expectedNg = wsExpectedOutput.Get2sComplement() < 0 ? 1 : 0;
            return TestOutput(wsExpectedOutput, expectedZr, expectedNg);
        }

        private bool TestOutput(WireSet expectedOutput, int expectedZr, int expectedNg)
        {
            if (Zero.Value != expectedZr)
            {
                return false;
            }
            if (Negative.Value != expectedNg)
            {
                return false;
            }

            for (int i = 0; i < Output.Size; i++)
            {
                if (Output[i].Value != expectedOutput[i].Value)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
