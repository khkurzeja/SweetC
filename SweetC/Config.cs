using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SweetC
{
    class Config
    {
        public List<String> scPaths;

        public Config(String configPath)
        {
            String text = File.ReadAllText(configPath);

            scPaths = new List<string>();

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
                        throw new Exception("Empty section name in config file at line " + (i + 1));
                    sectionName = line.Substring(2).Trim();
                    if (sectionName.Length == 0)
                        throw new Exception("Empty section name in config file at line " + (i+1));
                }
                else if (isSymbol(line, 0, "|"))
                {
                    if (line.Length == 1)
                        throw new Exception("Empty value in config file at line " + (i + 1));
                    String value = line.Substring(1).Trim();

                    if (sectionName.Equals("relative", StringComparison.CurrentCultureIgnoreCase))
                        scPaths.Add(Directory.GetCurrentDirectory() + "\\" + value);  //**TODO: Need to set relative paths to be relative to the scConfig file, not the exe file.
                    else if (sectionName.Equals("absolute", StringComparison.CurrentCultureIgnoreCase))
                        scPaths.Add(value);
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
