using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV;
using Emgu.CV.Structure;

namespace HandGestures.Utility
{
    public class ColorOperation
    {
        public static Bgr Mul(Bgr col, double mul)
        {
            return new Bgr(col.Blue * mul, col.Green * mul, col.Red * mul);
        }
        public static Bgr Mul(Bgr col1, Bgr col2)
        {
            return new Bgr(col1.Blue * col2.Blue, col1.Green * col2.Green, col1.Red * col2.Red);
        }

        public static Bgr Div(Bgr col, double div)
        {
            return new Bgr(col.Blue / div, col.Green / div, col.Red / div);
        }
        public static Bgr Div(Bgr col1, Bgr col2)
        {
            return new Bgr(col1.Blue / col2.Blue, col1.Green / col2.Green, col1.Red / col2.Red);
        }

        public static Bgr Sum(Bgr col, double add)
        {
            return new Bgr(col.Blue + add, col.Green + add, col.Red + add);
        }
        public static Bgr Sum(Bgr col1, Bgr col2)
        {
            return new Bgr(col1.Blue + col2.Blue, col1.Green + col2.Green, col1.Red + col2.Red);
        }

        public static Bgr Sub(Bgr col, double sub)
        {
            return new Bgr(col.Blue - sub, col.Green - sub, col.Red - sub);
        }
        public static Bgr Sub(Bgr col1, Bgr col2)
        {
            return new Bgr(col1.Blue - col2.Blue, col1.Green - col2.Green, col1.Red - col2.Red);
        }

        public static Image<Bgr, Byte> UpperFilterAutoChannel(Image<Bgr, Byte> src, Bgr filter)
        {
            Image<Bgr, Byte> res = src.CopyBlank();

            int channel = -1;
            if (filter.Red > filter.Green && filter.Red > filter.Blue) channel = 0; //RED
            if (filter.Green > filter.Red && filter.Green > filter.Blue) channel = 1; //GREEN
            if (filter.Blue > filter.Red && filter.Blue > filter.Green) channel = 2; //BLUE

            byte[, ,] srcData = src.Data;
            byte[, ,] resData = res.Data;
            for (int x = 0; x < src.Width; x++)
            {
                for (int y = 0; y < src.Height; y++)
                {
                    if ((channel == 0 && (srcData[y, x, 0] <= filter.Blue && srcData[y, x, 1] <= filter.Green && srcData[y, x, 2] >= filter.Red)) ||
                        (channel == 1 && (srcData[y, x, 0] <= filter.Blue && srcData[y, x, 1] >= filter.Green && srcData[y, x, 2] <= filter.Red)) ||
                        (channel == 2 && (srcData[y, x, 0] >= filter.Blue && srcData[y, x, 1] <= filter.Green && srcData[y, x, 2] <= filter.Red)))
                    {
                        resData[y, x, 0] = srcData[y, x, 0];
                        resData[y, x, 1] = srcData[y, x, 1];
                        resData[y, x, 2] = srcData[y, x, 2];
                    }
                }
            }

            return res;
        }
    }
}
