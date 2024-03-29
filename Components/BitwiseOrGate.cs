﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    //A two input bitwise gate takes as input two WireSets containing n wires, and computes a bitwise function - z_i=f(x_i,y_i)
    class BitwiseOrGate : BitwiseDominantTwoInputGateBase
    {
        //your code here

        public BitwiseOrGate(int iSize)
            : base(iSize, 1)
        {
            //your code here
        }

        //an implementation of the ToString method is called, e.g. when we use Console.WriteLine(or)
        //this is very helpful during debugging
        public override string ToString()
        {
            return "Or " + Input1 + ", " + Input2 + " -> " + Output;
        }

        protected override TwoInputGate CreateGate()
        {
            return new OrGate();
        }
    }
}
