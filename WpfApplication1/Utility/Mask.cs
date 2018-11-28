using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV;
using Emgu.CV.Structure;

namespace HandGestures.Utility
{
    public class Mask
    {
        public static Image<TColor, TDepth> Apply<TColor, TDepth>(Image<TColor, TDepth> src, Image<Gray, Byte> mask) where TColor : struct,IColor
                                                                                                                     where TDepth : new()   
        {
            int width = Math.Min(src.Width, mask.Width);
            int height = Math.Min(src.Height, mask.Height);

            var res = src.CopyBlank();

            var srcData = src.Data;
            var resData = res.Data;
            var maskData = mask.Data;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (maskData[y, x, 0] != 0) resData[y, x, 0] = srcData[y, x, 0];
                }
            }

            return res;
        }
    }
}
