using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

using HandGestures.Utility;

namespace HandGestures.Structs
{
    public class LED
    {
        private Rectangle _haloBox;
        private List<Rectangle> _insideLightBoxes = new List<Rectangle>();
        private List<Rectangle> _collidingLightBoxes = new List<Rectangle>();
        private Bgr _haloColor;
        private Bgr _lightColor;

        public Rectangle HaloBox
        {
            get { return _haloBox; }
            set { _haloBox = value; }
        }
        public List<Rectangle> InsideLightBoxes
        {
            get { return _insideLightBoxes; }
            set { _insideLightBoxes = value; }
        }
        public List<Rectangle> CollidingLightBoxes
        {
            get { return _collidingLightBoxes; }
            set { _collidingLightBoxes = value; }
        }
        public Bgr HaloColor
        {
            get { return _haloColor; }
            set { _haloColor = value; }
        }
        public Bgr LightColor
        {
            get { return _lightColor; }
            set { _lightColor = value; }
        }

        //READ-ONLY DERIVED
        public List<Rectangle> LightBoxes
        {
            get
            {
                List<Rectangle> res = new List<Rectangle>();
                res.AddRange(InsideLightBoxes);
                res.AddRange(CollidingLightBoxes);
                return res.OrderByDescending(x => GeometryExt.Area(x)).ToList();
            }
        }
        public Rectangle MainLightBox
        {
            get
            {
                if (LightBoxes.Count > 0) return LightBoxes[0];
                return new Rectangle();
            }
        }
        public Point HaloCenter
        {
            get { return GeometryExt.Center(HaloBox); }
        }
        public Point MainLightCenter
        {
            get { return GeometryExt.Center(MainLightBox); }
        }
    }
}
