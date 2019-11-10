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
            TestGate<AndGate>();
            TestGate<OrGate>();
            TestGate<XorGate>();
            TestGate(new MultiBitAndGate(5));
            TestGate(new MultiBitOrGate(5));

            TestGate<MuxGate>();
            TestGate<Demux>();
            TestGate(new BitwiseAndGate(5));
            TestGate(new BitwiseOrGate(5));
            TestGate(new BitwiseNotGate(5), false);
            TestGate(new BitwiseMux(5));
            TestGate(new BitwiseDemux(5));
            TestGate(new MultiwayMuxGate(3));
            TestGate(new BitwiseMultiwayMux(2, 2));
            TestGate(new MultiwayDemuxGate(3));
            TestGate(new BitwiseMultiwayDemux(2, 3));

            TestWireSet();

            Console.WriteLine("done");
        }

        private static void TestGate<T>() where T : Gate, new()
        {
            TestGate(new T());
        }

        private static void TestGate(Gate gate, bool testCorruptedNand = true)
        {
            //Test that the unit testing works properly
            if (!gate.TestGate())
                Console.WriteLine("bugbug");

            if (testCorruptedNand)
            {
                //Now we ruin the nand gates that are used in all other gates. The gate should not work properly after this.
                NAndGate.Corrupt = true;
                if (gate.TestGate())
                    Console.WriteLine("bugbug");

                NAndGate.Corrupt = false;
            }
        }

        private static void TestWireSet()
        {
            var wireSet = new WireSet(4);

            TestWireSet_Value(wireSet, 11);
            TestWireSet_Value(wireSet, 0);
            TestWireSet_2sComplement(wireSet, -5);
            TestWireSet_2sComplement(wireSet, 5);
            TestWireSet_2sComplement(wireSet, -7);
            TestWireSet_2sComplement(wireSet, 6);
        }

        private static void TestWireSet_Value(WireSet wireSet, int value)
        {
            wireSet.SetValue(value);
            if (value != wireSet.GetValue())
            {
                Console.WriteLine("bugbug wireset");
            }
        }

        private static void TestWireSet_2sComplement(WireSet wireSet, int value)
        {
            wireSet.Set2sComplement(value);
            if (value != wireSet.Get2sComplement())
            {
                Console.WriteLine("bugbug wireset");
            }
        }

        internal static string ArrayToString<T>(T[] arr)
        {
            return $"{{{string.Join(", ", arr)}}}";
        }

        internal static void PrintArray<T>(T[] arr)
        {
            Console.WriteLine(ArrayToString(arr));
        }
    }
}
