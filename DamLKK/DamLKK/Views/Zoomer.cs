using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DamLKK.Views
{
    public enum SCALE_BUTTONS
    {
        NONE,
        IN,
        ZOOM_IN,
        ZOOM_OUT,
        ZOOM_VALUE
    }
    public class Zoomer
    {
        RectangleF rcBoundary;

        public RectangleF Boundary
        {
            get { return rcBoundary; }
        }
        RectangleF rcZoomIn;
        RectangleF rcZoomOut;
        RectangleF rcZoomSteps;
        double zoomMin = 0.01;
        double zoomMax = 100;
        double zoomStep = 1.81;
        int zoomSteps = 15;
        public Zoomer() { OnZoomChange += OnZoom; }
        public event EventHandler OnZoomChange;
        private void OnZoom(object sender, EventArgs e) {}
        double zoomValue = 1.0;
        public double ZoomValue { get { return zoomValue; } }
        public bool OnZoomIn(PointF pt){return rcZoomIn.Contains(pt);}
        public bool OnZoomOut(PointF pt) { return rcZoomOut.Contains(pt); }
        public bool OnThis(PointF pt){return rcBoundary.Contains(pt);}
        public bool OnZoomStep(PointF pt) { return rcZoomSteps.Contains(pt); }
        public SCALE_BUTTONS MouseMove(PointF pt)
        {
            if( OnZoomIn(pt) )
                return SCALE_BUTTONS.ZOOM_IN;
            if (OnZoomOut(pt))
                return SCALE_BUTTONS.ZOOM_OUT;
            if (OnZoomStep(pt))
                return SCALE_BUTTONS.ZOOM_VALUE;
            if (OnThis(pt))
                return SCALE_BUTTONS.IN;
            return SCALE_BUTTONS.NONE;
        }
        // down: 0 down, 1 up, 2 moving
        public SCALE_BUTTONS HitTest(PointF pt, bool isMoving)
        {
            if (OnZoomIn(pt))
            {
                if( !isMoving )
                {
                    zoomValue *= zoomStep;
                    OnZoomChange.Invoke(null, null);
                }
                return SCALE_BUTTONS.ZOOM_IN;
            }
            if (OnZoomOut(pt))
            {
                if(!isMoving)
                {
                    zoomValue /= zoomStep;
                    OnZoomChange.Invoke(null, null);
                }
                return SCALE_BUTTONS.ZOOM_OUT;
            }
            if( OnZoomStep(pt) )
            {
                //CalcZoomValue(pt);
                //OnZoomChange.Invoke(null, null);
                return SCALE_BUTTONS.ZOOM_VALUE;
            }
            if (OnThis(pt))
                return SCALE_BUTTONS.IN;
            return SCALE_BUTTONS.NONE;
        }
        public void CalcZoomValue(PointF pt)
        {
            double z = (pt.Y - rcZoomSteps.Top) / rcZoomSteps.Height;
            z = (1-z) * zoomSteps;
            z = zoomMin * Math.Pow(zoomStep, z);
            zoomValue = z;

            OnZoomChange.Invoke(null, null);
        }
        public void DrawScale(Graphics g, RectangleF rc, Font ft, double zoom, double zmin, double zmax, double zstep, int steps)
        {
            int margin = 20;
            rc.Location = new PointF(rc.Left + margin, rc.Top + margin + 140);
            rc.Width = 80;
            rc.Height = 200;

            zoomValue = zoom;
            zoomMin = zmin;
            zoomMax = zmax;
            zoomStep = zstep;
            zoomSteps = steps;
            //zoomValue = zoom;
            //int opacity = 0x80;

            float top = rc.Top + margin;
            float left = rc.Left;
            float right = left + rc.Width;
            float bottom = top + rc.Height;
            float height = bottom - top;
            float width = right - left;
            rc = new RectangleF(left, top, width, height);
            RectangleF rcReal = new RectangleF(rc.Left, rc.Top, 16, height);
            rcReal = Utils.Graph.Rect.CenterRect(rcReal, rc);

            RectangleF zoomadd = new RectangleF(rcReal.Left, rcReal.Top, rcReal.Width, rcReal.Width);
            RectangleF zoomminus = new RectangleF(rcReal.Left, rcReal.Bottom-rcReal.Width, rcReal.Width, rcReal.Width);
            DrawFrame(g, zoomadd, Color.Black);
            DrawCross(g, zoomadd, Color.Black);
            DrawFrame(g, zoomminus, Color.Black);
            DrawMinus(g, zoomminus, Color.Black);

            // 骨架
            height = rcReal.Height - zoomadd.Height - zoomminus.Height - 2*2;

            RectangleF rcStepsBorder = new RectangleF(zoomadd.Left+2, zoomadd.Bottom + 4, zoomadd.Width-2*2, height-4);
            RectangleF rcBone = new RectangleF(rcStepsBorder.Left, rcStepsBorder.Top, 4, rcStepsBorder.Height);
            rcBone = Utils.Graph.Rect.CenterRect(rcBone, rcStepsBorder);
            Rectangle rcBone1 = Utils.Graph.Rect.Translate(rcBone);

            // 格子
            height = rcBone1.Height;
            List<Rectangle> rcSteps = new List<Rectangle>();
            float stepInterval = height / steps;
            int gridHeight = 2;
            for (int i = 0; i < steps; i++ )
            {
                Rectangle r = new Rectangle((int)rcStepsBorder.Left, (int)(rcBone1.Top+stepInterval/2+(i)*stepInterval), (int)rcStepsBorder.Width, gridHeight);
                rcSteps.Add(r);
                DrawRect(g, r);
            }
            rcZoomSteps = Rectangle.Union(rcSteps.First(), rcSteps.Last());

            DrawRect(g, rcBone1);

            height = rcSteps.Last().Top - rcSteps.First().Top;
            float zoomFrameHeight = zoomadd.Height/2;
            float log = (float)Math.Log((zoom/zmin),zstep);
            float zoomPos = (1-log/(steps))*height;
            float zoomTop = rcSteps.First().Top + zoomPos-zoomFrameHeight/2;
            Rectangle rcZoomFrame = new Rectangle((int)zoomadd.Left, (int)zoomTop, (int)zoomadd.Width, (int)zoomFrameHeight);
            DrawRect(g, rcZoomFrame);
            DrawMinus(g, rcZoomFrame, Color.Black);

            rcBoundary = RectangleF.Union(zoomadd, zoomminus);
            rcBoundary.Width += zoomadd.Width*2;
            rcBoundary.Location = new PointF(rcBoundary.Left-zoomadd.Width, rcBoundary.Top);
            rcBoundary.Inflate(2, 2);
            rcZoomIn = zoomadd;
            rcZoomOut = zoomminus;
            //g.DrawRectangle(Pens.OrangeRed, Utils.Graph.Rect.Translate(rcBoundary));

            RectangleF rcStr = new RectangleF(0, zoomminus.Bottom + 5, 130, 20);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Near;
            Utils.Graph.OutGlow.DrawOutglowText(g, "x" + zoom.ToString("0.00"), ft, rcStr, sf, Brushes.Black, Brushes.WhiteSmoke);
        }
        Rectangle T(RectangleF rc)
        {
            return Utils.Graph.Rect.Translate(rc);
        }
        void DrawRect(Graphics g, Rectangle rc)
        {
            g.FillRectangle(Brushes.White, rc);
            g.DrawRectangle(Pens.Black, rc);
        }
        void DrawFrame(Graphics g, RectangleF rc, Color c)
        {
            Rectangle r = Utils.Graph.Rect.Translate(rc);
            g.FillRectangle(Brushes.White, r);
            using(Pen p = new Pen(c))
                g.DrawRectangle(p, r);
        }
        void DrawCross(Graphics g, RectangleF rc, Color c)
        {
            PointF center = Utils.Graph.Rect.CenterPoint(rc);
            int off = (int)(rc.Width*0.6)/2;
            PointF p1 = new PointF(center.X, center.Y - off);
            PointF p2 = new PointF(center.X, center.Y + off);
            PointF p3 = new PointF(center.X - off, center.Y);
            PointF p4 = new PointF(center.X + off, center.Y);
            using (Pen p = new Pen(c))
            {
                g.DrawLine(p, p1, p2);
                g.DrawLine(p, p3, p4);
            }
        }
        void DrawMinus(Graphics g, RectangleF rc, Color c)
        {
            PointF center = Utils.Graph.Rect.CenterPoint(rc);
            int off = (int)(rc.Width * 0.6) / 2;
            PointF p3 = new PointF(center.X - off, center.Y);
            PointF p4 = new PointF(center.X + off, center.Y);
            using (Pen p = new Pen(c))
            {
                g.DrawLine(p, p3, p4);
            }
        }
    }
}
