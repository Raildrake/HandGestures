using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HandGestures.GUI.Windows
{
    /// <summary>
    /// Logica di interazione per WindowNumericValuePicker.xaml
    /// </summary>
    public partial class WindowNumericValuePicker : Window
    {
        private bool _ok = false;
        private Type _type;

        public bool OK
        {
            get { return _ok; }
            set { _ok = value; }
        }
        public Object Value
        {
            get
            {
                if (_type == typeof(double)) return (double)sldValue.Value;
                if (_type == typeof(float)) return (float)sldValue.Value;
                if (_type == typeof(int)) return (int)sldValue.Value;
                return sldValue.Value;
            }
        }

        public WindowNumericValuePicker()
        {
            InitializeComponent();
        }

        public void Load(Object value, Type type)
        {
            _type = type;
            if (_type == typeof(double))
            {
                sldValue.Value = (double)value;
                sldValue.SmallChange = 0.1;
            }
            if (_type == typeof(float))
            {
                sldValue.Value = (float)value;
                sldValue.SmallChange = 0.1;
            }
            if (_type == typeof(int))
            {
                sldValue.Value = (int)value;
                sldValue.SmallChange = 1;
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            OK = false;
            this.Close();
        }

        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            OK = true;
            this.Close();
        }

    }
}
