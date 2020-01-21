using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCompiler
{
    class CPUEmulator
    {
        public int A { get; private set; }
        public int D {get; private set;}
        public int PC {get; private set;}
        public Dictionary<string,int> Registers { get; private set; }

        public Dictionary<string,int> Labels { get; private set; }
        public int[] M { get; private set; }

        public List<string> Code;
        public CPUEmulator()
        {
            M = new int[10000];

            Registers = new Dictionary<string, int>();
            Registers["SP"] = 0;
            Registers["LCL"] = 1;
            Registers["ARG"] = 2;
            Registers["GLOBAL"] = 3;
            Registers["RESULT"] = 4;
            Registers["OPERAND1"] = 5;
            Registers["OPERAND2"] = 6;
            Registers["ADDRESS"] = 7;
        }

        public void Run(int cSteps)
        {
            Labels = new Dictionary<string, int>();
            for (int i = 0; i < Code.Count; i++)
            {
                string sLine = Code[i].Trim().Replace(" ", "");
                if (sLine.StartsWith("("))
                    Labels[sLine.Substring(1, sLine.Length - 2)] = i + 1;
            }
            PC = 0;
            for(int i = 0; i < cSteps && PC < Code.Count;i++)
            {
                int iPreviousA = A;

                string sLine = Code[PC].Trim().Replace(" ", "");
                Console.WriteLine("A=" + A + ", D=" + D + ", M=" + M[A] + ", PC=" + PC + ", line=" + sLine);
                if (sLine.StartsWith("@"))
                {
                    string sValue = sLine.Substring(1);
                    int iValue = 0;
                    if (int.TryParse(sValue, out iValue))
                        A = iValue;
                    else
                    {
                        if (Registers.ContainsKey(sValue))
                            A = Registers[sValue];
                        else if (Labels.ContainsKey(sValue))
                            A = Labels[sValue];
                        else
                            throw new Exception("Not spporting symbol " + sValue);

                    }
                    PC++;
                }
                else if (sLine.StartsWith("(") || sLine.StartsWith("//"))
                {
                    PC++;
                }
                else 
                {

                    string sDest = "", sCompute = "", sJMP = "";
                    if (sLine.Contains("="))
                    {
                        sDest = sLine.Split('=')[0];
                        sLine = sLine.Substring(sDest.Length + 1);
                    }
                    if (sLine.Contains(";"))
                    {
                        sJMP = sLine.Split(';')[1];
                        sLine = sLine.Replace(";" + sJMP, "");
                    }
                    sCompute = sLine;

                    int iCompute = Compute(sCompute);
                    if (sDest.Contains("D"))
                        D = iCompute;
                    if (sDest.Contains("M"))
                        M[iPreviousA] = iCompute;
                    if (sDest.Contains("A"))
                        A = iCompute;

                    if (sJMP != "" && Jump(sJMP, iCompute))
                        PC = iPreviousA;
                    else
                        PC++;
                }
            }
        }

        private bool Jump(string sJMP, int iCompute)
        {
            if (sJMP == "JEQ" && iCompute == 0)
                return true;
            if (sJMP == "JGT" && iCompute > 0)
                return true;
            if (sJMP == "JLT" && iCompute < 0)
                return true;
            if (sJMP == "JGE" && iCompute >= 0)
                return true;
            if (sJMP == "JLE" && iCompute <= 0)
                return true;
            if (sJMP == "JNE" && iCompute != 0)
                return true;
            if (sJMP == "JMP")
                return true;
            return false;
        }

        private int Compute(string sCompute)
        {
            
            string sOperand1 = sCompute[0] + "";
            string sOperand2 = "";
            string sOperator ="";
            if (sCompute.Length > 1)
            {
                sOperand2 = sCompute[2] + "";
                sOperator = sCompute[1] + "";
            }
            int iOperand1 = 0;
            if (sOperand1 == "A")
                iOperand1 = A;
            if (sOperand1 == "D")
                iOperand1 = D;
            if (sOperand1 == "M")
                iOperand1 = M[A];
            if (sOperand1 == "1")
                iOperand1 = 1;
            if (sOperand1 == "0")
                iOperand1 = 0;

            if (sCompute.Length == 1)
                return iOperand1;

            int iOperand2 = 0;
            if (sOperand2 == "A")
                iOperand2 = A;
            if (sOperand2 == "D")
                iOperand2 = D;
            if (sOperand2 == "M")
                iOperand2 = M[A];
            if (sOperand2 == "1")
                iOperand2 = 1;
            if (sOperand2 == "0")
                iOperand2 = 0;

            if (sOperator == "+")
                return iOperand1 + iOperand2;
            if (sOperator == "-")
                return iOperand1 - iOperand2;
            if (sOperator == "&")
                return iOperand1 & iOperand2;
            if (sOperator == "|")
                return iOperand1 | iOperand2;

            return 0;
        }

        public void Set(string sRegister, int iValue)
        {
            M[Registers[sRegister]] = iValue;
        }
    }
}
