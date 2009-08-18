using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DamLKK.Views
{
    public class RectSelector
    {
        public RectSelector(){}
        public Rectangle T(RectangleF rc) { return Utils.Graph.Rect.Translate(rc); }
        public void DrawSelector(Graphics g, Geo.DMRectangle rc1, Font font, Geo.Coord c, PointF cur, LayerView layer)
        {
            rc1.Normalize();
            Geo.Coord down = rc1.LeftTop;
            Geo.Coord cursor = rc1.RightBottom;

            RectangleF rc = rc1.RF;
            StringFormat sf1 = new StringFormat();
            StringFormat sf2 = new StringFormat();
            sf1.Alignment = StringAlignment.Near;
            sf1.LineAlignment = StringAlignment.Near;
            sf1.Trimming = StringTrimming.EllipsisCharacter;
            sf1.FormatFlags = StringFormatFlags.NoWrap;
            sf2.Alignment = StringAlignment.Far;
            sf2.LineAlignment = StringAlignment.Far;
            sf2.Trimming = StringTrimming.EllipsisCharacter;
            sf2.FormatFlags = StringFormatFlags.NoWrap;

            using (Pen p = new Pen(Color.Black))
            {
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                p.DashPattern = new float[] { 4, 4};
                g.DrawRectangle(p, T(rc));
            }

            Geo.Coord realCursor = layer.DamAxisCursor(cursor.PF);/*layer.ScreenToDam(cursor.PF);*/
            Geo.Coord realDown = layer.DamAxisCursor(down.PF);/*layer.ScreenToDam(down.PF);*/
            g.DrawString(realDown.ToString(), font, Brushes.Black, rc, sf1);
            g.DrawString(realCursor.ToString(), font, Brushes.Black, rc, sf2);

//             rc = new RectangleF(cur.X, cur.Y - 20, 1000, 20);
//             StringFormat sf = new StringFormat();
//             sf.Alignment = StringAlignment.Near;
//             sf.LineAlignment = StringAlignment.Far;
//             Utils.Graph.OutGlow.DrawOutglowText(g, c.ToString(), font, rc, sf, Brushes.Black, Brushes.White);

        }
    }
}
