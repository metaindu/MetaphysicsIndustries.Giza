
/*****************************************************************************
 *                                                                           *
 *  ParserSkeletonBuilder.cs                                                 *
 *  4 March 2010                                                             *
 *  Project: Giza                                                            *
 *  Written by: Richard Sartor                                               *
 *  Copyright � 2010 Metaphysics Industries, Inc.                            *
 *                                                                           *
 *  Generates a MI.Build class for the skeleton of a simple parser from      *
 *    a collection of state-graph nodes.                                     *
 *                                                                           *
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using MetaphysicsIndustries.Collections;
using MetaphysicsIndustries.Build;
using System.Text.RegularExpressions;
using System.Reflection;

namespace MetaphysicsIndustries.Giza
{
    public partial class SpannerBuilder
    {
        public class DefinitionData
        {
            public string DefName = string.Empty;

            public EnumType Enum;

            public Method Method;

            public Method NextStatesMethod;
            public Method SubspanMethod;
            public Method TagMethod;

            public Dictionary<Node, Field> fieldsByNode = new Dictionary<Node, Field>();
        }

        public Class ParserClassFromDefinitions(SimpleDefinitionNode[] defs)
        {
            Definition[] unrefs;
            Definition[] defs2 = SpannerServices.PrepareDefinitions(defs, out unrefs);


            Dictionary<Definition, DefinitionData> datas = new Dictionary<Definition, DefinitionData>();

            foreach (Definition def2 in defs2)
            {
                datas[def2] = ConstructData(def2);
            }

            foreach (Definition def2 in defs2)
            {
                PopulateData(def2, datas);
            }

            Class c = new Class();
            c.BaseClass = (Class)SystemType.GetSystemType(typeof(BaseSpanner));
            c.Visibility = TypeVisibility.Public;
            c.ParentNamespace = new Namespace();
            c.ParentNamespace.Name = "MetaphysicsIndustries.Giza";

            foreach (Definition def in unrefs)
            {
                Method m = new Method();
                m.Name = "Get" + datas[def].DefName;
                m.ReturnType = SystemType.GetSystemType(typeof(Span));
                m.Parameters.Add(new Parameter(SystemTypes.String, "input"));
                LocalVar i = new LocalVar(SystemTypes.Int32, "i", new IntegerLiteralExpression(0));
                m.LocalVars.Add(i);
                LocalVar span =
                    new LocalVar(
                        SystemType.GetSystemType(typeof(Span)),
                        "span",
                        new MethodCallExpression(
                            new ThisExpression(),
                            datas[def].Method,
                            new ParameterAccessExpression(m.Parameters[0]),
                            new LocalVarAccessExpression(i)));
                m.LocalVars.Add(span);
                m.Statements.Add(
                    new IfStatement(
                        new NotEqualExpression(
                            new LocalVarAccessExpression(span),
                            new NullExpression()),
                        new ExpressionStatement(
                            new AssignmentExpression(
                                new FieldAccessExpression(
                                    new LocalVarAccessExpression(span),
                                     SystemType.GetSystemType(typeof(Span)).GetFieldByName("Tag")),
                                new StringLiteralExpression(def.Name)))));
                m.Statements.Add(new ReturnStatement(new LocalVarAccessExpression(span)));

                c.Methods.Add(m);
            }

            foreach (DefinitionData data in datas.Values)
            {
                c.NestedTypes.Add(data.Enum);
                c.Methods.Add(data.Method);
                //c.Methods.Add(data.TextMethod);
                c.Methods.Add(data.NextStatesMethod);
                c.Methods.Add(data.SubspanMethod);
                c.Methods.Add(data.TagMethod);
            }

            return c;
        }

        public Expression BuildConditionalFromCharClass(CharClass cc, Variable ch)
        {
            Expression root = null;
            Expression access = (ch is LocalVar ?
                (Expression)new LocalVarAccessExpression((LocalVar)ch) :
                (Expression)new ParameterAccessExpression((Parameter)ch));

            if (cc.Letter && cc.Digit)
            {
                root = new MethodCallExpression(
                    null,
                    SystemTypes.Char.GetMethodByName("IsLetterOrDigit"),
                    access);
            }
            else if (cc.Letter)
            {
                root = new MethodCallExpression(
                    null,
                    SystemTypes.Char.GetMethodByName("IsLetter"),
                    access);
            }
            else if (cc.Digit)
            {
                root = new MethodCallExpression(
                    null,
                    SystemTypes.Char.GetMethodByName("IsDigit"),
                    access);
            }

            if (cc.Whitespace)
            {
                MethodCallExpression expr = 
                    new MethodCallExpression(
                        null,
                        SystemTypes.Char.GetMethodByName("IsWhiteSpace"),
                        access);
                if (root == null)
                {
                    root = expr;
                }
                else
                {
                    root = new LogicalOrExpression(root, expr);
                }
            }

            char[] chs = cc.GetNonClassChars();
            if (chs != null && chs.Length > 0)
            {
                foreach (char ch2 in chs)
                {
                    EqualExpression expr = new EqualExpression(
                        access,
                        new CharLiteralExpression(ch2));

                    if (root == null)
                    {
                        root = expr;
                    }
                    else
                    {
                        root = new LogicalOrExpression(root, expr);
                    }
                }
            }

            if (cc.Exclude)
            {
                if (root == null)
                {
                    return new BooleanLiteralExpression(false);
                }

                return new LogicalNotExpression(root);
            }

            if (root != null)
            {
                return root;
            }
            else
            {
                return new BooleanLiteralExpression(true);
            }
        }

        public static CharClass BuildCharClassFromNode(Node node)
        {
            switch (node.Type)
            {
                //case NodeType.start:
                //case NodeType.end:
                //    return new CharClass();

                case NodeType.charclass:
                    return ((CharClassNode)node).CharClass;

                case NodeType.literal:
                    return new CharClass(new char[] { ((LiteralNode)node).Char });

                case NodeType.defref:
                    CharClass cc = null;
                    foreach (Node next in ((DefRefNode)node).DefRef.start.NextNodes)
                    {
                        System.Diagnostics.Debug.Assert(next != node);
                        CharClass cc2 = BuildCharClassFromNode(next);
                        if (cc == null)
                        {
                            cc = cc2;
                        }
                        else
                        {
                            cc = CharClass.Union(cc, cc2);
                        }
                    }
                    if (((DefRefNode)node).DefRef.IgnoreCase)
                    {
                        cc = cc.GetIgnoreCase();
                    }
                    return cc;

                default:
                    throw new InvalidOperationException();
            }
        }

        public string GenerateNameForEnumFromDef(Definition def)
        {
            return CleanName(def) + "State";
        }

        //public string GenerateNameForMethodFromDef(DefinitionNode def)
        //{
        //    string str = CleanName(def);
        //    if (char.IsLetter(str[0]))
        //    {
        //        str = char.ToUpper(str[0]).ToString() + str.Substring(1);
        //    }

        //    return "Get" + str;
        //}

        public static string CleanName(Definition def)
        {
            return Regex.Replace(def.Name, @"[^\w\d\s]", string.Empty);
        }



        private void PopulateData(Definition def, Dictionary<Definition, DefinitionData> datas)
        {
            DefinitionData data = datas[def];

            data.SubspanMethod = ConstructSubSpanGetter(def, datas);

            Method m = data.Method;
            m.Statements.Add(
                new ReturnStatement(
                    new MethodCallExpression(
                        new ThisExpression(),
                        SystemType.GetSystemType(typeof(BaseSpanner)).GetMethodByName("GetItem"),
                        new ParameterAccessExpression(m.Parameters[0]),
                        new ParameterAccessExpression(m.Parameters[1]),
                        new BooleanLiteralExpression(def.IgnoreWhitespace),
                        new StringLiteralExpression(def.Name),
                        new CastExpression(SystemTypes.Int32, new FieldAccessExpression(null, data.fieldsByNode[def.start])),
                        new CastExpression(SystemTypes.Int32, new FieldAccessExpression(null, data.fieldsByNode[def.end])),
                        new MethodReferenceExpression(new ThisExpression(), data.NextStatesMethod),
                        //new MethodReferenceExpression(new ThisExpression(), data.TextMethod),
                        new MethodReferenceExpression(new ThisExpression(), data.TagMethod),
                        new MethodReferenceExpression(new ThisExpression(), data.SubspanMethod))));
        }

        private DefinitionData ConstructData(Definition def)
        {
            string defname = SpannerServices.CleanTag(def.Name);
            DefinitionData data = new DefinitionData();
            data.DefName = defname;

            EnumType enu = new EnumType();
            enu.Name = GenerateNameForEnumFromDef(def);
            enu.Visibility = TypeVisibility.Private;
            data.Enum = enu;
            Node[] nodes = def.Nodes.ToArray();
            List<string> fieldNames = new List<string>();
            foreach (Node node in nodes)
            {
                fieldNames.Add(FieldNameFromTag(node.Tag));
            }
            string[] newnames = SpannerServices.MakeTagsUnique(fieldNames.ToArray());
            int i;
            for (i = 0; i < nodes.Length; i++)
            {
                fieldNames[i] = newnames[i];
            }
            for (i = 0; i < nodes.Length; i++)
            {
                Node node = nodes[i];
                Field field = new Field();
                field.Name = fieldNames[i];

                data.fieldsByNode[node] = field;
                enu.Fields.Add(field);
            }



            data.NextStatesMethod = ConstructValidNextsMethod(def, defname, data.fieldsByNode, enu);
            data.TagMethod = ConstructNodeTagGetter(def, defname, data.fieldsByNode, enu);
            //data.TextMethod = ConstructNodeTextGetter(def, defname, data.fieldsByNode, enu);





            data.Method = new Method();
            data.Method.Name = "Get" + defname;
            data.Method.ReturnType = SystemType.GetSystemType(typeof(Span));
            data.Method.Parameters.Add(new Parameter(SystemTypes.String, "input"));
            data.Method.Parameters.Add(new Parameter(SystemTypes.Int32, "i", false, true));

            return data;
        }

        private string FieldNameFromTag(string tag)
        {
            return SpannerServices.CleanTag(tag);
        }

        //private static Method ConstructNodeTextGetter(DefinitionNode def, string defname, Dictionary<Node, Field> fieldsByNode, EnumType enu)
        //{
        //    Method m = new Method();
        //    m.Name = "GetStateText_" + def.Name;
        //    m.ReturnType = SystemTypes.String;
        //    m.Parameters.Add(new Parameter(SystemTypes.Int32, "state"));

        //    SwitchStatement sw = new SwitchStatement();
        //    m.Statements.Add(sw);

        //    sw.Condition = new CastExpression(enu, new ParameterAccessExpression(m.Parameters[0]));

        //    foreach (Node node in def.Nodes)
        //    {
        //        SwitchCase c = new SwitchCase();
        //        sw.Cases.Add(c);
        //        c.CaseLabels.Add(new FieldAccessExpression(null, fieldsByNode[node]));
        //        c.Statements.Add(
        //            new ReturnStatement(
        //                new StringLiteralExpression(
        //                    node.Text)));
        //    }

        //    m.Statements.Add(
        //        new ThrowStatement(
        //            new NewExpression(
        //                SystemType.GetSystemType(typeof(ArgumentOutOfRangeException)),
        //                new StringLiteralExpression("state"))));

        //    return m;
        //}

        private static Method ConstructNodeTagGetter(Definition def, string defname, Dictionary<Node, Field> fieldsByNode, EnumType enu)
        {
            Method m = new Method();
            m.Name = "GetStateTag_" + defname;
            m.ReturnType = SystemTypes.String;
            m.Parameters.Add(new Parameter(SystemTypes.Int32, "state"));

            SwitchStatement sw = new SwitchStatement();
            m.Statements.Add(sw);

            sw.Condition = new CastExpression(enu, new ParameterAccessExpression(m.Parameters[0]));

            foreach (Node node in def.Nodes)
            {
                SwitchCase c = new SwitchCase();
                sw.Cases.Add(c);
                c.CaseLabels.Add(new FieldAccessExpression(null, fieldsByNode[node]));
                c.Statements.Add(
                    new ReturnStatement(
                        new StringLiteralExpression(
                            node.Tag)));
            }

            m.Statements.Add(
                new ThrowStatement(
                    new NewExpression(
                        SystemType.GetSystemType(typeof(ArgumentOutOfRangeException)),
                        new StringLiteralExpression("state"))));

            return m;
        }

        private static Method ConstructSubSpanGetter(Definition def, Dictionary<Definition, DefinitionData> datas)
        {
            //throw new NotImplementedException();
            Method m = new Method();
            m.Name = "GetSubSpan_" + datas[def].DefName;
            m.ReturnType = SystemType.GetSystemType(typeof(Span));
            m.Parameters.Add(new Parameter(SystemTypes.String, "input"));
            m.Parameters.Add(new Parameter(SystemTypes.Int32, "i", false, true));
            m.Parameters.Add(new Parameter(SystemTypes.Int32, "nextState"));

            SwitchStatement sw = new SwitchStatement();
            m.Statements.Add(sw);
            sw.Condition =
                new CastExpression(
                    datas[def].Enum,
                    new ParameterAccessExpression(m.Parameters[2]));//nextState
            foreach (Node node in def.Nodes)
            {
                if (node.Type == NodeType.defref)
                {
                    SwitchCase c = new SwitchCase();
                    sw.Cases.Add(c);
                    c.CaseLabels.Add(new FieldAccessExpression(null, datas[def].fieldsByNode[node]));
                    c.Statements.Add(
                        new ReturnStatement(
                            new MethodCallExpression(
                                new ThisExpression(),
                                datas[((DefRefNode)node).DefRef].Method,
                                new ParameterAccessExpression(m.Parameters[0]),
                                new ParameterAccessExpression(m.Parameters[1]))));
                }
            }
            sw.Default.Statements.Add(
                new ReturnStatement(
                    new NewExpression(
                        SystemType.GetSystemType(typeof(Span)),
                        new ParameterAccessExpression(m.Parameters[1]),
                        new IntegerLiteralExpression(1),
                        new ParameterAccessExpression(m.Parameters[0]))));

            return m;
        }

        private Method ConstructValidNextsMethod(Definition def, string defname, Dictionary<Node, Field> fieldsByNode, EnumType enu)
        {
            Build.Type intListType = SystemType.GetSystemType(typeof(List<int>));
            Method m = new Method();
            m.Name = "GetValidNextStates_" + defname;
            m.ReturnType = SystemType.GetSystemType(typeof(int[]));
            m.Parameters.Add(new Parameter(SystemTypes.Int32, "currentState"));
            m.Parameters.Add(new Parameter(SystemTypes.Char, "ch"));
            m.Parameters.Add(new Parameter(SystemTypes.Boolean, "linksToEnd", true, false));
            LocalVar validNexts = new LocalVar(intListType, "validNextStates", new NewExpression(intListType));
            LocalVarAccessExpression validNextsAccess = new LocalVarAccessExpression(validNexts);
            ParameterAccessExpression linksToEndAccess = new ParameterAccessExpression(m.Parameters[2]);
            m.LocalVars.Add(validNexts);
            m.Statements.Add(
                new ExpressionStatement(
                    new AssignmentExpression(
                        linksToEndAccess,
                        new BooleanLiteralExpression(false))));
            SwitchStatement sw = new SwitchStatement();
            m.Statements.Add(sw);
            sw.Condition = new CastExpression(enu, new ParameterAccessExpression(m.Parameters[0])); //currentState
            foreach (Node node in def.Nodes)
            {
                SwitchCase c = new SwitchCase();
                c.CaseLabels.Add(new FieldAccessExpression(null, fieldsByNode[node]));
                sw.Cases.Add(c);
                foreach (Node next in node.NextNodes)
                {
                    if (next == def.end)
                    {
                        c.Statements.Add(
                            new ExpressionStatement(
                                new AssignmentExpression(
                                    linksToEndAccess,
                                    new BooleanLiteralExpression(true))));
                    }
                    else
                    {
                        CharClass cc = BuildCharClassFromNode(next);
                        if (def.IgnoreCase)
                        {
                            cc = cc.GetIgnoreCase();
                        }
                        c.Statements.Add(
                            new IfStatement(
                                BuildConditionalFromCharClass(cc, m.Parameters[1]), // Parameters[1] --> ch
                                new ExpressionStatement(
                                    new MethodCallExpression(
                                        validNextsAccess,
                                        SystemType.GetSystemType(typeof(List<int>)).GetMethodByName("Add"),
                                        new CastExpression(
                                            SystemTypes.Int32,
                                            new FieldAccessExpression(null, fieldsByNode[next]))))));
                    }
                }

                c.Statements.Add(new BreakStatement());
            }
            sw.Default.Statements.Add(
                new ThrowStatement(
                    new NewExpression(
                        SystemType.GetSystemType(typeof(InvalidOperationException)),
                        new StringLiteralExpression("Invalid state"))));

            m.Statements.Add(
                new ReturnStatement(
                    new MethodCallExpression(
                        validNextsAccess,
                        validNexts.Type.GetMethodByName("ToArray"))));

            return m;
        }

    }
}
