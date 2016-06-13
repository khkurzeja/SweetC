using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SweetC
{
    class Module
    {
        public String path;  // Relative path from exe to module file
        public String folderPath;  // Relative path from exe to folder module file is in
        public String[] folderPathParts;  // folderPath split by forward or back slash
        public String name;  // The name of the module file without the extension

        public List<String> scPaths;
        public List<String[]> scPathParts;

        public Module(String modulePath)
        {
            path = modulePath;
            int lastSlash = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
            if (lastSlash != -1)
            {
                folderPath = path.Substring(0, lastSlash);
                folderPathParts = folderPath.Split(new char[] { '/', '\\' });
                name = path.Substring(lastSlash + 1, path.LastIndexOf('.') - lastSlash - 1);
            }
            else
            {
                folderPath = "";
                folderPathParts = new string[] { };
                name = path.Substring(0, path.LastIndexOf('.'));
            }

            String text = File.ReadAllText(modulePath);

            scPaths = new List<string>();
            scPathParts = new List<String[]>();

            String[] lines = text.Split('\n');

            String sectionName = null;

            for (int i = 0; i < lines.Length; i++)
            {
                String line = lines[i];

                if (line.Trim().Length == 0)
                    continue;

                if (isSymbol(line, 0, "#"))
                {
                    // Ignore comments.
                }
                else if (isSymbol(line, 0, "::"))
                {
                    if (line.Length == 2)
                        throw new Exception("Empty section name in module file " + modulePath + " at line " + (i + 1));
                    sectionName = line.Substring(2).Trim();
                    if (sectionName.Length == 0)
                        throw new Exception("Empty section name in module file " + modulePath + " at line " + (i+1));
                }
                else if (isSymbol(line, 0, "|"))
                {
                    if (line.Length == 1)
                        throw new Exception("Empty value in module file " + modulePath + " at line " + (i + 1));
                    String value = line.Substring(1).Trim();

                    if (sectionName.Equals("files", StringComparison.CurrentCultureIgnoreCase))
                    {
                        scPaths.Add(Directory.GetCurrentDirectory() + "\\" + folderPath + "\\" + value);
                        scPathParts.Add(value.Split(new char[] { '/', '\\' }));
                    }
                    //else if (sectionName.Equals("absolute", StringComparison.CurrentCultureIgnoreCase))
                    //    scPaths.Add(value);
                }
                else
                {
                    throw new Exception("Unrecognized starting symbol in config at line " + (i+1));
                }
            }
        }

        private bool isSymbol(String text, int index, String symbol)
        {
            if (index + symbol.Length > text.Length)
                return false;

            return text.Substring(index, symbol.Length) == symbol;
        }
    }
}
