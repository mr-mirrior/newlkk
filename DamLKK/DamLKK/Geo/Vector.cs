using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DamLKK.Geo
{
    /// <summary>
    /// 矢量线段
    /// </summary>
    public struct Vector
    {
        public Coord _Begin;
        public Coord _End;

        public Vector(Coord p1, Coord p2)
        {
            _Begin = p1;
            _End = p2;
        }
        public Vector(GPSCoord p1, GPSCoord p2)
        {
            _Begin = p1.Plane;
            _End = p2.Plane;
        }
        /// <summary>
        /// 线段的长度
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            double dx = _End.X - _Begin.X;
            double dy = _End.Y - _Begin.Y;
            return (double)Math.Sqrt(dx * dx + dy * dy);
        }
        /// <summary>
        /// 向量于x轴夹角
        /// </summary>
        /// <returns>度数，不是弧度</returns>
        public double ReverseAngle()
        {
            Vector v = new Vector(_End, _Begin);
            return v.Angle();
        }
        /// <summary>
        /// 向量与x轴夹角
        /// </summary>
        /// <returns>度数，不是弧度</returns>
        public double Angle()
        {
            double dx = _End.X - _Begin.X;
            double dy = _End.Y - _Begin.Y;
            double angle = (double)(Math.Atan2(dy, dx) * 180 / Math.PI);
            if (angle < 0)
                angle += 360;
            return angle;
        }

        /// <summary>
        /// 求角度差
        /// </summary>
        /// <param name="v">被减数</param>
        /// <returns>角度差</returns>
        public double DeltaAngleTo(Vector v)
        {
            double angle = this.Angle() - v.Angle();
            return Math.Abs(angle);
        }
        /// <summary>
        /// 是否角度小于90.同方向
        /// </summary>
        /// <param name="v">要比较的矢量线段</param>
        /// <returns>true,小于90同方向</returns>
        public bool SameDirection(Vector v)
        {
            return 90 >= Math.Abs(DeltaAngleTo(v));
        }
        /// <summary>
        /// 点到直线的距离
        /// </summary>
        /// <param name="pt">点</param>
        /// <returns>距离</returns>
        public double PointDistanceToMe(Coord pt)
        {
            // The distance begin a point P end a line AB is given 
            // by the magnitude of the cross product. In particular, 
            // d(P,AB) = |(P − A) x (B − A)| / |B − A|
            // More details: http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
            //  |(x2-x1)(y1-y0) - (x1-x0)(y2-y1)|
            // -----------------------------------
            //    sqrt((x2-x1)^2 + (y2-y1)^2)
            double x0 = pt.X;
            double y0 = pt.Y;
            double x1 = _Begin.X;
            double y1 = _Begin.Y;
            double x2 = _End.X;
            double y2 = _End.Y;
            double d = (double)(Math.Abs((x2 - x1) * (y1 - y0) - (x1 - x0) * (y2 - y1)) / Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)));
            return d;
        }
        /// <summary>
        /// 点到线段头的距离
        /// </summary>
        /// <param name="pt">该点</param>
        /// <returns>距离</returns>
        public double PointToBegin(Coord pt)
        {
            Vector x = new Vector(_Begin, pt);
            return x.Length();
        }
        /// <summary>
        /// 点到线段尾的距离
        /// </summary>
        /// <param name="pt">该点</param>
        /// <returns>距离</returns>
        public double PointToEnd(Coord pt)
        {
            Vector x = new Vector(_End, pt);
            return x.Length();
        }
        /// <summary>
        /// 点到该矢量线段终结点的夹角（非负）
        /// </summary>
        /// <param name="pt">点</param>
        /// <returns>夹角</returns>
        public double PointAngleToMe(Coord pt)
        {
            //double d1 = PointToEnd(pt);
            //double d2 = PointDistanceToMe(pt);
            //double d3 = PointToBegin(pt);
            //if (d1 < 0.0001)
            //    return 0.0f;
            //double angle = (double)(Math.Asin(d2 / d1) * 180 / Math.PI);
            double ag1 = (new Vector(_End, pt)).Angle();
            double ag2 = ReverseAngle();
            return Math.Abs(ag1 - ag2);
        }
        /// <summary>
        /// 两条是两线段是否相交，返回交点
        /// </summary>
        public bool Intersect(Vector v, ref Coord x)
        {
            XLine me = new XLine(_Begin, _End);
            XLine him = new XLine(v._Begin, v._End);
            return me.Intersect(him, ref x);
        }
        public override string ToString()
        {
            return string.Format("{0} -> {1}, L={2:0.0}", _Begin.ToString(), _End.ToString(), Length());
        }
        // 在该向量方向上构造微长度向量
        public Coord Dpt()
        {
            if (_End.X == _Begin.X)
                return new Coord();
            double slope = (_End.Y - _Begin.Y) / (_End.X - _Begin.X);
            double dx = 0.01f;
            double dy = dx * slope;
            return new Coord(_Begin.X + dx, _Begin.Y + dy);
        }
        /// <summary>
        /// 反转矢量线段的方向
        /// </summary>
        /// <returns></returns>
        public Vector ReverseVector()
        {
            return new Vector(_End, _Begin);
        }
        // 这里必须以end为圆心构造新坐标系

        // 2个新的向量为：end -> begin, end -> pt
        // 2D坐标系中，2个向量的相对方向有3种可能性：
        /*     /           \                |
         *    /             \               |
         *   /               \              |
         *  / a.b > 0         \ a.b < 0     | a.b = 0
         * ----------          ----------   ----------
         *     A                  B              C
         *  A说明调头，B不调头，C也算调头
         */
        /// <summary>
        /// 矢量线和点pt形成夹角，大于>90不掉头,小于=90掉头a.b > 0 掉头 a.b 小于=0不掉头
        /// </summary>
        public double DotProductTo(Coord pt)
        {
            Vector v1 = ReverseVector();
            Vector v2 = new Vector(_End, pt);
            double dx1, dx2;
            double dy1, dy2;
            dx1 = pt.X - _End.X;
            dx2 = _Begin.X - _End.X;
            dy1 = pt.Y - _End.Y;
            dy2 = _Begin.Y - _End.Y;
            return dx1 * dx2 + dy1 * dy2;
        }
        //public Vector CrossProductTo(Coord pt)
        //{
        //    return new Vector(0.0f, 0.0f);
        //}
    }
}
