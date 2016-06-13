using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace SweetC
{
    class Program
    {
        static void Main(string[] args)
        {
            String scModule = "src/Main.m";
            if (args.Length == 1)
            {
                scModule = args[0];
            }

            Module module = new Module(scModule);
            //Console.WriteLine(module.folderPath + " | " + module.name);
            List<String> scPaths = module.scPaths;

            Console.WriteLine("Building C:");
            foreach (String path in scPaths)
                Console.WriteLine(path);
            Console.WriteLine();

            // Next need to parse each file found in config path list.
            // Then need to write a second parser which runs first and creates a symbol table.

            List<String> usedFilenames = new List<string>();

            // Generate C files
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream s = assembly.GetManifestResourceStream("SweetC.SweetC.egt"))
            using (BinaryReader r = new BinaryReader(s))
            {
                GoldParser gold = new GoldParser();
                gold.Setup(r);

                Directory.CreateDirectory("bin");
                Directory.CreateDirectory("bin/c");
                Directory.CreateDirectory("bin/c/" + module.name);

                for (int i = 0; i < scPaths.Count; i++)
                {
                    String path = scPaths[i];
                    String[] pathParts = module.scPathParts[i];

                    gold.Parse(path);

                    if (gold.FailMessage != null)
                        Console.WriteLine(gold.FailMessage);
                    else
                    {
                        try
                        {
                            Compiler creator = new Compiler(gold);
                            String c = creator.BuildC();
                            int lastSlash = Math.Max(path.LastIndexOf("/"), path.LastIndexOf("\\")) + 1;
                            int lastDot = path.LastIndexOf(".");

                            String filename = path.Substring(lastSlash, lastDot - lastSlash);
                            String origFilename = filename;
                            int iters = 2;
                            while (usedFilenames.Contains(filename))
                            {
                                filename = origFilename + "_" + iters;
                                iters++;
                            }
                            usedFilenames.Add(filename);

                            c = "#include \"" + filename + ".h\"\n#include <stdlib.h>\n" + c;  // Make sure the c file includes the h file and stdlib.h. Maybe should only include stdlib in files that use new*

                            File.WriteAllText("bin/c/" + module.name + "/" + filename + ".c", c);
                        }
                        catch (Exception e) { Console.WriteLine("Error in " + path + ": " + e.Message); }
                    }
                }
            }

            // Run makeheaders to generate h files
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "makeheaders.exe";
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "bin/c/" + module.name + "/*.c";

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception e) { Console.WriteLine(e); }


            // Generate makefile


            // Use MinGW or GCC with Cygwin to compile and run the c program.


            Console.WriteLine("Finish.");

            Console.ReadKey();
        }
    }
}
