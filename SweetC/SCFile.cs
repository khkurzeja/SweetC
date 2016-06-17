using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweetC
{
    class SCFile
    {
        public String name;
        public String path;

        public SCFile(String pPath)
        {
            path = pPath;

            int lastSlash = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
            int lastDot = path.LastIndexOf('.');
            if (lastSlash != -1)
            {
                if (lastDot != -1)
                    name = path.Substring(lastSlash + 1, path.LastIndexOf('.') - lastSlash - 1);
                else
                    name = path.Substring(lastSlash + 1);
            }
            else
            {
                if (lastDot != -1)
                    name = path.Substring(0, path.LastIndexOf('.'));
                else
                    name = path;
            }
        }
    }
}
