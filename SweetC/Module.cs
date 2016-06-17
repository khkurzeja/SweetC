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
        public Module parent;

        public String name;  // The name of the module file without the extension
        public String folderPath;  // Relative path from exe to folder module file is in

        public List<SCFile> files;  // Full paths to each sc file in the module.

        public List<Module> modules;

        public SymbolTable symbolTable;  // Maybe shouldn't go here.


        //public List<String> scPaths;
        //public List<String[]> scPathParts;

        public Module(String path) : this(path, null) { }
        public Module(String path, Module parent)
        {
            this.parent = parent;
            symbolTable = new SymbolTable();

            int lastSlash = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
            int lastDot = path.LastIndexOf('.');
            if (lastSlash != -1)
            {
                folderPath = path.Substring(0, lastSlash);
                if (lastDot != -1)
                    name = path.Substring(lastSlash + 1, path.LastIndexOf('.') - lastSlash - 1);
                else
                    name = path.Substring(lastSlash + 1);
            }
            else
            {
                folderPath = "";
                if (lastDot != -1)
                    name = path.Substring(0, path.LastIndexOf('.'));
                else
                    name = path;
            }

            files = new List<SCFile>();
            modules = new List<Module>();

            String text = File.ReadAllText(path);
            String[] lines = text.Split('\n');

            String sectionName = null;


            // Parse file
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
                        throw new Exception("Empty section name in module file " + path + " at line " + (i + 1));
                    sectionName = line.Substring(2).Trim();
                    if (sectionName.Length == 0)
                        throw new Exception("Empty section name in module file " + path + " at line " + (i+1));
                }
                else if (isSymbol(line, 0, "|"))
                {
                    if (line.Length == 1)
                        throw new Exception("Empty value in module file " + path + " at line " + (i + 1));
                    String value = line.Substring(1).Trim();

                    if (sectionName.Equals("files", StringComparison.CurrentCultureIgnoreCase))
                    {
                        addFile(value);
                    }
                    else if (sectionName.Equals("modules", StringComparison.CurrentCultureIgnoreCase))
                    {
                        modules.Add( new Module(folderPath + "/" + value, this) );
                    }
                }
                else
                {
                    throw new Exception("Unrecognized starting symbol in config at line " + (i+1));
                }
            }
        }

        private void addFile(String path)
        {
            SCFile newFile = new SCFile(Directory.GetCurrentDirectory() + "\\" + folderPath + "\\" + path);

            foreach (SCFile file in files)
                if (file.name.Equals(newFile.name, StringComparison.CurrentCultureIgnoreCase))
                    throw new Exception("Modules cannot contain more than one file with the same name. Module " + name + " contains more than one file called " + file.name);

            files.Add(newFile);
        }

        private bool isSymbol(String text, int index, String symbol)
        {
            if (index + symbol.Length > text.Length)
                return false;

            return text.Substring(index, symbol.Length) == symbol;
        }
    }
}
