using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV;
using Emgu.CV.Structure;

namespace HandGestures.Utility
{
    public class Tolerance
    {
        public static Bgr Upper(Bgr col, double tol)
        {
            return new Bgr(col.Blue + tol, col.Green + tol, col.Red + tol);
        }
        public static Bgr Lower(Bgr col, double tol)
        {
            return new Bgr(col.Blue - tol, col.Green - tol, col.Red - tol);
        }
    }
}
