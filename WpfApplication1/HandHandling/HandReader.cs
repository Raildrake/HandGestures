using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

using HandGestures.Detection;
using HandGestures.Utility;
using HandGestures.Structs;

namespace HandGestures.HandHandling
{
    struct LEDBoxTuple
    {
        public LED led;
        public Rectangle box;

        public LEDBoxTuple(LED led, Rectangle box)
        {
            this.led = led;
            this.box = box;
        }
    }
    class HandReader
    {
        private Bgr _lightColor;
        private Bgr _indexColor;
        private Bgr _sideColor;
        private Bgr _indexFilter;
        private Bgr _sideFilter;
        private double _tolerance;
        private double _lightTolerance;

        public Bgr LightColor
        {
            get { return _lightColor; }
            set { _lightColor = value; }
        }
        public Bgr IndexColor
        {
            get { return _indexColor; }
            set { _indexColor = value; }
        }
        public Bgr SideColor
        {
            get { return _sideColor; }
            set { _sideColor = value; }
        }
        public Bgr IndexFilter
        {
            get { return _indexFilter; }
            set { _indexFilter = value; }
        }
        public Bgr SideFilter
        {
            get { return _sideFilter; }
            set { _sideFilter = value; }
        }
        public double Tolerance
        {
            get { return _tolerance; }
            set { _tolerance = value; }
        }
        public double LightTolerance
        {
            get { return _lightTolerance; }
            set { _lightTolerance = value; }
        }

        //OUT
        private int _lightCount;

        public int LightCount
        {
            get { return _lightCount; }
            set { _lightCount = value; }
        }


        public LEDDetector indexLEDDetect;
        public LEDDetector sideLEDDetect;

        public HandReader(Bgr lightColor, Bgr indexColor, Bgr sideColor, Bgr indexFilter, Bgr sideFilter, double tolerance, double lightTolerance)
        {
            this.LightColor = lightColor;
            this.IndexColor = indexColor;
            this.SideColor = sideColor;
            this.IndexFilter = indexFilter;
            this.SideFilter = sideFilter;
            this.Tolerance = tolerance;
            this.LightTolerance = lightTolerance;
            Update();
        }

        public void Update()
        {
            indexLEDDetect = new LEDDetector(IndexColor, LightColor, Tolerance, LightTolerance, IndexFilter);
            sideLEDDetect = new LEDDetector(SideColor, LightColor, Tolerance, LightTolerance, SideFilter);
        }

        public GloveHand Read(Image<Bgr,Byte> src)
        {
            //double areaDiv = 55.0;

            var redLeds = indexLEDDetect.GetAll(src,out _lightCount).OrderByDescending(x => GeometryExt.Area(x.HaloBox)).ToList();
            var blueLeds = sideLEDDetect.GetAll(src,out _lightCount).OrderByDescending(x => GeometryExt.Area(x.HaloBox)).ToList();

            if (redLeds.Count == 0 && blueLeds.Count == 0) return null;


            List<LEDBoxTuple> ledBoxes = new List<LEDBoxTuple>();
            foreach (var l in blueLeds)
            {
                foreach (var box in l.LightBoxes)
                {
                    ledBoxes.Add(new LEDBoxTuple(l, box));
                }
            }
            ledBoxes = ledBoxes.OrderByDescending(x => GeometryExt.Area(x.box)).ToList();

            GloveHand res = new GloveHand();
            if (redLeds.Count >= 1)
            {
                res.IndexFinger = FingerLED.FromLED(redLeds[0], redLeds[0].MainLightBox);
            }
            double avgBigArea = 0.0;
            if (res.IndexFinger != null) avgBigArea = GeometryExt.Area(res.IndexFinger.LightBox);
            if (ledBoxes.Count > 0 && GeometryExt.Area(ledBoxes[0].box) > avgBigArea) GeometryExt.Area(ledBoxes[0].box);

            if (blueLeds.Count == 0) res.SideFingers = new FingerLED[0]; //non si vedono led blu
            else
            {
                List<FingerLED> splitLEDS = new List<FingerLED>();
                for (int k = 0; k < ledBoxes.Count; k++)
                {
                    if (res.IndexFinger == null || GeometryExt.Distance(ledBoxes[k].box,res.IndexFinger.LightBox)>=20.0)
                    {
                        if (splitLEDS.Count == 0 || GeometryExt.Distance(splitLEDS[0].LightBox, ledBoxes[k].box) >= 10.0)
                        {
                            splitLEDS.Add(FingerLED.FromLED(ledBoxes[k].led, ledBoxes[k].box));
                        }
                    }
                    if (splitLEDS.Count == 2) break;
                }
                /*while (splitLEDS.Count < 2 && res.IndexFinger != null)
                {
                    splitLEDS.Add(FingerLED.FromLED(redLeds[0], redLeds[0].MainLightBox));
                }*/
                res.SideFingers = splitLEDS.ToArray();
            }
            return res;
        }
    }
}
