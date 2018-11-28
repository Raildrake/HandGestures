using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;

using HandGestures.HandHandling;

namespace HandGestures.Runtime
{
    public class Loader
    {
        public static Dictionary<String, BaseController> LoadAll(String dir)
        {
            Dictionary<String, BaseController> res = new Dictionary<String, BaseController>();
            foreach (var f in Directory.GetFiles(Environment.CurrentDirectory + "\\" + dir))
            {
                String name = f.Substring(f.LastIndexOf("\\")).Replace(".dll", "").Replace("\\", "");
                res.Add(name, Load(f, name));
            }

            return res;
        }
        public static BaseController Load(String file, String name)
        {
            AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "true";
            
            byte[] rawAssembly = loadFile(file);
            //Assembly assembly = AppDomain.CurrentDomain.Load(rawAssembly);
            Assembly assembly = Assembly.Load(rawAssembly);
            Type t = assembly.GetType("HandGestures.HandHandling."+name);
            Type t2 = assembly.GetType("HandGestures.HandHandling.BaseController");
            Type t3 = typeof(BaseController);
            bool cc = t2 == t3;
            //BaseController resClass = (BaseController)Activator.CreateInstance(t);
            BaseController resClass = (BaseController)assembly.CreateInstance("HandGestures.HandHandling." + name, true);
            return resClass;
        }
        private static byte[] loadFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            fs = null;
            return buffer;
        }
    }
}
