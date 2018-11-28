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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HandGestures.GUI
{
    /// <summary>
    /// Logica di interazione per LabelSlider.xaml
    /// </summary>
    public partial class LabelSlider : UserControl
    {
        public event RoutedPropertyChangedEventHandler<double> ValueChanged;

        public double Minimum
        {
            get { return sldValue.Minimum; }
            set { sldValue.Minimum = value; }
        }
        public double Maximum
        {
            get { return sldValue.Maximum; }
            set { sldValue.Maximum = value; }
        }
        public double Value
        {
            get { return sldValue.Value; }
            set { sldValue.Value = value; }
        }
        public double TickFrequency
        {
            get { return sldValue.TickFrequency; }
            set { sldValue.TickFrequency = value; }
        }
        public double SmallChange
        {
            get { return sldValue.SmallChange; }
            set { sldValue.SmallChange = value; }
        }

        public LabelSlider()
        {
            InitializeComponent();
        }

        private void sldValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtValue.Text!=Math.Round(sldValue.Value, 2).ToString())
                txtValue.Text = Math.Round(sldValue.Value, 2).ToString();
            if (ValueChanged != null) ValueChanged(this, e);
        }

        private void txtValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            double val = 0;
            if (!double.TryParse(txtValue.Text, out val)) return;

            if (sldValue.Value != val)
                sldValue.Value = val;
        }
    }
}
