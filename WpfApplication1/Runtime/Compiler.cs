using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace HandGestures.Runtime
{
    class Compiler
    {
        public static void CompileAll(String srcDir, String outDir)
        {
            foreach (var f in Directory.GetFiles(Environment.CurrentDirectory + "\\" + srcDir))
            {
                String name = f.Substring(f.LastIndexOf("\\")).Replace(".cs", "").Replace("\\", "");
                Compile(f, outDir + "\\" + name + ".dll");
            }
        }
        public static void Compile(String src, String output)
        {
            CSharpCodeProvider csp = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            cp.OutputAssembly = Environment.CurrentDirectory+"\\"+output;
            cp.ReferencedAssemblies.Add("Emgu.CV.dll");
            cp.ReferencedAssemblies.Add("Emgu.CV.UI.dll");
            cp.ReferencedAssemblies.Add("Emgu.Util.dll");
            //cp.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            //cp.ReferencedAssemblies.Add("PresentationCore.dll");
            //cp.ReferencedAssemblies.Add("PresentationFramework.dll");
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            //cp.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
            cp.ReferencedAssemblies.Add("System.Drawing.dll");
            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            //cp.ReferencedAssemblies.Add("System.Xaml.dll");
            cp.ReferencedAssemblies.Add("System.Xml.dll");
            //cp.ReferencedAssemblies.Add("System.Xml.Linq.dll");
            //cp.ReferencedAssemblies.Add("WindowsBase.dll");
            cp.ReferencedAssemblies.Add("mscorlib.dll");
            cp.ReferencedAssemblies.Add("HandGestures.exe");

            cp.WarningLevel = 3;

            cp.CompilerOptions = "/target:library /optimize";
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            System.CodeDom.Compiler.TempFileCollection tfc = new TempFileCollection(Environment.CurrentDirectory + "\\tmp", true);
            CompilerResults cr = new CompilerResults(tfc);

            cr = csp.CompileAssemblyFromFile(cp, new String[] { src });

            if (cr.Errors.Count > 0)
            {
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine(ce.ErrorNumber + ": " + ce.ErrorText);
                }
            }

            System.Collections.Specialized.StringCollection sc = cr.Output;
            foreach (string s in sc)
            {
                Console.WriteLine(s);
            }
        }
    }
}
