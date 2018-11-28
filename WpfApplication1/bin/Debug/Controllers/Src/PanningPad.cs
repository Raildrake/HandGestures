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
    class PanningPad : BaseController
    {
        public PanningPad()
        {
            DeclareVar("MinDist", typeof(double), 25.0);
            DeclareVar("MaxDistMul", typeof(double), 3.0);
            DeclareVar("MaxDistActionMul", typeof(double), 1.5);
            DeclareVar("Sensivity", typeof(double), 3.0);
            DeclareVar("Acceleration", typeof(double), 1.5);
            DeclareVar("Trail", typeof(int), 3);
            DeclareVar("VerticalMul", typeof(double), 1.5);
        }

        protected override void OnDraw(ref Image<Bgr, Byte> src)
        {
            double maxDistMul = (double)GetVar("MaxDistMul");
            double minDist = (double)GetVar("MinDist");
            double verticalMul = (double)GetVar("VerticalMul");

            GloveHand lastSides = HandRecord.LastRightWithBothSides();
            if (lastSides == null) return;

            Bgr col = new Bgr(0, 255, 0);
            Bgr col2 = new Bgr(0, 200, 0);
            if (GeometryExt.Distance(lastSides.SideFingers[0].LightCenter, lastSides.SideFingers[1].LightCenter) > minDist*maxDistMul)
            {
                col = new Bgr(0, 0, 255);
                col2 = new Bgr(0, 0, 200);
            }
            if (GeometryExt.Distance(lastSides.SideFingers[0].LightCenter, lastSides.SideFingers[1].LightCenter) < minDist)
            {
                col = new Bgr(200, 200, 200);
                col2 = new Bgr(150, 150, 150);
            }
            if (Math.Abs(lastSides.SideFingers[0].LightCenter.Y - lastSides.SideFingers[1].LightCenter.Y) >=
                        Math.Abs(lastSides.SideFingers[0].LightCenter.X - lastSides.SideFingers[1].LightCenter.X) * verticalMul) // i side sono messi in verticale
            {
                col2 = new Bgr(255, 0, 0);
            }

            if (lastSides.SideFingers.Length == 2)
            {
                System.Drawing.Point p1 = lastSides.SideFingers[0].LightCenter;
                System.Drawing.Point p2 = lastSides.SideFingers[1].LightCenter;
                float smallC = ((float)GeometryExt.Distance(p1, p2) / 2) - 8;
                float bigC = ((float)GeometryExt.Distance(p1, p2) / 2) + 8;
                if (smallC < 0) smallC = 0;
                if (bigC < 0) bigC = 0;
                src.Draw(new CircleF(lastSides.SideMiddlePoint, smallC), col2, 3);
                src.Draw(new CircleF(lastSides.SideMiddlePoint, bigC), col, 3);
                src.Draw(new CircleF(lastSides.SideMiddlePoint, 5), new Bgr(255, 0, 255), -1);
            }
        }
        protected override void OnStep()
        {
            double sensivity = (double)GetVar("Sensivity");
            double acceleration = (double)GetVar("Acceleration");
            double minDist = (double)GetVar("MinDist");
            double maxDistMul = (double)GetVar("MaxDistMul");
            double maxDistActionMul = (double)GetVar("MaxDistActionMul");
            int trail = (int)GetVar("Trail");
            double verticalMul = (double)GetVar("VerticalMul");

            GloveHand lastRightSides = HandRecord.LastRightWithBothSides();
            GloveHand lastRightIndex = HandRecord.LastRightWithIndex();
            if (lastRightSides == null) return;

            Point delta = HandRecord.RightSideMovementDelta(trail);

            if (GeometryExt.Distance(lastRightSides.SideFingers[0].LightCenter, lastRightSides.SideFingers[1].LightCenter) >= minDist)
            {
                double acc2 = GeometryExt.Distance(lastRightSides.SideFingers[0].LightCenter, lastRightSides.SideFingers[1].LightCenter) / (minDist * maxDistMul);
                double xdiff = -delta.X * (sensivity * acc2);
                double ydiff = delta.Y * (sensivity * acc2);
                xdiff = Math.Sign(xdiff) * Math.Pow(Math.Abs(xdiff), acceleration);
                ydiff = Math.Sign(ydiff) * Math.Pow(Math.Abs(ydiff), acceleration);

                Mouse.MoveBy((int)xdiff, (int)ydiff);
            }

            if (lastRightIndex != null && lastRightIndex.SideFingers.Length == 2)
            {
                if (GeometryExt.Distance(lastRightIndex.SideFingers[0].LightCenter, lastRightIndex.SideFingers[1].LightCenter) >= minDist*maxDistActionMul)
                {
                    if (Math.Abs(lastRightIndex.SideFingers[0].LightCenter.Y - lastRightIndex.SideFingers[1].LightCenter.Y) <
                        Math.Abs(lastRightIndex.SideFingers[0].LightCenter.X - lastRightIndex.SideFingers[1].LightCenter.X)*verticalMul) // i side sono messi in orizzontale
                    {
                        Mouse.Release(MOUSE_BUTTON.RIGHT);
                        Mouse.Release(MOUSE_BUTTON.MIDDLE);
                        if (lastRightIndex.IndexFinger.LightCenter.Y > lastRightIndex.SideMiddlePoint.Y)
                            Mouse.Press(MOUSE_BUTTON.LEFT);
                        else
                            Mouse.Release(MOUSE_BUTTON.LEFT);


                        /*if (lastRightIndex.IndexFinger.LightCenter.X < Math.Min(lastRightIndex.SideFingers[0].LightCenter.X,
                                                                                lastRightIndex.SideFingers[1].LightCenter.X))
                            Mouse.Press(MOUSE_BUTTON.MIDDLE);
                        else
                            Mouse.Release(MOUSE_BUTTON.MIDDLE);*/
                    }
                    else //in verticale
                    {
                        if (GeometryExt.Distance(lastRightIndex.IndexFinger.LightCenter,lastRightIndex.SideMiddlePoint) <= 15.0)
                        {
                            Mouse.Press(MOUSE_BUTTON.RIGHT);
                            Mouse.Press(MOUSE_BUTTON.LEFT);
                            Mouse.Release(MOUSE_BUTTON.MIDDLE);
                        }
                        else if (lastRightIndex.IndexFinger.LightCenter.X < lastRightIndex.SideMiddlePoint.X)
                        {
                            Mouse.Press(MOUSE_BUTTON.RIGHT);
                            Mouse.Release(MOUSE_BUTTON.LEFT);
                            Mouse.Release(MOUSE_BUTTON.MIDDLE);
                        }
                        else if (lastRightIndex.IndexFinger.LightCenter.X > lastRightIndex.SideMiddlePoint.X)
                        {
                            Mouse.Press(MOUSE_BUTTON.MIDDLE);
                        }
                    }
                }
            }
        }
    }
}
