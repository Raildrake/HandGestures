using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

using HandGestures.Utility;
using HandGestures.Structs;

namespace HandGestures.Detection
{
    class LEDDetector
    {
        private Bgr _haloColor;
        private Bgr _lightColor;
        private Bgr _upperFilter;
        private double _baseTolerance = 5.0;
        private double _lightTolerance = 25.0;
        private double _minArea = 0.0;
        private double _maxArea = 9999999999.0;
        private double _minLightArea = 0.0;
        private double _maxLightArea = 9999999999.0;
        private double _ledApproximation = 0.001;
        private double _mergeDist = 15.0;

        //INTERNAL
        private Emgu.CV.CvEnum.CHAIN_APPROX_METHOD approximationMethod = Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_TC89_KCOS;
        private Emgu.CV.CvEnum.RETR_TYPE retrieveType = Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_CCOMP;
        private double areaMul = 0;

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
        public Bgr UpperFilter
        {
            get { return _upperFilter; }
            set { _upperFilter = value; }
        }
        public double BaseTolerance
        {
            get { return _baseTolerance; }
            set { _baseTolerance = value; }
        }
        public double LightTolerance
        {
            get { return _lightTolerance; }
            set { _lightTolerance = value; }
        }
        public double MinArea
        {
            get { return _minArea; }
            set { _minArea = value; }
        }
        public double MaxArea
        {
            get { return _maxArea; }
            set { _maxArea = value; }
        }
        public double MinLightArea
        {
            get { return _minLightArea; }
            set { _minLightArea = value; }
        }
        public double MaxLightArea
        {
            get { return _maxLightArea; }
            set { _maxLightArea = value; }
        }
        public double LEDApproximation
        {
            get { return _ledApproximation; }
            set { _ledApproximation = value; }
        }
        public double MergeDist
        {
            get { return _mergeDist; }
            set { _mergeDist = value; }
        }

        public LEDDetector(Bgr haloColor, Bgr lightColor, double tol, double ltol, Bgr upperFilter)
        {
            this.HaloColor = haloColor;
            this.LightColor = lightColor;
            this.BaseTolerance = tol;
            this.LightTolerance = ltol;
            this.UpperFilter = upperFilter;
        }

        public Image<Bgr, Byte> BaseImage(Image<Bgr,Byte> src)
        {
            //remove noise
            //res.SetValue(new Bgr(0, 255, 0), res.InRange(new Bgr(80, 80, 180), new Bgr(160, 160, 255)));
            var res = src.SmoothBlur(3, 3, true).PyrDown().PyrUp();
            //LEDCleaner.Clean(ref res);
            //res=res.
            return res.Sub(UpperFilter);
        }
        public Image<Gray, Byte> RangeImage(Image<Bgr,Byte> src)
        {
            return BaseImage(src).InRange(ColorOperation.Sub(HaloColor, BaseTolerance), ColorOperation.Sum(HaloColor, BaseTolerance));
        }
        public Image<Gray, Byte> LightImage(Image<Bgr,Byte> src)
        {
            return src.InRange(Tolerance.Lower(LightColor,LightTolerance), Tolerance.Upper(LightColor,LightTolerance));
        }

