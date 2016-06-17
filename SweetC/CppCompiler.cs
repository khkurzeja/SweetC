using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweetC
{
    class CppCompiler
    {
        private GoldParser gold;


        public CppCompiler(GoldParser pGold)
        {
            gold = pGold;
        }

        public String BuildCpp()
        {
            StringBuilder cpp = new StringBuilder();

            BuildCpp(cpp, gold.Root, new Dictionary<string, object>());

            return cpp.ToString();
        }

        private void BuildCpp(StringBuilder cpp, GOLD.Reduction reduction, Dictionary<String, Object> data)
        {
            GOLD.Production production = reduction.Parent;
            switch (reduction.Parent.Head().Text())
            {
                case "<File>":
                    {
                        GOLD.Reduction cIncludes = (GOLD.Reduction)reduction[0].Data;
                        GOLD.Reduction decls = (GOLD.Reduction)reduction[1].Data;
                        BuildCpp(cpp, cIncludes, data);
                        BuildCpp(cpp, decls, data);
                        break;
                    }

                case "<C Includes>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        if (handle.Count() > 0)
                        {
                            GOLD.Reduction cInclude = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction cIncludes = (GOLD.Reduction)reduction[1].Data;
                            BuildCpp(cpp, cInclude, data);
                            BuildCpp(cpp, cIncludes, data);
                        }
                        break;
                    }

                case "<C Include>":
                    {
                        String includePath = (String)reduction[1].Data;
                        cpp.Append("#include " + includePath + "\n");
                        break;
                    }

                case "<Decls>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        if (handle.Count() > 0)
                        {
                            GOLD.Reduction decl = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction decls = (GOLD.Reduction)reduction[1].Data;
                            BuildCpp(cpp, decl, data);
                            BuildCpp(cpp, decls, data);
                        }
                        break;
                    }

                //**TODO
                /*case "<Func Decl>":
                    {
                        bool isMember = false;
                        String datatype = null;
                        if (data != null)
                        {
                            if (data.ContainsKey("IsMember") && (bool)data["IsMember"] == true)
                            {
                                if (!data.ContainsKey("Datatype"))
                                    throw new Exception("Must have 'Datatype' if Func Decl has 'IsMember' in data");

                                isMember = true;
                                datatype = (String)data["Datatype"];
                            }
                        }

                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);
                        if (handleText == "Id '(' ')' <Block>")
                        {
                            String id = (String)reduction[0].Data;
                            GOLD.Reduction block = (GOLD.Reduction)reduction[3].Data;
                            if (isMember)
                            {
                                c.Append("void _" + id + "_" + datatype + "(" + datatype + "* this)");
                            }
                            else
                            {
                                c.Append("void " + id + "()");
                            }
                            BuildC(c, block);
                            c.Append("\n");
                        }
                        else if (handleText == "Id '(' ')' '->' <Type> <Block>")
                        {
                            String id = (String)reduction[0].Data;
                            GOLD.Reduction type = (GOLD.Reduction)reduction[4].Data;
                            GOLD.Reduction block = (GOLD.Reduction)reduction[5].Data;
                            BuildC(c, type);
                            if (isMember)
                            {
                                c.Append(" _" + id + "_" + datatype + "(" + datatype + "* this)");
                            }
                            else
                            {
                                c.Append(" " + id + "()");
                            }
                            BuildC(c, block);
                            c.Append("\n");
                        }
                        else if (handleText == "Id '(' <Func Params> ')' <Block>")
                        {
                            String id = (String)reduction[0].Data;
                            GOLD.Reduction funcParams = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction block = (GOLD.Reduction)reduction[4].Data;
                            if (isMember)
                            {
                                c.Append("void _" + id + "_" + datatype + "(" + datatype + "* this, ");
                            }
                            else
                            {
                                c.Append("void " + id + "(");
                            }
                            BuildC(c, funcParams);
                            c.Append(") ");
                            BuildC(c, block);
                            c.Append("\n");
                        }
                        else if (handleText == "Id '(' <Func Params> ')' '->' <Type> <Block>")
                        {
                            String id = (String)reduction[0].Data;
                            GOLD.Reduction funcParams = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction type = (GOLD.Reduction)reduction[5].Data;
                            GOLD.Reduction block = (GOLD.Reduction)reduction[6].Data;
                            BuildC(c, type);
                            if (isMember)
                            {
                                c.Append(" _" + id + "_" + datatype + "(" + datatype + "* this, ");
                            }
                            else
                            {
                                c.Append(" " + id + "(");
                            }
                            BuildC(c, funcParams);
                            c.Append(") ");
                            BuildC(c, block);
                            c.Append("\n");
                        }
                        break;
                    }*/

                case "<Func Params>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);
                        if (handleText == "<Func Param List> ',' <Func Params>")
                        {
                            GOLD.Reduction paramList = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction moreParams = (GOLD.Reduction)reduction[2].Data;
                            BuildCpp(cpp, paramList, data);
                            cpp.Append(", ");
                            BuildCpp(cpp, moreParams, data);
                        }
                        break;
                    }

                case "<Func Param List>":
                    {
                        GOLD.Reduction idList = (GOLD.Reduction)reduction[0].Data;
                        GOLD.Reduction type = (GOLD.Reduction)reduction[2].Data;

                        StringBuilder typeString = new StringBuilder();
                        BuildCpp(typeString, type, data);

                        StringBuilder idListString = new StringBuilder();
                        BuildCpp(idListString, idList, data);
                        idListString.Replace(",", ", " + typeString);  //**TODO: Consider changing no to use replace

                        cpp.Append(typeString + " " + idListString);
                        break;
                    }

                case "<Type>":
                    {
                        GOLD.Reduction mod = (GOLD.Reduction)reduction[0].Data;
                        GOLD.Reduction signage = (GOLD.Reduction)reduction[1].Data;
                        GOLD.Reduction baseName = (GOLD.Reduction)reduction[2].Data;
                        BuildCpp(cpp, signage, data);
                        BuildCpp(cpp, baseName, data);
                        BuildCpp(cpp, mod, data);
                        break;
                    }

                case "<Signage>":
                    {
                        if (production.Handle().Count() > 0)
                        {
                            String signage = (String)reduction[0].Data;
                            cpp.Append(signage + " ");
                        }
                        break;
                    }

                case "<Base>":
                    {
                        String name = (String)reduction[0].Data;
                        cpp.Append(name);
                        break;
                    }

                case "<Type Mod>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        if (handle.Count() > 0)
                        {
                            String handleText = handle.Text(" ", false);
                            if (handleText == "<Type Mod> '*'")
                            {
                                GOLD.Reduction mod = (GOLD.Reduction)reduction[0].Data;
                                cpp.Append("*");
                                BuildCpp(cpp, mod, data);
                            }
                        }
                        break;
                    }

                //**TODO
                /*case "<Datatype Decl>":
                    {
                        String id = (String)reduction[0].Data;
                        GOLD.Reduction datatypeDef = (GOLD.Reduction)reduction[2].Data;

                        Dictionary<string, object> childData = new Dictionary<string, object>();
                        childData.Add("Datatype", id);
                        childData.Add("FuncString", new StringBuilder());  // The datatype functions need to be built in this StringBuilder
                        childData.Add("ConstructorCount", 0);
                        childData.Add("DestructorCount", 0);

                        c.Append("\n#if INTERFACE\n");
                        c.Append("typedef struct {\n");
                        BuildC(c, datatypeDef, childData);
                        c.Append("} " + id + ";\n");
                        c.Append("#endif\n\n");

                        if ((int)childData["ConstructorCount"] == 0)
                        {
                            c.Append(id + "* _new_" + id + "(" + id + "* this){ return this; }\n\n");
                        }
                        if ((int)childData["DestructorCount"] == 0)
                        {
                            c.Append("void _del_" + id + "(" + id + "* this){}\n\n");
                        }

                        c.Append((StringBuilder)childData["FuncString"]);

                        break;
                    }*/

                //**TODO
                /*case "<Datatype Def>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);
                        if (handleText == "<Var Decl> <Datatype Def>")
                        {
                            GOLD.Reduction varDecl = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction datatypeDef = (GOLD.Reduction)reduction[1].Data;
                            BuildC(c, varDecl);
                            BuildC(c, datatypeDef, data);
                        }
                        else if (handleText == "<Func Decl> <Datatype Def>")
                        {
                            if (data == null)
                                throw new Exception("data must exist to create datatype func decl");

                            GOLD.Reduction funcDecl = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction datatypeDef = (GOLD.Reduction)reduction[1].Data;

                            StringBuilder builder = (StringBuilder)data["FuncString"];
                            String datatype = (String)data["Datatype"];

                            Dictionary<string, object> funcData = new Dictionary<string, object>();
                            funcData.Add("Datatype", datatype);
                            funcData.Add("IsMember", true);
                            BuildC(builder, funcDecl, funcData);
                            //builder.Append("\n");

                            BuildC(c, datatypeDef, data);
                        }
                        else if (handleText == "<Constructor Decl> <Datatype Def>")
                        {
                            if (data == null)
                                throw new Exception("data must exist to create datatype constructor decl");

                            data["ConstructorCount"] = ((int)data["ConstructorCount"]) + 1;

                            GOLD.Reduction constructorDecl = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction datatypeDef = (GOLD.Reduction)reduction[1].Data;

                            StringBuilder builder = (StringBuilder)data["FuncString"];
                            String datatype = (String)data["Datatype"];

                            Dictionary<string, object> constructorData = new Dictionary<string, object>();
                            constructorData.Add("Datatype", datatype);
                            BuildC(builder, constructorDecl, constructorData);
                            builder.Append("\n");

                            BuildC(c, datatypeDef, data);
                        }
                        else if (handleText == "<Destructor Decl> <Datatype Def>")
                        {
                            if (data == null)
                                throw new Exception("data must exist to create datatype destructor decl");

                            data["DestructorCount"] = ((int)data["DestructorCount"]) + 1;

                            GOLD.Reduction destructorDecl = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction datatypeDef = (GOLD.Reduction)reduction[1].Data;

                            StringBuilder builder = (StringBuilder)data["FuncString"];
                            String datatype = (String)data["Datatype"];

                            Dictionary<string, object> destructorData = new Dictionary<string, object>();
                            destructorData.Add("Datatype", datatype);
                            BuildC(builder, destructorDecl, destructorData);
                            builder.Append("\n");

                            BuildC(c, datatypeDef, data);
                        }
                        break;
                    }*/

                //**TODO
                /*case "<Constructor Decl>":
                    {
                        if (data == null)
                            throw new Exception("Cannot create constructor without 'Datatype' data");
                        string type = (String)data["Datatype"];

                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);
                        if (handleText == "new '(' ')' <Block>")
                        {
                            GOLD.Reduction block = (GOLD.Reduction)reduction[3].Data;

                            c.Append(type + "* _new_" + type + "(" + type + "* this) ");
                            Dictionary<string, object> blockData = new Dictionary<string, object>();
                            blockData.Add("ReturnThis", true);
                            BuildC(c, block, blockData);
                        }
                        else if (handleText == "new '(' <Func Params> ')' <Block>")
                        {
                            GOLD.Reduction funcParams = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction block = (GOLD.Reduction)reduction[4].Data;

                            c.Append(type + "* _new_" + type + "(" + type + "* this, ");  //**TODO: Need to mangle this name
                            BuildC(c, funcParams);
                            c.Append(") ");
                            Dictionary<string, object> blockData = new Dictionary<string, object>();
                            blockData.Add("ReturnThis", true);
                            BuildC(c, block, blockData);
                        }
                        break;
                    }*/

                //*TODO
                /*case "<Destructor Decl>":
                    {
                        if (data == null)
                            throw new Exception("Cannot create destructor without 'Datatype' data");
                        string type = (String)data["Datatype"];

                        GOLD.Reduction block = (GOLD.Reduction)reduction[3].Data;

                        c.Append("void _del_" + type + "(" + type + "* this) ");
                        Dictionary<string, object> blockData = new Dictionary<string, object>();
                        BuildC(c, block, blockData);

                        break;
                    }*/

                //**TODO
                /*case "<Var Decl>":
                    {
                        bool canExecuteStatements = false;  // ie. Set to true if inside a function and to false if declaring global variable.

                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);

                        if (handleText == "<Id List> ':' <Type> ';'")
                        {
                            GOLD.Reduction list = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction type = (GOLD.Reduction)reduction[2].Data;

                            StringBuilder typeString = new StringBuilder();
                            BuildC(typeString, type);

                            Dictionary<string, object> listData = new Dictionary<string, object>();
                            listData.Add("Datatype", typeString.ToString());

                            BuildC(c, list, listData);
                            c.Append("\n");
                        }
                        else if (handleText == "<Id List> ':' <Type> '=' <Expression> ';'")
                        {
                            GOLD.Reduction list = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction type = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction expr = (GOLD.Reduction)reduction[4].Data;

                            StringBuilder typeString = new StringBuilder();
                            BuildC(typeString, type);

                            StringBuilder exprString = new StringBuilder();
                            BuildC(exprString, expr);

                            Dictionary<string, object> listData = new Dictionary<string, object>();
                            listData.Add("Datatype", typeString.ToString());
                            listData.Add("ListLength", 0);

                            StringBuilder listString = new StringBuilder();
                            BuildC(listString, list, listData);

                            int listLength = (int)listData["ListLength"];
                            listData.Remove("ListLength");  // Remove ListLength to prevent confusion since we don't need it anymore.

                            if (listLength == 1)
                            {
                                listData.Add("Assign", exprString.ToString());
                                BuildC(c, list, listData);
                                c.Append("\n");
                            }
                            else
                            {
                                // Cache expression in temp variable and assign it to each variable in list so it is only evaluated once.
                                if (canExecuteStatements)
                                {
                                    BuildC(c, list, listData);
                                    c.Append("\n{" + typeString + " temp=" + exprString + "; ");
                                    listData.Remove("Datatype");  // Remove datatype so we do not redeclare each variable.
                                    listData["Assign"] = "temp";
                                    BuildC(c, list, listData);
                                    c.Append(";}\n");
                                }
                                else
                                {
                                    listData.Add("Assign", exprString.ToString());
                                    BuildC(c, list, listData);
                                    c.Append("\n");
                                }

                                //**TODO: Make sure this method of caching the expression does not cause problems when destructors are added.
                            }
                        }
                        else
                        {
                            throw new Exception("Failed to construct Var Decl. This is a compiler bug.");
                        }
                        break;
                    }*/

                //**TODO
                /*case "<Id List>":
                    {
                        string datatype = null;
                        bool countListLength = false;
                        string assign = "";
                        if (data != null)
                        {
                            if (data.ContainsKey("Datatype"))
                            {
                                datatype = (String)data["Datatype"];
                            }
                            if (data.ContainsKey("ListLength"))
                            {
                                countListLength = true;
                            }
                            if (data.ContainsKey("Assign"))
                            {
                                assign = " = " + (String)data["Assign"];
                            }
                        }

                        GOLD.SymbolList handle = production.Handle();
                        if (handle.Count() == 1)
                        {
                            String id = (String)reduction[0].Data;

                            if (countListLength)
                                data["ListLength"] = ((int)data["ListLength"]) + 1;

                            if (datatype == null)
                                c.Append(id + assign);
                            else
                                c.Append(datatype + " " + id + assign + ";");
                        }
                        else
                        {
                            String id = (String)reduction[0].Data;
                            GOLD.Reduction list = (GOLD.Reduction)reduction[2].Data;

                            if (countListLength)
                                data["ListLength"] = ((int)data["ListLength"]) + 1;

                            if (datatype == null)
                                c.Append(id + assign + ", ");
                            else
                                c.Append(datatype + " " + id + assign + "; ");
                            BuildC(c, list, data);
                        }
                        break;
                    }*/

                case "<Stm>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);
                        if (handleText == "if '(' <Expression> ')' <Stm>")
                        {
                            GOLD.Reduction condition = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction stm = (GOLD.Reduction)reduction[4].Data;
                            cpp.Append("if (");
                            BuildCpp(cpp, condition, data);
                            cpp.Append(") {");
                            BuildCpp(cpp, stm, data);
                            cpp.Append("}");
                        }
                        else if (handleText == "if '(' <Expression> ')' <Then Stm> else <Stm>")
                        {
                            GOLD.Reduction condition = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction thenStm = (GOLD.Reduction)reduction[4].Data;
                            GOLD.Reduction stm = (GOLD.Reduction)reduction[6].Data;
                            cpp.Append("if (");
                            BuildCpp(cpp, condition, data);
                            cpp.Append(") {");
                            BuildCpp(cpp, thenStm, data);
                            cpp.Append("} else {");
                            BuildCpp(cpp, stm, data);
                            cpp.Append("}");
                        }
                        else if (handleText == "while '(' <Expression> ')' <Stm>")
                        {
                            GOLD.Reduction condition = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction stm = (GOLD.Reduction)reduction[4].Data;
                            cpp.Append("while (");
                            BuildCpp(cpp, condition, data);
                            cpp.Append(") {");
                            BuildCpp(cpp, stm, data);
                            cpp.Append("}");
                        }
                        else if (handleText == "for '(' <First For Arg> <For Arg> ';' <For Arg> ')' <Then Stm>")
                        {
                            GOLD.Reduction arg1 = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction arg2 = (GOLD.Reduction)reduction[3].Data;
                            GOLD.Reduction arg3 = (GOLD.Reduction)reduction[5].Data;
                            GOLD.Reduction stm = (GOLD.Reduction)reduction[7].Data;
                            cpp.Append("for (");
                            BuildCpp(cpp, arg1, data);

                            BuildCpp(cpp, arg2, data);
                            cpp.Append("; ");
                            BuildCpp(cpp, arg3, data);
                            cpp.Append(") {");
                            BuildCpp(cpp, stm, data);
                            cpp.Append("}");
                        }
                        break;
                    }

                case "<Then Stm>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);
                        if (handleText == "if '(' <Expression> ')' <Then Stm> else <Then Stm>")
                        {
                            GOLD.Reduction condition = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction thenStm1 = (GOLD.Reduction)reduction[4].Data;
                            GOLD.Reduction thenStm2 = (GOLD.Reduction)reduction[2].Data;
                            cpp.Append("if (");
                            BuildCpp(cpp, condition, data);
                            cpp.Append(") {");
                            BuildCpp(cpp, thenStm1, data);
                            cpp.Append("} else {");
                            BuildCpp(cpp, thenStm2, data);
                            cpp.Append("}");
                        }
                        else if (handleText == "while '(' <Expression> ')' <Then Stm>")
                        {
                            GOLD.Reduction condition = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction thenStm = (GOLD.Reduction)reduction[4].Data;
                            cpp.Append("while (");
                            BuildCpp(cpp, condition, data);
                            cpp.Append(") {");
                            BuildCpp(cpp, thenStm, data);
                            cpp.Append("}");
                        }
                        else if (handleText == "for '(' <First For Arg> <For Arg> ';' <For Arg> ')' <Then Stm>")
                        {
                            GOLD.Reduction arg1 = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction arg2 = (GOLD.Reduction)reduction[3].Data;
                            GOLD.Reduction arg3 = (GOLD.Reduction)reduction[5].Data;
                            GOLD.Reduction stm = (GOLD.Reduction)reduction[7].Data;
                            cpp.Append("for (");
                            BuildCpp(cpp, arg1, data);

                            BuildCpp(cpp, arg2, data);
                            cpp.Append("; ");
                            BuildCpp(cpp, arg3, data);
                            cpp.Append(") {");
                            BuildCpp(cpp, stm, data);
                            cpp.Append("}");
                        }
                        break;
                    }

                case "<First For Arg>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);
                        if (handleText == "<Expression> ';'")
                        {
                            GOLD.Reduction expr = (GOLD.Reduction)reduction[0].Data;
                            BuildCpp(cpp, expr, data);
                            cpp.Append("; ");
                        }
                        else if (handleText == "<Var Decl>")
                        {
                            GOLD.Reduction varDecl = (GOLD.Reduction)reduction[0].Data;
                            BuildCpp(cpp, varDecl, data);
                        }
                        else if (handleText == "';'")
                        {
                            cpp.Append("; ");
                        }
                        break;
                    }

                case "<For Arg>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        if (handle.Count() > 0)
                        {
                            GOLD.Reduction expr = (GOLD.Reduction)reduction[0].Data;
                            BuildCpp(cpp, expr, data);
                        }
                        break;
                    }

                case "<Normal Stm>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);
                        if (handleText == "do <Stm> while '(' <Expression> ')'")
                        {
                            GOLD.Reduction stm = (GOLD.Reduction)reduction[1].Data;
                            GOLD.Reduction condition = (GOLD.Reduction)reduction[4].Data;
                            cpp.Append("do {");
                            BuildCpp(cpp, stm, data);
                            cpp.Append("} while(");
                            BuildCpp(cpp, condition, data);
                            cpp.Append(")");
                        }
                        else if (handleText == "switch '(' <Expression> ')' '{' <Case Stms> '}'")
                        {
                            GOLD.Reduction value = (GOLD.Reduction)reduction[2].Data;
                            GOLD.Reduction cases = (GOLD.Reduction)reduction[5].Data;
                            cpp.Append("switch (");
                            BuildCpp(cpp, value, data);
                            cpp.Append(") {");
                            BuildCpp(cpp, cases, data);
                            cpp.Append("}\n");
                        }
                        else if (handleText == "<Expression> ';'")
                        {
                            GOLD.Reduction expr = (GOLD.Reduction)reduction[0].Data;
                            BuildCpp(cpp, expr, data);
                            cpp.Append(";\n");
                        }
                        else if (handleText == "break ';'")
                        {
                            cpp.Append("break;\n");
                        }
                        else if (handleText == "continue ';'")
                        {
                            cpp.Append("continue;\n");
                        }
                        else if (handleText == "return ';'")
                        {
                            cpp.Append("return;\n");
                        }
                        else if (handleText == "return <Expression> ';'")
                        {
                            GOLD.Reduction expr = (GOLD.Reduction)reduction[1].Data;
                            cpp.Append("return ");
                            BuildCpp(cpp, expr, data);
                            cpp.Append(";\n");
                        }
                        else if (handleText == "';'")
                        {
                            cpp.Append(";");
                        }
                        else
                        {
                            throw new Exception("Failed to construct Normal Stm. This is a compiler bug.");
                        }
                        break;
                    }

                //**TODO: Lots to change in block. Putting it off for now.
                /*case "<Block>":
                    {
                        bool createStackType = false;
                        string stackType = null;
                        bool returnThis = false;

                        if (data.ContainsKey("CreateStackType"))
                        {
                            createStackType = (bool)data["CreateStackType"];
                            if (createStackType)
                            {
                                stackType = (string)data["StackType"];
                            }
                        }
                        if (data.ContainsKey("ReturnThis"))
                        {
                            returnThis = (bool)data["ReturnThis"];
                        }

                        GOLD.Reduction list = (GOLD.Reduction)reduction[1].Data;
                        c.Append("{\n");

                        if (createStackType)
                            c.Append(stackType + " this;\n");

                        BuildC(c, list);

                        if (returnThis)
                            c.Append("return this;\n");

                        c.Append("}\n");
                        break;
                    }*/

                case "<Stm List>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        if (handle.Count() > 0)
                        {
                            GOLD.Reduction stm = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction list = (GOLD.Reduction)reduction[1].Data;
                            BuildCpp(cpp, stm, data);
                            BuildCpp(cpp, list, data);
                        }
                        break;
                    }

                case "<Op Assign>":
                    {
                        GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                        String assign = (String)reduction[1].Data;
                        GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                        cpp.Append("(");
                        BuildCpp(cpp, pre, data);
                        cpp.Append(" " + assign + " ");
                        BuildCpp(cpp, post, data);
                        cpp.Append(")");
                        break;
                    }

                case "<Op Ternary>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        GOLD.Reduction condition = (GOLD.Reduction)reduction[0].Data;
                        GOLD.Reduction trueResult = (GOLD.Reduction)reduction[2].Data;
                        GOLD.Reduction falseResult = (GOLD.Reduction)reduction[4].Data;
                        cpp.Append("(");
                        BuildCpp(cpp, condition, data);
                        cpp.Append("?");
                        BuildCpp(cpp, trueResult, data);
                        cpp.Append(":");
                        BuildCpp(cpp, falseResult, data);
                        cpp.Append(")");
                        break;
                    }

                case "<Op Or>":
                    {
                        GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                        GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                        cpp.Append("(");
                        BuildCpp(cpp, pre, data);
                        cpp.Append("||");
                        BuildCpp(cpp, post, data);
                        cpp.Append(")");
                        break;
                    }

                case "<Op And>":
                    {
                        GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                        GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                        cpp.Append("(");
                        BuildCpp(cpp, pre, data);
                        cpp.Append("&&");
                        BuildCpp(cpp, post, data);
                        cpp.Append(")");
                        break;
                    }

                case "<Op Equate>":
                    {
                        GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                        String op = (String)reduction[1].Data;
                        GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                        cpp.Append("(");
                        BuildCpp(cpp, pre, data);
                        cpp.Append(op);
                        BuildCpp(cpp, post, data);
                        cpp.Append(")");
                        break;
                    }

                case "<Op Compare>":
                    {
                        GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                        String op = (String)reduction[1].Data;
                        GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                        cpp.Append("(");
                        BuildCpp(cpp, pre, data);
                        cpp.Append(op);
                        BuildCpp(cpp, post, data);
                        cpp.Append(")");
                        break;
                    }

                case "<Op Add>":
                    {
                        GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                        String op = (String)reduction[1].Data;
                        GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                        cpp.Append("(");
                        BuildCpp(cpp, pre, data);
                        cpp.Append(op);
                        BuildCpp(cpp, post, data);
                        cpp.Append(")");
                        break;
                    }

                case "<Op Mult>":
                    {
                        GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                        String op = (String)reduction[1].Data;
                        GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                        cpp.Append("(");
                        BuildCpp(cpp, pre, data);
                        cpp.Append(op);
                        BuildCpp(cpp, post, data);
                        cpp.Append(")");
                        break;
                    }

                case "<Op Bitwise>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        cpp.Append("(");
                        if (handle[1].Name() == ".&")
                        {
                            GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                            BuildCpp(cpp, pre, data);
                            cpp.Append("&");
                            BuildCpp(cpp, post, data);
                        }
                        else if (handle[1].Name() == ".|")
                        {
                            GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                            BuildCpp(cpp, pre, data);
                            cpp.Append("|");
                            BuildCpp(cpp, post, data);
                        }
                        else if (handle[1].Name() == ".^")
                        {
                            GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                            BuildCpp(cpp, pre, data);
                            cpp.Append("^");
                            BuildCpp(cpp, post, data);
                        }
                        cpp.Append(")");
                        break;
                    }

                case "<Op Shift>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        cpp.Append("(");
                        if (handle[1].Name() == ".<")
                        {
                            GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                            BuildCpp(cpp, pre, data);
                            cpp.Append("<<");
                            BuildCpp(cpp, post, data);
                        }
                        else if (handle[1].Name() == ".>")
                        {
                            GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                            BuildCpp(cpp, pre, data);
                            cpp.Append(">>");
                            BuildCpp(cpp, post, data);
                        }
                        cpp.Append(")");
                        break;
                    }

                case "<Op Unary>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        String handleText = production.Handle().Text(" ", false);
                        cpp.Append("(");
                        if (handle[0].Name() == "!")
                        {
                            GOLD.Reduction post = (GOLD.Reduction)reduction[1].Data;
                            cpp.Append("!");
                            BuildCpp(cpp, post, data);
                        }
                        else if (handle[0].Name() == "-")
                        {
                            GOLD.Reduction post = (GOLD.Reduction)reduction[1].Data;
                            cpp.Append("-");
                            BuildCpp(cpp, post, data);
                        }
                        else if (handle[0].Name() == ".!")
                        {
                            GOLD.Reduction post = (GOLD.Reduction)reduction[1].Data;
                            cpp.Append("~");
                            BuildCpp(cpp, post, data);
                        }
                        else if (handle[0].Name() == "@")
                        {
                            GOLD.Reduction post = (GOLD.Reduction)reduction[1].Data;
                            cpp.Append("&");
                            BuildCpp(cpp, post, data);
                        }
                        else if (handleText == "'[' <Type> ']' <Op Unary>")
                        {
                            GOLD.Reduction type = (GOLD.Reduction)reduction[1].Data;
                            GOLD.Reduction post = (GOLD.Reduction)reduction[3].Data;
                            cpp.Append("(");
                            BuildCpp(cpp, type, data);
                            cpp.Append(")");
                            BuildCpp(cpp, post, data);
                        }
                        else if (handleText == "sizeof '(' <Type> ')'")
                        {
                            GOLD.Reduction type = (GOLD.Reduction)reduction[2].Data;
                            cpp.Append("sizeof(");
                            BuildCpp(cpp, type, data);
                            cpp.Append(")");
                        }
                        else if (handleText == "sizeof '(' <Expression> ')'")
                        {
                            GOLD.Reduction access = (GOLD.Reduction)reduction[2].Data;
                            cpp.Append("sizeof(");
                            BuildCpp(cpp, access, data);
                            cpp.Append(")");
                        }
                        else
                        {
                            throw new Exception("Could not construct unary. This is a compiler bug.");
                        }
                        cpp.Append(")");
                        break;
                    }

                case "<Op This>":
                    {
                        GOLD.Reduction access = (GOLD.Reduction)reduction[1].Data;
                        cpp.Append("this->");
                        BuildCpp(cpp, access, data);
                        break;
                    }

                case "<Op Access>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        if (handle[1].Text() == ".")
                        {
                            GOLD.Reduction pre = (GOLD.Reduction)reduction[0].Data;  //**TODO: Might need to dereference pre if it is a pointer. Should maybe do a preprocessing parse that creates a symbol table.
                            GOLD.Reduction post = (GOLD.Reduction)reduction[2].Data;
                            BuildCpp(cpp, pre, data);
                            cpp.Append(".");
                            BuildCpp(cpp, post, data);
                        }
                        else if (handle[1].Text() == "'!'")
                        {
                            GOLD.Reduction value = (GOLD.Reduction)reduction[0].Data;
                            cpp.Append("(*");
                            BuildCpp(cpp, value, data);
                            cpp.Append(")");
                        }
                        break;
                    }

                case "<Value>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);
                        if (handleText == "Id '(' ')'")
                        {
                            String id = (String)reduction[0].Data;
                            cpp.Append(id + "()");
                        }
                        else if (handleText == "Id '(' <Expression List> ')'")
                        {
                            String id = (String)reduction[0].Data;
                            GOLD.Reduction expr = (GOLD.Reduction)reduction[2].Data;
                            cpp.Append(id + "(");
                            BuildCpp(cpp, expr, data);
                            cpp.Append(")");
                        }
                        else if (handleText == "'(' <Expression> ')'")
                        {
                            GOLD.Reduction expr = (GOLD.Reduction)reduction[1].Data;
                            cpp.Append("(");
                            BuildCpp(cpp, expr, data);
                            cpp.Append(")");
                        }
                        else if (handleText == "new Id '(' ')'")
                        {
                            String id = (String)reduction[1].Data;
                            cpp.Append("*_new_" + id + "(&(" + id + "){})");  //**TODO: call member func here
                        }
                        else if (handleText == "new Id '(' <Expression List> ')'")
                        {
                            String id = (String)reduction[1].Data;
                            GOLD.Reduction expr = (GOLD.Reduction)reduction[3].Data;
                            cpp.Append("*_new_" + id + "(&(" + id + "){}, ");  //**TODO
                            BuildCpp(cpp, expr, data);
                            cpp.Append(")");
                        }
                        else if (handleText == "new '*' Id '(' ')'")
                        {
                            String id = (String)reduction[2].Data;
                            cpp.Append("_new_" + id + "((" + id + "*)malloc(sizeof(" + id + ")))");  //**TODO
                        }
                        else if (handleText == "new '*' Id '(' <Expression List> ')'")
                        {
                            String id = (String)reduction[2].Data;
                            GOLD.Reduction expr = (GOLD.Reduction)reduction[4].Data;
                            cpp.Append("_new_" + id + "((" + id + "*)malloc(sizeof(" + id + ")), ");
                            BuildCpp(cpp, expr, data);  //**TODO
                            cpp.Append(")");
                        }
                        else  // We always have a terminal in this case.
                        {
                            String pre = (String)reduction[0].Data;  //**TODO: Need to convert binary and octal values into valid C values.
                            cpp.Append(pre);
                        }
                        break;
                    }

                case "<Expression List>":
                    {
                        GOLD.SymbolList handle = production.Handle();
                        String handleText = handle.Text(" ", false);
                        if (handleText == "<Expression> ',' <Expression List>")
                        {
                            GOLD.Reduction expr = (GOLD.Reduction)reduction[0].Data;
                            GOLD.Reduction exprList = (GOLD.Reduction)reduction[2].Data;
                            BuildCpp(cpp, expr, data);
                            cpp.Append(", ");
                            BuildCpp(cpp, exprList, data);
                        }
                        break;
                    }
            }
        }
    }
}
