using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

using HandGestures.Utility;

namespace HandGestures.Detection
{
    class SkinDetector
    {
        private Image<Bgr, Byte> _source;
        private List<Bgr> _skinColors = new List<Bgr>();
        private double _baseTolerance = 25.0;

        public Image<Bgr, Byte> Source
        {
            get { return _source; }
            set { _source = value; }
        }
        public List<Bgr> SkinColors
        {
            get { return _skinColors; }
            set { _skinColors = value; }
        }
        public double BaseTolerance
        {
            get { return _baseTolerance; }
            set { _baseTolerance = value; }
        }

        private List<int> _lastCount = new List<int>();

        public SkinDetector() { }

        public Image<Gray, Byte> RangeSkin()
        {
            Image<Gray, Byte> res = Source.CopyBlank().Convert<Gray, Byte>();

            double extraTolerance = 0.0;
            if (_lastCount.Count>0 && _lastCount.Average() > 0)
            {
                extraTolerance = (double)(-_lastCount.Average()) / 2000.0;
            }

            foreach (var s in SkinColors)
            {
                //res.Add(Source.InRange(Tolerance.Lower(s,25.0), Tolerance.Upper(s,25.0)));
                res += Source.InRange(Tolerance.Lower(s, BaseTolerance + extraTolerance), Tolerance.Upper(s, BaseTolerance + extraTolerance));
            }

            Image<Gray, Byte> res2 = Source.CopyBlank().Convert<Gray, Byte>();
            using (MemStorage storage = new MemStorage())
            {
                for (Contour<System.Drawing.Point> contours = res.FindContours(); contours != null; contours = contours.HNext)
                {
                    //Contour<System.Drawing.Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.01, storage);
                    res2.Draw(contours, new Gray(255), -1);
                }
            }
            AddCount(res2.CountNonzero()[0]);
            return res2;
        }

        private void AddCount(int count)
        {
            _lastCount.Add(count);
            while (_lastCount.Count > 10) _lastCount.RemoveAt(0);
        }

        public static SkinDetector HumanSkin()
        {
            SkinDetector res = new SkinDetector();
            res.SkinColors.Add(new Bgr(199, 194, 222));
            res.SkinColors.Add(new Bgr(120, 109, 181));
            res.SkinColors.Add(new Bgr(165, 165, 217));
            return res;
        }
    }
}
