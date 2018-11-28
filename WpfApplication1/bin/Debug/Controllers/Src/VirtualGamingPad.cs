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
    class VirtualGamingPad : BaseController
    {
        public VirtualGamingPad()
        {
            DeclareVar("MiddlePoint", typeof(Point), new Point(320, 300));
            DeclareVar("DeadZone", typeof(float), 60.0f);
            DeclareVar("Sensivity", typeof(double), 0.5);
            DeclareVar("AlwaysClick", typeof(bool), true);
        }

        protected override Image<Bgr, Byte> OnDraw(Image<Bgr, Byte> src)
        {
            Point middlePoint = (Point)GetVar("MiddlePoint");
            float deadZone = (float)GetVar("DeadZone");

            src.Draw(new CircleF(middlePoint, deadZone), new Bgr(100, 100, 100), -1);
            src.Draw(new CircleF(middlePoint, (float)3), new Bgr(0, 0, 255), -1);

            GloveHand lastSides = HandRecord.LastRightWithBothSides();
            if (lastSides == null) return src;

            if (GeometryExt.Distance(lastSides.SideMiddlePoint, middlePoint) > deadZone)
            {
                src.Draw(new LineSegment2D(lastSides.SideMiddlePoint, middlePoint), new Bgr(255, 0, 255), 3);
            }

            return src;
        }
        protected override void OnStep()
        {
            Point middlePoint = (Point)GetVar("MiddlePoint");
            float deadZone = (float)GetVar("DeadZone");
            double sensivity = (double)GetVar("Sensivity");
            bool alwaysClick = (bool)GetVar("AlwaysClick");

            GloveHand lastRightSides = HandRecord.LastRightWithBothSides();
            GloveHand lastRightIndex = HandRecord.LastRightWithIndex();
            if (lastRightSides == null) return;

            double len = GeometryExt.Distance(lastRightSides.SideMiddlePoint, middlePoint);
            Point delta = HandRecord.RightSideMovementDelta(2, 10, 15);

            if (len > deadZone && Math.Abs(delta.X)>=1 && Math.Abs(delta.Y)>=1)
            {
                double xdiff = (-(lastRightSides.SideMiddlePoint.X - middlePoint.X) / len) * (len - deadZone);
                double ydiff = ((lastRightSides.SideMiddlePoint.Y - middlePoint.Y) / len) * (len - deadZone);

                double mxdiff = GeometryExt.Center(Screen.PrimaryScreen.Bounds).X + xdiff * sensivity;
                double mydiff = GeometryExt.Center(Screen.PrimaryScreen.Bounds).Y + ydiff * sensivity;
                //Console.WriteLine(mxdiff+":"+mydiff);
                Console.WriteLine(GeometryExt.Center(Screen.PrimaryScreen.Bounds));
                Mouse.SetPos((int)mxdiff, (int)mydiff);
            }
            if (alwaysClick || len <= deadZone)
            {
                if (lastRightIndex != null && lastRightIndex.SideFingers.Length == 2)
                {
                    if (GeometryExt.Distance(lastRightIndex.SideFingers[0].LightCenter, lastRightIndex.SideFingers[1].LightCenter) >= deadZone)
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
                            Mouse.Release(true);
                            if (lastRightIndex.IndexFinger.LightCenter.X < lastRightIndex.SideMiddlePoint.X)
                                Mouse.Press(false);
                            else
                                Mouse.Release(false);
                        }
                    }
                }
            }
        }
    }
}
