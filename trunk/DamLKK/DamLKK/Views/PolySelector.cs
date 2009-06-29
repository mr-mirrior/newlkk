using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DamLKK.Views
{
    public class PolySelector
    {
        public static void DrawPolySelect(Graphics g, _Model.Polygon pl, Font ft, Geo.Coord c, PointF cursor)
        {
            //System.Diagnostics.Debug.Print("{0}", pl.Vertex.Count);
            pl.FillColor = Color.Transparent;
            pl.LineColor = Color.Black;
            pl.LineDashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            pl.LineDashPattern = new float[] { 4, 4 };
            pl.Draw(g);

            RectangleF rc = new RectangleF(cursor.X, cursor.Y-20, 1000, 20);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Far;
            Utils.Graph.OutGlow.DrawOutglowText(g, c.ToString(), ft, rc, sf, Brushes.Black, Brushes.White);
        }
    }
}
