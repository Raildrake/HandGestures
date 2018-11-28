using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Emgu.CV.Structure;

using HandGestures.Utility;
using HandGestures.Structs;

namespace HandGestures.HandHandling
{
    public class FingerLED
    {
        private Rectangle _haloBox;
        private Rectangle _lightBox = new Rectangle();
        private Bgr _haloColor;
        private Bgr _lightColor;

        public Rectangle HaloBox
        {
            get { return _haloBox; }
            set { _haloBox = value; }
        }
        public Rectangle LightBox
        {
            get { return _lightBox; }
            set { _lightBox = value; }
        }
        public Bgr HaloColor
        {
            get { return _haloColor; }
            set { _haloColor = value; }
        }
        public Bgr LightColor
        {
            get { return _lightColor; }
            set { _lightColor = value; }
        }

        //READ-ONLY DERIVED
        public Point HaloCenter
        {
            get { return GeometryExt.Center(HaloBox); }
        }
        public Point LightCenter
        {
            get { return GeometryExt.Center(LightBox); }
        }


        public static FingerLED FromLED(LED led, Rectangle lightBox)
        {
            FingerLED res = new FingerLED();
            res.HaloBox = led.HaloBox;
            res.LightBox = lightBox;
            res.HaloColor = led.HaloColor;
            res.LightColor = led.LightColor;
            return res;
        }
    }
}
