using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class Program
    {
        static void Main(string[] args)
        {
            //This is an example of a testing code that you should run for all the gates that you create

            //Create a gate
            TestGate(new MultiBitOrGate(4));
        }

        private static void TestGate<T>() where T : Gate, new()
        {
            TestGate(new T());
        }

        private static void TestGate(Gate gate)
        {
            //Test that the unit testing works properly
            if (!gate.TestGate())
                Console.WriteLine("bugbug");

            //Now we ruin the nand gates that are used in all other gates. The gate should not work properly after this.
            NAndGate.Corrupt = true;
            if (gate.TestGate())
                Console.WriteLine("bugbug");


            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
