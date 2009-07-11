using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DamLKK.Forms
{
    /// <鹰眼>
    /// 鹰眼
    /// </鹰眼>
    public partial class EagleEye : Form
    {
        const float LOWEST = 1184f;      //底线
        const float HIGHEST = 1303f;     //高线

        List<List<DamLKK.Geo.Coord>> _MiniData;
 
        List<Color> _MiniColor;
        List<DamLKK._Model.Unit> _WorkUntis;

        /// <summary>
        /// 图上正在工作的单元，开仓时候添加进该单元便可出发修改画图
        /// </summary>
        public List<DamLKK._Model.Unit> WorkUntis
        {
            get { return _WorkUntis; }
            set { _WorkUntis = value; }
        }


        RectangleF _Boundary = new RectangleF();    //边界线
        RectangleF _ScrBoundary = new RectangleF();   //屏幕边界
        List<_Model.Polygon> _Polygons = new List<_Model.Polygon>();   //所有坝段的图形
        /// <summary>
        /// 所有坝段的多边形(屏幕坐标)
        /// </summary>
        public List<_Model.Polygon> Polygons
        {
            get { return _Polygons; }
            set { _Polygons = value; }
        }

    
        Timer _Timer = new Timer();     //闪烁时间
        Color _Color = Color.Black;      //初始闪烁颜色
        PointF _Offset;                  //偏移坐标


        public EagleEye()
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

        public EagleEye(List<List<DamLKK.Geo.Coord>> p_Data,List<Color> p_Color,List<DamLKK._Model.Unit> p_Units)
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;

            _MiniColor = p_Color;
            _MiniData = p_Data;
            _WorkUntis = p_Units;

            Geo.DMMatrix mtx = new Geo.DMMatrix();
            mtx.Zoom = 1;
            mtx.Degrees = 180;

            foreach (List<DamLKK.Geo.Coord> plcoord in _MiniData)
            {
                _Polygons.Add(new DamLKK._Model.Polygon(plcoord));
            }

            for(int i=0;i<_Polygons.Count;i++)
            {
                _Polygons[i].FillColor = _MiniColor[i];
                _Polygons[i].SetVertex(_MiniData[i]);
                _Polygons[i].CreateScreen(mtx);

                if (_ScrBoundary.Width == 0)
                    _ScrBoundary = _Polygons[i].ScreenBoundary.RF;
                else
                    _ScrBoundary = RectangleF.Union(_ScrBoundary, _Polygons[i].ScreenBoundary.RF);

                if (_Boundary.Width == 0)
                    _Boundary = _Polygons[i].Boundary.RF;
                else
                    _Boundary = RectangleF.Union(_Boundary, _Polygons[i].Boundary.RF);

            }

            foreach (_Model.Polygon pl in _Polygons)
            {
                Geo.Coord c = new Geo.Coord(_ScrBoundary.Location);
                pl.OffsetScreen(c.Negative());
            }
            _ScrBoundary.Offset(-_ScrBoundary.Left, -_ScrBoundary.Top);

            int dx = this.Width - ClientRectangle.Width;
            int dy = this.Height - ClientRectangle.Height;
            float h = (float)this.ClientRectangle.Width / _ScrBoundary.Width;
            float v = (float)this.ClientRectangle.Height / _ScrBoundary.Height;
            float zoom = Math.Min(h, v);
        }


        /// <检查全屏y>
        /// 检查全屏 如果全屏最大化 取消边界控制点
        /// </检查全屏>
        /// <param name="full"></param>
        private void CheckFullscr(bool full)
        {
            if (full && this.WindowState == FormWindowState.Maximized)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.ControlBox = false;
                this.Text = null;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Text = "鹰眼";
                this.ControlBox = true;
            }
        }

  
        /// <重置发生>
        /// 重置发生
        /// </重置发生>
        private void EagleEye_Resize(object sender, EventArgs e)
        {
            if (_Boundary.Width == 0)
                return;

            CheckFullscr(true);

            int dx = this.Width - ClientRectangle.Width;
            int dy = this.Height - ClientRectangle.Height;
            int w = ClientRectangle.Width;
            int h = ClientRectangle.Height;
            w = h * 3;
            w += dx;
            h += dy;

            float zoom = (float)ClientRectangle.Width / _Boundary.Width;   //工作矩形的宽度/屏幕上边界的宽度

            Geo.DMMatrix mtx = new Geo.DMMatrix();
            mtx.Zoom = zoom;
            mtx.Degrees = 180;
            _ScrBoundary = new RectangleF();

            System.Diagnostics.Debug.Print("{0}/{1}={2}", ClientRectangle.Width, _Boundary.Width, mtx.Zoom.ToString());

            foreach (_Model.Polygon pl in _Polygons)
            {
                pl.CreateScreen(mtx);
                if (_ScrBoundary.Width == 0)
                    _ScrBoundary = pl.ScreenBoundary.RF;
                else
                    _ScrBoundary = RectangleF.Union(_ScrBoundary, pl.ScreenBoundary.RF);
            }

            Geo.Coord c = new Geo.Coord(_ScrBoundary.Location);
            foreach (DamLKK._Model.Polygon pl in _Polygons)
            {
                pl.OffsetScreen(c.Negative());
            }
            _ScrBoundary.Offset(-_ScrBoundary.Left, -_ScrBoundary.Top);
            Refresh();
        }

        private void EagleEye_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CheckFullscr(false);
        }

        /// <透明>
        /// 被激活就不透明，不被激活就半透明
        /// </透明>
        private void EagleEye_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        private void EagleEye_Deactivate(object sender, EventArgs e)
        {
            this.Opacity = 0.5;
        }

        int _Current = -1; //记录下标
        Point _Cursor;   //存储触发事件的鼠标位置
        private void EagleEye_MouseMove(object sender, MouseEventArgs e)
        {
            CheckFocus();
            if (_Cursor.Equals(e.Location))
                return;
            _Cursor = e.Location;

            float dy = (this.ClientRectangle.Height - _ScrBoundary.Height) / 2;
            PointF pt = e.Location;
            pt.Y -= dy;
            int i = 0;
            bool found = false;
            for (i = 0; i < _Polygons.Count; i++)
            {
                if (_Polygons[i].IsScreenVisible(new Geo.Coord(pt)))
                {
                    if (i != _Current)
                    {
                        _Polygons[i].FillColor = Color.YellowGreen;
                        tpp.SetToolTip(this, DamLKK._Model.Dam.GetInstance().Blocks[i].BlockName);
                        if (_Current != -1)
                            _Polygons[_Current].FillColor = _MiniColor[_Current];
                        _Current = i;
                    }
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                if (_Current != -1)
                    _Polygons[_Current].FillColor = _MiniColor[_Current];
                tpp.SetToolTip(this, null);
                _Current = -1;
            }
            Refresh();
        }

        /// <如果鼠标移动到控件设置控件获得焦点>
        /// 如果鼠标移动到控件设置控件获得焦点
        /// </如果鼠标移动到控件设置控件获得焦点>
        private void CheckFocus()
        {
            if (this.Focused == false)
                this.Focus();
        }

        private void EagleEye_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void EagleEye_Load(object sender, EventArgs e)
        {
            EagleEye_Resize(null, null);
            _Timer.Interval = 500;
            _Timer.Tick += OnTick;
            _Timer.Start();

            Rectangle rc = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            this.Location = new Point(rc.Right - this.Width - 230, rc.Bottom - this.Height - 30);

            //OnWorkingLayersChange(null, null);

            this.WindowState = FormWindowState.Maximized;
            CheckFullscr(true);
        }


        /// <闪烁>
        /// 闪烁
        /// </闪烁>
        private void OnTick(object sender, EventArgs e)
        {
            if (_Color == Color.Black)
                _Color = Color.White;
            else
                _Color = Color.Black;
            Invalidate();
        }

        /// <高程转y坐标>
        /// 高程转y坐标
        /// </高程转y坐标>
        private float ElevationToY(float e)
        {
            float l = LOWEST;
            float h = HIGHEST;
            float x = e;
            float percent = (x - l) / (h - l);
            float y = _ScrBoundary.Height * (1 - percent);
            return y;
        }
        /// <y坐标转高程>
        /// y坐标转高程
        /// </y坐标转高程>
        private float YToElevation(float y)
        {
            y -= _Offset.Y;
            y /= _ScrBoundary.Height;
            y = 1 - y;
            y *= (HIGHEST - LOWEST);
            y += LOWEST;
            return y;
        }


        /// <重载OnPaint方法>
        /// 重载OnPaint方法
        /// </重载OnPaint方法>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                StringFormat sf5 = new StringFormat();
                sf5.Alignment = StringAlignment.Center;
                sf5.LineAlignment = StringAlignment.Near;
                using (Font ft1 = new Font(Font.FontFamily, 32, FontStyle.Bold))
                {
                    string str = "龙   开   口   水   电   站" + "\n 大   坝   填   筑   质   量   GPS   监   控   系   统";
                    g.DrawString(str, ft1, Brushes.Black, this.ClientRectangle, sf5);
                }
            }

            float dx = 0;
            float dy = (this.ClientRectangle.Height - _ScrBoundary.Height) / 2;
            _Offset = new PointF(dx, dy);
            g.TranslateTransform(dx, dy);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (DamLKK._Model.Polygon pl in _Polygons)
            {
                pl.Draw(g);
            }

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            #region - --------------------------画正在工作的单元 ---------------------------

            for (int i = 0; i < _WorkUntis.Count; i++)
            {
                float y = ElevationToY((float)_WorkUntis[i].StartZ);
                PointF p1 = new PointF(ClientRectangle.Left, ClientRectangle.Top + y);
                PointF p2 = new PointF(ClientRectangle.Right, ClientRectangle.Top + y);
                List<_Model.Polygon> pls = new List<DamLKK._Model.Polygon>();
                foreach (DamLKK._Model.Block b in _WorkUntis[i].Blocks)
                {
                    Region rg = _Polygons[b.BlockID - 1].SetDrawClip(g);
                    using (Pen p = new Pen(_Color, 2))
                        g.DrawLine(p, p1, p2);
                    g.Clip = rg;
                }

            }

            #endregion

            #region -- -----------------------------画高低线 -----------------------------------
            float lowy = ElevationToY(LOWEST);
            float hiy = ElevationToY(HIGHEST);
            PointF plow1 = new PointF(this.ClientRectangle.Left, this.ClientRectangle.Top + lowy);
            PointF plow2 = new PointF(this.ClientRectangle.Right, plow1.Y);
            PointF phi1 = new PointF(plow1.X, this.ClientRectangle.Top + hiy);
            PointF phi2 = new PointF(this.ClientRectangle.Right, this.ClientRectangle.Top + hiy);
            g.DrawLine(Pens.Black, plow1, plow2);
            g.DrawLine(Pens.Black, phi1, phi2);
            RectangleF rc1 = new RectangleF(plow1, new SizeF(this.ClientRectangle.Width, 20));
            RectangleF rc2 = new RectangleF(phi1.X, phi1.Y - 20, this.ClientRectangle.Width, 20);
            StringFormat sf1 = new StringFormat();
            StringFormat sf2 = new StringFormat();
            sf1.Alignment = StringAlignment.Near;
            sf2.Alignment = StringAlignment.Far;
            sf1.LineAlignment = StringAlignment.Far;
            sf2.LineAlignment = StringAlignment.Far;
            g.DrawString("最低：" + LOWEST.ToString() + "米", Font, Brushes.Black, rc1, sf1);
            g.DrawString("最高" + HIGHEST.ToString() + "米", Font, Brushes.Black, rc2, sf2);
            #endregion

           

            #region ------------------------------- 画该处高程 ---------------------------------
            //////////////////////////////////////////////////////////////////////////
            PointF pp1 = new PointF(ClientRectangle.Left, _Cursor.Y - _Offset.Y);
            PointF pp2 = new PointF(ClientRectangle.Right, pp1.Y);
            if (pp1.Y <= 0 || pp1.Y > _ScrBoundary.Height || !this.Focused)
                return;
            //虚线
            using (Pen p = new Pen(Color.White, 3))
                g.DrawLine(p, pp1, pp2);
            using (Pen p = new Pen(Color.Black))
            {
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                p.DashPattern = new float[] { 6, 6 };
                g.DrawLine(p, pp1, pp2);
            }
            RectangleF rc = new RectangleF(pp1.X, pp2.Y - 25, ClientRectangle.Width, 20);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Far;
            string s = "该处高程=" + YToElevation(_Cursor.Y).ToString("0.00") + "米";
            RectangleF rc3 = rc;

            Font ft = new Font(Font, FontStyle.Bold);
            Utils.Graph.OutGlow.DrawOutglowText(g, s, ft, rc3, sf, Brushes.Black, Brushes.WhiteSmoke);
            #endregion

           
        }

        /// <summary>
        /// 实现点击打开单元里的所有正在工作的仓面
        /// </summary>
        private void EagleEye_MouseClick(object sender, MouseEventArgs e)
        {
            //string unitname = CurrentUnit;

            if (_Current == -1)
                return;

            _Model.Unit unit = DamLKK._Model.Dam.GetInstance().WorkUnitFromName(_Current+1,YToElevation(e.Location.Y));

            if (unit == null)
            {
                Utils.MB.OK("抱歉，该分区未发现正在工作的仓面。");
                return;
            }  

            string confirm = string.Format("目前单元{0}正在工作的起始高程为{1}米，现在打开吗？", unit.Name, unit.StartZ.ToString("0.0"));
            if (Utils.MB.OKCancelQ(confirm))
            {
                List<double> tags = DB.UnitDAO.GetInstance().GetTagsInUnit(unit.ID, true);
                foreach (double tag in tags)
                {
                    ToolsWindow.GetInstance().OpenLayer(unit, tag);
                    CheckFullscr(false);
                }
                return;
            }
            else
                return;
            

        }

        private void EagleEye_KeyDown(object sender, KeyEventArgs e)
        {
            Main.GetInstance().ProcessKeys(sender, e);
        }
    }
}
