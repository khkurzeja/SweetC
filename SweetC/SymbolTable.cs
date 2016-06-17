using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweetC
{
    class SymbolTable
    {
        private SymbolTable parent;
        private List<SymbolTable> children;

        private Dictionary<String, List<IdentType>> symbols;

        public SymbolTable() : this(null) { }
        public SymbolTable(SymbolTable pParent)
        {
            parent = pParent;
            children = new List<SymbolTable>();
            symbols = new Dictionary<string, List<IdentType>>();
        }

        public void addSymbol(String name, IdentType type)
        {
            if (!symbols.ContainsKey(name))
            {
                symbols[name] = new List<IdentType>();
                symbols[name].Add(type);
            }
            else
            {
                if (symbols[name][0].typeClass != IdentType.Class.Function || type.typeClass != IdentType.Class.Function)  // Only allow function overloading
                {
                    throw new Exception("Trying to overload name " + name + ". Only function types may be overloaded.");
                }
                else
                {
                    symbols[name].Add(type);
                }
            }
        }

        public List<IdentType> getSymbolTypes(String name)
        {
            if (symbols.ContainsKey(name))
                return symbols[name];
            else
            {
                if (parent == null)
                    return null;
                else
                    return parent.getSymbolTypes(name);
            }
        }

        //public void removeSymbol(String name)
        //{
        //    if (symbols.ContainsKey(name))
        //        symbols.Remove(name);
        //}

        public void addChild(SymbolTable child)
        {
            child.parent = this;
            children.Add(child);
        }
    }
}
