using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using MetaphysicsIndustries.Build;

namespace MetaphysicsIndustries.Giza
{
    public class CSharpCompiler
    {
        private static readonly char[] _chars = { 
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 
            '_', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', };

        private static CSharpCodeProvider _cscp = new CSharpCodeProvider();

        private static Random _random = new Random();

        private static CSharpRenderer _renderer = new CSharpRenderer();

        public Assembly CompileAssemblyFromSource(CompilerParameters options, params string[] sources)
        {
            CompilerResults results;
            Assembly[] a;
            string s;

            a = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly an in a)
            {
                s = an.CodeBase;
                s = (new System.Uri(s)).LocalPath;
                options.ReferencedAssemblies.Add(s);
            }

            options.OutputAssembly = "z"+GenerateRandomIdentifier();

            results = _cscp.CompileAssemblyFromSource(options, sources);

            if ((results.Errors != null && results.Errors.Count > 0) || results.CompiledAssembly == null)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError err in results.Errors)
                {
                    sb.AppendLine(err.ToString());
                }

                throw new InvalidOperationException("There were errors while compiling: \r\n" +
                    sb.ToString());
            }

            return results.CompiledAssembly;
        }

        public static readonly string[] DefaultUsings = new string[] { "System", "System.Collections.Generic", "MetaphysicsIndustries.Collections" };
        public Assembly CompileAssemblyFromSource(string source)
        {
            return CompileAssemblyFromSource(source, new string[0]);
        }
        public Assembly CompileAssemblyFromSource(string source, string[] usings)
        {
            CompilerParameters options = new CompilerParameters();

            options = new CompilerParameters();
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;

            string s = string.Empty;
            foreach (string use in usings)
            {
                s += "using " + use + ";\r\n";
            }

            return CompileAssemblyFromSource(options, s + source);
        }

        public MemberInfo[] CompileTypeMembers(string source, string[] usings)
        {
            string className = GenerateRandomIdentifier();
            string str =
                "public class " + className +
                "{" +
                    source + "\r\n" + 
                "}";

            Assembly a = CompileAssemblyFromSource(str,usings);

            return a.GetType(className).GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public MethodInfo CompileMethodBody(string source, Parameter[] parameters, string[] usings)
        {
            Build.Type returnType2 = SystemTypes.Void;

            return CompileMethodBody(source, returnType2, parameters, usings);
        }
        public MethodInfo CompileMethodBody(string source, Build.Type returnType, Parameter[] parameters, string[] usings)
        {
            string methodName = GenerateRandomIdentifier();
            string str =
                "public static " +
                _renderer.RenderTypeReference(returnType, new CSharpRenderContext()) +
                " " + methodName + "(" +
                string.Join(", ", RenderParameters(parameters)) + ")" +
                "{" +
                    source + "\r\n" +
                "}";

            return (MethodInfo)((CompileTypeMembers(str,usings))[0]);
        }

        //private Parameter[] ConvertParameters(ParameterInfo[] parameters)
        //{
        //    return Array.ConvertAll<ParameterInfo, Parameter>(parameters, ConvertParameter);
        //}
        //private Parameter ConvertParameter(ParameterInfo parameter)
        //{
        //    return new Parameter(SystemType.GetSystemType(parameter.ParameterType), parameter.Name);
        //}
        private string[] RenderParameters(Parameter[] parameters)
        {
            return Array.ConvertAll<Parameter, string>(parameters, RenderParameter);
        }
        private static string RenderParameter(Parameter parameter)
        {
            return _renderer.RenderParameter(parameter, new CSharpRenderContext());
        }

        private string GenerateRandomIdentifier()
        {
            char[] letters = new char[10];
            int i;

            letters[0] = '_';
            letters[1] = '_';
            letters[2] = _chars[_random.Next(_chars.Length - 10)];
            for (i = 3; i < letters.Length; i++)
            {
                letters[i] = _chars[_random.Next(_chars.Length)];
            }

            return new string(letters);
        }
    }
}