        public Image<Bgr, Byte> Detect(Image<Bgr,Byte> src)
        {
            Image<Gray, Byte> res = RangeImage(src);
            Image<Gray, Byte> resLight = LightImage(src);

            var res2 = BaseImage(src);
            List<Rectangle> _lightRects = new List<Rectangle>();
            using (MemStorage storage = new MemStorage())
            {
                for (Contour<System.Drawing.Point> contours = res.FindContours(approximationMethod,retrieveType,storage); contours != null; contours = contours.HNext)
                {
                    Contour<System.Drawing.Point> currentContour = contours.ApproxPoly(contours.Perimeter * LEDApproximation, storage);
                    if (GeometryExt.Area(currentContour.BoundingRectangle) >= MinArea && GeometryExt.Area(currentContour.BoundingRectangle) <= MaxArea)
                    {
                        //res2.Draw(currentContour.BoundingRectangle, new Bgr(100, 100, 200), 2);
                        _lightRects.Add(currentContour.BoundingRectangle);
                    }
                }
            }
            for (int k = 0; k < _lightRects.Count; k++)
            {
                for (int k2 = 0; k2 < _lightRects.Count; k2++)
                {
                    if (k != k2)
                    {
                        if (GeometryExt.Distance(_lightRects[k], _lightRects[k2]) < MergeDist)
                        {
                            _lightRects[k] = GeometryExt.Join(_lightRects[k], _lightRects[k2]);
                            _lightRects.RemoveAt(k2);
                            k = -1; break;
                        }
                    }
                }
            }
            foreach (var r in _lightRects)
            {
                res2.Draw(r, new Bgr(100, 100, 200), 2);
            }
            using (MemStorage storage = new MemStorage()) //LIGHTS
            {
                for (Contour<System.Drawing.Point> contours = resLight.FindContours(approximationMethod,retrieveType,storage); contours != null; contours = contours.HNext)
                {
                    Contour<System.Drawing.Point> currentContour = contours.ApproxPoly(contours.Perimeter * LEDApproximation, storage);
                    if (GeometryExt.Area(currentContour.BoundingRectangle) >= MinLightArea && GeometryExt.Area(currentContour.BoundingRectangle) <= MaxLightArea)
                    {
                        res2.Draw(currentContour.BoundingRectangle, new Bgr(0, 255, 255), 2);
                    }
                }
            }
            res.Dispose();
            resLight.Dispose();
            return res2;
        }

        public List<LED> GetAll(Image<Bgr,Byte> src, out int lightCount)
        {
            List<LED> leds = new List<LED>();

            var range = RangeImage(src);
            var rangeLight = LightImage(src);

            List<Rectangle> _lightRects = new List<Rectangle>();

            using (MemStorage lightStorage = new MemStorage())
            {
                for (Contour<Point> contours = rangeLight.FindContours(approximationMethod, retrieveType, lightStorage); contours != null; contours = contours.HNext)
                {
                    Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * LEDApproximation, lightStorage);
                    if (GeometryExt.Area(currentContour.BoundingRectangle) >= MinLightArea && GeometryExt.Area(currentContour.BoundingRectangle) <= MaxLightArea)
                    {
                        _lightRects.Add(currentContour.BoundingRectangle);
                    }
                }
                for (int k = 0; k < _lightRects.Count; k++)
                {
                    for (int k2 = 0; k2 < _lightRects.Count; k2++)
                    {
                        if (k != k2)
                        {
                            if (GeometryExt.Distance(_lightRects[k], _lightRects[k2]) < MergeDist)
                            {
                                _lightRects[k] = GeometryExt.Join(_lightRects[k], _lightRects[k2]);
                                _lightRects.RemoveAt(k2);
                                k = -1; break;
                            }
                        }
                    }
                }
                lightCount = _lightRects.Count;

                using (MemStorage storage = new MemStorage())
                {
                    for (Contour<Point> contours = range.FindContours(approximationMethod, retrieveType, storage); contours != null; contours = contours.HNext)
                    {
                        Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * LEDApproximation, storage);
                        if (GeometryExt.Area(currentContour.BoundingRectangle) >= MinArea && GeometryExt.Area(currentContour.BoundingRectangle) <= MaxArea)
                        {
                            LED led = new LED();
                            led.HaloBox = currentContour.BoundingRectangle;
                            led.HaloColor = HaloColor;
                            led.LightColor = LightColor;

                            for (int k = 0; k < _lightRects.Count; k++) //check inside ones with priority
                            {
                                Rectangle r = _lightRects[k];
                                if (GeometryExt.Area(currentContour.BoundingRectangle) >= GeometryExt.Area(r) * areaMul)
                                {
                                    if (Collision.Inside(currentContour.BoundingRectangle, r))
                                    {
                                        led.InsideLightBoxes.Add(r);
                                    }
                                    else if (Collision.Check(currentContour.BoundingRectangle, r))
                                    {
                                        led.CollidingLightBoxes.Add(r);
                                    }
                                }
                            }
                            if (led.MainLightBox != new Rectangle()) //good led found, add it to results
                            {
                                led.InsideLightBoxes = led.InsideLightBoxes.OrderByDescending(x => GeometryExt.Area(x)).ToList();
                                led.CollidingLightBoxes = led.CollidingLightBoxes.OrderByDescending(x => GeometryExt.Area(x)).ToList();
                                leds.Add(led);
                            }
                        }
                    }
                }
            }
            range.Dispose();
            rangeLight.Dispose();
            return leds;
        }
    }
}
