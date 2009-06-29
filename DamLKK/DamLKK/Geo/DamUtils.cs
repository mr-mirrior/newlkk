using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DamLKK.Geo
{
    /// <summary>
    /// 大坝工具
    /// </summary>
    public static class DamUtils
    {
        private const double ZOOM = 1;

        /// <将列表中的点按照原点偏移（取最小x，最小y） ref 远点（最小x，最小y）>
        /// 将列表中的点按照原点偏移（取最小x，最小y） ref 远点（最小x，最小y）
        /// </将列表中的点按照原点偏移（取最小x，最小y） ref 远点（最小x，最小y）>
        public static List<Coord> RelativeCoords(List<Coord> pts, ref Coord origin)
        {
            if (pts.Count == 0)
                return null;

            List<Coord> newpts = new List<Coord>();
            double minx = pts[0].X, miny = pts[0].Y;
            foreach (Coord p in pts)
            {
                minx = Math.Min(minx, p.X);
                miny = Math.Min(miny, p.Y);
            }
            foreach (Coord p in pts)
            {
                newpts.Add(new Coord(ZOOM * (p.X - minx), ZOOM * (p.Y - miny)));
            }
            origin.X = ZOOM * minx;
            origin.Y = ZOOM * miny;
            return newpts;
        }

        /// <将列表中的点取x最小最大y最小最大返回一个DM长方形>
        /// 将列表中的点取x最小最大y最小最大返回一个DM长方形
        /// </将列表中的点取x最小最大y最小最大返回一个DM长方形>
        public static DMRectangle MinBoundary(List<Coord> pts)
        {
            if (pts.Count == 0)
                return new DMRectangle();

            List<Coord> copy = new List<Coord>(pts);

            double l, t, r, b;
            copy.Sort(Coord.XCompare);
            l = copy.First().X;
            r = copy.Last().X;
            copy.Sort(Coord.YCompare);
            t = copy.First().Y;
            b = copy.Last().Y;

            return new DMRectangle(l, t, r-l, b-t);
        }

        /// <角度转弧度>
        /// 角度转弧度
        /// </s角度转弧度>
        public static double Degree2Radian(double degree)
        {
            return Math.PI * degree / 180;
        }
        /// <弧度转角度>
        /// 弧度转角度
        /// </弧度转角度>
        public static double Radian2Degree(double radian)
        {
            return radian * 180 / Math.PI;
        }

        /// <以at为原点旋转pt-at为轴旋转degree角度>
        /// 以at为原点旋转pt-at为轴旋转degree角度
        /// </以at为原点旋转pt-at为轴旋转degree角度>
        public static Coord RotateDegree(Coord pt, Coord at, double degree)
        {
            return RotateRadian(pt, at, Degree2Radian(degree));
        }

        /// <以at为原点旋转pt-at为轴旋转theta弧度>
        /// 以at为原点旋转pt-at为轴旋转theta弧度
        /// </以at为原点旋转pt-at为轴旋转theta弧度>
        public static Coord RotateRadian(Coord pt, Coord at, double theta)
        {
            double cos = Math.Cos(theta);
            double sin = Math.Sin(theta);

            double x = pt.X;
            double y = pt.Y;
            x -= at.X;
            y -= at.Y;

            double xnew = x * cos - y * sin;
            double ynew = y * cos + x * sin;

            xnew += at.X;
            ynew += at.Y;

            Coord newpt = new Coord(xnew, ynew);
            return newpt;
        }

        /// < 将所有点以at为原点旋转pt-at为轴旋转degree角度>
        /// 将所有点以at为原点旋转pt-at为轴旋转degree角度
        /// </ 将所有点以at为原点旋转pt-at为轴旋转degree角度>
        public static List<Coord> RotateDegree(List<Coord> pts, Coord at, double theta)
        {
            theta %= 360;
            if (theta == 0.00)
                return pts;

            theta = Degree2Radian(theta);
            List<Coord> newpts = new List<Coord>();
            foreach (Coord pt in pts)
            {
                newpts.Add(RotateRadian(pt, at, theta));
            }
            return newpts;
        }

        /// <Coord转PointF>
        /// Coord转PointF
        /// </Coord转PointF>
        public static List<PointF> TranslatePoints(List<Coord> pts)
        {
            List<PointF> lst = new List<PointF>();
            foreach (Coord c in pts)
            {
                lst.Add(c.PF);
            }
            return lst;
        }

      

        /// < GPSCoord转PointF>
        /// GPSCoord转PointF
        /// </ GPSCoord转PointF>
        public static PointF[] Translate(List<GPSCoord> lst)
        {
            PointF[] res = new PointF[lst.Count];
            for (int i = 0; i < lst.Count; i++ )
            {
                res[i] = lst[i].Plane.PF;
            }
            return res;
        }
    }
}
