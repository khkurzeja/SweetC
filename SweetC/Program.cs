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
            String rootModulePath = "src/Main.m";
            if (args.Length == 1)
            {
                rootModulePath = args[0];
            }

            Module rootModule = null;
            try
            {
                rootModule = new Module(rootModulePath);
            }
            catch (Exception e) { Console.WriteLine("Error: " + e.Message); }


            // Build C
            if (rootModule != null)
            {
                Console.WriteLine("Building Cpp:");

                Directory.CreateDirectory("bin");
                Directory.CreateDirectory("bin/cpp");

                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream s = assembly.GetManifestResourceStream("SweetC.SweetC.egt"))
                using (BinaryReader r = new BinaryReader(s))
                {
                    GoldParser gold = new GoldParser();
                    gold.Setup(r);

                    Stack<Module> moduleStack = new Stack<Module>();
                    moduleStack.Push(rootModule);

                    List<Module> processedModules = new List<Module>();  // Put a module here the first time it is popped.

                    while (moduleStack.Count > 0)
                    {
                        Module module = moduleStack.Pop();
                        bool processed = processedModules.Contains(module);

                        if (module.modules.Count > 0 && !processed)
                        {
                            moduleStack.Push(module);  // Push back on stack to process after processing children.
                            foreach (Module childModule in module.modules)
                                moduleStack.Push(childModule);
                        }
                        else
                        {
                            // Create output directory if it does not exist
                            Stack<Module> moduleOutputPath = new Stack<Module>();
                            Module part = module;
                            while (part != null)
                            {
                                moduleOutputPath.Push(part);
                                part = part.parent;
                            }

                            String moduleOutputDir = "bin/cpp";
                            while (moduleOutputPath.Count > 0)
                            {
                                part = moduleOutputPath.Pop();
                                moduleOutputDir += "/" + part.name;
                                Directory.CreateDirectory(moduleOutputDir);
                            }

                            // Build symbol table
                            /*foreach (SCFile file in module.files)
                            {
                                Console.WriteLine("Building Symbol Table for " + module.name + " :: " + file.name + " (" + file.path + ")");

                                try
                                {
                                    gold.Parse(file.path);

                                    if (gold.FailMessage != null)
                                        Console.WriteLine(gold.FailMessage);
                                    else
                                    {
                                        Compiler creator = new Compiler(gold);
                                        creator.BuildSymbolTable(module.symbolTable);
                                    }
                                }
                                catch (Exception e) { Console.WriteLine("Error in " + file.path + ": " + e.Message); }
                            }*/

                            // Build Cpp
                            foreach (SCFile file in module.files)
                            {
                                Console.WriteLine("Building Cpp for " + module.name + " :: " + file.name + " (" + file.path + ")");

                                try
                                {
                                    gold.Parse(file.path);

                                    if (gold.FailMessage != null)
                                        Console.WriteLine(gold.FailMessage);
                                    else
                                    {
                                        CppCompiler creator = new CppCompiler(gold);
                                        String cpp = creator.BuildCpp();
                                        cpp = "#include \"" + file.name + ".hpp\"\n#include <stdlib.h>\n" + cpp;

                                        File.WriteAllText(moduleOutputDir + "/" + file.name + ".cpp", cpp);
                                    }
                                }
                                catch (Exception e) { Console.WriteLine("Error in " + file.path + ": " + e.Message); }
                            }
                        }

                        if (!processed)
                            processedModules.Add(module);
                    }
                }
            }


            // Build h
            if (rootModule != null)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = "makeheaders.exe";
                startInfo.Arguments = "";

                Stack<Module> moduleStack = new Stack<Module>();
                moduleStack.Push(rootModule);
                while (moduleStack.Count > 0)
                {
                    Module module = moduleStack.Pop();
                    foreach (Module childModule in module.modules)
                        moduleStack.Push(childModule);

                    Stack<Module> moduleOutputPath = new Stack<Module>();
                    Module part = module;
                    while (part != null)
                    {
                        moduleOutputPath.Push(part);
                        part = part.parent;
                    }

                    String moduleOutputDir = "bin/cpp";
                    while (moduleOutputPath.Count > 0)
                    {
                        part = moduleOutputPath.Pop();
                        moduleOutputDir += "/" + part.name;
                    }

                    startInfo.Arguments += " " + moduleOutputDir + "/*.cpp";
                }

                try
                {
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        exeProcess.WaitForExit();
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }
            }


            /*//Console.WriteLine(module.folderPath + " | " + module.name);
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
            catch (Exception e) { Console.WriteLine(e); }*/


            // Generate makefile


            // Use MinGW or GCC with Cygwin to compile and run the c program.


            Console.WriteLine("Finish.");

            Console.ReadKey();
        }
    }
}
