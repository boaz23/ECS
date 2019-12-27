using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    public class Assembler
    {
        private const int WORD_SIZE = 16;

        private const int EMPTY_SYMBOL_VALUE = -1;
        private const int VIRTUAL_REGISTERS_COUNT = 16;

        private Dictionary<string, int[]> m_dControl, m_dJmp; //these dictionaries map command mnemonics to machine code - they are initialized at the bottom of the class
        private readonly char[] m_dest;

        //more data structures here (symbol map, ...)
        private Dictionary<string, int> m_symbolsMap;
        private LinkedList<string> m_symbols;

        public Assembler()
        {
            InitCommandDictionaries();
            InitSymbolsDataStructures();
            m_dest = new char[] { 'A', 'D', 'M' };
        }

        //this method is called from the outside to run the assembler translation
        public void TranslateAssemblyFile(string sInputAssemblyFile, string sOutputMachineCodeFile)
        {
            //read the raw input, including comments, errors, ...
            StreamReader sr = new StreamReader(sInputAssemblyFile);
            List<string> lLines = new List<string>();
            while (!sr.EndOfStream)
            {
                lLines.Add(sr.ReadLine());
            }
            sr.Close();
            //translate to machine code
            List<string> lTranslated = TranslateAssemblyFile(lLines);
            //write the output to the machine code file
            StreamWriter sw = new StreamWriter(sOutputMachineCodeFile);
            foreach (string sLine in lTranslated)
                sw.WriteLine(sLine);
            sw.Close();
        }

        //translate assembly into machine code
        private List<string> TranslateAssemblyFile(List<string> lLines)
        {
            //implementation order:
            //first, implement "TranslateAssemblyToMachineCode", and check if the examples "Add", "MaxL" are translated correctly.
            //next, implement "CreateSymbolTable", and modify the method "TranslateAssemblyToMachineCode" so it will support symbols (translating symbols to numbers). check this on the examples that don't contain macros
            //the last thing you need to do, is to implement "ExpendMacro", and test it on the example: "SquareMacro.asm".
            //init data structures here 

            //expand the macros
            List<string> lAfterMacroExpansion = ExpendMacros(lLines);

            //first pass - create symbol table and remove lable lines
            List<string> lAfterSymbolsParsing = CreateSymbolTable(lAfterMacroExpansion);

            //second pass - replace symbols with numbers, and translate to machine code
            List<string> lAfterTranslation = TranslateAssemblyToMachineCode(lAfterSymbolsParsing);
            return lAfterTranslation;
        }

        
        //first pass - replace all macros with real assembly
        private List<string> ExpendMacros(List<string> lLines)
        {
            //You do not need to change this function, you only need to implement the "ExapndMacro" method (that gets a single line == string)
            List<string> lAfterExpansion = new List<string>();
            for (int i = 0; i < lLines.Count; i++)
            {
                //remove all redudant characters
                string sLine = CleanWhiteSpacesAndComments(lLines[i]);
                if (sLine == "")
                    continue;
                //if the line contains a macro, expand it, otherwise the line remains the same
                List<string> lExpanded = ExapndMacro(sLine);
                //we may get multiple lines from a macro expansion
                foreach (string sExpanded in lExpanded)
                {
                    lAfterExpansion.Add(sExpanded);
                }
            }
            return lAfterExpansion;
        }

        //expand a single macro line
        private List<string> ExapndMacro(string sLine)
        {
            List<string> lExpanded = new List<string>();

            if (IsCCommand(sLine))
            {
                //your code here - check for indirect addessing and for jmp shortcuts
                //read the word file to see all the macros you need to support
                string sDest, sCompute, sJmp;
                GetCommandParts(sLine, out sDest, out sCompute, out sJmp);
                ExpandMacro(lExpanded, sLine, sDest, sCompute, sJmp);
            }
            if (lExpanded.Count == 0)
                lExpanded.Add(sLine);
            return lExpanded;
        }

        //second pass - record all symbols - labels and variables
        private List<string> CreateSymbolTable(List<string> lLines)
        {
            List<string> newLines = AddSymbols(lLines);
            ResolveUnsetSymbols();
            return newLines;
        }

        private List<string> AddSymbols(List<string> lLines)
        {
            string sLine = "";
            int iLine = 0;
            List<string> newLines = new List<string>();
            for (int i = 0; i < lLines.Count; i++)
            {
                sLine = lLines[i];
                bool isValidLine;
                if (IsLabelLine(sLine))
                {
                    //record label in symbol table
                    //do not add the label line to the result
                    isValidLine = ParseLabelLineSymbol(sLine, iLine);
                }
                else if (IsACommand(sLine))
                {
                    //may contain a variable - if so, record it to the symbol table (if it doesn't exist there yet...)
                    ParseALabelCommandSymbol(sLine);
                    iLine++;
                    isValidLine = true;
                    newLines.Add(sLine);
                }
                else if (IsCCommand(sLine))
                {
                    //do nothing here
                    iLine++;
                    isValidLine = true;
                    newLines.Add(sLine);
                }
                else
                    isValidLine = false;

                if (!isValidLine)
                {
                    throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
                }
            }

            return newLines;
        }

        private void ParseALabelCommandSymbol(string sLine)
        {
            if (!IsNumberACommand(sLine))
            {
                string label = GetACommandValue(sLine);
                if (!m_symbolsMap.ContainsKey(label))
                {
                    m_symbolsMap.Add(label, EMPTY_SYMBOL_VALUE);
                    m_symbols.AddLast(label);
                }
            }
        }

        //third pass - translate lines into machine code, replacing symbols with numbers
        private List<string> TranslateAssemblyToMachineCode(List<string> lLines)
        {
            string sLine = "";
            List<string> lAfterPass = new List<string>();
            for (int i = 0; i < lLines.Count; i++)
            {
                sLine = lLines[i];
                string translatedLine;
                if (IsACommand(sLine))
                {
                    //translate an A command into a sequence of bits
                    translatedLine = TranslateACommand(sLine);
                }
                else if (IsCCommand(sLine))
                {
                    //translate an C command into a sequence of bits
                    //take a look at the dictionaries m_dControl, m_dJmp, and where they are initialized (InitCommandDictionaries), to understand how to you them here
                    translatedLine = TranslateCCommand(sLine);
                }
                else
                {
                    translatedLine = null;
                }

                if (translatedLine != null)
                {
                    lAfterPass.Add(translatedLine);
                }
                else
                {
                    throw new FormatException("Cannot parse line " + i + ": " + lLines[i]);
                }
            }
            return lAfterPass;
        }

        //helper functions for translating numbers or bits into strings
        private string ToString(int[] aBits)
        {
            string sBinary = "";
            for (int i = 0; i < aBits.Length; i++)
                sBinary += aBits[i];
            return sBinary;
        }

        private string ToBinary(int x)
        {
            string sBinary = "";
            for (int i = 0; i < WORD_SIZE; i++)
            {
                sBinary = (x % 2) + sBinary;
                x = x / 2;
            }
            return sBinary;
        }


        //helper function for splitting the various fields of a C command
        private void GetCommandParts(string sLine, out string sDest, out string sControl, out string sJmp)
        {
            if (sLine.Contains('='))
            {
                int idx = sLine.IndexOf('=');
                sDest = sLine.Substring(0, idx);
                sLine = sLine.Substring(idx + 1);
            }
            else
                sDest = "";
            if (sLine.Contains(';'))
            {
                int idx = sLine.IndexOf(';');
                sControl = sLine.Substring(0, idx);
                sJmp = sLine.Substring(idx + 1);

            }
            else
            {
                sControl = sLine;
                sJmp = "";
            }
        }

        private bool IsCCommand(string sLine)
        {
            return !IsLabelLine(sLine) && sLine[0] != '@';
        }

        private bool IsACommand(string sLine)
        {
            return sLine[0] == '@';
        }

        private bool IsLabelLine(string sLine)
        {
            if (sLine.StartsWith("(") && sLine.EndsWith(")"))
                return true;
            return false;
        }

        private string CleanWhiteSpacesAndComments(string sDirty)
        {
            string sClean = "";
            for (int i = 0 ; i < sDirty.Length ; i++)
            {
                char c = sDirty[i];
                if (c == '/' && i < sDirty.Length - 1 && sDirty[i + 1] == '/') // this is a comment
                    return sClean;
                if (c > ' ' && c <= '~')//ignore white spaces
                    sClean += c;
            }
            return sClean;
        }


        private void InitCommandDictionaries()
        {
            m_dControl = new Dictionary<string, int[]>();

            m_dControl["0"] = new int[] { 0, 1, 0, 1, 0, 1, 0 };
            m_dControl["1"] = new int[] { 0, 1, 1, 1, 1, 1, 1 };
            m_dControl["-1"] = new int[] { 0, 1, 1, 1, 0, 1, 0 };
            m_dControl["D"] = new int[] { 0, 0, 0, 1, 1, 0, 0 };
            m_dControl["A"] = new int[] { 0, 1, 1, 0, 0, 0, 0 };
            m_dControl["!D"] = new int[] { 0, 0, 0, 1, 1, 0, 1 };
            m_dControl["!A"] = new int[] { 0, 1, 1, 0, 0, 0, 1 };
            m_dControl["-D"] = new int[] { 0, 0, 0, 1, 1, 1, 1 };
            m_dControl["-A"] = new int[] { 0, 1, 1, 0, 0,1, 1 };
            m_dControl["D+1"] = new int[] { 0, 0, 1, 1, 1, 1, 1 };
            m_dControl["A+1"] = new int[] { 0, 1, 1, 0, 1, 1, 1 };
            m_dControl["D-1"] = new int[] { 0, 0, 0, 1, 1, 1, 0 };
            m_dControl["A-1"] = new int[] { 0, 1, 1, 0, 0, 1, 0 };
            m_dControl["D+A"] = new int[] { 0, 0, 0, 0, 0, 1, 0 };
            m_dControl["D-A"] = new int[] { 0, 0, 1, 0, 0, 1, 1 };
            m_dControl["A-D"] = new int[] { 0, 0, 0, 0, 1,1, 1 };
            m_dControl["D&A"] = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            m_dControl["D|A"] = new int[] { 0, 0, 1, 0,1, 0, 1 };

            m_dControl["M"] = new int[] { 1, 1, 1, 0, 0, 0, 0 };
            m_dControl["!M"] = new int[] { 1, 1, 1, 0, 0, 0, 1 };
            m_dControl["-M"] = new int[] { 1, 1, 1, 0, 0, 1, 1 };
            m_dControl["M+1"] = new int[] { 1, 1, 1, 0, 1, 1, 1 };
            m_dControl["M-1"] = new int[] { 1, 1, 1, 0, 0, 1, 0 };
            m_dControl["D+M"] = new int[] { 1, 0, 0, 0, 0, 1, 0 };
            m_dControl["D-M"] = new int[] { 1, 0, 1, 0, 0, 1, 1 };
            m_dControl["M-D"] = new int[] { 1, 0, 0, 0, 1, 1, 1 };
            m_dControl["D&M"] = new int[] { 1, 0, 0, 0, 0, 0, 0 };
            m_dControl["D|M"] = new int[] { 1, 0, 1, 0, 1, 0, 1 };

            m_dControl["A+D"] = m_dControl["D+A"];
            m_dControl["M+D"] = m_dControl["D+M"];


            m_dJmp = new Dictionary<string, int[]>();

            m_dJmp[""] = new int[] { 0, 0, 0 };
            m_dJmp["JGT"] = new int[] { 0, 0, 1 };
            m_dJmp["JEQ"] = new int[] { 0, 1, 0 };
            m_dJmp["JGE"] = new int[] { 0, 1, 1 };
            m_dJmp["JLT"] = new int[] { 1, 0, 0 };
            m_dJmp["JNE"] = new int[] { 1, 0, 1 };
            m_dJmp["JLE"] = new int[] { 1, 1, 0 };
            m_dJmp["JMP"] = new int[] { 1, 1, 1 };
        }

        private string TranslateACommand(string line)
        {
            int a;
            if (IsNumberACommand(line))
            {
                a = GetNumberACommandNumber(line);
            }
            else
            {
                a = GetLabelACommandNumber(line);
            }

            return TranslateACommandNumberToBits(a);
        }

        private int GetNumberACommandNumber(string line)
        {
            int a;
            if (!TryParseANumber(line, out a))
            {
                throw new AssemblerException("Expected a number for A command");
            }

            return a;
        }

        private bool TryParseANumber(string line, out int a)
        {
            return int.TryParse(GetACommandValue(line), out a);
        }

        private bool IsNumberACommand(string line)
        {
            return IsNumber(line[1]);
        }

        private bool IsNumber(char c)
        {
            return char.IsDigit(c);
        }

        private bool IsValidANumber(int a)
        {
            return 0 <= a && a < Math.Pow(2, WORD_SIZE - 1);
        }

        private int GetLabelACommandNumber(string line)
        {
            string label = GetACommandValue(line);
            if (!m_symbolsMap.ContainsKey(label))
            {
                throw new AssemblerException($"Use of undefined label '{label}'");
            }

            return m_symbolsMap[label];
        }

        private string TranslateACommandNumberToBits(int a)
        {
            if (!IsValidANumber(a))
            {
                throw new AssemblerException("Number for A command is out of range (valid values are [0,2^15-1])");
            }

            return ToBinary(a);
        }

        private string TranslateCCommand(string sLine)
        {
            string translatedLine;
            string sDest, sControl, sJmp;
            GetCommandParts(sLine, out sDest, out sControl, out sJmp);
            translatedLine = TranslateCCommandFields(sDest, sControl, sJmp);
            return translatedLine;
        }

        private void ValidateDest(string dest)
        {
            if (!IsValidDest(dest))
            {
                throw new AssemblerException($"'{dest}' is not a valid dest field");
            }
        }

        private bool IsValidDest(string dest)
        {
            for (int i = 0; i < m_dest.Length && i < dest.Length; i++)
            {
                if (!IsValidDestChar(dest[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidDestChar(char c)
        {
            return m_dest.Contains(c);
        }

        private string TranslateDestToBits(string dest)
        {
            string destBits = "";
            for (int i = 0; i < m_dest.Length; i++)
            {
                destBits += GetDestBit(dest, m_dest[i]);
            }

            return destBits;
        }

        private string GetDestBit(string dest, char reg)
        {
            if (dest.Contains(reg))
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        private string TranslateCCommandFields(string sDest, string sControl, string sJmp)
        {
            ValidateCCommandFields(sDest, sControl, sJmp);
            return GetCCommandFieldBits(sDest, sControl, sJmp);
        }

        private void ValidateCCommandFields(string sDest, string sControl, string sJmp)
        {
            ValidateDest(sDest);
            if (!IsValidComp(sControl))
            {
                throw new AssemblerException($"'{sControl}' is not a valid comp field");
            }
            if (!m_dJmp.ContainsKey(sJmp))
            {
                throw new AssemblerException($"'{sJmp}' is not a valid jump field");
            }
        }

        private bool IsValidComp(string sControl)
        {
            return m_dControl.ContainsKey(sControl);
        }

        private string GetCCommandFieldBits(string sDest, string sControl, string sJmp)
        {
            return "111" + ToString(m_dControl[sControl]) + TranslateDestToBits(sDest) + ToString(m_dJmp[sJmp]);
        }

        private string GetLabelFromLabelLine(string line)
        {
            return line.Substring(1, line.Length - 2);
        }

        private bool IsValidLabel(string label)
        {
            return label != "";
        }

        private bool ParseLabelLineSymbol(string sLine, int iLine)
        {
            string label = GetLabelFromLabelLine(sLine);
            bool isValidLabel = IsValidLabel(label);
            if (isValidLabel)
            {
                if (m_symbolsMap.ContainsKey(label) && DoesLabelHasValue(label))
                {
                    throw new AssemblerException("Duplicate definition of a line label");
                }
                m_symbolsMap[label] = iLine;
                m_symbols.AddLast(label);
            }
            else
            {
                throw new AssemblerException("Line label name cannot be empty");
            }

            return isValidLabel;
        }

        private string GetACommandValue(string line)
        {
            return line.Substring(1);
        }

        private void InitSymbolsDataStructures()
        {
            m_symbols = new LinkedList<string>();
            m_symbolsMap = new Dictionary<string, int>
            {
                ["SCREEN"] = 0x4000,
                ["KEYBOARD"] = 0x6000,
            };

            for (int i = 0; i < VIRTUAL_REGISTERS_COUNT; i++)
            {
                m_symbolsMap["R" + i] = i;
            }
        }

        private void ResolveUnsetSymbols()
        {
            int nextLabelValue = VIRTUAL_REGISTERS_COUNT;
            foreach (string label in m_symbols)
            {
                if (!DoesLabelHasValue(label))
                {
                    m_symbolsMap[label] = nextLabelValue++;
                }
            }
        }

        private bool DoesLabelHasValue(string label)
        {
            return m_symbolsMap[label] != EMPTY_SYMBOL_VALUE;
        }

        private void ExpandMacro(List<string> lExpanded, string sLine, string sDest, string sCompute, string sJmp)
        {
            if (IsValidDest(sDest))
            {
                if (IsValidComp(sCompute))
                {
                    string sJmpTo;
                    string sCmd;
                    GetJumpParts(sLine, out sCmd, out sJmpTo);
                    if (sJmpTo == "")
                    {
                        // assume regular C command
                        // do nothing
                    }
                    else
                    {
                        // assume dest=comp;jump:jump_to
                        ExpandJumpMacro(lExpanded, sJmpTo, sCmd);
                    }
                }
                else if (sDest != "")
                {
                    if (sJmp == "")
                    {
                        // assume dest=label
                        ExpandLabelToDestAssignmentMacro(lExpanded, sDest, sCompute);
                    }
                    else
                    {
                        // unkown command, just pass it to the next stage
                        // do nothing, it should not be handled here
                    }
                }
                else if (sCompute.EndsWith("++"))
                {
                    // assume label++
                    ExpandIncrementLabelMacro(lExpanded, sCompute);
                }
                else if (sCompute.EndsWith("--"))
                {
                    // assume label--
                    ExpandDecrementLabelMacro(lExpanded, sCompute);
                }
            }
            else
            {
                if (IsValidComp(sCompute))
                {
                    // assume label=comp
                    ExandCompToLabelAssignmentMacro(lExpanded, sDest, sCompute);
                }
                else if (IsNumber(sDest[0]))
                {
                    // assume label=number
                    ExapndLabelImmediateAddressingMacro(lExpanded, sDest, sCompute);
                }
                else
                {
                    // assume label=label
                    ExpandLabelToLabelAssignmentMacro(lExpanded, sDest, sCompute);
                }
            }
        }

        private static void ExpandLabelToLabelAssignmentMacro(List<string> lExpanded, string sDest, string sCompute)
        {
            string labelDest = sDest;
            string labelSrc = sCompute;
            lExpanded.Add($"@{labelSrc}");
            lExpanded.Add("D=M");
            lExpanded.Add($"@{labelDest}");
            lExpanded.Add("M=D");
        }

        private static void ExapndLabelImmediateAddressingMacro(List<string> lExpanded, string sDest, string sCompute)
        {
            string label = sDest;
            string sNumber = sCompute;
            lExpanded.Add($"@{sNumber}");
            lExpanded.Add("D=A");
            lExpanded.Add($"@{label}");
            lExpanded.Add("M=D");
        }

        private static void ExandCompToLabelAssignmentMacro(List<string> lExpanded, string sDest, string sCompute)
        {
            string label = sDest;
            lExpanded.Add($"@{label}");
            lExpanded.Add($"M={sCompute}");
        }

        private static void ExpandDecrementLabelMacro(List<string> lExpanded, string sCompute)
        {
            string label = sCompute.Substring(0, sCompute.Length - 2);
            lExpanded.Add($"@{label}");
            lExpanded.Add("M=M-1");
        }

        private static void ExpandIncrementLabelMacro(List<string> lExpanded, string sCompute)
        {
            string label = sCompute.Substring(0, sCompute.Length - 2);
            lExpanded.Add($"@{label}");
            lExpanded.Add("M=M+1");
        }

        private static void ExpandLabelToDestAssignmentMacro(List<string> lExpanded, string sDest, string sCompute)
        {
            string label = sCompute;
            lExpanded.Add($"@{label}");
            lExpanded.Add($"{sDest}=M");
        }

        private static void ExpandJumpMacro(List<string> lExpanded, string sJmpTo, string sCmd)
        {
            lExpanded.Add($"@{sJmpTo}");
            lExpanded.Add($"{sCmd}");
        }

        private void GetJumpParts(string jump, out string jmp, out string jmpTo)
        {
            int idx = jump.IndexOf(':');
            if (idx > -1)
            {
                jmp = jump.Substring(0, idx);
                jmpTo = jump.Substring(idx + 1);
            }
            else
            {
                jmp = jump;
                jmpTo = "";
            }
        }
    }
}
