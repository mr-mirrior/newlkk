using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DamLKK.Geo
{
    public static class PolygonUtils
    {
        /// <summary>
        /// 多边形面积
        /// </summary>
        /// <param name="vertex">多边形边界点列表</param>
        public static double AreaOfPolygon(List<Coord> vertex)
        {
            if (vertex == null)
                return 0;
            // A = 0.5 * Sigma[i=0, N-1] { (Xi + Xi+1)( XiYi+1 - Xi+1Yi) }
            int i, j;
            double area = 0;

            for (i = 0; i < vertex.Count; i++)
            {
                j = (i + 1) % vertex.Count;
                area += vertex[i].X * vertex[j].Y;
                area -= vertex[i].Y * vertex[j].X;
            }

            area /= 2;
            return (area < 0 ? -area : area);
        }
        /// <summary>
        /// 多边形质心（返回质心坐标点）
        /// </summary>
        /// <param name="vertex">多边形边界点列表</param>
        public static Coord CentroidOfPolygon(List<Coord> vertex)
        {
            double area = AreaOfPolygon(vertex);
            if (area < 0.0001)
                return new Coord();
            int i, j;
            double x = 0, y = 0;
            for (i = 0; i < vertex.Count; i++)
            {
                j = (i + 1) % vertex.Count;
                x += (vertex[i].X + vertex[j].X) * (vertex[i].X * vertex[j].Y - vertex[j].X * vertex[i].Y);
                y += (vertex[i].Y + vertex[j].Y) * (vertex[i].X * vertex[j].Y - vertex[j].X * vertex[i].Y);
            }
            x /= 6 * area;
            y /= 6 * area;
            return new Coord(x, y);
        }
    }
    /// <summary>
    /// 边界点
    /// </summary>
    public class BorderPoint
        {
            public enum PointAttribute
            {
                Unknown = 0,
                Connection = 1,     // 连接点

                HVertex = 2,        // 水平方向顶点
                VVertex = 4,
                Transition = 8,   // 过渡点

                Downing = 0x100,
                Upping = 0x200
            }
            int _X = 0;
            int _Y = 0;

            PointAttribute attr = PointAttribute.Connection;
            //初始化函数
            void Set(int _x, int _y) { _X = _x; _Y = _y; }
            void Set(double _x, double _y) { Set((int)_x, (int)_y); }
           
            public BorderPoint(int _x, int _y) { Set(_x, _y); }
            public BorderPoint(double _x, double _y) { Set(_x, _y); }
            public BorderPoint(Point pt) { Set(pt.X, pt.Y); }
            public BorderPoint(Coord pt) { Set(pt.X, pt.Y); }

            public int X { get { return _X; } set { _X = value; } }
            public int Y { get { return _Y; } set { _Y = value; } }

            public Point Point() { return new Point(_X, _Y); }
            public Coord Coord() { return new Coord(_X, _Y); }

           
            public override string ToString()
            {
                return string.Format("{0},{1}:{2}", _X, _Y, attr.ToString());
            }
            /// <summary>
            /// 是否相等
            /// </summary>
            public bool Equals(BorderPoint pt)
            {
                return (_X == pt._X && _Y == pt._Y);
            }
            /// <summary>
            /// 2个点是否是相当接近的（坐标小于1）
            /// </summary>
            public bool IsAdjacent(BorderPoint pt)
            {
                int delta_x = Math.Abs(_X - pt._X);
                int delta_y = Math.Abs(_Y - pt._Y);

                return delta_x <= 1 && delta_y <= 1;
            }
            /// <summary>
            /// 获取属性
            /// </summary>
            bool GetAttribute(PointAttribute a)
            {
                return (0 != (attr & a));
            }
            /// <summary>
            /// 设置属性
            /// </summary>>
            void SetAttribute(PointAttribute a, bool value)
            {
                if (value)
                    attr |= a;
                else
                    attr &= ~a;  //~求补 ，反转每一位
            }

            bool Connection
            {
                get { return GetAttribute(PointAttribute.Connection); }
                set { if (value) Transition = false; SetAttribute(PointAttribute.Connection, value); }
            }
            bool Transition
            {
                get { return GetAttribute(PointAttribute.Transition); }
                set { if (value) Connection = false; SetAttribute(PointAttribute.Transition, value); }
            }
            bool HVertex
            {
                get { return GetAttribute(PointAttribute.HVertex); }
                set { VVertex = false; SetAttribute(PointAttribute.HVertex, value); }
            }
            bool VVertex
            {
                get { return GetAttribute(PointAttribute.VVertex); }
                set { HVertex = false; SetAttribute(PointAttribute.VVertex, value); }
            }
            bool Downing
            {
                get { return GetAttribute(PointAttribute.Downing); }
                set { Upping = false; SetAttribute(PointAttribute.Downing, value); }
            }
            bool Upping
            {
                get { return GetAttribute(PointAttribute.Upping); }
                set { Downing = false; SetAttribute(PointAttribute.Upping, value); }
            }
        }
        internal class BorderRectangle
        {
            BorderPoint left = new BorderPoint(0, 0);
            BorderPoint right = new BorderPoint(0, 0);
            public BorderRectangle() { }
            public int Left { get { return left.X; } set { left.X = value; } }
            public int Top { get { return left.Y; } set { left.Y = value; } }
            public int Right { get { return right.X; } set { right.X = value; } }
            public int Bottom { get { return right.Y; } set { right.Y = value; } }
            public int Width { get { return right.X - left.X; } }
            public int Height { get { return right.Y - left.Y; } }
            public bool IsPtInside(BorderPoint pt)
            {
                bool result = true;
                result &= (pt.X >= Left && pt.X <= Right);
                result &= (pt.Y >= Top && pt.Y <= Bottom);
                return result;
            }
        }
        /// <summary>
        /// 多边形2
        /// </summary>
        public class BorderShapeII : ICloneable
        {

            public BorderShapeII()
            {
                _BorderII = new List<Coord>();
            }
            public BorderShapeII(List<Coord> pts)
            {
                if (pts.Count < 2)
                    return;
                _BorderII = new List<Coord>(pts);
                if (_BorderII.First().Equals(_BorderII.Last()))
                {
                    _BorderII.RemoveAt(_BorderII.Count - 1);
                    _IsClosed = true;
                }
            }
            public BorderShapeII(DMRectangle rc)
            {
                AddPoint(rc.LeftTop);
                AddPoint(new Coord(rc.Left, rc.Top + rc.Height));
                AddPoint(new Coord(rc.Left + rc.Width, rc.Top + rc.Height));
                AddPoint(new Coord(rc.Left + rc.Width, rc.Top));
                AddPoint(rc.LeftTop);
            }

            private bool _IsClosed = false;
            private List<Coord> _BorderII;
            /// <summary>
            ///  return (idx + count) % count;
            /// </summary>
            private static int CycleIndex(int idx, int count)
            {
                return (idx + count) % count;
            }
            /// <summary>
            /// 查找点pt是lv列表中那个矢量图起始点,返回lv列表中的下标，如果没有返回-1
            /// </summary>
            /// <param name="lv">要查找的列表</param>
            /// <param name="pt">此点</param>
            /// <returns>下标</returns>
            private static int FindBeginWith(List<Vector> lv, Coord pt)
            {
                int idx = 0;
                foreach (Vector v in lv)
                {
                    if (v._Begin.Equals(pt))
                        return idx;
                    idx++;
                }
                return -1;
            }
            /// <summary>
            /// 查找点pt是lv列表中那个矢量图起始点,返回lv列表中的下标，如果没有返回-1
            /// </summary>
            /// <param name="lv">要查找的列表</param>
            /// <param name="pt">此点</param>
            /// <returns>下标</returns>
            private static int FindEndWith(List<Vector> lv, Coord pt)
            {
                int idx = 0;
                foreach (Vector v in lv)
                {
                    if (v._End.Equals(pt))
                        return idx;
                    idx++;
                }
                return -1;
            }
            /// <summary>
            /// 调试输出System.Diagnostics.Debug.Print(fmt, pm)
            /// </summary>
            private static void TRACE(string fmt, params object[] pm)
            {
                System.Diagnostics.Debug.Print(fmt, pm);
            }
            /// <summary>
            /// 调试输出System.Diagnostics.Debug.Print(fmt)
            /// </summary>
            private static void TRACE(string fmt)
            {
                System.Diagnostics.Debug.Print(fmt);
            }

            private static void SortX(ref List<Coord> pts, Vector v)
            {
                if (pts == null)
                    return;
                if (pts.Count == 0)
                    return;

                Coord pt = pts[0];
                pts.Sort(delegate(Coord pt1, Coord pt2)
                {
                    double delta = v.PointToBegin(pt1) - v.PointToBegin(pt2);
                    return Math.Sign(delta);
                });
                if (!pt.Equals(pts[0]))
                {
                    TRACE("Sorted List<Coord>");
                }
            }

            /// <summary>
            /// 向图形中添加一个点，如果是这个图形的起始点则返回true,_Closed=true。
            /// 如果点不在图形点列表中就将点追加到列表中，返回false
            /// </summary>
            /// <param name="pt"></param>
            /// <returns></returns>
            private bool AddPoint(Coord pt)
            {
                if (IsClosed)
                    return true;
                if (_BorderII.Count != 0)
                {
                    if (pt.Equals(_BorderII[0]))
                    {
                        _IsClosed = true;
                        return true;
                    }
                }
                if (!IsPtExists(pt))
                    _BorderII.Add(pt);
                return false;
            }
            private bool AddPointNoClose(Coord pt)
            {
                if (!IsPtExists(pt))
                    _BorderII.Add(pt);
                else
                    return false;

                return true;
            }
            /// <summary>
            /// 点是否在图形列表中
            /// </summary>
            private bool IsPtExists(Coord pt)
            {
                return (_BorderII.IndexOf(pt) != -1);
            }
            /// <summary>
            /// 获得多边形列表中下标为的idx和idx-1点形成的矢量边
            /// </summary>
            /// <param name="idx"></param>
            /// <returns></returns>
            private Vector Edge(int idx)
            {
                if (_BorderII.Count < 2)
                    return new Vector();
                int p1 = CycleIndex(idx, _BorderII.Count);
                int p0 = CycleIndex(idx - 1, _BorderII.Count);
                return new Vector(_BorderII[p0], _BorderII[p1]);
            }
            /// <summary>
            ///  拿参数多边形q对本多边形进行切操作，返回所切的所有边
            /// </summary>
            private List<Vector> VisibleEdgesIn(BorderShapeII q)
            {
                List<Vector> vs = new List<Vector>();
                List<Coord> pts = new List<Coord>();
                Coord x = new Coord();
                for (int i = 0; i < Count; i++)
                {
                    Vector pv = Edge(i + 1);
                    pts.Clear();
                    for (int j = 0; j < q.Count; j++)
                    {
                        Vector qv = q.Edge(j + 1);
                        if (pv.Intersect(qv, ref x))
                        {
                            pts.Add(x);
                        }
                    }
                    SortX(ref pts, pv);
                    if (q.IsInsideIII(pv._Begin))
                        pts.Insert(0, pv._Begin);
                    if (q.IsInsideIII(pv._End))
                        pts.Add(pv._End);
                    for (int j = 0; j < pts.Count - 1; j++)
                    {
                        int j1 = (j + 1 + pts.Count) % pts.Count;
                        vs.Add(new Vector(pts[j], pts[j1]));
                    }
                }

                return vs;
            }
            /// <summary>
            /// 结合2个列表的返回一个多边形2,如果2个列表其中有一个为空，则返回null
            /// </summary>
            private BorderShapeII CombineEdges(List<Vector> v1, List<Vector> v2)
            {
                if (v1.Count == 0 && v2.Count == 0)
                    return null;

                Vector v;
                if (v1.Count != 0)
                    v = v1[0];
                else
                    v = v2[0];

                BorderShapeII x = new BorderShapeII();

                int idx = 0;
                for (int i = 0; i < v1.Count + v2.Count; i++)
                {
                    if (x.AddPoint(v._Begin))
                        break;
                    idx = FindBeginWith(v1, v._End);
                    if (-1 == idx)
                    {
                        idx = FindBeginWith(v2, v._End);
                        if (-1 == idx)
                        {
                            return null;
                        }
                        else
                            v = v2[idx];
                    }
                    else
                        v = v1[idx];
                }
                return x;
            }

            public bool IsClosed { get { return _IsClosed; } }
            public int Count { get { return _BorderII.Count; } }
            public bool IsEmpty { get { return Count == 0; } }
            public List<Coord> Data { get { return _BorderII; } }
            /// <summary>
            /// 两个多变形2相交
            /// </summary>
            /// <param name="rc"></param>
            public void Intersect(DMRectangle rc)
            {
                BorderShapeII r = new BorderShapeII(rc);
                Intersect(r);
            }
            /// <summary>
            /// 清楚点列表,_IsClosed=false;
            /// </summary>
            public void Clear()
            {
                _BorderII.Clear();
                _IsClosed = false;
            }

            public object Clone()
            {
                BorderShapeII n = new BorderShapeII(_BorderII);
                n._IsClosed = _IsClosed;
                return n;
            }

            // Joseph O'Rourke
            /// <summary>
            /// 求点q是否在多边形2之内
            /// </summary>
            public bool IsInsideII(Coord q)
            {
                int i, i1;      /* point index; i1 = i-1 mod n */
                //int d;          /* dimension index */
                double x;          /* x intersection of e with ray */
                int Rcross = 0; /* number of right edge/ray crossings */
                int Lcross = 0; /* number of left edge/ray crossings */

                Coord[] P = _BorderII.ToArray();
                int n = P.Length;
                //printf("\n==>InPoly: q = "); PrintPoint(q); putchar('\n');

                /* Shift so that q is the origin. Note this destroys the polygon.
                   This is done for pedogical clarity. */
                for (i = 0; i < n; i++)
                {
                    //for (d = 0; d < DIM; d++)
                    //    P[i][d] = P[i][d] - q[d];
                    P[i].X -= q.X;
                    P[i].Y -= q.Y;
                }

                /* For each edge e=(i-1,i), see if crosses ray. */
                for (i = 0; i < n; i++)
                {
                    /* First see if q=(0,0) is a vertex. */
                    //if (P[i][X] == 0 && P[i][Y] == 0) return 'v';
                    if (P[i].X == 0 && P[i].Y == 0) return true;
                    i1 = (i + n - 1) % n;
                    /* printf("e=(%d,%d)\t", i1, i); */

                    /* if e "straddles" the x-axis... */
                    /* The commented-out statement is logically equivalent to the one 
                       following. */
                    /* if( ( ( P[i][Y] > 0 ) && ( P[i1][Y] <= 0 ) ) ||
                       ( ( P[i1][Y] > 0 ) && ( P[i] [Y] <= 0 ) ) ) { */

                    if ((P[i].Y > 0) != (P[i1].Y > 0))
                    {

                        /* e straddles ray, so compute intersection with ray. */
                        x = (P[i].X * (double)P[i1].Y - P[i1].X * (double)P[i].Y)
                      / (double)(P[i1].Y - P[i].Y);
                        /* printf("straddles: x = %g\t", x); */

                        /* crosses ray if strictly positive intersection. */
                        if (x > 0) Rcross++;
                    }
                    /* printf("Right cross=%d\t", Rcross); */

                    /* if e straddles the x-axis when reversed... */
                    /* if( ( ( P[i] [Y] < 0 ) && ( P[i1][Y] >= 0 ) ) ||
                       ( ( P[i1][Y] < 0 ) && ( P[i] [Y] >= 0 ) ) )  { */

                    if ((P[i].Y < 0) != (P[i1].Y < 0))
                    {

                        /* e straddles ray, so compute intersection with ray. */
                        x = (P[i].X * P[i1].Y - P[i1].X * P[i].Y)
                            / (double)(P[i1].Y - P[i].Y);
                        /* printf("straddles: x = %g\t", x); */

                        /* crosses ray if strictly positive intersection. */
                        if (x < 0) Lcross++;
                    }
                    /* printf("Left cross=%d\n", Lcross); */
                }

                /* q on the edge if left and right cross are not the same parity. */
                if ((Rcross % 2) != (Lcross % 2))
                    //    return 'e';
                    return false;

                /* q inside iff an odd number of crossings. */
                if ((Rcross % 2) == 1)
                    //return 'i';
                    return true;
                //else return 'o';
                else return false;
            }


            
            /// <summary>
            /// //求矢量[p0, p1], [p0, p2]的叉积
            ///p0是顶点 
            ///若结果等于0，则这三点共线
            ///若结果大于0，则p0p2在p0p1的逆时针方向
            ///若结果小于0，则p0p2在p0p1的顺时针方向
            /// </summary>
            private double WhichSide(Coord P0, Coord P1, Coord P2)
            {
                return ((P1.X - P0.X) * (P2.Y - P0.Y)
                        - (P2.X - P0.X) * (P1.Y - P0.Y));
            }
            /// <summary>
            /// 如果点和多边形所有边的交点是偶数则在多边形外,false。如果不为偶数则在多边形内,true;
            /// http://mniwjb.blog.sohu.com/58905924.html
            /// </summary>
            public bool IsInsideIII(Coord q)
            {
                int winding = 0;
                for (int i = 0; i < _BorderII.Count; i++)
                {
                    int i1 = CycleIndex(i + 1, _BorderII.Count);
                    Coord ptthis = _BorderII[i];
                    Coord ptnext = _BorderII[i1];
                    if (ptthis.Y <= q.Y)
                    {
                        if (ptnext.Y > q.Y)
                            if (WhichSide(ptthis, ptnext, q) > 0)
                                winding++;
                    }
                    else
                    {
                        if (ptnext.Y <= q.Y)
                            if (WhichSide(ptthis, ptnext, q) < 0)
                                winding--;
                    }
                }
                if (winding == 0)
                    return false;
                return true;
            }
            /// <summary>
            /// 将2个图形互相做切然后将所有切边组合成为新的多边形，并将新多边形覆盖掉this
            /// </summary>
            /// <param name="q"></param>
            public void Intersect(BorderShapeII q)
            {
                List<Vector> v1 = this.VisibleEdgesIn(q);
                List<Vector> v2 = q.VisibleEdgesIn(this);
                BorderShapeII intersect = this.CombineEdges(v1, v2);
                if (intersect == null)
                {
                    q.Data.Reverse();
                    v1 = this.VisibleEdgesIn(q);
                    v2 = q.VisibleEdgesIn(this);
                    intersect = this.CombineEdges(v1, v2);
                    if( intersect == null )
                    {
                        Clear();
                        return;
                    }
                }
                if (intersect.Count <= 2)
                {
                    Clear();
                    return;
                }
                intersect.Data.Add(intersect.Data.First());
                _BorderII = intersect._BorderII;
                _IsClosed = true;
            }
        }
}
