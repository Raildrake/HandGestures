using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV;
using Emgu.CV.Structure;

using HandGestures.Utility;
using HandGestures.Runtime;

namespace HandGestures.HandHandling
{
    public abstract class BaseController
    {
        private GestureRecording _handRecord;
        private Dictionary<String, Variable> _variables = new Dictionary<String, Variable>();

        public GestureRecording HandRecord
        {
            get { return _handRecord; }
            set { _handRecord = value; }
        }
        private Dictionary<String, Variable> Variables
        {
            get { return _variables; }
            set { _variables = value; }
        }

        public BaseController() { }

        public void Draw(ref Image<Bgr, Byte> src) { OnDraw(ref src); }
        public void Step() { OnStep(); }

        protected abstract void OnDraw(ref Image<Bgr, Byte> src);
        protected abstract void OnStep();

        public bool DeclareVar(String name, Type type, Object startingValue)
        {
            if (Variables.ContainsKey(name)) return false;
            Variables.Add(name, new Variable(type,startingValue));
            return true;
        }
        public Object GetVar(String name)
        {
            if (!Variables.ContainsKey(name)) return null;
            return Variables[name].Value;
        }
        public void SetVar(String name, Object value)
        {
            if (!Variables.ContainsKey(name)) return;
            Variables[name].Value = value;
        }
        public void UpdateVar(String name, Variable v)
        {
            if (Variables.ContainsKey(name)) Variables[name] = v;
            //else Variables.Add(name, v);
        }
        public void SaveVars(String file)
        {
            VariableSaver.Save(Variables, file);
        }
        public Dictionary<String, Variable> GetAllVars()
        {
            return Variables;
        }
    }
}
