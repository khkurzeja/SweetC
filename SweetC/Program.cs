using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SweetC
{
    class Program
    {
        static void Main(string[] args)
        {
            GoldParser gold = new GoldParser();
            gold.Setup(@"SweetC.egt");
            gold.Parse( System.IO.File.OpenText(@"SampleProgram.txt") );

            if (gold.FailMessage != null)
                Console.WriteLine(gold.FailMessage);
            else
            {
                Console.WriteLine("Parse successful:");

                try
                {
                    Compiler creator = new Compiler(gold);
                    String c = creator.BuildC();
                    Console.WriteLine();
                    Console.WriteLine(c);
                }
                catch (Exception e) { Console.WriteLine("Error: " + e.Message); }
            }

            Console.ReadKey();
        }
    }
}
