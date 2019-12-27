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
            const string FilePath = @"Assembly examples\Product.hack";

            Machine16 machine = new Machine16(bUseKeyboard: false, bUseScreen: true);
            machine.Code.LoadFromFile(FilePath);
            Func<bool>  whileCondition = GetRunningCondition(FilePath, machine);

            machine.Data[0] = 9;
            machine.Data[1] = 7;
            for (int i = 100; i <= 200; i++)
            {
                machine.Data[i] = 5 + i;
            }

            DateTime dtStart = DateTime.Now;
            machine.Reset();

            int consecutiveZeros = 0;
            while (whileCondition() && !ReadConsecutiveZeros(machine.Code, machine.CPU.PC, ref consecutiveZeros))
            {
                machine.CPU.PrintState();
                Console.WriteLine();
                Clock.ClockDown();
                Clock.ClockUp();
            }

            Console.WriteLine(machine.Data[2]);
            Console.WriteLine("Done " + (DateTime.Now - dtStart).TotalSeconds);
            Console.ReadLine();
        }

        private static Func<bool> GetRunningCondition(string FilePath, Machine16 machine)
        {
            Func<bool> whileCondition;
            int lastInstruction = FindLastInstruction(machine.Code);
            bool isLastInstructionJump = IsLastInstructionJump(machine.Code, lastInstruction);
            if (isLastInstructionJump)
            {
                whileCondition = () => !(machine.CPU.PC == lastInstruction && machine.CPU.AReg == lastInstruction - 1);
            }
            else
            {
                whileCondition = () => machine.CPU.PC <= lastInstruction;
            }

            return whileCondition;
        }

        private static bool IsLastInstructionJump(Memory code, int lastInstruction)
        {
            int instruction = code[lastInstruction] & 0b1001111111111111;
            return (instruction & 0b1000000000000111) == 0b1000000000000111;
        }

        private static int FindLastInstruction(Memory code)
        {
            int consecutiveZeros = 0;
            for (int i = 0; i < code.Size; i++)
            {
                if (ReadConsecutiveZeros(code, i, ref consecutiveZeros))
                {
                    return i - consecutiveZeros;
                }
            }

            return code.Size - 1;
        }

        private static bool ReadConsecutiveZeros(Memory code, int i, ref int counter)
        {
            if (code[i] == 0)
            {
                counter++;
                if (counter == 10)
                {
                    return true;
                }
            }
            else
            {
                counter = 0;
            }

            return false;
        }
    }
}
