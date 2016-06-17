using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweetC
{
    class IdentType
    {
        public enum Class { Variable, Function, Datatype }

        public Class typeClass;

        // Variable
        public String type;

        // Function
        public String returnType;
        public String[] paramTypes;

        // Datatype


        public IdentType(Class pTypeClass)
        {
            typeClass = pTypeClass;
        }


        public static bool Equals(IdentType a, IdentType b)
        {
            if (a.typeClass != b.typeClass)
                return false;

            // Might want to change how we check equality. I'll see later.
            if (a.typeClass == Class.Variable)
                return a.type == b.type;
            else if (a.typeClass == Class.Function)
            {
                if (a.returnType != b.returnType)
                    return false;
                if (a.paramTypes.Length != b.paramTypes.Length)
                    return false;
                for (int i = 0; i < a.paramTypes.Length; i++)
                    if (a.paramTypes[i] != b.paramTypes[i])
                        return false;
                return true;
            }

            return false;
        }

        public static IdentType CreateVariable(String type)
        {
            IdentType it = new IdentType(Class.Variable);
            it.type = type;
            return it;
        }

        public static IdentType CreateFunction(String returnType, String[] paramTypes)
        {
            IdentType it = new IdentType(Class.Function);
            it.returnType = returnType;
            it.paramTypes = paramTypes;
            return it;
        }

        public static IdentType CreateDatatype()
        {
            IdentType it = new IdentType(Class.Datatype);
            return it;
        }
    }
}
