using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembler a = new Assembler();
            //to run tests, call the "TranslateAssemblyFile" function like this:
            //string sourceFileLocation = the path to your source file
            //string destFileLocation = the path to your dest file
            //a.TranslateAssemblyFile(sourceFileLocation, destFileLocation);

            a.TranslateAssemblyFile("Add.asm", "Add.mc");
            //TranslateAssemblyFile("Add");
            //TranslateAssemblyFile("MaxL");
            //TranslateAssemblyFile("Max");
            //TranslateAssemblyFile("Div");
            //TranslateAssemblyFile("Power");
            //TranslateAssemblyFile("Fibonacci");
            //TranslateAssemblyFile("ScreenExample");
            //TranslateAssemblyFile("SquareMacro");
        }

        static void TranslateAssemblyFile(string fileName)
        {
            new Assembler().TranslateAssemblyFile(Path.ChangeExtension(fileName, ".asm"), Path.ChangeExtension(fileName, ".mc"));
        }
    }
}
