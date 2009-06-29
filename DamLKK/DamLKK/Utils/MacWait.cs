using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DamLKK.Utils.MacWait
{
    public partial class MacWait : UserControl
    {
        public MacWait()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
        }

        int step = width + 16 * 2;
        int slashWidth = 16;
        const int width = 256;
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rcThis = this.ClientRectangle;
            int top = 30;
            int h = 32;
            int w = width;
            Rectangle rc = new Rectangle(0, 0, w, h);
            rc = Utils.Graph.Rect.CenterRect(rc, rcThis);
            rc = new Rectangle(rc.Left, top, rc.Width, rc.Height);

            Rectangle rcLabel = lbPrompt.ClientRectangle;
            rcLabel = Utils.Graph.Rect.CenterRect(rcLabel, rcThis);
            lbPrompt.Location = new Point(rcLabel.Location.X, rc.Bottom + 10);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillRectangle(Brushes.White, rc);

            g.Clip = new Region(rc);
            for (int i = 0; i <= rc.Width; i += slashWidth * 2)
            {
                Point[] pts = new Point[4];
                pts[0] = new Point( (i+step)%(w+slashWidth*2) -slashWidth +rc.Left, rc.Top);
                pts[1] = new Point(pts[0].X + slashWidth, rc.Top);
                pts[2] = new Point(pts[0].X, rc.Bottom);
                pts[3] = new Point(pts[0].X-slashWidth, rc.Bottom);
                GraphicsPath gp = new GraphicsPath();
                gp.AddLines(pts);
                gp.CloseAllFigures();
                using(LinearGradientBrush b = new LinearGradientBrush(rc, this.ForeColor, Color.White, LinearGradientMode.Vertical))
                {
                    Blend bl = new Blend();
                    bl.Factors = new float[]    { 0.0f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.5f, 0.4f, 0.3f, 0.2f, 0.0f};
                    bl.Positions = new float[] { 0.0f, 0.3f, 0.35f, 0.4f, 0.45f, 0.5f, 0.55f, 0.6f, 0.65f, 0.7f, 1.0f};
                    b.Blend = bl;
                    g.FillPath(b, gp);
                }
                gp.Dispose();
            }

//             rc.Inflate(-1, -1);
//             g.DrawRectangle(Pens.Black, rc);
        }
        public string Prompt { get { return lbPrompt.Text; } set { lbPrompt.Text = value; } }

        private void OnTick(object sender, EventArgs e)
        {
            step -= 1;
            //step %= width+slashWidth*2;
            if (step < 0)
                step = width + slashWidth * 2;
            Refresh();
            //Application.DoEvents();
        }
        Timer updater = new Timer();
        private void MacWait_Load(object sender, EventArgs e)
        {
            updater.Interval = 1;
            updater.Tick += OnTick;
            updater.Start();
        }
    }
}
