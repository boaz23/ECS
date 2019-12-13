using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleComponents;

namespace Machine
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            const string FilePath = @"D:\Programming - University\Electronic Computing Systems\HackMachine\Assembly examples\Copy1To0.hack";

            Machine16 machine = new Machine16(false, false);
            machine.Code.LoadFromFile(FilePath);
            int linesCount = File.ReadAllLines(FilePath).Length;

            machine.Data[0] = 100;
            machine.Data[1] = 15;
            DateTime dtStart = DateTime.Now;
            machine.Reset();
            for (int i = 0; i < linesCount; i++)
            {
                machine.CPU.PrintState();
                Console.WriteLine();
                Clock.ClockDown();
                Clock.ClockUp();
            }


            Console.WriteLine(machine.Data[0]);
            Console.WriteLine("Done " + (DateTime.Now - dtStart).TotalSeconds);
            Console.ReadLine();
        }
    }
}
