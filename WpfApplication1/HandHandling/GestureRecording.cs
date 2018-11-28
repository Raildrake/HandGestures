using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HandGestures.HandHandling
{
    public class GestureRecording
    {
        private List<GloveHand> _rightHistory = new List<GloveHand>();
        private List<GloveHand> _leftHistory = new List<GloveHand>();
        private int _maxHistory = 10;

        public List<GloveHand> RightHistory
        {
            get { return _rightHistory; }
            set { _rightHistory = value; }
        }
        public List<GloveHand> LeftHistory
        {
            get { return _leftHistory; }
            set { _leftHistory = value; }
        }
        public int MaxHistory
        {
            get { return _maxHistory; }
            set { _maxHistory = value; }
        }

        private void Record(List<GloveHand> h, GloveHand hand)
        {
            h.Add(hand);
            while (h.Count > MaxHistory) h.RemoveAt(0);
        }
        public void RecordRight(GloveHand hand)
        {
            Record(RightHistory, hand);
        }
        public void RecordLeft(GloveHand hand)
        {
            Record(LeftHistory, hand);
        }

        private GloveHand Last(List<GloveHand> h)
        {
            return h.LastOrDefault();
        }
        public GloveHand LastRight()
        {
            return Last(RightHistory);
        }
        public GloveHand LastLeft()
        {
            return Last(LeftHistory);
        }

        private GloveHand LastWithBothSides(List<GloveHand> h)
        {
            return h.LastOrDefault(x => x.SideFingers.Length == 2);
        }
        public GloveHand LastRightWithBothSides()
        {
            return LastWithBothSides(RightHistory);
        }
        public GloveHand LastLeftWithBothSides()
        {
            return LastWithBothSides(LeftHistory);
        }

        private List<GloveHand> WithBothSides(List<GloveHand> h)
        {
            return (from s in h
                    where s.SideFingers.Length == 2
                    select s).ToList();
        }
        public List<GloveHand> RightWithBothSides()
        {
            return WithBothSides(RightHistory);
        }
        public List<GloveHand> LeftWithBothSides()
        {
            return WithBothSides(LeftHistory);
        }

        private GloveHand LastWithIndex(List<GloveHand> h)
        {
            return h.LastOrDefault(x => x.IndexFinger != null);
        }
        public GloveHand LastRightWithIndex()
        {
            return LastWithIndex(RightHistory);
        }
        public GloveHand LastLeftWithIndex()
        {
            return LastWithIndex(LeftHistory);
        }

        private Point SideMovementDelta(List<GloveHand> h, int count, double max, double ignoreAbove)
        {
            List<GloveHand> side = WithBothSides(h);
            if (side.Count < 2) return new Point();

            int rCount = Math.Min(count,side.Count);
            if (count==-1) rCount=side.Count;

            double wX = 0.0;
            double wY = 0.0;
            for (int k = side.Count - rCount + 1; k < side.Count; k++)
            {
                int pos = (k - (side.Count - rCount));
                double exp = (double)pos / (double)rCount;

                double xdiff = side[k].SideMiddlePoint.X - side[k - 1].SideMiddlePoint.X;
                double ydiff = side[k].SideMiddlePoint.Y - side[k - 1].SideMiddlePoint.Y;

                if (xdiff > max) xdiff = max;
                if (xdiff < -max) xdiff = -max;
                if (ydiff > max) ydiff = max;
                if (ydiff < -max) ydiff = -max;

                if (xdiff > ignoreAbove) continue;
                if (xdiff < -ignoreAbove) continue;
                if (ydiff > ignoreAbove) continue;
                if (ydiff < -ignoreAbove) continue;

                wX += xdiff * exp;
                wY += ydiff * exp;
            }
            return new Point((int)wX, (int)wY);
        }
        public Point RightSideMovementDelta(int count, double max, double ignoreAbove)
        {
            return SideMovementDelta(RightHistory, count, max, ignoreAbove);
        }
        public Point LeftSideMovementDelta(int count, double max, double ignoreAbove)
        {
            return SideMovementDelta(LeftHistory, count, max, ignoreAbove);
        }

        private Point SideMovementDelta(List<GloveHand> h, int count)
        {
            List<GloveHand> side = WithBothSides(h);
            if (side.Count < 2) return new Point();

            int rCount = Math.Min(count, side.Count);
            if (count == -1) rCount = side.Count;

            double wX = 0.0;
            double wY = 0.0;
            for (int k = side.Count - rCount + 1; k < side.Count; k++)
            {
                int pos = (k - (side.Count - rCount));
                double exp = (double)pos / (double)rCount;

                double xdiff = side[k].SideMiddlePoint.X - side[k - 1].SideMiddlePoint.X;
                double ydiff = side[k].SideMiddlePoint.Y - side[k - 1].SideMiddlePoint.Y;

                wX += xdiff * exp;
                wY += ydiff * exp;
            }
            return new Point((int)wX, (int)wY);
        }
        public Point RightSideMovementDelta(int count)
        {
            return SideMovementDelta(RightHistory, count);
        }
        public Point LeftSideMovementDelta(int count)
        {
            return SideMovementDelta(LeftHistory, count);
        }
    }
}
