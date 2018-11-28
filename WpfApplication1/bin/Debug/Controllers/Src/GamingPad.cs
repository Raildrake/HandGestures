using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;

using HandGestures.Utility;
using HandGestures.Runtime;

namespace HandGestures.HandHandling
{
    class GamingPad : BaseController
    {
        public GamingPad()
        {
            DeclareVar("Origin", typeof(Point), new Point(70, 200));
            DeclareVar("Size", typeof(Point), new Point(220, 160));
            DeclareVar("Sensivity", typeof(double), 1.0);
            DeclareVar("AlwaysClick", typeof(bool), true);
        }

        protected override Image<Bgr, Byte> OnDraw(Image<Bgr, Byte> src)
        {
            Point origin = (Point)GetVar("Origin");
            Point size = (Point)GetVar("Size");

            Rectangle box = new Rectangle(origin.X, origin.Y, size.X, size.Y);
            GloveHand lastRightSides = HandRecord.LastRightWithBothSides();
            if (lastRightSides!=null && Collision.Inside(box, lastRightSides.SideMiddlePoint))
            {
                src.Draw(box, new Bgr(100, 100, 255), -1);
            }
            else
            {
                src.Draw(box, new Bgr(100, 100, 100), -1);
            }

            return src;
        }

        Point _lastPos = new Point();
        protected override void OnStep()
        {
            Point origin = (Point)GetVar("Origin");
            Point size = (Point)GetVar("Size");
            double sensivity = (double)GetVar("Sensivity");
            bool alwaysClick = (bool)GetVar("AlwaysClick");

            GloveHand lastRightSides = HandRecord.LastRightWithBothSides();
            GloveHand lastRightIndex = HandRecord.LastRightWithIndex();
            if (lastRightSides == null) return;

            Rectangle box = new Rectangle(origin.X, origin.Y, size.X, size.Y);

            if (Collision.Inside(box, lastRightSides.SideMiddlePoint)) //we can move
            {
                int ydiff = (int)((double)(lastRightSides.SideMiddlePoint.Y - origin.Y) / (double)size.Y * (double)Screen.PrimaryScreen.Bounds.Height);
                int xdiff = (int)((double)(size.X - (lastRightSides.SideMiddlePoint.X - origin.X)) / (double)size.X * (double)Screen.PrimaryScreen.Bounds.Width);
                Mouse.SetPos(xdiff, ydiff);
            }
            _lastPos=lastRightSides.SideMiddlePoint;
            if (alwaysClick || Collision.Inside(box, lastRightSides.SideMiddlePoint))
            {
                if (lastRightIndex != null && lastRightIndex.SideFingers.Length == 2)
                {
                    if (GeometryExt.Distance(lastRightIndex.SideFingers[0].LightCenter, lastRightIndex.SideFingers[1].LightCenter) >= size.X / 3.0)
                    {
                        if (Math.Abs(lastRightIndex.SideFingers[0].LightCenter.Y - lastRightIndex.SideFingers[1].LightCenter.Y) <
                            Math.Abs(lastRightIndex.SideFingers[0].LightCenter.X - lastRightIndex.SideFingers[1].LightCenter.X)) // i side sono messi in orizzontale
                        {
                            Mouse.Release(false);
                            if (lastRightIndex.IndexFinger.LightCenter.Y > lastRightIndex.SideMiddlePoint.Y)
                                Mouse.Press(true);
                            else
                                Mouse.Release(true);
                        }
                        else //in verticale
                        {
                            Mouse.Press(false);
                            if (lastRightIndex.IndexFinger.LightCenter.X > lastRightIndex.SideMiddlePoint.X)
                                Mouse.Press(true);
                            else
                                Mouse.Release(true);
                        }
                    }
                }
            }
        }
    }
}
