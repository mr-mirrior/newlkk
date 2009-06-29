using System.Drawing.Drawing2D;
using System.Drawing;
using System;
using System.Windows.Forms;

namespace DamLKK.Views
{
    public class Compass
    {
        InvokeDelegate TickOperation = null;
        RectangleF boundary = new RectangleF();
        RectangleF left, right;
        public RectangleF Boundary { get { return boundary; } }
        // 顺时针

        public event EventHandler OnCW;
        // 逆时针

        public event EventHandler OnCCW;
        public Compass() 
        { 
            OnCW += delegate(object o, EventArgs e) { }; 
            OnCCW += delegate(object o, EventArgs e) { };
            timer.Interval = 10;
            timer.Tick += OnTick; 
        }

        public bool Contains(PointF pt)
        {
            return boundary.Contains(pt);
        }
        Timer timer = new Timer();
        private void OnTick(object sender, EventArgs e)
        {
            if (TickOperation != null) TickOperation();
        }
        private void StartRotate()
        {
            timer.Start();
        }
        private void StopRotate()
        {
            timer.Stop();
        }
        private void CCW()
        {
            OnCCW.Invoke(null, null);
        }
        private void CW()
        {
            OnCW.Invoke(null, null);
        }
        // 鼠标左键：指南针左侧逆时针旋转，指南针右侧顺时针旋转
        public bool ClickOn(PointF pt, bool isDown)
        {
            if (isDown == true)
            {
                if (left.Contains(pt))
                {
                    TickOperation = CCW;
                    StartRotate();
                    return true;
                }
                else if( right.Contains(pt))
                {
                    TickOperation = CW;
                    StartRotate();
                    return true;
                }
            }
            StopRotate();
            return false;
        }
        public bool MoveOn(PointF pt)
        {
            if (!boundary.Contains(pt))
            {
                StopRotate();
                return false;
            }
            return true;
        }
        GraphicsPath compass = null;
        GraphicsPath[] fill = new GraphicsPath[4];
        GraphicsPath strNorth = null;
        PointF[] lst;
        PointF center;
        public void DrawCompass(Graphics g, RectangleF rc, double degree, Font font)
        {
            //degree += 66.863098144532+180;

//             if (compass == null)
            {
                compass = new GraphicsPath();
                strNorth = new GraphicsPath();
                for (int i = 0; i < fill.Length; i++ )
                {
                    fill[i] = new GraphicsPath();
                }
                int margin = 10;
                rc.Location = new PointF(rc.Left + margin, rc.Top + margin);
                rc.Width = rc.Height = 100;
                float top = rc.Top + font.SizeInPoints + margin/2;
                center = new PointF(rc.Left+rc.Width/2, top +rc.Height/2);
                float off = 4;
                //boundary.AddEllipse(rc);

                PointF p1 = new PointF(rc.Left + rc.Width / 2, top);
                PointF p2 = new PointF(p1.X + rc.Width / 2, p1.Y + rc.Height / 2);
                PointF p3 = new PointF(p1.X, p1.Y + rc.Height);
                PointF p4 = new PointF(p2.X - rc.Width, p2.Y);
                PointF p1x = new PointF(p1.X + off, p1.Y - off + rc.Height / 2);
                PointF p2x = new PointF(p1x.X, p1x.Y + off * 2);
                PointF p3x = new PointF(p2x.X - off * 2, p2x.Y);
                PointF p4x = new PointF(p3x.X, p3x.Y-off*2);

                boundary = new RectangleF(rc.Left, p1.Y, rc.Width, p3.Y-p1.Y);
                left = new RectangleF(boundary.Left, boundary.Top, boundary.Width / 2, boundary.Height);
                right = new RectangleF(left.Left + left.Width, left.Top, left.Width, left.Height);
                using (Pen p = new Pen(Color.Gray))
                    g.DrawLine(p, new PointF(right.Left, right.Top), new PointF(right.Left, right.Bottom));

                lst = new PointF[] { p1, p4x, p4, p3x, p3, p2x, p2, p1x, p1};
                compass.AddLines(lst);
                compass.CloseAllFigures();

                PointF[] f = new PointF[] { p1, center, p4x, p1};
                fill[0].AddLines(f);
                f = new PointF[] { p2, center, p1x, p2 };
                fill[1].AddLines(f);
                f = new PointF[] { p3, center, p2x, p3 };
                fill[2].AddLines(f);
                f = new PointF[] { p4, center, p3x, p4 };
                fill[3].AddLines(f);

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Near;
                sf.Alignment = StringAlignment.Center;
                RectangleF rcstr = rc;
                //rcstr.Offset(0, 5);
                strNorth.AddString("北", font.FontFamily, (int)font.Style, font.SizeInPoints, rcstr ,sf);

            }

            int opacity = 0x80;
            Matrix mtx = new Matrix();
            mtx.RotateAt((float)degree, center);
            GraphicsPath altCompass = (GraphicsPath)compass.Clone();
            altCompass.Transform(mtx);
            GraphicsPath altStr = (GraphicsPath)strNorth.Clone();
            altStr.Transform(mtx);
            RectangleF rc1;
            using (SolidBrush b = new SolidBrush(Color.White))
            {
                b.Color = Color.FromArgb(opacity, b.Color);
                g.FillPath(b, altCompass);
                rc1 = altCompass.GetBounds();
            }
//             using (Pen p = new Pen(Color.White,1.8f))
//                 g.DrawPath(p, altCompass);
            using (Pen p = new Pen(Color.DimGray))
            {
                p.Color = Color.FromArgb(opacity, p.Color);
                g.DrawPath(p, altCompass);
                //Utils.Graph.OutGlow.DrawOutglowPath(g, altCompass, Pens.DimGray, Pens.WhiteSmoke);
                rc1 = RectangleF.Union(rc1, altCompass.GetBounds());
            }

            using (SolidBrush b = new SolidBrush(Color.Chocolate))
            {
                b.Color = Color.FromArgb(opacity, b.Color);
                foreach (GraphicsPath gp in fill)
                {
                    GraphicsPath altFill = (GraphicsPath)gp.Clone();
                    altFill.Transform(mtx);
                    g.FillPath(b, altFill);
                    rc1 = RectangleF.Union(rc1, altFill.GetBounds());
                }
            }
            using(SolidBrush b = new SolidBrush(Color.Black))
            {
                b.Color = Color.FromArgb(opacity, b.Color);
                //g.FillPath(b, altStr);
                Utils.Graph.OutGlow.FillOutglowPath(g, altStr, b, Brushes.WhiteSmoke);
                rc1 = RectangleF.Union(rc1, altStr.GetBounds());
            }
            boundary = rc1;
        }

    }
}
