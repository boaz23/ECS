using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class represents a set of n wires (a cable)
    class WireSet
    {
        //Word size - number of bits in the register
        public int Size { get; private set; }
        
        public bool InputConected { get; private set; }

        //An indexer providing access to a single wire in the wireset
        public Wire this[int i]
        {
            get
            {
                return m_aWires[i];
            }
        }
        private Wire[] m_aWires;
        
        public WireSet(int iSize)
        {
            Size = iSize;
            InputConected = false;
            m_aWires = new Wire[iSize];
            for (int i = 0; i < m_aWires.Length; i++)
                m_aWires[i] = new Wire();
        }
        public override string ToString()
        {
            string s = "[";
            for (int i = m_aWires.Length - 1; i >= 0; i--)
                s += m_aWires[i].Value;
            s += "]";
            return s;
        }

        //Transform a positive integer value into binary and set the wires accordingly, with 0 being the LSB
        public void SetValue(int iValue)
        {
            if (iValue < 0)
            {
                throw new ArgumentException("value must be a non-negative integer.", nameof(iValue));
            }

            int[] bits = GetBits(iValue);
            SetValue(bits);
        }

        //Transform the binary code into a positive integer
        public int GetValue()
        {
            int exponent = 1;
            int value = 0;
            for (int i = 0; i < Size; i++)
            {
                value += exponent * m_aWires[i].Value;
                exponent *= 2;
            }

            return value;
        }

        //Transform an integer value into binary using 2`s complement and set the wires accordingly, with 0 being the LSB
        public void Set2sComplement(int iValue)
        {
            int[] bits = GetBits(iValue);
            if (iValue < 0)
            {
                BitwiseNot(bits);
                Increment(bits);
            }
            SetValue(bits);
        }

        //Transform the binary code in 2`s complement into an integer
        public int Get2sComplement()
        {
            int exponent = 1;
            int value = 0;
            for (int i = 0; i < Size; i++)
            {
                value += exponent * m_aWires[i].Value;
                exponent *= 2;
            }

            // if is negative
            if (m_aWires[Size - 1].Value == 1)
            {
                value = -(exponent - value);
            }

            return value;
        }

        public void ConnectInput(WireSet wIn)
        {
            if (InputConected)
                throw new InvalidOperationException("Cannot connect a wire to more than one inputs");
            if(wIn.Size != Size)
                throw new InvalidOperationException("Cannot connect two wiresets of different sizes.");
            for (int i = 0; i < m_aWires.Length; i++)
                m_aWires[i].ConnectInput(wIn[i]);

            InputConected = true;
            
        }

        private void SetValue(int[] bits)
        {
            for (int i = 0; i < Size && i < bits.Length; i++)
            {
                m_aWires[i].Value = bits[i];
            }
        }

        private int[] GetBits(int iValue)
        {
            int[] bits = new int[Size];
            for (int i = 0; i < Size && iValue != 0; i++)
            {
                bits[i] = Math.Abs(iValue % 2);
                iValue /= 2;
            }

            return bits;
        }

        private static void BitwiseNot(int[] bits)
        {
            for (int i = 0; i < bits.Length; i++)
            {
                bits[i] = 1 - bits[i];
            }
        }

        private static void Increment(int[] bits)
        {
            int i;
            for (i = 0; i < bits.Length && bits[i] != 0; i++)
            {
                bits[i] = 0;
            }

            if (i < bits.Length)
            {
                bits[i] = 1;
            }
        }
    }
}
