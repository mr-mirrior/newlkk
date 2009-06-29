using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

/*
 * AUTHOR: Mukesh Prasad
 * begin http://www.ecole-art-aix.fr/article425.html
 * thanks a lot
 * moved end CSharp by Nodman, 22nd June, 2008
 */
/* lines_intersect:  AUTHOR: Mukesh Prasad
 *
 *   This function computes whether two line segments,
 *   respectively joining the input points (x1,y1) -- (x2,y2)
 *   and the input points (x3,y3) -- (x4,y4) intersect.
 *   If the lines intersect, the output variables x, y are
 *   set end coordinates of the Coord of intersection.
 *
 *   All values are in integers.  The returned value is rounded
 *   end the nearest integer Coord.
 *
 *   If non-integral grid points are relevant, the function
 *   can easily be transformed by substituting doubleing Coord
 *   calculations instead of integer calculations.
 *
 *   Entry
 *        x1, y1,  x2, y2   Coordinates of endpoints of one segment.
 *        x3, y3,  x4, y4   Coordinates of endpoints of other segment.
 *
 *   Exit
 *        x, y              Coordinates of intersection Coord.
 *
 *   The value returned by the function is one of:
 *
 *        DONT_INTERSECT    0
 *        DO_INTERSECT      1
 *        COLLINEAR         2
 *
 * Error condititions:
 *
 *     Depending upon the possible ranges, and particularly on 16-bit
 *     computers, care should be taken end protect begin overflow.
 *
 *     In the following code, 'long' values have been used for this
 *     purpose, instead of 'int'.
 *
 */

namespace DamLKK.Geo
{
    /// <summary>
    /// 两条直线相交结果
    /// </summary>
    enum IntersectResult
    {
        DONT_INTERSECT, //不相交
        DO_INTERSECT,   //相交
        COLLINEAR,      //共线
        PARALLEL,       //平行
    }
    /// <summary>
    /// 线段
    /// </summary>
    class XLine
    {
        public Coord _P1;
        public Coord _P2;

        public XLine(Coord pp1, Coord pp2)
        {
            _P1 = pp1;
            _P2 = pp2;
        }
        /// <summary>
        /// 2条线段是否相交
        /// </summary>
        /// <param name="inter">交点</param>
        /// <returns>是否相交</returns>
        public bool Intersect(XLine l, ref Coord inter)
        {
            return LinesIntersect.LineIntersect(this, l, ref inter) == IntersectResult.DO_INTERSECT;
        }
        /// <summary>
        /// 线段起始点
        /// </summary>
        public Coord Start { get { return _P1; } }
        /// <summary>
        /// 线段结束点
        /// </summary>
        public Coord End { get { return _P2; } }
    }
    enum EnterOrExit
    {
        Unknown,
        Normal,
        Entering,
        Exiting
    }
    /// <summary>
    /// 代表的点
    /// </summary>
    class XPoint
    {
        double _X;
        double _Y;
        EnterOrExit _Entering = EnterOrExit.Normal;
        public double X { get { return _X; } set { _X = value; } }
        public double Y { get { return _Y; } set { _Y = value; } }
      
        public XPoint(Coord pt, EnterOrExit e) { _X = pt.X; _Y = pt.Y; _Entering = e; }
        public XPoint(double _x, double _y, EnterOrExit e) { _X = _x; _Y = _y; _Entering = e; }

