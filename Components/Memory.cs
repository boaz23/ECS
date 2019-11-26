using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a memory unit, containing k registers, each of size n bits.
    class Memory : SequentialGate
    {
        //The address size determines the number of registers
        public int AddressSize { get; private set; }
        //The word size determines the number of bits in each register
        public int WordSize { get; private set; }

        //Data input and output - a number with n bits
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        //The address of the active register
        public WireSet Address { get; private set; }
        //A bit setting the memory operation to read or write
        public Wire Load { get; private set; }

        //your code here

        private BitwiseMultiwayMux m_outputMux;
        private MultiwayDemuxGate m_addressDemux;
        private MultiBitRegister[] m_registers;

        public Memory(int iAddressSize, int iWordSize)
        {
            AddressSize = iAddressSize;
            WordSize = iWordSize;

            Input = new WireSet(WordSize);
            Output = new WireSet(WordSize);
            Address = new WireSet(AddressSize);
            Load = new Wire();

            //your code here
            ConnectWritingDemux();
            ConnectReadingMux();
            ConnectRegisters();
        }

        private void ConnectRegisters()
        {
            m_registers = new MultiBitRegister[(int)Math.Pow(2, AddressSize)];
            for (int i = 0; i < m_registers.Length; i++)
            {
                var register = new MultiBitRegister(WordSize);
                m_registers[i] = register;
                register.ConnectInput(Input);
                register.Load.ConnectInput(m_addressDemux.Outputs[i]);
                m_outputMux.Inputs[i].ConnectInput(register.Output);
            }
        }

        private void ConnectReadingMux()
        {
            m_outputMux = new BitwiseMultiwayMux(WordSize, AddressSize);
            m_outputMux.ConnectControl(Address);
            Output.ConnectInput(m_outputMux.Output);
        }

        private void ConnectWritingDemux()
        {
            m_addressDemux = new MultiwayDemuxGate(AddressSize);
            m_addressDemux.ConnectControl(Address);
            m_addressDemux.ConnectInput(Load);
        }

        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }
        public void ConnectAddress(WireSet wsAddress)
        {
            Address.ConnectInput(wsAddress);
        }


        public override void OnClockUp()
        {
        }

        public override void OnClockDown()
        {
        }

        public override string ToString()
        {
            return Output.ToString();
        }

        public override bool TestGate()
        {
            int maxInputValue = (int)Math.Pow(2, WordSize);
            for (int i = 0; i < m_registers.Length; i++)
            {
                Address.SetValue(i);
                ZeroOutRegister();
                if (!TestRegister(maxInputValue))
                {
                    return false;
                }
                ZeroOutRegister();
            }

            return true;
        }

        private bool TestRegister(int maxInputValue)
        {
            for (int i = 0; i < maxInputValue; i++)
            {
                Load.Value = 1;
                Input.SetValue(i);
                Clock.ClockDown();
                Clock.ClockUp();
                Input.SetValue(i + 1);
                if (Output.GetValue() != i)
                {
                    return false;
                }

                Load.Value = 0;
                Clock.ClockDown();
                Clock.ClockUp();
                if (Output.GetValue() != i)
                {
                    return false;
                }
            }

            return true;
        }

        private void ZeroOutRegister()
        {
            Load.Value = 1;
            Input.SetValue(0);
            Clock.ClockDown();
            Clock.ClockUp();
        }
    }
}
