using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV;
using Emgu.CV.Structure;

namespace HandGestures.Detection
{
    class MotionDetector
    {
        private List<Image<Gray, Byte>> _source = new List<Image<Gray, byte>>();
        private int _frameLen = 3;

        public List<Image<Gray, Byte>> Source
        {
            get { return _source; }
            set { _source = value; }
        }
        public int FrameLen
        {
            get { return _frameLen; }
            set { _frameLen = value; }
        }

        public MotionDetector() { }

        public Image<Gray, Byte> StillMotion(int min)
        {
            if (Source.Count < FrameLen + 1) return new Image<Gray, byte>(100, 100);

            Image<Gray, Byte> half = Source[0].CopyBlank();

            for (int k = 0; k < FrameLen; k++)
            {
                half += Source[k] / (double)FrameLen;
            }

            Image<Gray, Byte> diff = Source.Last().CopyBlank();
            byte[,,] lastData = Source.Last().Data;
            byte[,,] halfData = half.Data;
            byte[,,] diffData = diff.Data;
            for (int x = 0; x < diff.Width; x++)
            {
                for (int y = 0; y < diff.Height; y++)
                {
                    if (Math.Abs(lastData[y, x, 0] - halfData[y, x, 0]) > min)
                    {
                        diffData[y, x, 0] = 255;
                    }
                }
            }

            return diff;//.InRange(new Gray(min), new Gray(255));
        }

        public void Record(Image<Bgr, Byte> img)
        {
            Source.Add(img.Convert<Gray, Byte>().PyrDown().PyrUp());
            while (Source.Count > FrameLen + 1) Source.RemoveAt(0);
        }
    }
}
