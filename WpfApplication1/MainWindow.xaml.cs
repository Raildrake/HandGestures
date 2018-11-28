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
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

using HandGestures.Detection;
using HandGestures.Utility;
using HandGestures.HandHandling;
using HandGestures.Runtime;
using HandGestures.GUI.Windows;


namespace HandGestures
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Capture _capture = null;
        DispatcherTimer clkRef = new DispatcherTimer();
        HandReader handReader;
        GestureRecording gestureRecord = new GestureRecording();
        BaseController activeController;
        Dictionary<String, BaseController> controllerList;
        Dictionary<String, Variable> globalVars = new Dictionary<String, Variable>();
        Dictionary<Keys, bool> keyMap = new Dictionary<Keys, bool>();
        WindowPreview wPrev;

        bool varsInitialized = false;

        DateTime startingTime;
        int totalTicks = 0;

        public MainWindow()
        {
            InitializeComponent();


            _capture = new Capture(0);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_AUTO_EXPOSURE, 0);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_SATURATION, 100);
            _capture.Start();

            DeclareVar("Exposure", typeof(double), -5.0);
            DeclareVar("Brightness", typeof(double), 100.0);
            DeclareVar("Contrast", typeof(double), 0.0);
            DeclareVar("Tolerance", typeof(double), 5.0);
            DeclareVar("LightTolerance", typeof(double), 10.0);
            DeclareVar("IndexR", typeof(int), 255);
            DeclareVar("IndexG", typeof(int), 0);
            DeclareVar("IndexB", typeof(int), 0); 
            DeclareVar("SideR", typeof(int), 0);
            DeclareVar("SideG", typeof(int), 0);
            DeclareVar("SideB", typeof(int), 255);

            globalVars=VariableSaver.SafeLoad(globalVars, @"Settings\Settings.cfg");
            varsInitialized = true;

            sldExposure.Value = (double)GetVar("Exposure");
            sldBrightness.Value = (double)GetVar("Brightness");
            sldContrast.Value = (double)GetVar("Contrast");

            sldTolerance.Value = (double)GetVar("Tolerance");
            sldLightTolerance.Value = (double)GetVar("LightTolerance");
            txtIndexR.Text = GetVar("IndexR").ToString();
            txtIndexG.Text = GetVar("IndexG").ToString();
            txtIndexB.Text = GetVar("IndexB").ToString();
            txtSideR.Text = GetVar("SideR").ToString();
            txtSideG.Text = GetVar("SideG").ToString();
            txtSideB.Text = GetVar("SideB").ToString();

            UpdateReader();

            wPrev = new WindowPreview();
            wPrev.Show();
            System.Drawing.Rectangle screen=System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            wPrev.Top = screen.Height - wPrev.Height - 45;
            wPrev.Left = screen.Width - wPrev.Width;

            startingTime = DateTime.Now;

            clkRef.Interval = new TimeSpan(0, 0, 0, 0, 15);
            clkRef.Tick += new EventHandler(clkRef_Tick);
            clkRef.Start();
            
            LoadControllers();

            LoadConfigs();

            for (int k = 0; k < 255;k++ )
            {
                keyMap.Add((Keys)k, false);
            }

            InputManager.KeyboardHook.KeyDown += new InputManager.KeyboardHook.KeyDownEventHandler(KeyboardHook_KeyDown);
            InputManager.KeyboardHook.KeyUp += new InputManager.KeyboardHook.KeyUpEventHandler(KeyboardHook_KeyUp);
            InputManager.KeyboardHook.InstallHook();
        }

        void KeyboardHook_KeyUp(int vkCode)
        {
            Keys k = (Keys)vkCode;

            if (keyMap[Keys.LMenu] && keyMap[Keys.LControlKey] && keyMap[Keys.D1])
            {
                if (cmbControllers.Items.Count > 0)
                {
                    if (cmbControllers.SelectedIndex <= 0) cmbControllers.SelectedIndex = cmbControllers.Items.Count - 1;
                    else cmbControllers.SelectedIndex--;
                }
            }
            else if (keyMap[Keys.LMenu] && keyMap[Keys.LControlKey] && keyMap[Keys.D2])
            {
                if (cmbControllers.Items.Count > 0)
                {
                    if (cmbControllers.SelectedIndex >= cmbControllers.Items.Count - 1) cmbControllers.SelectedIndex = 0;
                    else cmbControllers.SelectedIndex++;
                }
            }
            else if (keyMap[Keys.LMenu] && keyMap[Keys.LControlKey] && keyMap[Keys.D3])
            {
                chkInputEnable.IsChecked = !chkInputEnable.IsChecked;
            } 

            keyMap[k] = false;
        }
        void KeyboardHook_KeyDown(int vkCode)
        {
            Keys k = (Keys)vkCode;
            keyMap[k] = true;
        }

        public bool DeclareVar(String name, Type type, Object startingValue)
        {
            if (globalVars.ContainsKey(name)) return false;
            globalVars.Add(name, new Variable(type, startingValue));
            return true;
        }
        public Object GetVar(String name)
        {
            if (!globalVars.ContainsKey(name)) return null;
            return globalVars[name].Value;
        }
        public void SetVar(String name, Object value)
        {
            if (!globalVars.ContainsKey(name)) return;
            globalVars[name].Value = value;
        }
        public void SaveVars()
        {
            VariableSaver.Save(globalVars, @"Settings\Settings.cfg");
        }

        public void UpdateCam()
        {
            if (_capture == null) return;
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_BRIGHTNESS, 100.0);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_EXPOSURE, (double)GetVar("Exposure"));
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_CONTRAST, (double)GetVar("Contrast"));
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_BRIGHTNESS, (double)GetVar("Brightness"));
        }
        public void UpdateReader()
        {
            double tol = (double)GetVar("Tolerance");
            double lightTol = (double)GetVar("LightTolerance");
            int indexR = (int)GetVar("IndexR");
            int indexG = (int)GetVar("IndexG");
            int indexB = (int)GetVar("IndexB");
            int sideR = (int)GetVar("SideR");
            int sideG = (int)GetVar("SideG");
            int sideB = (int)GetVar("SideB");

            handReader = new HandReader(new Bgr(255, 255, 255),
                                        new Bgr(indexB, indexG, indexR), new Bgr(sideB, sideG, sideR),
                                        new Bgr(255-indexB, 255-indexG, 255-indexR), new Bgr(255-sideB, 255-sideG, 255-sideR),
                                        tol, lightTol);
        }

        private void LoadControllers()
        {
            foreach (var f in System.IO.Directory.GetFiles(Environment.CurrentDirectory + "\\Controllers")) System.IO.File.Delete(f);
            Compiler.CompileAll(@"\Controllers\Src", @"\Controllers");
            controllerList = Loader.LoadAll("Controllers");
            cmbControllers.Items.Add("No Input");
            foreach (var a in controllerList)
            {
                a.Value.HandRecord = gestureRecord;
                cmbControllers.Items.Add(a.Key);
                VariableSaver.SafeLoadToController(a.Value, @"\Settings\Controllers\" + a.Key + ".cfg");
            }
            cmbControllers.SelectedIndex = 0;
        }

        void clkRef_Tick(object sender, EventArgs e)
        {
            try
            {
                Tick();
            }
            catch
            {
            }
        }
        private void Tick()
        {
            Image<Bgr, Byte> frame = _capture.QueryFrame().Copy();

            GloveHand h = handReader.Read(frame);
            Image<Bgr, Byte> outt = frame.Copy();
            if (h != null)
            {
                gestureRecord.RecordRight(h);

                if (activeController != null)
                {
                    bool canInput = false;

                    this.Dispatcher.Invoke((Action)delegate
                    {
                        canInput = (bool)chkInputEnable.IsChecked;
                    });

                    if (canInput) activeController.Step();

                    activeController.Draw(ref outt);
                    h.DrawHand(ref outt);
                }
                else
                {
                    h.DrawHand(ref outt);
                }
            }
            var outt2 = outt.Flip(Emgu.CV.CvEnum.FLIP.HORIZONTAL);
            this.Dispatcher.Invoke((Action)delegate
            {
                picOut.Source = BitmapSourceConvert.ToBitmapSource(outt2);
                wPrev.picPreview.Source = picOut.Source;
            });
            outt.Dispose();
            outt2.Dispose();

            frame.Dispose();

            double time = (DateTime.Now - startingTime).TotalSeconds;
            totalTicks++;

            this.Dispatcher.Invoke((Action)delegate
            {
                Title = "Light Count: " + handReader.LightCount + " [FPS:" + Math.Round((double)totalTicks/time, 2) + "]";
                wPrev.Title = cmbControllers.SelectedItem.ToString();
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _capture.Stop();
            wPrev.Close();
        }

        private void cmbControllers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbControllers.SelectedIndex == -1) { activeController = null; return; }
            String item=cmbControllers.SelectedItem.ToString();

            if (!controllerList.ContainsKey(item)) { activeController = null; return; }

            activeController = controllerList[item];

            listControllerSettings.ItemsSource = null;
            listControllerSettings.Items.Clear();
            listControllerSettings.ItemsSource = activeController.GetAllVars();
        }

        private void listControllerSettings_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listControllerSettings.SelectedIndex == -1) return;
            var selected = (KeyValuePair<String,Variable>)listControllerSettings.SelectedItem;
            Type type = selected.Value.Type;
            if (type == typeof(double) || type == typeof(float) || type==typeof(int))
            {
                WindowNumericValuePicker wnvp = new WindowNumericValuePicker();
                wnvp.Load(selected.Value.Value,selected.Value.Type);
                wnvp.ShowDialog();
                if (!wnvp.OK) return;
                selected.Value.Value = wnvp.Value;
                SaveController(activeController);
            }

            listControllerSettings.ItemsSource = null;
            listControllerSettings.Items.Clear();
            listControllerSettings.ItemsSource = activeController.GetAllVars();
        }

        public void SaveController(BaseController controller)
        {
            foreach (var a in controllerList)
            {
                if (a.Value == controller)
                {
                    controller.SaveVars(@"\Settings\Controllers\" + a.Key + ".cfg");
                }
            }
        }

        private void cmbRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRoom.SelectedIndex == -1) return;

            String configName = cmbRoom.SelectedItem.ToString();
            txtConfigName.Text = configName;

            globalVars = VariableSaver.SafeLoad(globalVars, @"\Settings\Rooms\" + configName + ".cfg");

            sldExposure.Value = (double)GetVar("Exposure");
            sldBrightness.Value = (double)GetVar("Brightness");
            sldContrast.Value = (double)GetVar("Contrast");
        }

        private void cmdSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            if (txtConfigName.Text.Trim().Length == 0)
            {
                System.Windows.MessageBox.Show("Pick a name!");
                return;
            }

            Dictionary<String, Variable> config = new Dictionary<String, Variable>();
            config.Add("Exposure", globalVars["Exposure"]);
            config.Add("Contrast", globalVars["Contrast"]);
            config.Add("Brightness", globalVars["Brightness"]);
            VariableSaver.Save(config, @"Settings\Rooms\" + txtConfigName.Text + ".cfg");
            LoadConfigs();
        }
        public void LoadConfigs()
        {
            cmbRoom.Items.Clear();
            foreach (var f in System.IO.Directory.GetFiles(Environment.CurrentDirectory + @"\Settings\Rooms\"))
            {
                String name = f.Substring(f.LastIndexOf("\\")).Replace("\\", "").Replace(".cfg", "");
                cmbRoom.Items.Add(name);
            }
            cmbRoom.SelectedIndex = -1;
        }

        private void sldExposure_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!varsInitialized) return;
            SetVar("Exposure", sldExposure.Value);
            SaveVars();
            UpdateCam();
        }
        private void sldBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!varsInitialized) return;
            SetVar("Brightness", sldBrightness.Value);
            SaveVars();
            UpdateCam();
        }
        private void sldContrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!varsInitialized) return;
            SetVar("Contrast", sldContrast.Value);
            SaveVars();
            UpdateCam();
        }
        private void sldTolerance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!varsInitialized) return;
            SetVar("Tolerance", sldTolerance.Value);
            SaveVars();
            UpdateReader();
        }

        private void sldLightTolerance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!varsInitialized) return;
            SetVar("LightTolerance", sldLightTolerance.Value);
            SaveVars();
            UpdateReader();
        }

        private void IndexColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            int red = 0;
            int green = 0;
            int blue = 0;
            if (!int.TryParse(txtIndexR.Text, out red) || !int.TryParse(txtIndexG.Text, out green) || !int.TryParse(txtIndexB.Text, out blue)) return;

            rectIndexColor.Fill = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));

            SetVar("IndexR", red);
            SetVar("IndexG", green);
            SetVar("IndexB", blue);
            SaveVars();
            UpdateReader();
        }

        private void SideColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            int red = 0;
            int green = 0;
            int blue = 0;
            if (!int.TryParse(txtSideR.Text, out red) || !int.TryParse(txtSideG.Text, out green) || !int.TryParse(txtSideB.Text, out blue)) return;

            rectSideColor.Fill = new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));

            SetVar("SideR", red);
            SetVar("SideG", green);
            SetVar("SideB", blue);
            SaveVars();
            UpdateReader();
        }
    }
}
