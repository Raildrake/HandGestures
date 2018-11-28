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
    class VirtualPad : BaseController
    {
        public VirtualPad()
        {
            DeclareVar("MiddlePoint", typeof(Point), new Point(320, 300));
            DeclareVar("DeadZone", typeof(float), 35.0f);
            DeclareVar("Sensivity", typeof(double), 5.0);
            DeclareVar("AlwaysClick", typeof(bool), true);
            DeclareVar("Acceleration", typeof(double), 1.5);
        }

        protected override void OnDraw(ref Image<Bgr, Byte> src)
        {
            Point middlePoint = (Point)GetVar("MiddlePoint");
            float deadZone = (float)GetVar("DeadZone");

            src.Draw(new CircleF(middlePoint, deadZone), new Bgr(100, 100, 100), -1);
            src.Draw(new CircleF(middlePoint, (float)3), new Bgr(0, 0, 255), -1);

            GloveHand lastSides = HandRecord.LastRightWithBothSides();
            if (lastSides == null) return;

            if (GeometryExt.Distance(lastSides.SideMiddlePoint, middlePoint) > deadZone)
            {
                src.Draw(new LineSegment2D(lastSides.SideMiddlePoint, middlePoint), new Bgr(255, 0, 255), 3);
            }

            if (lastSides.SideFingers.Length == 2)
            {
                System.Drawing.Point p1 = lastSides.SideFingers[0].LightCenter;
                System.Drawing.Point p2 = lastSides.SideFingers[1].LightCenter;
                float smallC = ((float)GeometryExt.Distance(p1, p2) / 2) - 8;
                float bigC = ((float)GeometryExt.Distance(p1, p2) / 2) + 8;
                if (smallC < 0) smallC = 0;
                if (bigC < 0) bigC = 0;
                src.Draw(new CircleF(lastSides.SideMiddlePoint, smallC), new Bgr(0, 255, 0), 3);
                src.Draw(new CircleF(lastSides.SideMiddlePoint, bigC), new Bgr(0, 200, 0), 3);
                src.Draw(new CircleF(lastSides.SideMiddlePoint, 5), new Bgr(255, 0, 255), -1);
            }
        }
        protected override void OnStep()
        {
            Point middlePoint = (Point)GetVar("MiddlePoint");
            float deadZone = (float)GetVar("DeadZone");
            double sensivity = (double)GetVar("Sensivity");
            double acceleration = (double)GetVar("Acceleration");
            bool alwaysClick = (bool)GetVar("AlwaysClick");

            GloveHand lastRightSides = HandRecord.LastRightWithBothSides();
            GloveHand lastRightIndex = HandRecord.LastRightWithIndex();
            if (lastRightSides == null) return;

            double len = GeometryExt.Distance(lastRightSides.SideMiddlePoint, middlePoint);

            if (len > deadZone)
            {
                double xdiff = (-(lastRightSides.SideMiddlePoint.X - middlePoint.X) / len) * (len - deadZone);
                double ydiff = ((lastRightSides.SideMiddlePoint.Y - middlePoint.Y) / len) * (len - deadZone);

                xdiff /= sensivity;
                ydiff /= sensivity;
                xdiff = Math.Sign(xdiff) * Math.Pow(Math.Abs(xdiff), acceleration);
                ydiff = Math.Sign(ydiff) * Math.Pow(Math.Abs(ydiff), acceleration);

                Mouse.MoveBy((int)xdiff, (int)ydiff);
            }
            if (alwaysClick || len <= deadZone)
            {
                if (lastRightIndex != null && lastRightIndex.SideFingers.Length == 2)
                {
                    if (GeometryExt.Distance(lastRightIndex.SideFingers[0].LightCenter, lastRightIndex.SideFingers[1].LightCenter) >= deadZone*1.5)
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
