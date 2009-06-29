using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DamLKK.Utils.Graph
{
    public static class OutGlow
    {
        public static void DrawOutglowText(Graphics g,  string text, Font ft, RectangleF rc, StringFormat sf, Brush fill, Brush outglow)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    RectangleF rc1 = rc;
                    rc1.Offset(i, j);
                    g.DrawString(text, ft, outglow, rc1, sf);
                }
            }
            g.DrawString(text, ft, fill, rc, sf);
        }
        public static void DrawOutglowPath(Graphics g, GraphicsPath gp, Pen fill, Pen outglow)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    GraphicsPath gp1 = (GraphicsPath)gp.Clone();
                    Matrix mx = new Matrix();
                    mx.Translate(i, j);
                    gp1.Transform(mx);
                    g.DrawPath(outglow, gp1);
                }
            }
            g.DrawPath(fill, gp);
        }
        public static void FillOutglowPath(Graphics g, GraphicsPath gp, Brush fill, Brush outglow)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    GraphicsPath gp1 = (GraphicsPath)gp.Clone();
                    Matrix mx = new Matrix();
                    mx.Translate(i, j);
                    gp1.Transform(mx);
                    g.FillPath(outglow, gp1);
                }
            }
            g.FillPath(fill, gp);
        }
    }
}
