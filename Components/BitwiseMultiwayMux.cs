﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //This class implements a mux with k input, each input with n wires. The output also has n wires.

    class BitwiseMultiwayMux : Gate
    {
        //Word size - number of bits in each output
        public int Size { get; private set; }

        //The number of control bits needed for k outputs
        public int ControlBits { get; private set; }

        public WireSet Output { get; private set; }
        public WireSet Control { get; private set; }
        public WireSet[] Inputs { get; private set; }

        //your code here
        private readonly MultiwayMuxGate[] m_multiwayMuxGates;

        public BitwiseMultiwayMux(int iSize, int cControlBits)
        {
            Size = iSize;
            Output = new WireSet(Size);
            Control = new WireSet(cControlBits);
            Inputs = new WireSet[(int)Math.Pow(2, cControlBits)];
            
            for (int i = 0; i < Inputs.Length; i++)
            {
                Inputs[i] = new WireSet(Size);
                
            }

            //your code here
            ControlBits = cControlBits;
            m_multiwayMuxGates = new MultiwayMuxGate[Size];
            for (int i = 0; i < Size; i++)
            {
                var multiwayMuxGate = new MultiwayMuxGate(ControlBits);
                m_multiwayMuxGates[i] = multiwayMuxGate;

                multiwayMuxGate.ConnectControl(Control);
                for (int j = 0; j < Inputs.Length; j++)
                {
                    multiwayMuxGate.ConnectInput(j, Inputs[j][i]);
                }
                Output[i].ConnectInput(multiwayMuxGate.Output);
            }
        }


        public void ConnectInput(int i, WireSet wsInput)
        {
            Inputs[i].ConnectInput(wsInput);
        }
        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }



        public override bool TestGate()
        {
            throw new NotImplementedException();
        }
    }
}
