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

        private const int SINGLE_BIT_SIZE = 1;
        private const int JUMP_CONTROLS_COUNT = 3;

        private const int NJ  = 0b000;
        private const int JGT = 0b001;
        private const int JEQ = 0b010;
        private const int JGE = 0b011;
        private const int JLT = 0b100;
        private const int JNE = 0b101;
        private const int JLE = 0b110;
        private const int JMP = 0b111;

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

        private WireSet m_wsCInstructionReset;
        private BitwiseAndGate m_gInstructionAnd;
        private BitwiseMux m_gInstructionMux;

        private NotGate m_gALoadInstructionNot;
        private OrGate m_gALoadOr;

        private Wire[] m_wJump;

        private NotGate m_gJGT_Not;
        private NotGate m_gJGE_Not;
        private NotGate m_gJNE_Not;
        private OrGate m_gJLE_Or;

        private void ConnectControls()
        {
            //1. connect control of mux 1 (selects entrance to register A)
            ChooseARegisterInput();

            //2. connect control to mux 2 (selects A or M entrance to the ALU)
            ChooseALU_AorM_Input();

            //3. consider all instruction bits only if C type instruction (MSB of instruction is 1)
            FilterInstructionBits();

            //4. connect ALU control bits
            ConnectALUControl();

            //5. connect control to register D (very simple)
            ConnectDRegisterLoad();

            //6. connect control to register A (a bit more complicated)
            ConnectARegisterLoad();

            //7. connect control to MemoryWrite
            ConnectMemoryWrite();

            //8. create inputs for jump mux
            ConnectJumpWires();

            //9. connect jump mux (this is the most complicated part)
            ConnectJumpMux();

            //10. connect PC load control
            ConnectPCLoad();
        }

        private void ChooseARegisterInput()
        {
            m_gAMux.ConnectControl(Instruction[Type]);
        }

        private void ChooseALU_AorM_Input()
        {
            m_gMAMux.ConnectControl(Instruction[A]);
        }

        private void FilterInstructionBits()
        {
            m_wsCInstructionReset = new WireSet(Size);
            int[] cInstructionResetWsValues = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
            for (int i = 0; i < Size; i++)
            {
                m_wsCInstructionReset[i].Value = cInstructionResetWsValues[i];
            }

            m_gInstructionAnd = new BitwiseAndGate(Size);
            m_gInstructionAnd.Input1.ConnectInput(Instruction);
            m_gInstructionAnd.Input2.ConnectInput(m_wsCInstructionReset);

            m_gInstructionMux = new BitwiseMux(Size);
            m_gInstructionMux.Input1.ConnectInput(m_gInstructionAnd.Output);
            m_gInstructionMux.Input2.ConnectInput(Instruction);
            m_gInstructionMux.ConnectControl(Instruction[Type]);
        }

        private void ConnectALUControl()
        {
            m_gALU.ZeroX.ConnectInput(Instruction[C1]);
            m_gALU.NotX.ConnectInput(Instruction[C2]);
            m_gALU.ZeroY.ConnectInput(Instruction[C3]);
            m_gALU.NotY.ConnectInput(Instruction[C4]);
            m_gALU.F.ConnectInput(Instruction[C5]);
            m_gALU.NotOutput.ConnectInput(Instruction[C6]);
        }

        private void ConnectDRegisterLoad()
        {
            m_rD.Load.ConnectInput(m_gInstructionMux.Output[D2]);
        }

        private void ConnectARegisterLoad()
        {
            m_gALoadInstructionNot = new NotGate();
            m_gALoadInstructionNot.Input.ConnectInput(Instruction[Type]);

            m_gALoadOr = new OrGate();
            m_gALoadOr.Input1.ConnectInput(m_gALoadInstructionNot.Output);
            m_gALoadOr.Input2.ConnectInput(Instruction[D1]);

            m_rA.Load.ConnectInput(m_gALoadOr.Output);
        }

        private void ConnectMemoryWrite()
        {
            MemoryWrite.ConnectInput(m_gInstructionMux.Output[D3]);
        }

        private void ConnectJumpWires()
        {
            InitializeJumpWireSets();
            ConnectJumpWire_NJ();
            ConnectJumpWire_JGT();
            ConnectJumpWire_JEQ();
            ConnectJumpWire_JGE();
            ConnectJumpWire_JLT();
            ConnectJumpWire_JNE();
            ConnectJumpWire_JLE();
            ConnectJumpWire_JMP();
        }

        private void InitializeJumpWireSets()
        {
            m_wJump = new Wire[(int)Math.Pow(2, JUMP_CONTROLS_COUNT)];
            for (int i = 0; i < m_wJump.Length; i++)
            {
                m_wJump[i] = new Wire();
            }
        }

        private void ConnectJumpWire_NJ()
        {
            m_wJump[NJ].Value = 0;
        }

        private void ConnectJumpWire_JGT()
        {
            m_gJLE_Or = new OrGate();
            m_gJLE_Or.Input1.ConnectInput(m_gALU.Zero);
            m_gJLE_Or.Input2.ConnectInput(m_gALU.Negative);

            m_gJGT_Not = new NotGate();
            m_gJGT_Not.ConnectInput(m_gJLE_Or.Output);

            m_wJump[JGT].ConnectInput(m_gJGT_Not.Output);
        }

        private void ConnectJumpWire_JEQ()
        {
            m_wJump[JEQ].ConnectInput(m_gALU.Zero);
        }

        private void ConnectJumpWire_JGE()
        {
            m_gJGE_Not = new NotGate();
            m_gJGE_Not.Input.ConnectInput(m_gALU.Negative);

            m_wJump[JGE].ConnectInput(m_gJGE_Not.Output);
        }

        private void ConnectJumpWire_JLT()
        {
            m_wJump[JLT].ConnectInput(m_gALU.Negative);
        }

        private void ConnectJumpWire_JNE()
        {
            m_gJNE_Not = new NotGate();
            m_gJNE_Not.Input.ConnectInput(m_gALU.Zero);

            m_wJump[JNE].ConnectInput(m_gJNE_Not.Output);
        }

        private void ConnectJumpWire_JLE()
        {
            m_wJump[JLE].ConnectInput(m_gJLE_Or.Output);
        }

        private void ConnectJumpWire_JMP()
        {
            m_wJump[JMP].Value = 1;
        }

        private void ConnectJumpMux()
        {
            InitializeJumpMux();
            ConnectJumpWiresToMux();
            ConnectJumpMuxControl();
        }

        private void InitializeJumpMux()
        {
            m_gJumpMux = new BitwiseMultiwayMux(SINGLE_BIT_SIZE, JUMP_CONTROLS_COUNT);
        }

        private void ConnectJumpWiresToMux()
        {
            for (int iJump = 0; iJump < m_wJump.Length; iJump++)
            {
                ConnectJumpWireToMux(iJump);
            }
        }

        private void ConnectJumpWireToMux(int iJump)
        {
            m_gJumpMux.Inputs[iJump][0].ConnectInput(m_wJump[iJump]);
        }

        private void ConnectJumpMuxControl()
        {
            m_gJumpMux.Control[0].ConnectInput(m_gInstructionMux.Output[J3]);
            m_gJumpMux.Control[1].ConnectInput(m_gInstructionMux.Output[J2]);
            m_gJumpMux.Control[2].ConnectInput(m_gInstructionMux.Output[J1]);
        }

        private void ConnectPCLoad()
        {
            m_rPC.ConnectLoad(m_gJumpMux.Output[0]);
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

        public int PC => m_rPC.Output.Value;
        public int AReg => m_rA.Output.Value;
    }
}
