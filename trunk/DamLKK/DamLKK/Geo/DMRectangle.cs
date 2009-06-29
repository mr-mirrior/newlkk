using System;
using System.Drawing;

namespace DamLKK.Geo
{
     ///<summary>
     ///大坝形状
     ///</summary>
    public struct DMRectangle
    {
        public DMRectangle(DMRectangle rc) { _LeftTop = new Coord(rc._LeftTop); _RightBottom = new Coord(rc._RightBottom); }
        public DMRectangle(RectangleF rc) { _LeftTop = new Coord(rc.Location); _RightBottom = new Coord(rc.Right, rc.Bottom); }
        Coord _LeftTop;
        Coord _RightBottom;
        public Coord LeftTop { get { return _LeftTop; } set { _LeftTop = value; } }
        public Coord RightBottom { get { return _RightBottom; } set { _RightBottom = value; } }
        public Coord LeftBottom { get { return new Coord(Left, Bottom); } }
        public Coord RightTop { get { return new Coord(Right, Top); } }
        public RectangleF RF { get { return new RectangleF((float)Left, (float)Top, (float)Width, (float)Height); } }
        public Rectangle RC { get { return Utils.Graph.Rect.Translate(RF); } }
        public bool IsEqual(DMRectangle rc){return this.LeftTop.IsEqual(rc.LeftTop) && this.RightBottom.IsEqual(rc.RightBottom);}
        public DMRectangle(double l, double t, double w, double h)
        {
            _LeftTop = new Coord();
            _RightBottom = new Coord();
            _LeftTop.X = l; _LeftTop.Y = t;
            _RightBottom.X = l+w; _RightBottom.Y = t+h;
        }
        public DMRectangle(float l, float t, float w, float h)
        {
            _LeftTop = new Coord();
            _RightBottom = new Coord();
            _LeftTop.XF = l; _LeftTop.YF = t;
            _RightBottom.XF = l+w; _RightBottom.YF = t+h;
        }
        public double Left { get { return _LeftTop.X; } set { _LeftTop.X = value; } }
        public double Top { get { return _LeftTop.Y; } set { _LeftTop.Y = value; } }
        public double Right { get { return _RightBottom.X; } set { _RightBottom.X = value; } }
        public double Bottom { get { return _RightBottom.Y; } set { _RightBottom.Y = value; } }

        public double Width { get { return Math.Abs(_RightBottom.X - _LeftTop.X); } }
        public double Height { get { return Math.Abs(_RightBottom.Y - _LeftTop.Y); } }

        /// <summary>
        /// 检查性的重置图形
        /// </summary>
        public void Normalize()
        {
            double l, t, r, b;
            l = Math.Min(_LeftTop.X, _RightBottom.X);
            t = Math.Min(_LeftTop.Y, _RightBottom.Y);
            r = Math.Max(_LeftTop.X, _RightBottom.X);
            b = Math.Max(_LeftTop.Y, _RightBottom.Y);
            _LeftTop.X = l;
            _LeftTop.Y = t;
            _RightBottom.X = r;
            _RightBottom.Y = b;
        }

        public void Offset(double x, double y)
        {
            _LeftTop.X += x;
            _LeftTop.Y += y;
            _RightBottom.X += x;
            _RightBottom.Y += y;
        }
        public void Offset(float x, float y) { Offset((double)x, (double)y); }
        public void Offset(Coord pt) { Offset(pt.X, pt.Y); }
        public void Offset(Point pt) { Offset(pt.X, pt.Y); }
        public void Offset(PointF pt) { Offset(pt.X, pt.Y); }
        public override string ToString()
        {
            return string.Format("X={0:0.0},Y={1:0.0},Width={2:0.0},Height={3:0.0}", _LeftTop.X, _LeftTop.Y, Width, Height);
        }
         ///<summary>
         ///中心点
         ///</summary>
        public Coord Center
        {
            get
            {
                return new Coord(_LeftTop.X + Width / 2, LeftTop.Y + Height / 2);
            }
        }

        /// <summary>
        /// 点是否在长方形内
        /// </summary>
        public bool Contains(PointF pt)
        {
            return pt.X >= this.Left && pt.X <= this.Right && pt.Y >= this.Top && pt.Y <= this.Bottom;
        }
    }
}
