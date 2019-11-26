using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class represents an n bit register that can maintain an n bit number
    class MultiBitRegister : Gate
    {
        public WireSet Input { get; private set; }
        public WireSet Output { get; private set; }
        //A bit setting the register operation to read or write
        public Wire Load { get; private set; }

        //Word size - number of bits in the register
        public int Size { get; private set; }

        private SingleBitRegister[] m_registers;

        public MultiBitRegister(int iSize)
        {
            Size = iSize;
            Input = new WireSet(Size);
            Output = new WireSet(Size);
            Load = new Wire();
            //your code here

            m_registers = new SingleBitRegister[Size];
            for (int i = 0; i < Size; i++)
            {
                var register = new SingleBitRegister();
                m_registers[i] = register;

                register.ConnectInput(Input[i]);
                Output[i].ConnectInput(register.Output);
                register.ConnectLoad(Load);
            }
        }

        public void ConnectInput(WireSet wsInput)
        {
            Input.ConnectInput(wsInput);
        }

        
        public override string ToString()
        {
            return Output.ToString();
        }


        public override bool TestGate()
        {
            int maxInput = (int)Math.Pow(2, Size);
            for (int i = 0; i < maxInput; i++)
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
    }
}
