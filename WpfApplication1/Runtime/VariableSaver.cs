using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

using HandGestures.HandHandling;

namespace HandGestures.Runtime
{
    class VariableSaver
    {
        public static void Save(Dictionary<String,Variable> vars, String file)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\" + file)) File.Delete(Environment.CurrentDirectory + "\\" + file);
            String res = "";
            foreach (var v in vars)
            {
                res += v.Key + "@" + v.Value.Value + "@" + v.Value.Type.ToString() + "\r\n";
            }
            File.WriteAllText(Environment.CurrentDirectory + "\\" + file, res);
        }
        public static Dictionary<String, Variable> Load(String file)
        {
            String[] lines = File.ReadAllLines(Environment.CurrentDirectory+"\\"+file);
            Dictionary<String, Variable> res = new Dictionary<String, Variable>();

            foreach (var l in lines)
            {
                String[] data = l.Split(new String[] { "@" }, StringSplitOptions.None);
                if (data.Length != 3) continue;

                String name = data[0];
                Type type = ParseType(data[2]);
                Object value = ParseValue(data[1], type);

                if (!res.ContainsKey(name)) res.Add(name, new Variable(type, value));
            }

            return res;
        }
        public static void LoadToController(BaseController controller, String file)
        {
            var vars = Load(file);
            foreach (var v in vars)
            {
                controller.UpdateVar(v.Key, v.Value);
            }
        }
        public static void SafeLoadToController(BaseController controller, String file)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\" + file)) LoadToController(controller, file);
            controller.SaveVars(file);
        }
        public static Dictionary<String,Variable> SafeLoad(Dictionary<String,Variable> vars, String file)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\" + file))
            {
                var varsTmp = Load(file);
                foreach (var v in varsTmp)
                {
                    if (vars.ContainsKey(v.Key)) vars[v.Key] = v.Value;
                }
            }
            Save(vars, file);
            return vars;
        }
        public static Type ParseType(String type)
        {
            switch (type.ToLower())
            {
                case "system.int32": return typeof(int);
                case "system.single": return typeof(float);
                case "system.double": return typeof(double);
                case "system.boolean": return typeof(bool);
                case "system.drawing.point": return typeof(Point);
            }
            return null;
        }
        public static Object ParseValue(String value, Type type)
        {
            PointConverter pC=new PointConverter();
            try
            {
                if (type == typeof(Point))
                {
                    //{X=100,Y=580}
                    value = value.Replace("{X=", "").Replace("}", "").Replace("Y=", "");
                    return new Point(int.Parse(value.Split(',')[0]), int.Parse(value.Split(',')[1]));
                }
                if (type == typeof(int)) return int.Parse(value);
                if (type == typeof(bool)) return bool.Parse(value);
                if (type == typeof(float)) return float.Parse(value);
                if (type == typeof(double)) return double.Parse(value);
            }
            catch
            {
                return null;
            }
            return value;
        }
    }
}
