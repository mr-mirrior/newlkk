using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace DamLKK.Forms
{
    public partial class Landscape : Form
    {
        Bitmap bmp;
        Views.LayerView layerview = null;

        public Views.LayerView LayerView
        {
            get { return layerview; }
            set { layerview = value; }
        }

        public Landscape()
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
        private void CreateBMP()
        {
            if (ClientRectangle.Width == 0 || ClientRectangle.Height == 0)
                return;
            if( bmp == null )
            {
                bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            }
            if( !bmp.Size.Equals(this.ClientRectangle.Size) )
            {
                bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (bmp != null)
            {
                e.Graphics.Clear(Color.White);
                e.Graphics.DrawImage(bmp, this.ClientRectangle);
            }
        }
        float zoom = 1.0f;
        Rectangle layerviewDisplay = new Rectangle();
        public void UpdateSize(Rectangle display)
        {
            if (display.Width == 0 || display.Height == 0)
                return;

            layerviewDisplay = display;
            float ratio = (float)display.Width / display.Height;
            int dh = this.Height - this.ClientRectangle.Height;
            int dw = this.Width - this.ClientRectangle.Width;
            int h = this.ClientRectangle.Height;
            int w = (int)(ratio * h);

            zoom = (float)w / display.Width;

            w += dw;
            h += dh;

            Size s = new Size(w, h);
            if (!this.Size.Equals(s))
            {
                this.Size = s;

            }

            CreateBMP();
        }
        public void UpdateLocation()
        {
            Rectangle desktop = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            int x = desktop.Right - this.Width - 20;
            int y = desktop.Bottom - this.Height - 20;
            this.Location = new Point(x, y);
        }
        public Graphics GetGraphics() 
        {
            if (zoom < 0.001)
                return null;
            if (bmp == null)
                return null;
            if (!this.Visible)
                return null;

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.ScaleTransform(zoom, zoom);
            return g;
        }
        Rectangle layerclient;
        public void ReleaseGraphics(Graphics gbmp, Point scrollpos, Rectangle client)
        {
            if (gbmp == null)
                return;

            gbmp.ResetTransform();
            gbmp.ScaleTransform(zoom, zoom);
            scrollpos = new Point(-scrollpos.X, -scrollpos.Y);
            client.Location = scrollpos;
            using (Pen p = new Pen(Color.Black))
                gbmp.DrawRectangle(p, client);

            if (gbmp != null)
                gbmp.Dispose();

            layerclient = client;
            this.Refresh();
        }

        private void Landscape_Resize(object sender, EventArgs e)
        {
            this.SuspendLayout();
            UpdateSize(layerviewDisplay);
            this.ResumeLayout();
            if (layerview != null)
            {
                layerview.Refresh();
            }
        }

        private void Landscape_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( e.CloseReason == CloseReason.UserClosing )
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        bool isDown = false;
        PointF cursor;
//         Point down;
        private void Landscape_MouseDown(object sender, MouseEventArgs e)
        {
            isDown = true;
            UpdatePosition();
        }

        private void Landscape_MouseUp(object sender, MouseEventArgs e)
        {
            isDown = false;
        }
        private void UpdatePosition()
        {
            if (layerview == null)
                return;

            if (isDown)
            {
                float x = cursor.X;
                float y = cursor.Y;
                float width = layerclient.Width;
                float height = layerclient.Height;
                x = x / zoom;
                y = y / zoom;
                layerview.MyScrollX = (int)(width / 2 - x);
                layerview.MyScrollY = (int)(height / 2 - y);
                layerview.MyRefresh();
            }
        }
        private void CheckFocus()
        {
            if (this.Focused == false)
                this.Focus();
        }
        private void Landscape_MouseMove(object sender, MouseEventArgs e)
        {
            CheckFocus();
            cursor = e.Location;
            UpdatePosition();
        }

        private void Landscape_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1.0;
        }

        private void Landscape_Deactivate(object sender, EventArgs e)
        {
            this.Opacity = 0.5;
        }

        private void Landscape_KeyDown(object sender, KeyEventArgs e)
        {
            layerview.ProcessKeys(e);
        }

    }
}