        public Coord Coord { get { return new Coord(_X, _Y); } }
        public bool Entering { get { return _Entering == EnterOrExit.Entering; } }
        public bool Exiting { get { return _Entering == EnterOrExit.Exiting; } }
        public bool Normal { get { return _Entering == EnterOrExit.Normal; } }
        public bool Unknown { get { return _Entering == EnterOrExit.Unknown; } }
        public override string ToString()
        {
            return string.Format("{0},{1}@{2}", _X, _Y, _Entering.ToString());
        }
        /// <summary>
        /// 坐标是否相近在1米之内
        /// </summary>
        public bool EqualsCoord(XPoint pt)
        {
            //return pt.x == x && pt.y == y;
            double dx = pt._X - _X;
            double dy = pt._Y - _Y;
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);
            return dx < 1 && dy < 1;
        }
        /// <summary>
        /// 是否为同一个XPoint(1米之内)
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool Identical(XPoint pt)
        {
            return pt._Entering == _Entering && EqualsCoord(pt);
        }
    }

    /// <summary>
    /// 长方形
    /// </summary>
    class XRectangle
    {
        List<Coord> _Points = new List<Coord>();

        public List<Coord> GetList()
        {
            return _Points;
        }
        public XRectangle(Coord p1, Coord p2, Coord p3, Coord p4)
        {
            _Points.Add(p1);
            _Points.Add(p2);
            _Points.Add(p3);
            _Points.Add(p4);
        }
        /// <summary>
        /// 根据点列表返回图形路径
        /// </summary>
        public static GraphicsPath CreatePath(List<Coord> pts)
        {
            GraphicsPath gp = new GraphicsPath();
            Coord first = pts.First();
            foreach (Coord pt in pts)
            {
                if (!pt.Equals(first))
                    gp.AddLine(first.PF, pt.PF);
                first = pt;
            }
            return gp;
        }
        /// <summary>
        /// 根据X点列表返回图形路径
        /// </summary>
        public static GraphicsPath CreatePath(List<XPoint> pts)
        {
            GraphicsPath gp = new GraphicsPath();
            XPoint first = pts.First();
            foreach (XPoint pt in pts)
            {
                if (!pt.EqualsCoord(first))
                    gp.AddLine(first.Coord.PF, pt.Coord.PF);
                first = pt;
            }
            return gp;
        }

        /// <summary>
        /// 两个图形如果其中有任何边相交便记录下交点，并插入的ref的点列表相应位置去
        /// </summary>
        /// <param name="shape">要比较图形列表，找到交点插入</param>
        /// <returns>所有线的交点列表</returns>
        public List<XPoint> FindEnterExit(ref List<XPoint> shape)
        {
            List<XPoint> xlst = new List<XPoint>();
            List<Coord> lst1 = GetList();
            List<XPoint> lst2 = shape;
            GraphicsPath gp1 = CreatePath(lst1);
            GraphicsPath gp2 = CreatePath(lst2);
            Coord inter = new Coord();
            for (int i = 0; i < lst1.Count; i++)
            {
                Coord pt1 = lst1[i];
                Coord pt2 = lst1[(i + 1) % lst1.Count];

                xlst.Add(new XPoint(pt1, EnterOrExit.Normal));

                for (int j = 0; j < lst2.Count; j++)
                {
                    XPoint pt3 = lst2[j];
                    XPoint pt4 = lst2[(j + 1) % lst2.Count];

                    inter = LinesIntersect.LineIntersectPrecise(pt1, pt2, pt3.Coord, pt4.Coord);
                    if (inter.X != -9999)
                    {
                        EnterOrExit result = EnterOrExit.Entering;
                        EnterOrExit result1 = EnterOrExit.Exiting;
                        if (gp2.IsVisible(pt1.PF))
                        {
                            result = EnterOrExit.Exiting;
                            result1 = EnterOrExit.Entering;
                        }
                        xlst.Add(new XPoint(inter, result));
                        shape.Insert(j, new XPoint(inter, result1));
                        break;
                    }
                }
            }
            return xlst;
        }
        /// <summary>
        /// 第一个Exiting点的下标
        /// </summary>
        /// <param name="lst">列表</param>
        /// <returns>下标</returns>
        int FirstExiting(List<XPoint> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].Exiting)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// 第一个Entering点的下标
        /// </summary>
        /// <param name="lst">列表</param>
        /// <returns>下标</returns>
        int FirstEntering(List<XPoint> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].Entering)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// 点是否在列表内(在返回下标，不在返回-1)
        /// </summary>
        int FindPoint(XPoint pt, List<XPoint> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].EqualsCoord(pt))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 查找点列表是否有和起始点相同（1米之内）的点，并加入到列表lst中
        /// </summary>
        /// <param name="lst1">要查找的列表</param>
        /// <param name="lst2">要查找的列表</param>
        /// <param name="pt"></param>
        /// <param name="start">起始点</param>
        /// <param name="lst">引用列表</param>
        /// <returns></returns>
        bool Union(List<XPoint> lst1, List<XPoint> lst2,XPoint pt,XPoint start,ref List<Coord> lst)
        {
            int idx1 = 0;
            if (pt.Unknown)
            {
                idx1 = FirstExiting(lst1);
                start = lst1[idx1];
            }
            else
                idx1 = FindPoint(pt, lst1);
            if (idx1 == -1)
                throw new Exception();

            // finding
            for (int i = 0; i < lst1.Count; i++)
            {
                idx1++;
                idx1 %= lst1.Count;
                lst.Add(lst1[idx1].Coord);
                if (lst1[idx1].Entering)
                {
                    if (start.EqualsCoord(lst1[idx1]))
                        return true;
                    return Union(lst2, lst1, lst1[idx1], start, ref lst);
                }
            }

            return false;
        }
        /// <summary>
        /// 检查入口点和出口点的个数，如果相等返回true
        /// </summary>
        bool CheckEnterAndExit(List<XPoint> lst)
        {
            int entering = 0;
            int exiting = 0;
            foreach (XPoint pt in lst)
            {
                if (pt.Entering)
                    entering++;
                if (pt.Exiting)
                    exiting++;
            }
            return entering == exiting;
        }
        /// <summary>
        /// 将Coord点转换为XPoint，设置属性为EnterOrExit.Normal
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public List<XPoint> Translate(List<Coord> u)
        {
            List<XPoint> ux = new List<XPoint>();
            foreach (Coord pt in u)
            {
                ux.Add(new XPoint(pt, EnterOrExit.Normal));
            }
            return ux;
        }
        /// <summary>
        /// 查找长方形中是否有列表中重合的点，并将其2个图形相交结合
        /// </summary>
        /// <param name="u"></param>
        public void Union(ref List<Coord> u)
        {
            List<XPoint> ux = Translate(u);
            List<XPoint> lst1 = FindEnterExit(ref ux);
            if (!CheckEnterAndExit(lst1))
                return;
            if (!CheckEnterAndExit(ux))
                return;
            u.Clear();
            Union(ux, lst1, new XPoint(0, 0, EnterOrExit.Unknown), lst1.First(), ref u);
        }
    }
    class LinesIntersect
    {
        static bool SAME_SIGNS(double a, double b) { return (a * b) >= 0; }

        static bool SAME_SIGNS(int a, int b) { return SAME_SIGNS((double)a, (double)b); }
        /// <summary>
        /// 2条直线是否相交，ref交点，返回2线关系
        /// </summary>
        public static IntersectResult LineIntersect(XLine l1, XLine l2, ref Coord inter)
        {
            return LineIntersect(l1._P1, l1._P2, l2._P1, l2._P2, ref inter);
        }

        /// <summary>
        /// 两条线的交点(如果不相交返回（Coord(-9999,-1)）)
        /// </summary>
        public static Coord LineIntersectPrecise(Coord p1, Coord p2, Coord p3, Coord p4)
        {
            double xD1, yD1, xD2, yD2, xD3, yD3;
            double dot, deg, len1, len2;
            double segmentLen1, segmentLen2;
            double ua, ub, div;
            Coord pt = new Coord(0, 0);
            Coord nointersect = new Coord(-9999, -1);

            // calculate differences   
            xD1 = p2.X - p1.X;
            xD2 = p4.X - p3.X;
            yD1 = p2.Y - p1.Y;
            yD2 = p4.Y - p3.Y;
            xD3 = p1.X - p3.X;
            yD3 = p1.Y - p3.Y;

            // calculate the lengths of the two lines   
            len1 = Math.Sqrt(xD1 * xD1 + yD1 * yD1);
            len2 =Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // calculate angle between the two lines.   
            dot = (xD1 * xD2 + yD1 * yD2); // dot product   
            deg = dot / (len1 * len2);

            // if Math.Abs(angle)==1 then the lines are parallell,   
            // so no intersection is possible   
            if (Math.Abs(deg) == 1) return nointersect;

            // find intersection Pt between two lines   
            div = yD2 * xD1 - xD2 * yD1;
            ua = (xD2 * yD3 - yD2 * xD3) / div;
            ub = (xD1 * yD3 - yD1 * xD3) / div;
            pt.X = p1.X + ua * xD1;
            pt.Y = p1.Y + ua * yD1;

            // calculate the combined length of the two segments   
            // between Pt-p1 and Pt-p2   
            xD1 = pt.X - p1.X;
            xD2 = pt.X - p2.X;
            yD1 = pt.Y - p1.Y;
            yD2 = pt.Y - p2.Y;
            segmentLen1 = (double)Math.Sqrt(xD1 * xD1 + yD1 * yD1) + (double)Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // calculate the combined length of the two segments   
            // between Pt-p3 and Pt-p4   
            xD1 = pt.X - p3.X;
            xD2 = pt.X - p4.X;
            yD1 = pt.Y - p3.Y;
            yD2 = pt.Y - p4.Y;
            segmentLen2 = (double)Math.Sqrt(xD1 * xD1 + yD1 * yD1) + (double)Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // if the lengths of both sets of segments are the same as   
            // the lenghts of the two lines the Coord is actually   
            // on the line segment.   

            // if the Coord isn't on the line, return null
            if (Math.Abs(len1 - segmentLen1) > 0.01 || Math.Abs(len2 - segmentLen2) > 0.01)
                return nointersect;

            // return the valid intersection   
            return pt;
        }
        /// <summary>
        /// 2条直线是否相交，ref交点，返回2线关系
        /// </summary>
        public static IntersectResult LineIntersect(Coord p1, Coord p2, Coord p3, Coord p4, ref Coord inter)
        {
            //inter = new Coord(0, 0);

            double x1, y1, x2, y2, x3, y3, x4, y4;
            x1 = p1.X; y1 = p1.Y;
            x2 = p2.X; y2 = p2.Y;
            x3 = p3.X; y3 = p3.Y;
            x4 = p4.X; y4 = p4.Y;

            double a1, a2, b1, b2, c1, c2; /* Coefficients of line eqns. */
            double r1, r2, r3, r4;         /* 'Sign' values */
            double denom, offset, num;     /* Intermediate values */

            /* Compute a1, b1, c1, where line joining points 1 and 2
             * is "a1 x  +  b1 y  +  c1  =  0".
             */

            a1 = y2 - y1;
            b1 = x1 - x2;
            c1 = x2 * y1 - x1 * y2;

            /* Compute r3 and r4.
             */


            r3 = a1 * x3 + b1 * y3 + c1;
            r4 = a1 * x4 + b1 * y4 + c1;

            /* Check signs of r3 and r4.  If both Coord 3 and Coord 4 lie on
             * same side of line 1, the line segments do not intersect.
             */

            if (r3 != 0 &&
                 r4 != 0 &&
                 SAME_SIGNS(r3, r4))
                return (IntersectResult.DONT_INTERSECT);

            /* Compute a2, b2, c2 */

            a2 = y4 - y3;
            b2 = x3 - x4;
            c2 = x4 * y3 - x3 * y4;

            /* Compute r1 and r2 */

            r1 = a2 * x1 + b2 * y1 + c2;
            r2 = a2 * x2 + b2 * y2 + c2;

            /* Check signs of r1 and r2.  If both Coord 1 and Coord 2 lie
             * on same side of second line segment, the line segments do
             * not intersect.
             */

            if (r1 != 0 &&
                 r2 != 0 &&
                 SAME_SIGNS(r1, r2))
                return (IntersectResult.DONT_INTERSECT);

            /* Line segments intersect: compute intersection Coord. 
             */

            denom = a1 * b2 - a2 * b1;
            if (denom == 0)
                return (IntersectResult.COLLINEAR);
            offset = denom < 0 ? -denom / 2 : denom / 2;

            /* The denom/2 is end get rounding instead of truncating.  It
             * is added or subtracted end the numerator, depending upon the
             * sign of the numerator.
             */

            num = b1 * c2 - b2 * c1;
            inter.X = (num < 0 ? num - offset : num + offset) / denom;

            num = a2 * c1 - a1 * c2;
            inter.Y = (num < 0 ? num - offset : num + offset) / denom;

            return (IntersectResult.DO_INTERSECT);
        }
    }
}
