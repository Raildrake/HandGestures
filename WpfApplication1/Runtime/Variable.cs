using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandGestures.Runtime
{
    public class Variable
    {
        private Type _type;
        private Object _value;

        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public String ShortType { get { return Type.Name; } }
        public Object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public Variable(Type type, Object value)
        {
            this.Type = type;
            this.Value = value;
        }
    }
}
