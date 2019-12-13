using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleComponents;

namespace Machine
{
    public class CPU16 
    {
        //this "enum" defines the different control bits names
        public const int J3 = 0, J2 = 1, J1 = 2, D3 = 3, D2 = 4, D1 = 5, C6 = 6, C5 = 7, C4 = 8, C3 = 9, C2 = 10, C1 = 11, A = 12, X2 = 13, X1 = 14, Type = 15;

        public int Size { get; private set; }

        //CPU inputs
        public WireSet Instruction { get; private set; }
        public WireSet MemoryInput { get; private set; }
        public Wire Reset { get; private set; }

        //CPU outputs
        public WireSet MemoryOutput { get; private set; }
        public Wire MemoryWrite { get; private set; }
        public WireSet MemoryAddress { get; private set; }
        public WireSet InstructionAddress { get; private set; }

        //CPU components
        private ALU m_gALU;
        private Counter m_rPC;
        private MultiBitRegister m_rA, m_rD;
        private BitwiseMux m_gAMux, m_gMAMux;

        //here we initialize and connect all the components, as in Figure 5.9 in the book
        public CPU16()
        {
            Size =  16;

            Instruction = new WireSet(Size);
            MemoryInput = new WireSet(Size);
            MemoryOutput = new WireSet(Size);
            MemoryAddress = new WireSet(Size);
            InstructionAddress = new WireSet(Size);
            MemoryWrite = new Wire();
            Reset = new Wire();

            m_gALU = new ALU(Size);
            m_rPC = new Counter(Size);
            m_rA = new MultiBitRegister(Size);
            m_rD = new MultiBitRegister(Size);

            m_gAMux = new BitwiseMux(Size);
            m_gMAMux = new BitwiseMux(Size);

            m_gAMux.ConnectInput1(Instruction);
            m_gAMux.ConnectInput2(m_gALU.Output);

            m_rA.ConnectInput(m_gAMux.Output);

            m_gMAMux.ConnectInput1(m_rA.Output);
            m_gMAMux.ConnectInput2(MemoryInput);
            m_gALU.InputY.ConnectInput(m_gMAMux.Output);

            m_gALU.InputX.ConnectInput(m_rD.Output);

            m_rD.ConnectInput(m_gALU.Output);

            MemoryOutput.ConnectInput(m_gALU.Output);
            MemoryAddress.ConnectInput(m_rA.Output);

            InstructionAddress.ConnectInput(m_rPC.Output);
            m_rPC.ConnectInput(m_rA.Output);
            m_rPC.ConnectReset(Reset);

            //now, we call the code that creates the control unit
            ConnectControls();
        }

        //add here components to implement the control unit 
        private BitwiseMultiwayMux m_gJumpMux;//an example of a control unit compnent - a mux that controls whether a jump is made
        private WireSet m_cInstructionResetWs;
        private BitwiseAndGate m_gInstructionAnd;
        private BitwiseMux m_gInstructionMux;
        private NotGate m_gALoadInstructionNot;
        private OrGate m_gALoadOr;

        private void ConnectControls()
        {
            //1. connect control of mux 1 (selects entrance to register A)
            m_gAMux.ConnectControl(Instruction[Instruction.Size - 1]);

            //2. connect control to mux 2 (selects A or M entrance to the ALU)
            m_gMAMux.ConnectControl(Instruction[12]);

            //3. consider all instruction bits only if C type instruction (MSB of instruction is 1)
            m_cInstructionResetWs = new WireSet(Size);
            int[] cInstructionResetWsValues = new int[] { 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, };
            for (int i = 0; i < Size; i++)
            {
                m_cInstructionResetWs[i].Value = cInstructionResetWsValues[i];
            }

            m_gInstructionAnd = new BitwiseAndGate(Size);
            m_gInstructionAnd.Input1.ConnectInput(Instruction);
            m_gInstructionAnd.Input2.ConnectInput(m_cInstructionResetWs);
            m_gInstructionMux = new BitwiseMux(Size);
            m_gInstructionMux.Input1.ConnectInput(m_gInstructionAnd.Output);
            m_gInstructionMux.Input2.ConnectInput(Instruction);
            m_gInstructionMux.ConnectControl(Instruction[Instruction.Size - 1]);

            //4. connect ALU control bits
            m_gALU.ZeroX.ConnectInput(m_gInstructionMux.Output[11]);
            m_gALU.NotX.ConnectInput(m_gInstructionMux.Output[10]);
            m_gALU.ZeroY.ConnectInput(m_gInstructionMux.Output[9]);
            m_gALU.NotY.ConnectInput(m_gInstructionMux.Output[8]);
            m_gALU.F.ConnectInput(m_gInstructionMux.Output[7]);
            m_gALU.NotOutput.ConnectInput(m_gInstructionMux.Output[6]);

            //5. connect control to register D (very simple)
            m_rD.Load.ConnectInput(m_gInstructionMux.Output[4]);

            //6. connect control to register A (a bit more complicated)
            m_gALoadInstructionNot = new NotGate();
            m_gALoadInstructionNot.Input.ConnectInput(Instruction[Instruction.Size - 1]);
            m_gALoadOr = new OrGate();
            m_gALoadOr.Input1.ConnectInput(m_gALoadInstructionNot.Output);
            m_gALoadOr.Input2.ConnectInput(Instruction[5]);
            m_rA.Load.ConnectInput(m_gALoadOr.Output);

            //7. connect control to MemoryWrite
            MemoryWrite.ConnectInput(Instruction[3]);

            //8. create inputs for jump mux


            //9. connect jump mux (this is the most complicated part)
            m_gJumpMux = new BitwiseMultiwayMux(1, 3);
            
            //10. connect PC load control
        }

        public override string ToString()
        {
            return "A=" + m_rA + ", D=" + m_rD + ", PC=" + m_rPC + ",Ins=" + Instruction;
        }

        private string GetInstructionString()
        {
            if (Instruction[Type].Value == 0)
                return "@" + Instruction.GetValue();
            return Instruction[Type].Value + "XX " +
               "a" + Instruction[A] + " " +
               "c" + Instruction[C1] + Instruction[C2] + Instruction[C3] + Instruction[C4] + Instruction[C5] + Instruction[C6] + " " +
               "d" + Instruction[D1] + Instruction[D2] + Instruction[D3] + " " +
               "j" + Instruction[J1] + Instruction[J2] + Instruction[J3];
        }

        //use this function in debugging to print the current status of the ALU. Feel free to add more things for printing.
        public void PrintState()
        {
            Console.WriteLine("CPU state:");
            Console.WriteLine("PC=" + m_rPC + "=" + m_rPC.Output.GetValue());
            Console.WriteLine("A=" + m_rA + "=" + m_rA.Output.GetValue());
            Console.WriteLine("D=" + m_rD + "=" + m_rD.Output.GetValue());
            Console.WriteLine("Ins=" + GetInstructionString());
            Console.WriteLine("ALU=" + m_gALU);
            Console.WriteLine("inM=" + MemoryInput);
            Console.WriteLine("outM=" + MemoryOutput);
            Console.WriteLine("addM=" + MemoryAddress);
        }
    }
}
