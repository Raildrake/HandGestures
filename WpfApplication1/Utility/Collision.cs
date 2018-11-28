using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace HandGestures.Utility
{
    public class Collision
    {
        public static bool Inside(Rectangle container, Rectangle check)
        {
            return (check.Top >= container.Top &&
                    check.Left >= container.Left &&
                    check.Bottom <= container.Bottom &&
                    check.Right <= container.Right);
        }

        public static bool Inside(Rectangle container, Point p)
        {
            return (p.Y >= container.Top &&
                    p.X >= container.Left &&
                    p.Y <= container.Bottom &&
                    p.X <= container.Right);
        }
        public static bool Check(Rectangle rect1, Rectangle rect2)
        {
            return !(rect2.Left > rect1.Right ||
                   rect2.Right < rect1.Left ||
                   rect2.Top > rect1.Bottom ||
                   rect2.Bottom < rect1.Top);
        }
    }
}
