using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace HandGestures.Utility
{
    public class GeometryExt
    {
        public static double Area(Rectangle r) {
            return r.Width * r.Height;
        }

        public static Point Center(Rectangle r)
        {
            return new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
        }

        public static double Distance(Point p)
        {
            return Distance(new Point(), p);
        }
        public static double Distance(Point p1, Point p2)
        {
            double xdiff = p2.X - p1.X;
            double ydiff = p2.Y - p1.Y;
            return Math.Sqrt(Math.Pow(xdiff, 2) + Math.Pow(ydiff, 2));
        }
        public static double Distance(Rectangle r1, Rectangle r2)
        {
            return Distance(Center(r1), Center(r2));
        }

        public static Point PointBetween(Point p1, Point p2, double factor)
        {
            double xdiff = p2.X - p1.X;
            double ydiff = p2.Y - p1.Y;
            return new Point((int)(p1.X + xdiff * factor), (int)(p1.Y + ydiff * factor));
        }

        public static Rectangle Join(Rectangle r1, Rectangle r2)
        {
            int top = Math.Min(r1.Top, r2.Top);
            int left = Math.Min(r1.Left, r2.Left);
            int right = Math.Max(r1.Right, r2.Right);
            int bottom = Math.Max(r1.Bottom, r2.Bottom);

            return new Rectangle(left, top, right - left, bottom - top);
        }

        public static Point Middle(Point p1, Point p2)
        {
            return PointBetween(p1, p2, 0.5);
        }
        public static Point Middle(Rectangle r1, Rectangle r2)
        {
            return PointBetween(GeometryExt.Center(r1), GeometryExt.Center(r2), 0.5);
        }
        public static Point Middle(Point p1, Point p2, Point p3)
        {
            return new Point((p1.X + p2.X + p3.X) / 3, (p1.Y + p2.Y + p3.Y) / 3);
        }
    }
}
