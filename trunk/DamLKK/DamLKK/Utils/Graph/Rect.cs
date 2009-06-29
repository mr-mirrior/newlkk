using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DamLKK.Utils.Graph
{
    public static class Rect
    {
        public static PointF CenterPoint(RectangleF rc)
        {
            return new PointF((rc.Left+rc.Right)/2, (rc.Top+rc.Bottom)/2);
        }
        public static Rectangle Translate(RectangleF rc)
        {
            return new Rectangle((int)rc.Left, (int)rc.Top, (int)rc.Width, (int)rc.Height);
        }
        public static List<PointF> Translate(List<Geo.Coord> lc)
        {
            List<PointF> lst = new List<PointF>();
            foreach (Geo.Coord c in lc)
            {
                lst.Add(c.PF);
            }
            return lst;
        }
        public static List<Geo.Coord> Translate(List<PointF> lst)
        {
            List<Geo.Coord> lc = new List<Geo.Coord>();
            foreach (PointF pt in lst)
            {
                lc.Add(new Geo.Coord(pt));
            }
            return lc;
        }
        public static RectangleF Translate(Rectangle rc)
        {
            return new RectangleF(rc.Left, rc.Top, rc.Width, rc.Height);
        }
        public static void DeOffset(ref Rectangle rc, Point pt)
        {
            rc.Offset(-pt.X, -pt.Y);
        }
        public static Rectangle CenterRect(Rectangle sub, Rectangle container)
        {
            RectangleF s = sub;
            RectangleF c = container;
            RectangleF r = CenterRect(s, c);
            return new Rectangle((int)r.Left, (int)r.Top, (int)r.Width, (int)r.Height);
        }
        public static RectangleF CenterRect(RectangleF sub, RectangleF container)
        {
            float w1 = sub.Width;
            float h1 = sub.Height;
            float w2 = container.Width;
            float h2 = container.Height;

            float x, y;
            x = w2 - w1;
            y = h2 - h1;
            x /= 2;
            y /= 2;
            if (x < 0)
                x = sub.Left;
            else
                x += container.Left;
            if (y < 0)
                y = sub.Top;
            else
                y += container.Top;

            return new RectangleF(x , y, w1, h1);
        }
    }
}
