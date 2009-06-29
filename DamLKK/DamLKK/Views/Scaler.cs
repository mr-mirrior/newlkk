using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DamLKK.Views
{
    public class Scaler
    {
        public Scaler() {}

        RectangleF boundary;

        public RectangleF Boundary
        {
            get { return boundary; }
        }
        private Rectangle T(RectangleF rc)
        {
            return Utils.Graph.Rect.Translate(rc);
        }
        public void DrawScaler(Graphics g, RectangleF rc, double zoom, Font font)
        {
            int margin = 20;
            rc.Location = new PointF(rc.Left + margin, rc.Bottom - 40);
            rc.Width = 200;
            rc.Height = 50;

            boundary = rc;
            //g.DrawRectangle(Pens.OrangeRed, T(rc));

            double pixels = 100;
            double meters = pixels * (1.0/zoom);
            double intMeters = (int)meters;
            double actual = 0;
            int count = 0;
            string strUnit = "";
            if (intMeters == 0)
            {
                // 小于1米

                intMeters = HighDigitOnly(meters, out count);
                actual = intMeters * Math.Pow(10, -count);
                switch (count)
                {
                    case 3:
                        strUnit = "毫米";
                        break;
                    case 2:
                        strUnit = "厘米";
                        break;
                    case 1:
                        strUnit = "厘米";
                        intMeters *= 10;
                        break;
                    default:
                        strUnit = "缩放比例过小";
                        intMeters = double.NaN;
                        break;
                }
            }
            else
            {
                // 大于1米

                intMeters = HighDigitOnly((int)intMeters, out count);
                actual = intMeters * Math.Pow(10, count);
                switch (count)
                {
                    case 0:
                        strUnit = "米";
                        break;
                    case 1:
                        strUnit = "米";
                        intMeters *= 10;
                        break;
                    case 2:
                        strUnit = "米";
                        intMeters *= 100;
                        break;
                    case 3:
                        strUnit = "公里";
                        break;
                    default:
                        strUnit = "公里";
                        intMeters *= Math.Pow(10, count - 3);
                        break;
                }
            }

            double pixelsAlt = actual * zoom;
            //System.Diagnostics.Debug.Print("{0} {1}, {2:0.0}", (int)intMeters, strUnit, pixelsAlt);
            int lineWidth = 2;
            int height = 8;
            RectangleF rc1 = new RectangleF(rc.Left, rc.Top+20, lineWidth, height);
            RectangleF rc2 = new RectangleF(rc.Left, rc1.Bottom-lineWidth, (float)pixelsAlt, lineWidth);
            RectangleF rc3 = new RectangleF(rc2.Right - lineWidth, rc1.Top, lineWidth, height);
            RectangleF rc4 = new RectangleF(rc.Left, rc.Top, rc2.Width, rc.Height);
            GraphicsPath gp = new GraphicsPath();
            PointF pt1 = rc1.Location;
            PointF pt2 = new PointF(rc1.Left, rc1.Bottom);
            PointF pt3 = new PointF(rc2.Right, rc1.Bottom);
            PointF pt4 = new PointF(rc2.Right, rc1.Top);
            PointF[] pts = new PointF[] { pt1,pt2,pt3,pt4 };
            gp.AddLines(pts);
//             g.FillRectangle(Brushes.Black, T(rc1));
//             g.FillRectangle(Brushes.Black, T(rc2));
//             g.FillRectangle(Brushes.Black, T(rc3));
            SmoothingMode sm = g.SmoothingMode;
            using(Pen pin = new Pen(Color.Black, 2), pout = new Pen(Color.White, 4))
            {
                g.SmoothingMode = SmoothingMode.None;
                g.DrawPath(pout, gp);
                g.DrawPath(pin, gp);
            }
            g.SmoothingMode = sm;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;
            //g.DrawString(string.Format("{0}{1}", (int)intMeters, strUnit), font, Brushes.Black, rc4, sf);
            string str = string.Format("{0}{1}", (int)intMeters, strUnit);
            Utils.Graph.OutGlow.DrawOutglowText(g, str, font, rc4, sf, Brushes.Black, Brushes.WhiteSmoke);

            boundary.Inflate(2, 2);
        }
        private int HighDigitOnly(int x, out int count)
        {
            count = 0;
            while(x > 10)
            {
                x /= 10;
                count++;
            }
            //x *= (int)Math.Pow(10, count);
            return x;
        }
        private double HighDigitOnly(double x, out int count)
        {
            count = 0;
            while (x<1)
            {
                x *= 10;
                count++;
            }
            x = (int)x;
            //x *= Math.Pow(10, -count);
            return x;
        }
    }
}
