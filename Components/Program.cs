﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Stopwatch stopwatch = Stopwatch.StartNew();

            TestGates();
            //TestWireSet();

            stopwatch.Stop();
            Console.WriteLine("Tests time: {0}", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("done");
        }

        private static void TestGates()
        {
            //TestGate<AndGate>();
            //TestGate<OrGate>();
            //TestGate<XorGate>();
            //TestGate(new MultiBitAndGate(5));
            //TestGate(new MultiBitOrGate(5));

            //TestGate<MuxGate>();
            //TestGate<Demux>();
            //TestGate(new BitwiseAndGate(5));
            //TestGate(new BitwiseOrGate(5));
            //TestGate(new BitwiseNotGate(5), false);
            //TestGate(new BitwiseMux(5));
            //TestGate(new BitwiseDemux(5));
            //TestGate(new MultiwayMuxGate(3));
            //TestGate(new BitwiseMultiwayMux(2, 3));
            //TestGate(new MultiwayDemuxGate(3));
            //TestGate(new BitwiseMultiwayDemux(2, 3));

            //TestGate<HalfAdder>();
            //TestGate<FullAdder>();
            //TestGate(new MultiBitAdder(4));
            //TestGate(new ALU(4));

            TestGate<SingleBitRegister>();
            TestGate(new MultiBitRegister(4));
            TestGate(new Memory(3, 4));
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

        private static void TestGate(SequentialGate gate)
        {
            if (!gate.TestGate())
            {
                Console.WriteLine("bugbug");
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
            TestWireSet_2sComplement(wireSet, -8);
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
