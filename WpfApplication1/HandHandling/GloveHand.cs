using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

using HandGestures.Utility;
using HandGestures.Structs;

namespace HandGestures.HandHandling
{
    public class GloveHand
    {
        private FingerLED _indexFinger;
        private FingerLED[] _sideFingers = new FingerLED[0];

        public FingerLED IndexFinger
        {
            get { return _indexFinger; }
            set { _indexFinger = value; }
        }
        public FingerLED[] SideFingers
        {
            get { return _sideFingers; }
            set { _sideFingers = value; }
        }

        //READ-ONLY DERIVED
        public FingerLED[] Fingers
        {
            get
            {
                List<FingerLED> res = new List<FingerLED>();
                if (IndexFinger != null) res.Add(IndexFinger);
                if (SideFingers != null) res.AddRange(SideFingers);
                return res.ToArray();
            }
        }
        public Point MiddlePoint
        {
            get
            {
                if (Fingers.Length == 1) return Fingers[0].LightCenter;
                if (Fingers.Length == 2) return GeometryExt.Middle(Fingers[0].LightCenter, Fingers[1].LightCenter);
                if (Fingers.Length == 3) return GeometryExt.Middle(Fingers[0].LightCenter, Fingers[1].LightCenter, Fingers[2].LightCenter);
                return new Point();
            }
        }
        public Point SideMiddlePoint
        {
            get
            {
                if (SideFingers.Length == 1) return SideFingers[0].LightCenter;
                if (SideFingers.Length == 2) return GeometryExt.Middle(SideFingers[0].LightCenter, SideFingers[1].LightCenter);
                return new Point();
            }
        }

        public void DrawHand(ref Image<Bgr,Byte> src)
        {
            if (IndexFinger != null)
            {
                src.Draw(new CircleF(IndexFinger.LightCenter, 6), new Bgr(0, 0, 255), 3);
                src.Draw(new CircleF(IndexFinger.LightCenter, 12), new Bgr(0, 0, 200), 3);
            }
            foreach (var s in SideFingers)
            {
                src.Draw(new CircleF(s.LightCenter, 6), new Bgr(255, 0, 0), 3);
                src.Draw(new CircleF(s.LightCenter, 12), new Bgr(200, 0, 0), 3);
            }
        }
    }
}
