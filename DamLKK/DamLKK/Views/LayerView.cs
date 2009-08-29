#define OPTIMIZED_PAINTING
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using DamLKK.Geo;
using System.Runtime.InteropServices;
using DamLKK._Model;

namespace DamLKK.Views
{
    public partial class LayerView : UserControl, IDisposable
    {
        public LayerView()
        {
            InitializeComponent();
        }

        public LayerView(Layer p_Layer)
        {
            InitializeComponent();
            _MyLayer = p_Layer;
        }

        public new void Dispose() { _MyLayer.Dispose(); _Landscape.Close(); this.Dispose(true); GC.SuppressFinalize(this); }

        Layer _MyLayer=new Layer();
        /// <summary>
        /// 显示的层实体
        /// </summary>
        public Layer MyLayer
        {
            get { return _MyLayer; }
            set { _MyLayer = value; }
        }

        bool _IsPreview;  //是否预览

        public bool IsPreview
        {
            get { return _IsPreview; }
            set { _IsPreview = value; }
        }


        const int PAD_HORZ = 5;
        const int PAD_VERT = 5;
        const double ZOOM_MIN = 1;
        const double ZOOM_MAX = 100;
        const int ZOOM_STEPS = 20;
        double ZOOM_STEP_FACTOR = Math.Pow(ZOOM_MAX / ZOOM_MIN, (double)1 / ZOOM_STEPS);

        PointF _OrigDownPos;
        PointF _DownPos;
        PointF _CursorPos;

        // 是否反走样绘图
        bool _Smooth = true;
        [DefaultValue(true)]
        public bool Smooth { get { return _Smooth; } set { _Smooth = value; MyRefresh(); } }
        bool _IsMoving=false;
        bool _IsZooming=false;
        bool _IsDown = false;
        bool _IsRectSelecting = false;
        bool _LockCursor = false;
        bool _IsPolySelecting = false;
        bool _IsDeckInput = false;

        public bool IsDeckInput
        {
            get { return _IsDeckInput; }
            set { _IsDeckInput = value; }
        }

        bool _IsMenuDeckOn = false;

        private bool IsRectSelecting
        {
            get { return _IsRectSelecting; }
            set
            {
                bool was = _IsRectSelecting;
                _IsRectSelecting = value;
                MyRefresh();
                if (was && !value)
                {
                    DMRectangle rc = new DMRectangle();
                    rc.LeftTop = new Coord(_CursorPos);
                    rc.RightBottom = new Coord(_OrigDownPos);
                    rc.Normalize();
                    rc.Offset(-_MoveOffset.X, -_MoveOffset.Y);
                    Coord p1 = rc.LeftTop;
                    Coord p2 = rc.RightTop;
                    Coord p3 = rc.RightBottom;
                    Coord p4 = rc.LeftBottom;
                    List<Coord> lc = new List<Coord>(new Coord[] { p1, p4, p3, p2, p1 });
                    DeckClip(lc);
                }
            }
        }
        public float MouseMoveDeltaVert { get { float y = _CursorPos.Y - _DownPos.Y; _DownPos =_CursorPos; return y; } }
        public bool IsPolySelecting
        {
            get { return _IsPolySelecting; }
            set
            {
                if (_IsPolySelecting && !value)
                {
                    // 确认选择
                    DeckClip(_DeckSelectPolygon);
                    _DeckSelectPolygon.Clear();
                    _LockCursor = false;
                    RestoreCursor();
                }
                if (_IsDeckInput)
                {
                    DeckClip(_DeckSelectPolygon);
                    _DeckSelectPolygon.Clear();
                    _IsDeckInput = false;
                }
                _IsPolySelecting = value;
            }
        }

        bool _IsPainting = false;
        Cursor _Magnify;     //鼠标指针图像
        Zoomer _Zoomer = new Zoomer();  //缩放对象
        Compass _Compass = new Compass();//罗盘
        Forms.Landscape _Landscape = new Forms.Landscape(); //鸟瞰图
        Timer _TmPaint = new Timer();     //重绘时间
        RectSelector _Selector = new RectSelector();

        Operations _CurrentOp = Operations.OBSERVE;
        public List<Coord> _DeckSelectPolygon = new List<Coord>();   //点选仓面的点列表


        HScrollBar hscr = new HScrollBar();
        VScrollBar vscr = new VScrollBar();
        #region -光标-

        public Operations Operation
        {
            get { return _CurrentOp; }
            set
            {
                bool cancel = false;
                OnOperationChange(_CurrentOp, value, ref cancel);
                if (cancel)
                    return;

                _CurrentOp = value;
                Cursor = OperationCursor();
            }
        }

        /// <summary>
        /// 显示鹰眼
        /// </summary>
        /// <param name="show"></param>
        public void ShowLandscape(bool show)
        {
            if (show == false)
                _Landscape.Visible = show;
            else
                CheckScrollVisible();
        }


        private void OnOperationChange(Operations old, Operations newop, ref bool cancel)
        {
            if (old != Operations.DECK_RECT && newop == Operations.DECK_RECT)
            {
                _DeckSelectPolygon.Clear();
            }
            if (newop == Operations.ROOL_COUNT)
                tpp.SetToolTip(this, null);
        }

        /// <summary>
        /// 操作光标
        /// </summary>
        /// <returns></returns>
        private Cursor OperationCursor()
        {
            return OperationsToCursor.Cursor(_CurrentOp, _Magnify);
        }
        #endregion

        double _Zoom = 10;

        /// <summary>
        /// 层试图的放大系数
        /// </summary>
        public double Zoom
        {
            get { return _Zoom; }
            set
            {
                if (AlwaysFitScreen)
                    return;
                _Zoom = value;
                if (_Zoom < ZOOM_MIN)
                    _Zoom = ZOOM_MIN;
                if (_Zoom > ZOOM_MAX)
                    _Zoom = ZOOM_MAX;
            }
        }

        bool _AlwaysFitScreen = false;  //总在屏幕最适合的位置

        /// <summary>
        /// 是否在屏幕最适合的位置
        /// </summary>
        public bool AlwaysFitScreen { get { return _AlwaysFitScreen; } set { _AlwaysFitScreen = value; } }

        double _RotateDegrees = 0;
        /// <summary>
        /// 旋转的度数
        /// </summary>
        public double RotateDegrees { get { return _RotateDegrees; } set { value %= 360; _RotateDegrees = value; } }

        Point _MoveOffset = new Point(PAD_HORZ, PAD_VERT);

        internal volatile bool _Dirty = true;

        private bool IsDirty
        {
            get { return _Dirty; }
            set { _Dirty = value; }
        }

        public int MyScrollX
        {
            get { return _MyScrollPos.X; }
            set
            {
                int x = value;
                x = Math.Max(ClientRectangle.Width - _MyScrollSize.Width, x);
                x = Math.Min(0, x);
                _MyScrollPos = new Point(x, _MyScrollPos.Y);

                hscr.Value = -MyScrollX;
                IsDirty = true;
            }
        }
        public int MyScrollY
        {
            get { return _MyScrollPos.Y; }
            set
            {
                int y = value;
                y = Math.Max(ClientRectangle.Height -_MyScrollSize.Height, y);
                y = Math.Min(0, y);
                _MyScrollPos = new Point(_MyScrollPos.X, y);

                if (-y >= vscr.Minimum && -y < vscr.Maximum)
                    vscr.Value = -y;
                IsDirty = true;
            }
        }

        /// <初始化>
        /// 初始化
        /// </初始化>
        public void Init()
        {
            _MyLayer.OwnerView = this;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;

            if (IsPreview)
                return;
            this.AutoScroll = false;

            Operation = Operations.SCROLL_FREE;
            _Magnify = new Cursor(GetType(), "MAGNIFY.CUR");
            _Zoomer.OnZoomChange += OnZoomChange;
            _Compass.OnCCW += OnCCW;
            _Compass.OnCW += OnCW;

            _Landscape.Visible = false;
            _Landscape.Show(this);
            Rectangle rc = this.ClientRectangle;
            rc = this.RectangleToScreen(rc);
            _Landscape.Location = new Point(rc.Right - _Landscape.Width, rc.Bottom - _Landscape.Height);

            _Landscape.LayerView = this;
            _Landscape.Visible = false;

#if OPTIMIZED_PAINTING
            _TmPaint.Interval = _Model.Config.I.REFRESH_TIME;
            _TmPaint.Tick -= OnTickPaint;
            _TmPaint.Tick += OnTickPaint;
            _TmPaint.Start();
#endif
        }

        public bool IsEqual(Unit unit, Elevation e) { if (unit.Name.Equals(this._MyLayer.MyUnit.Name) && e.Height == this._MyLayer.MyElevation.Height)return true; return false; }
        public bool IsEqual(LayerView lv) { if (lv.Name.Equals(this.Name) && lv._MyLayer.MyElevation.Height == this._MyLayer.MyElevation.Height) return true; return false; }

        /// <summary>
        /// 仓面裁剪
        /// </summary>
        /// <param name="shape"></param>
        private void DeckClip(List<Coord> shape)
        {
            // rc: 未经处理的屏幕坐标，经过滚动处理
            //System.Diagnostics.Debug.Print(rc.ToString());
            _Model.Polygon pl = new _Model.Polygon(shape);
            _MyLayer.CutBy(pl);
        }

        private void OnTickPaint(object sender, EventArgs e)
        {
            if (IsDirty)
                this.Refresh();
        }


        private void OnZoomChange(object sender, EventArgs e)
        {
            Zoom =_Zoomer.ZoomValue;
      
            UpdateGraphics();
        }

        private void OnCW(object sender, EventArgs e)
        {
            RotateDegrees += .1;
            UpdateGraphics();
        }
        private void OnCCW(object sender, EventArgs e)
        {
            RotateDegrees -= .1;
            UpdateGraphics();
        }

        /// <summary>
        /// 当选择激活层时
        /// </summary>
        public void OnActiveTab()
        {
            _IsMenuDeckOn = false;
            ShowLandscape(true);
        }
        /// <summary>
        /// 当层失去激活状态时
        /// </summary>
        public void OnLostTab()
        {
            _MyLayer.HideActiveDeck();
            ShowLandscape(false);
        }

        private void SetTitle()
        {
            if (_MyLayer == null) return;
            this.Text = _MyLayer.MyUnit.Name + _MyLayer.MyElevation.Height.ToString("0.0");
        }

        public bool OpenLayer(_Model.Unit p_unit, _Model.Elevation e)
        {
            if (p_unit == null || e == null)
                return false;

            if (_MyLayer.MyUnit == null || IsPreview)
            {
                _MyLayer.MyUnit = p_unit;
                _MyLayer.MyElevation = e;
                SetTitle();
            }

 
            if (!ReadFile(p_unit, e))
                return false;
            if (!DrawUnit(p_unit,e))
                return false;

            if (!IsPreview)
            {

                _MyLayer.OnMouseEnter += OnLayerEnter;
                _MyLayer.OnMouseEnterDeck += OnDeckEnter;
            }
            UpdateGraphics();
            return true;
        }

        object _UpdateLock = new object();
        /// <更新图形>
        /// 更新图形
        /// </更新图形>
        public void UpdateGraphics()
        {
            lock (_UpdateLock)
            {
                double oldzoom = _MyLayer.Zoom;
                Point pt = _MyScrollPos;

                _MyLayer.Zoom = _Zoom;
                _MyLayer.AlwaysFitScreen = _AlwaysFitScreen;
                _MyLayer.RotateDegree = _RotateDegrees;
                Rectangle rc = this.ClientRectangle;
                rc.Offset(_MoveOffset);
                rc.Offset(_MoveOffset);
                rc.Width -= 4 * 5;// moveOffset.X;
                rc.Height -= 4 * 5;// moveOffset.Y;
                _MyLayer.CreateScreen(rc);
                SetAutoScroll();

                MyScrollX = (int)(pt.X * _Zoom / oldzoom);
                MyScrollY = (int)(pt.Y * _Zoom / oldzoom);

            }
            RequestPaint();
        }

        public void RequestPaint()
        {
            if (IsPreview)
                this.Refresh();
            else
                MyRefresh();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void MyRefresh()
        {
            if (IsPreview)
            {
                this.Refresh();
            }
            else
            {
                lock (_UpdateLock)
                    IsDirty = true;
            }
        }

        /// <summary>
        /// 设置滚动条
        /// </summary>
        private void SetAutoScroll()
        {
            try
            {
                if (IsPreview)
                    return;
                Size sz = _MyLayer.VisibleSize;
                sz.Width += PAD_HORZ * 2;
                sz.Height += PAD_VERT * 2;
                _MyScrollSize = sz;
                _MyDisplayRectangle = new Rectangle(0, 0, sz.Width, sz.Height);

                CheckScrollVisible();

                if (hscr.Visible)
                {
                    hscr.Minimum = 0;
                    hscr.Maximum = _MyDisplayRectangle.Width - ClientRectangle.Width + PAD_HORZ;
                    hscr.SmallChange = hscr.Maximum / 100;
                    hscr.LargeChange = hscr.Maximum / 10;
                    hscr.Maximum += hscr.LargeChange;
                    hscr.Value = 0;
                }
                if (vscr.Visible)
                {
                    vscr.Minimum = 0;
                    vscr.Maximum = _MyDisplayRectangle.Height - ClientRectangle.Height + PAD_VERT;
                    vscr.SmallChange = vscr.Maximum / 100;
                    vscr.LargeChange = vscr.Maximum / 10;
                    vscr.Maximum += vscr.LargeChange;
                    vscr.Value = 0;
                }

                _Landscape.UpdateSize(_MyDisplayRectangle);
                _Landscape.UpdateLocation();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 恢复坐标
        /// </summary>
        private Geo.Coord RestoreCoord(PointF pt)
        {
            return RestoreCoord(new Geo.Coord(pt));
        }
        private Geo.Coord RestoreCoord(Coord c)
        {
            return c.Offset(-_MoveOffset.X, -_MoveOffset.Y);
        }

        /// <summary>
        /// 画鼠标点坐标
        /// </summary>
        /// <param name="g"></param>
        private void DrawAxisCood(Graphics g)
        {
            RectangleF rc = new RectangleF(_CursorPos.X, _CursorPos.Y - 20, 1000, 20);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Far;
            Utils.Graph.OutGlow.DrawOutglowText(g, DamAxisCursor().ToString(), Font, rc, sf, Brushes.Black, Brushes.White);
        }

        /// <summary>
        /// 鼠标点光标
        /// </summary>
        public Coord DamAxisCursor()
        {
            Coord dampt = ScreenToDam(DePadding(_CursorPos));
            Coord damaxis = dampt.ToDamAxisCoord();
           
            return damaxis;
        }

        public Coord DamAxisCursor(PointF cur)
        {
            Coord dampt = ScreenToDam(DePadding(cur));
            Coord damaxis = dampt.ToDamAxisCoord();
            return damaxis;
        }

        /// <summary>
        /// 去掉垫子（上下5像素）
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private PointF DePadding(PointF pt)
        {
            return new PointF(pt.X + PAD_HORZ, pt.Y + PAD_VERT);
        }

        /// <summary>
        /// 屏幕坐标转大坝坐标
        /// </summary>
        private Geo.Coord ScreenToDam(PointF pt)
        {
            pt.X -= _MoveOffset.X;
            pt.Y -= _MoveOffset.Y;
            return _MyLayer.ScreenToEarth(pt);
        }

        /// <summary>
        /// 画选中的图形
        /// </summary>
        /// <param name="g"></param>
        private void DrawRectSelect(Graphics g)
        {
            if (Operation != Operations.DECK_RECT )
                return;
            if (!IsRectSelecting)
            {
                DrawAxisCood(g);
                return;
            }
            DMRectangle rc = new DMRectangle();
            PointF down = new PointF(_DownPos.X - _MoveOffset.X, _DownPos.Y - _MoveOffset.Y);
            PointF cursor = new PointF(_CursorPos.X - _MoveOffset.X, _CursorPos.Y - _MoveOffset.Y);
            rc.LeftTop = new Coord(down);
            rc.RightBottom = new Coord(cursor);
            if (rc.Width == 0 || rc.Height == 0)
                return;
            _Selector.DrawSelector(g, rc, Font, DamAxisCursor(), cursor, this);
        }

        /// <summary>
        /// 判断图形是否是闭合的
        /// </summary>
        /// <param name="lastpoint"></param>
        /// <returns></returns>
        private bool IsPolySelectingClosed(Coord lastpoint)
        {
            if (_DeckSelectPolygon.Count <= 2) return false;
            Coord first = _DeckSelectPolygon.First();
            Coord delta = lastpoint - first;
            double x = Math.Abs(delta.X);
            double y = Math.Abs(delta.Y);
            return x < 10 && y < 10;
        }

        /// <summary>
        /// 重置光标
        /// </summary>
        private void RestoreCursor()
        {
            if (_LockCursor)
                return;
            if (Cursor != OperationCursor())
                Cursor = OperationCursor();
        }

        /// <summary>
        /// 画选择的仓面
        /// </summary>
        private void DrawPolySelect(Graphics g)
        {
            if (Operation != Operations.DECK_POLYGON)
                return;
            List<Coord> lc = new List<Coord>(_DeckSelectPolygon);
            Coord cursor = RestoreCoord(_CursorPos);
            lc.Add(cursor);
            _Model.Polygon pl = new _Model.Polygon();
            pl.NeedClose = false;
            pl.SetScreenVertex(lc);;
            PolySelector.DrawPolySelect(g, pl, this.Font, DamAxisCursor(), _CursorPos);
            if (IsPolySelectingClosed(cursor))
            {
                _LockCursor = true;
                Cursor = Cursors.Hand;
            }
            else
            {
                _LockCursor = false;
                RestoreCursor();
            }
        }

        /// <summary>
        /// 检查那个层获得焦点
        /// </summary>
        private void CheckFocus()
        {
            if (!this.Focused)
            {
                this.Focus();
                if (IsPreview)
                    return;
                Forms.ToolsWindow.GetInstance().CurrentLayer = this;
                Forms.ToolsWindow.GetInstance().HidePreview();
            }
        }

        /// <summary>
        /// 鼠标移动时发生
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point location = MyCoordSys(e.Location);

            _CursorPos = ScrollPoint(e.Location);
            CheckFocus();
            if (_IsZooming)
            {
                _Zoomer.CalcZoomValue(e.Location);
                return;
            }
            if (_IsDown)
                _IsMoving = true;
            else
                _IsMoving = false;
            if (_IsDown && (Operation == Operations.SCROLL_FREE || Operation == Operations.ROOL_COUNT))
            {
                RoamFree();
                return;
            }
            if (_Compass.MoveOn(e.Location))
            {
                Cursor = Cursors.Hand;
                return;
            }
            if ((_Zoomer.MouseMove(e.Location) != SCALE_BUTTONS.NONE))
            {
                Cursor = Cursors.Hand;
                if (_IsDown)
                {
                    _Zoomer.HitTest(e.Location, true);
                }
                return;
            }

            RestoreCursor();
            MyRefresh();
        }



        /// <summary>
        /// 画层
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            lock (_UpdateLock)
            {
                _IsPainting = true;

                if (!this.Visible)
                {
                    _IsPainting = false;
                    return;
                }

                Graphics g = e.Graphics;
                Graphics lg = _Landscape.GetGraphics();
                g.FillRectangle(Brushes.White, this.DisplayRectangle);

                g.TranslateTransform(MyScrollX + _MoveOffset.X, MyScrollY + _MoveOffset.Y);

                if (_MyLayer.Polygons.Count==0||_MyLayer.MyPolygon==null)
                {
                    _IsPainting = false;
                    return;
                }
                if (_Smooth && !_IsMoving && !_IsZooming)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                }
                // 画层
                _MyLayer.Draw(g, RestoreCoord(_CursorPos), true, _IsMoving || _IsZooming, Font);
                if (lg != null) _MyLayer.Draw(lg, RestoreCoord(_CursorPos), false, _IsMoving || _IsZooming, Font);

                if (!_IsPreview)
                {
                    DrawRectSelect(g);
                    DrawPolySelect(g);
                }

                g.ResetTransform();
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                DrawDebugInfo(g);
                if (!IsPreview)
                {
                    // 显示其他信息
                    DrawCompass(g);
                    DrawZoom(g);
                    DrawScale(g);
                }

                if (!IsPreview)
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Near;
                    using (Font ft = new Font(Font.FontFamily, 32, FontStyle.Bold))
                    using (Brush b = new SolidBrush(Color.FromArgb(0xFF, Color.White)), b1 = new SolidBrush(Color.FromArgb(0xFF, Color.Black)))
                    {
                        string str = "龙   开   口   水   电   站" + "\n大   坝   碾   压   质   量   GPS   监   控   系   统";
                        GraphicsPath gp = new GraphicsPath();
                        gp.AddString(str, Font.FontFamily, (int)FontStyle.Bold, 32, this.ClientRectangle, sf);
                        g.FillPath(b, gp);
                        using (Pen p = new Pen(Color.FromArgb(0xFF, Color.Black)))
                            g.DrawPath(p, gp);
                    }

                }
                _Landscape.ReleaseGraphics(lg, _MyScrollPos, ClientRectangle);
                _Dirty = false;
                _IsPainting = false;
            }
        }

        /// <summary>
        /// 画缩放条
        /// </summary>
        private void DrawZoom(Graphics g)
        {
            Rectangle rc = this.ClientRectangle;
            double zm = _MyLayer.Zoom;
            _Zoomer.DrawScale(g, rc, Font, zm, ZOOM_MIN, ZOOM_MAX, ZOOM_STEP_FACTOR, ZOOM_STEPS);
        }


        Scaler _Scaler = new Scaler();
        /// <summary>
        /// 画刻度
        /// </summary>
        private void DrawScale(Graphics g)
        {
            _Scaler.DrawScaler(g, this.ClientRectangle, _Zoom, Font);
        }

        /// <summary>
        /// 画指南针
        /// </summary>
        private void DrawCompass(Graphics g)
        {
            if (IsPreview)
                return;
            _Compass.DrawCompass(g, this.ClientRectangle, RotateDegrees, Font);
        }


        /// <summary>
        /// 画鼠标指向位置的信息
        /// </summary>
        private void DrawDebugInfo(Graphics g)
        {
            RectangleF rc = this.ClientRectangle;
            rc.Location = new PointF(rc.Location.X, rc.Location.Y);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Far;
            PointF pt = MyCoordSys(_CursorPos);

            int[] count = _MyLayer.RollCount(_CursorPos);

            if (count == null)
                count=new int[]{0,0};

            Coord dampt = ScreenToDam(DePadding(_CursorPos));
            
            Coord damaxis = dampt.ToDamAxisCoord();
            //dampt = damaxis.ToEarthCoord();
            //dampt.Y = -dampt.Y;
            PointF scrPt = pt;
            scrPt = new PointF(pt.X, _MyDisplayRectangle.Height - _CursorPos.Y - PAD_VERT * 2);
            string dbg = string.Format("层:{0}, 大地{1}, 施工{2}",
                _MyLayer.Name, dampt.ToString(), string.Format("X={0:0.00},Y={1:0.00};", damaxis.X, damaxis.Y));

            
            
            Utils.Graph.OutGlow.DrawOutglowText(g, dbg, Font, rc, sf, Brushes.Black, Brushes.WhiteSmoke);

            if (IsPreview || Operation != Operations.ROOL_COUNT)
                return;
            rc = _Zoomer.Boundary;
            rc.Offset(0, rc.Height + 50);
            int emSize = (count[0]+count[1]) * 3 + 12;
            emSize = Math.Min(emSize, 80);
            string strCount = string.Format("静碾{0}遍,振碾{1}遍", count[0],count[1]);
            //if (count == null)
            //    strCount = "未碾压";
            Font ft = new Font(Font.FontFamily, emSize, (count[0] + count[1]) == 0 ? FontStyle.Regular : FontStyle.Bold, GraphicsUnit.Pixel);
            SizeF size = g.MeasureString(strCount, ft);
            rc.Location = new PointF(0, rc.Location.Y);
            rc.Height = (int)size.Height + 1;
            rc.Width = 130;
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Near;
            sf.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
            sf.Trimming = StringTrimming.None;
            Color cl = Color.OrangeRed;
            int goodCount = 8;
            _Model.Deck deck = _MyLayer.VisibleDeck;
            if (deck != null)
            {
                goodCount = deck.NOLibRollCount + deck.LibRollCount;
            
            if ((count[0] + count[1]) >= goodCount||count[0]>deck.NOLibRollCount||count[1]>deck.LibRollCount)
                cl = Color.OliveDrab;
            }
            rc.Location = DeScrollPoint(_CursorPos);
            
            GraphicsPath gp = new GraphicsPath();
            gp.AddString(strCount, Font.FontFamily, (int)((count[0] + count[1]) == 0 ? FontStyle.Regular : FontStyle.Bold), emSize, rc, sf);
           
            using (Pen p1 = new Pen(Color.FromArgb(0x0, cl)), p2 = new Pen(Color.FromArgb(0x40, Color.White)))
                Utils.Graph.OutGlow.DrawOutglowPath(g, gp, p1, p2);
            using (Brush p1 = new SolidBrush(cl), p2 = new SolidBrush(Color.Transparent))
                Utils.Graph.OutGlow.FillOutglowPath(g, gp, p1, p2);

            // 坐标信息
            sf.LineAlignment = StringAlignment.Far;
            rc = new RectangleF(rc.Left, rc.Top - 20, rc.Width, 20);
            rc.Offset(5, 0);
            Utils.Graph.OutGlow.DrawOutglowText(g, damaxis.ToString(), Font, rc, sf, Brushes.Black, Brushes.WhiteSmoke);
        }

        /// <summary>
        /// 滚动条上掉坐标 以及反滚动条上的
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private PointF DeScrollPoint(PointF pt)
        {
            pt.X += MyScrollX + PAD_HORZ;
            pt.Y += MyScrollY + PAD_VERT;
            return pt;
        }
        private PointF ScrollPoint(PointF pt)
        {
            pt.X -= MyScrollX + PAD_HORZ;
            pt.Y -= MyScrollY + PAD_VERT;
            return pt;
        }

        /// <summary>
        /// 坐标转换
        /// </summary>
        PointF MyCoordSys(PointF pt)
        {
            return new PointF(pt.X, ClientSize.Height - pt.Y - PAD_VERT * 2);
        }

        Point MyCoordSys(Point pt)
        {
            return new Point(pt.X, ClientSize.Height - pt.Y - PAD_VERT * 2);
        }


        /// <summary>
        /// 检查滚动条是否在显示
        /// </summary>
        public void CheckScrollVisible()
        {
            if (ClientRectangle.Width <= _MyDisplayRectangle.Width)
            {
                hscr.Visible = true;
            }
            else
                hscr.Visible = false;
            if (ClientRectangle.Height <= _MyDisplayRectangle.Height)
            {
                vscr.Visible = true;
            }
            else
                vscr.Visible = false;

            if (hscr.Visible || vscr.Visible)
                _Landscape.Visible = true;
            else
                _Landscape.Visible = false;
        }

        private void CheckMenu(ToolStripMenuItem mi, _Model.DrawingComponent dc)
        {
            _IsMenuDeckOn = false;
            _Model.Deck deck = _MyLayer.VisibleDeck;
            if (deck != null)
            {
                bool check = deck.IsDrawing(dc);
                deck.ShowDrawingComponent(dc, !check);
                Refresh();
                mi.Checked = !check;
            }
        }

        //#region - 滚动相关 -
        Point _MyScrollPos = new Point(0, 0);
        Size _MyScrollSize = new Size();
        Rectangle _MyDisplayRectangle = new Rectangle();

        /// <summary>
        /// 屏幕试穿。。
        /// </summary>
        public void FitScreenOnce()
        {
            MyScrollX = 0;
            MyScrollY = 0;
            _AlwaysFitScreen = true;
            UpdateGraphics();
            _Zoom = _MyLayer.Zoom;
            _AlwaysFitScreen = false;
            _MyLayer.AlwaysFitScreen = false;
        }

        private new bool OnKeyDown(KeyEventArgs e)
        {
            return ProcessKeys(e);
        }

        public bool ProcessKeys(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (_IsRectSelecting)
                {
                    // 取消选择
                    CancelPolySelecting();
                    CancelRectSelecting();
                    e.SuppressKeyPress = true;
                    return true;
                }
            }
            if (e.KeyCode == Keys.Space)
            {
            }
            /*
             * 左键 单击 切换可见

                开仓、关仓：CTRL+ALT+SHIFT+D
                删除仓面：delete

                轨迹：F5
                碾压编数：F6
                超速：F7
                碾压及：F8

                F：F9
                仓面信息：F10
                车辆安排：F11
            */
            switch (e.KeyCode)
            {
                case Keys.D:
                    if (e.Control && e.Shift && e.Alt)
                        if (_MyLayer.VisibleDeck != null)
                        {
                            _Model.Deck dk = _MyLayer.VisibleDeck;
                            if (dk.WorkState== DeckWorkState.WORK)
                                _MyLayer.DeckControl.Stop(dk);
                            else
                                _MyLayer.DeckControl.Start(dk);
                        }
                    return true;
                case Keys.F5:
                    miSkeleton_Click(null, null);
                    return true;
                case Keys.F6:
                    miRollingCount_Click(null, null);
                    return true;
                case Keys.F7:
                    miOverspeed_Click(null, null);
                    return true;
                case Keys.F8:
                    miVehicleInfo_Click(null, null);
                    return true;
                case Keys.F9:
                    tsReport_Click(null, null);
                    return true;
                //case Keys.F10:
                //    miDatTrackMap_Click(null, null);
                //    return true;
                case Keys.F11:
                    miAssignment_Click(null, null);
                    return true;
                case Keys.F12:
                    CreateExperiment(this.Zoom);
                    return true;
            }

            return Forms.Main.GetInstance.ProcessKeys(this, e);
        }

        /// <summary>
        /// 显示该层
        /// </summary>
        public bool ShowLayer(Unit p_unit, Elevation p_elev)
        {
            if (p_unit == null || p_elev == null)
                return false;

            if (_MyLayer.MyUnit == null || IsPreview)
            {
                _MyLayer.MyUnit = p_unit;
                _MyLayer.MyElevation = p_elev;
                //layer.FullPath = e.FullName;
                this.Text = _MyLayer.Name;
            }

            //fullPath = e.FullName;
            if (!ReadFile(p_unit,p_elev))
                return false;

            if (!DrawUnit(p_unit, p_elev))
                return false;



            if (!IsPreview)
            {
                _MyLayer.OnMouseEnter += OnLayerEnter;
                _MyLayer.OnMouseEnterDeck += OnDeckEnter;
            }
            UpdateGraphics();
            return true;
        }

        private bool DrawUnit(Unit p,Elevation e)
        {
            if (p == null)
                return false;
            List<Coord> lst = new List<Coord>();

            if (p.Polygon == null)
                return false;
            lst=p.Polygon.Vertex;

            if (lst == null)
                return false;
            _MyLayer.DrawMe(lst, p, e, !IsPreview);

            return true;
        }

        /// <summary>
        /// 读层边界点文件
        /// </summary>
        public bool ReadFile(Unit p,Elevation e)
        {
            if (p == null)
                return false;
            List<Coord> lst =new List<Coord>();
            foreach (Block b in p.Blocks)
            {
                lst.AddRange(Utils.FileHelper.ReadLayer(_Model.Config.BLOCK_VERTEX + b.BlockName + "\\" + b.BlockName + ".txt", false,false));
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            
            if (lst == null)
                return false;
            _MyLayer.DrawBlocks(lst, p, e, !IsPreview);

            return true;
        }

        private void OnLayerEnter(object sender, EventArgs e)
        {
            if (sender == null)
            {
                tpp.SetToolTip(this, null);
                return;
            }
            if (sender is _Model.Polygon)
            {
                _Model.Polygon pl = (_Model.Polygon)sender;
                Unit unit = _MyLayer.MyUnit;
                _Model.Elevation el = _MyLayer.MyElevation;
                SetTip(this, _MyLayer.Name + " 面积：" + pl.ActualArea.ToString("0.00") + "平方米", "层信息");
            }
        }

        private void OnDeckEnter(object sender, EventArgs e)
        {
            if (sender == null)
            {
                tpp.SetToolTip(this, null);
                return;
            }
            if (sender is _Model.Deck)
            {
                _Model.Deck dk = (_Model.Deck)sender;
                string tip = string.Format("{0} \"{1}\" 面积：{2:0.00}平方米，{3}",
                    dk.Unit.Name, dk.Name, dk.Polygon.ActualArea, dk.WorkState);
                if (dk.WorkState== DeckWorkState.END && dk.POP != -1)
                {
                    tip += "，标准遍数百分比";
                    tip += dk.POP.ToString("P02");
                }
                SetTip(this,
                    tip,
                    "碾压层信息");
            }
        }

        /// <summary>
        /// 设置tooltips
        /// </summary>
        private void SetTip(Control c, string caption, string title)
        {
            if (Operation == Operations.ROOL_COUNT)
                return;

            string orig = tpp.GetToolTip(c);
            if (orig.Equals(caption))
                return;
            tpp.SetToolTip(c, null);
            tpp.ToolTipTitle = title;
            tpp.SetToolTip(c, caption);
        }

        #region - 漫游 -
        private void RoamFree()
        {
            PointF d = DeScrollPoint(_DownPos);
            PointF c = DeScrollPoint(_CursorPos);
            Point p = new Point((int)(d.X - c.X), (int)(d.Y - c.Y));

            int x = MyScrollX;
            int y = MyScrollY;
            int dx = (int)(-p.X * 1);
            int dy = (int)(-p.Y * 1);

            if (dx == 0 && dy == 0)
                return;

            MyScrollX += dx;
            MyScrollY += dy;
         
            MyRefresh();
        }
        // d > 0 往上滚，否则往下滚
        // op = 0: 垂直滚动
        // op = 1: 水平滚动
        // op = 2: 全方位滚动

        private void RoamStepping(int op, int d)
        {

            int x = MyScrollX;
            int y = MyScrollY;

            if (op == 0)
                MyScrollY += d;
            else if (op == 1)
                MyScrollX += d;
            else
            {
                MyScrollX += d;
                MyScrollY += d;
            }

            MyRefresh();
        }
        #endregion


        #region ------------------------事件处理-------------------------------

        private void OnEnter(object sender, EventArgs e) { this.Focus(); }
        private void LayerView_Leave(object sender, EventArgs e) { _Landscape.Visible = false; }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (OnKeyDown(e)) return;
            if (Forms.Main.GetInstance.ProcessKeys(this, e)) return;
        }
        private void OnKeyUp(object sender, KeyEventArgs e) { }
        //        private void OnMouseMove(object sender, MouseEventArgs e) { cursorPos = ScrollPoint(e.Location);  OnMouseMove(e); }
        private void OnMouseEnter(object sender, EventArgs e) { }
        private void OnMouseLeave(object sender, EventArgs e) { }
        private void OnMouseDBClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _IsMoving = false;
                IsRectSelecting = false;
                IsPolySelecting = false;
                _IsDown = false;
                _IsMenuDeckOn = false;
                _MyLayer.SetActiveDeck();
                MyRefresh();
            }
        }
        private void OnMouseClick(object sender, MouseEventArgs e) { OnClick(e); }
        private void OnMouseRightClick(object sender, MouseEventArgs e) { }
        //protected override void OnMouseWheel(MouseEventArgs e) { OnMouseWheel(e); }
        private void OnMouseWheel(object sender, MouseEventArgs e) { }
        private void OnMouseDown(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) OnMouseLeftDown(e); if (e.Button == MouseButtons.Right) OnMouseRightDown(e); }
        private void OnMouseUp(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) OnMouseLeftUp(e); if (e.Button == MouseButtons.Right) OnMouseRightUp(e); }
        private void OnResize(object sender, EventArgs e) { }
        private void OnLoad(object sender, EventArgs e) { Init(); }
        private void OnMouseLeftDown(MouseEventArgs e)
        {
            Point location = MyCoordSys(e.Location);
            if (_IsMenuDeckOn)
            {
                CloseDeckMenu();
                return;
            }
            _IsDown = true;
            _DownPos = _CursorPos;

            if (_Compass.ClickOn(e.Location, true))
                return;
            SCALE_BUTTONS sb = _Zoomer.HitTest(e.Location, false);
            if (sb != SCALE_BUTTONS.NONE)
            {
                if (sb == SCALE_BUTTONS.ZOOM_VALUE)
                {
                    _IsZooming = true;
                    _Zoomer.CalcZoomValue(e.Location);
                    OnZoomChange(null, null);
                }
                return;
            }
            if (Operation == Operations.DECK_RECT || Operation == Operations.DECK_POLYGON)
            {
                OnDeckMouseLeftDown(e);
                return;
            }
            MouseOperation(e);
        }
        private void OnMouseLeftUp(MouseEventArgs e)
        {
            Point location = MyCoordSys(e.Location);

            _IsDown = false;
            _IsZooming = false;
            if (_Compass.ClickOn(e.Location, false))
                return;

            if (_IsMoving)
                _IsMoving = false;
            else
            {
            }
            //             RestoreOp();
        }

        /// <summary>
        /// 鼠标右键点击
        /// </summary>
        /// <param name="e"></param>
        private void OnMouseRightDown(MouseEventArgs e)
        {
            Point location = MyCoordSys(e.Location);

            if (_IsMenuDeckOn)
                return;
            CancelRectSelecting();
            CancelPolySelecting();

        }

        /// <summary>
        /// 右键弹起
        /// </summary>
        /// <param name="e"></param>
        private void OnMouseRightUp(MouseEventArgs e)
        {
            if (Operation == Operations.ZOOM)
            {
                MouseOperation(e);
                return;
            }
            if (_IsMenuDeckOn)
            {
                CloseDeckMenu();
                return;
            }
            if (_MyLayer.FocusedDeck != null)
            {
                OpenDeckMenu(e.Location);
                //  菜单项处理在事件部分
                return;
            }
            MouseOperation(e);

        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            RoamStepping(0, e.Delta);
            MyRefresh();
        }


        private void CancelRectSelecting() { _IsRectSelecting = false; MyRefresh(); }
        // 多边形选择
        private void CancelPolySelecting()
        {
            // 每次取消最后一个选择点

            // 如果列表中只剩下一个点，或者根本没开始，就完全取消选择状态

            if (_DeckSelectPolygon.Count <= 1)
            {
                _IsPolySelecting = false;
                _DeckSelectPolygon.Clear();
                MyRefresh();
                return;
            }
            _DeckSelectPolygon.RemoveAt(_DeckSelectPolygon.Count - 1);
            MyRefresh();
        }

        private void OpenDeckMenu(Point where)                                       //仓面操作
        {
            _Model.Deck visible = _MyLayer.VisibleDeck;
            _Model.Deck focus = _MyLayer.CurrentDeck;
            if (focus == null)
                return;
            Point pt = this.PointToScreen(where);
            _IsMenuDeckOn = true;
            miDeckName.Text = "\"" + focus.Name + "\"";

            miActive.Checked = focus.IsVisible;
            miSkeleton.Checked = _MyLayer.CurrentDeck.IsDrawing(_Model.DrawingComponent.SKELETON);
            miRollingCount.Checked = _MyLayer.CurrentDeck.IsDrawing(_Model.DrawingComponent.BAND);
            miOverspeed.Checked = _MyLayer.CurrentDeck.IsDrawing(_Model.DrawingComponent.OVERSPEED);
            miVehicleInfo.Checked = _MyLayer.CurrentDeck.IsDrawing(_Model.DrawingComponent.VEHICLE);
            miArrows.Checked = _MyLayer.CurrentDeck.IsDrawing(_Model.DrawingComponent.ARROWS);
            if (_Control.LoginControl.User.Authority != DamLKK._Control.LoginResult.ADMIN)
            {
                this.miDatTrackMap.Enabled = false;
            }
            if (_Control.LoginControl.User.Authority == DamLKK._Control.LoginResult.VIEW)
            {
                this.miProperties.Enabled = false;
                this.miAssignment.Enabled = false;
                this.miStartDeck.Enabled = false;
                this.miEndDeck.Enabled = false;
                this.miDelete.Enabled = false;
                this.btnChange.Enabled = false;
                this.tmiNotRolling.Enabled = false;
            }

#if !DEBUG
            if (_MyLayer.CurrentDeck.WorkState != DeckWorkState.WAIT)
            {
                this.tmiNotRolling.Enabled = false;
            }
            if (_MyLayer.CurrentDeck.WorkState == DeckWorkState.END)
            {
                this.btnChange.Enabled = false;
            }
#endif
            menuDeck.Show(pt);

            bool showStart = _MyLayer.CurrentDeck.IsVisible;
            bool showStop = _MyLayer.CurrentDeck.IsVisible;
            bool showPrandAs = _MyLayer.CurrentDeck.IsVisible;

            if (focus == null)
            {
                showStart = false;
                showStop = false;
            }
            else
            {
                showStart &= !(_MyLayer.CurrentDeck.WorkState==DeckWorkState.WORK);
                showStop &= (_MyLayer.CurrentDeck.WorkState == DeckWorkState.WORK);
            }

            if (_Control.LoginControl.User.Authority != DamLKK._Control.LoginResult.VIEW)
            {
                miStartDeck.Enabled = showStart;
                miEndDeck.Enabled = showStop;
                miProperties.Enabled = showPrandAs;
                miAssignment.Enabled = showPrandAs;
                //btnChange.Enabled = showPrandAs;
            }
            else
                btnChange.Enabled = false;
        }
        private void CloseDeckMenu()
        {
            _IsMenuDeckOn = false;
            menuDeck.Hide();
        }
        private void LayerView_Enter(object sender, EventArgs e)
        {

        }

        private void LayerView_Scroll(object sender, ScrollEventArgs e)
        {
            int delta = e.NewValue;
       
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                MyScrollX = -delta;
            }
            else
            {
                MyScrollY = -delta;
            }
            MyRefresh();
        }

        /// <summary>
        /// 结束选点
        /// </summary>
        private void EndRectSelecting()
        {
            if (Operation != Operations.DECK_RECT)
                return;
            IsRectSelecting = !IsRectSelecting;
            if (IsRectSelecting)
            {
                _OrigDownPos = _DownPos;
            }
            MyRefresh();
        }
        
        /// <summary>
        /// 开始选点
        /// </summary>
        private void PolySelecting()
        {
            if (Operation != Operations.DECK_POLYGON)
                return;
            IsPolySelecting = true;
            Coord down = RestoreCoord(_DownPos);
            _DeckSelectPolygon.Add(down);
            if (_DeckSelectPolygon.Count <= 2)
            {
                MyRefresh();
                return;
            }

            Coord first = _DeckSelectPolygon.First();

            if (IsPolySelectingClosed(down))
            {
                _DeckSelectPolygon[_DeckSelectPolygon.Count - 1] = first;
                List<Coord> dkvertex = new List<Coord>();
                
                if (Forms.ToolsWindow.GetInstance()._IsTempSelectDeck)
                {
                    _LockCursor = false;
                    _IsPolySelecting = false;
                    _CurrentOp = Operations.NONE;
                    RestoreCursor();
                    foreach (Coord c in _DeckSelectPolygon)
                    {
                        dkvertex.Add(ScreenToDam(c.PF));
                    }
                    _DeckSelectPolygon.Clear();
                    _MyLayer.VisibleDeck.Polygon = new Polygon(dkvertex);
                    if (DB.DeckDAO.GetInstance().UpdateVertex(_MyLayer.VisibleDeck))
                    {
                        Utils.MB.OK("数据库修改成功！");
                        DamLKK._Control.GPSServer.OpenDeck();
                        UpdateGraphics();
                    }
                    else 
                    {
                        Utils.MB.Warning("数据库修改失败！");
                    }
                    Forms.ToolsWindow.GetInstance()._IsTempSelectDeck = false;

                    return;
                }

                IsPolySelecting = false;
                _CurrentOp = Operations.NONE;
            }
        }

        private void OnDeckMouseLeftDown(MouseEventArgs e)
        {
            EndRectSelecting();
            PolySelecting();
        }

        private void MouseOperation(MouseEventArgs e)
        {
            switch (Operation)
            {
                case Operations.MOVE:
                    break;
                case Operations.ROTATE:
                    if (e.Button == MouseButtons.Left)
                        RotateStepping(30);
                    if (e.Button == MouseButtons.Right)
                        RotateStepping(-30);
                    break;
                case Operations.SCROLL_HORZ:
                    RoamStepping(1, e.Delta);
                    break;
                case Operations.SCROLL_VERT:
                    RoamStepping(0, e.Delta);
                    break;
                case Operations.SCROLL_ALL:
                    RoamStepping(2, e.Delta);
                    break;
                case Operations.ZOOM:
                    if (e.Button == MouseButtons.Left)
                    {
                        ZoomStepping(20);
                    }
                    if (e.Button == MouseButtons.Right)
                    {
                        ZoomStepping(-20);
                    }
                    break;
                default:
                    break;
            }
        }

        // d > 0 放大，否则缩小

        private void ZoomStepping(int deltaPercent)
        {
            // ALT+SHIFT+滚轮：大规模放大、缩小

            // ALT+滚轮：微调放大、缩小

            //             double delta = .5;
            //             if (ModifierKeys == (Keys.Control|Keys.Shift))
            //                 delta = 5;
            Zoom *= (double)(100 + deltaPercent) / 100;

            UpdateGraphics();
        }
        private void Rotating()
        {
            float delta = MouseMoveDeltaVert;
            RotateDegrees += delta;
            UpdateGraphics();
        }
        private void RotateStepping(double deltaDegrees)
        {
            RotateDegrees += deltaDegrees;
            UpdateGraphics();
        }
        #endregion


        #region ---------------------- 仓面菜单处理 -------------------------------------
        /// <summary>
        /// 修改仓面范围
        /// </summary>
        private void btnChange_Click(object sender, EventArgs e)
        {
            _IsMenuDeckOn = false;
            if (_MyLayer.VisibleDeck != null)
                MyLayer.DeckControl.ChangVertex(_MyLayer.VisibleDeck);
        }

        /// <summary>
        /// 设置仓面可见
        /// </summary>
        private void miActive_Click(object sender, EventArgs e)
        {
            _IsMenuDeckOn = false;
            _MyLayer.SetActiveDeck();
        }
        //开仓
        private void miStartDeck_Click(object sender, EventArgs e)
        {
            _IsMenuDeckOn = false;
            if (_MyLayer.VisibleDeck != null)
                MyLayer.DeckControl.Start(_MyLayer.VisibleDeck);
        }
        //关仓
        private void miEndDeck_Click(object sender, EventArgs e)
        {
            _IsMenuDeckOn = false;
            if (_MyLayer.VisibleDeck != null)
                _MyLayer.DeckControl.Stop(_MyLayer.VisibleDeck);
        }
        //删除仓面
        private void miDelete_Click(object sender, EventArgs e)
        {
            _IsMenuDeckOn = false;
            _MyLayer.DeleteCurrentDeck();
        }
        //修改仓面属性
        private void miProperties_Click(object sender, EventArgs e)
        {
            _IsMenuDeckOn = false;
            _MyLayer.ModifyCurrentDeck();
        }
        //车辆派遣
        private void miAssignment_Click(object sender, EventArgs e)
        {
            _IsMenuDeckOn = false;
            _MyLayer.AssignVehicle();
        }
        //车辆派遣历史
        private void miLookHistory_Click(object sender, EventArgs e)
        {
            _MyLayer.DeckControl.LookVehicleHistory(_MyLayer.VisibleDeck);
        }

        //显示轨迹骨架
        private void miSkeleton_Click(object sender, EventArgs e)
        {
            CheckMenu(miSkeleton, _Model.DrawingComponent.SKELETON);
        }
        //超速指示
        private void miOverspeed_Click(object sender, EventArgs e)
        {
            CheckMenu(miOverspeed, _Model.DrawingComponent.OVERSPEED);
        }
        //碾压机信息
        private void miVehicleInfo_Click(object sender, EventArgs e)
        {
            CheckMenu(miVehicleInfo, _Model.DrawingComponent.VEHICLE);
        }
        //轨迹箭头
        private void miArrows_Click(object sender, EventArgs e)
        {
            CheckMenu(miArrows, _Model.DrawingComponent.ARROWS);
        }

        Forms.Waiting dlg = new Forms.Waiting();
        //生成图形报告
        private void tsReport_Click(object sender, EventArgs e)
        {
            _IsMenuDeckOn = false;
            if (_MyLayer.VisibleDeck == null)
                return;

            if (!Utils.MB.OKCancelQ("您确定生成图形报告吗？"))
            {
                return;
            }
            dlg.Dispose();
            dlg = new Forms.Waiting();
            dlg.Start(this, "正在计算，请稍候……", ReportOK, 1000);
        }
        //是否显示轨迹
        private void miSkeleton_Click_1(object sender, EventArgs e)
        {
            CheckMenu(miSkeleton, _Model.DrawingComponent.SKELETON);
        }
        //是否显示边数
        private void miRollingCount_Click(object sender, EventArgs e)
        {
            CheckMenu(miRollingCount, _Model.DrawingComponent.BAND);
        }
        //超速指示
        private void miOverspeed_Click_1(object sender, EventArgs e)
        {
            CheckMenu(miOverspeed, _Model.DrawingComponent.OVERSPEED);
        }
        //碾压机信息
        private void miVehicleInfo_Click_1(object sender, EventArgs e)
        {
            CheckMenu(miVehicleInfo, _Model.DrawingComponent.VEHICLE);
        }
       

        private void 生成压实厚度图TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlg.Dispose();
            dlg = new Forms.Waiting();
            dlg.Start(this, "正在计算，请稍候……", ReportThicknest, 1000);
        }

        private void ReportThicknest()
        {
            //Bitmap[] bp=DB.datamap.DataMapManager.draw(layer.VisibleDeck.DeckInfo.BlockID, layer.VisibleDeck.DeckInfo.DesignZ, layer.VisibleDeck.DeckInfo.SegmentID);
            Bitmap[] bp =DM.DB.datamap.DataMapManager4.draw(_MyLayer.VisibleDeck.Unit.ID, _MyLayer.VisibleDeck.Elevation.Height, _MyLayer.VisibleDeck.ID);
            if (bp == null)
                Utils.MB.Warning("此仓面或者此仓面的下层仓面没有生成数据图，请确认这两个仓面都已在结束碾压监控状态出过图形报告！");
            else
            {
                Image image = (Image)bp[0];
                Image image2 = (Image)bp[1];
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@"C:\OUTPUT\" + _MyLayer.VisibleDeck.Name);
                if (!di.Exists)
                {
                    di.Create();
                }
#if DEBUG
                image.Save(@"C:\OUTPUT\" + _MyLayer.VisibleDeck.Name.Trim() + _MyLayer.VisibleDeck.Elevation.Height.ToString("0.0") + _MyLayer.VisibleDeck.ID.ToString() + "thickness.png");
                //image2.Save(@"C:\OUTPUT\" + _MyLayer.VisibleDeck.Unit.Name + _MyLayer.VisibleDeck.Elevation.Height.ToString("0.0") + _MyLayer.VisibleDeck.ID.ToString() + "elevation.png");
#else
                image.Save(@"C:\OUTPUT\" + _MyLayer.VisibleDeck.Name.Trim() + @"\" + _MyLayer.VisibleDeck.Elevation.Height.ToString("0.0") + _MyLayer.VisibleDeck.ID.ToString() + "thickness.png");
                //image2.Save(@"C:\OUTPUT\" + _MyLayer.VisibleDeck.Name.Trim() + @"\" + _MyLayer.VisibleDeck.Elevation.Height.ToString() + _MyLayer.VisibleDeck.Elevation.Height.ToString("0.0") + _MyLayer.VisibleDeck.ID.ToString() + "elevation.png");
#endif
                image.Dispose();
                image2.Dispose();
                System.IO.FileInfo fi = new System.IO.FileInfo(@"C:\OUTPUT\" + _MyLayer.VisibleDeck.Name.Trim() + @"\" + _MyLayer.VisibleDeck.Unit.Name + _MyLayer.VisibleDeck.Elevation.Height.ToString("0.0") + _MyLayer.VisibleDeck.ID.ToString() + "thickness.png");
                //if (fi.Exists)
#if !DEBUG
                Utils.Sys.SysUtils.StartProgram(fi.FullName, null);
#else
                Utils.Sys.SysUtils.StartProgram(@"C:\OUTPUT\" + _MyLayer.VisibleDeck.Name + _MyLayer.VisibleDeck.Elevation.Height.ToString("0.0") + _MyLayer.VisibleDeck.ID.ToString() + "thickness.png", null);
#endif
            }
            dlg.Finished = true;
        }

        private void ReportOK()
        {
            if (_MyLayer.VisibleDeck == null)
                return;
           
            bool result;
            lock (_UpdateLock)
            {
                result = _MyLayer.VisibleDeck.CreateRollCountReport(_Zoom, false);
            }
            dlg.Finished = true;
            if (!result)
                return;
            System.IO.FileInfo fi = new System.IO.FileInfo(this._MyLayer.CurrentDeck._Rolladdress+"roll.png");
            if (result)
            {
#if !DEBUG
                Utils.Sys.SysUtils.StartProgram(fi.FullName, null);
#else
                Utils.Sys.SysUtils.StartProgram(@"C:\OUTPUT\" + _MyLayer.VisibleDeck.Unit.Name + _MyLayer.VisibleDeck.Elevation.Height.ToString("0.0") + _MyLayer.VisibleDeck.ID.ToString() + "roll.png", null);
#endif
            }
        }

      
        #endregion




#region -----------------------------测试-------------------------------
        List<Geo.GPSCoord> tracking;
        Timer timerTracking = new Timer();
        int trackingCount = 0;
        private void CreateExperiment(double zm)
        {
            // EXPERIMENT
            if (!IsPreview && _MyLayer.DeckControl.Decks.Count != 0)
            {
            }
            else
                return;
            
            _Model.Deck dk = _MyLayer.DeckControl.Decks[0];
            dk.VehicleControl.Clear();

            if (tracking == null)
                tracking = DamLKK.Utils.FileHelper.ReadTracking(@"C:\TrackingExp.txt");

            Coord origin = dk.Polygon.Boundary.LeftTop;
            _Model.TrackGPS.PreFilter(ref tracking);
            _Model.Roller v = new _Model.Roller(dk);

            //dk.VehicleControl.AddVehicle(v);
            //_Model.TrackGPS t = new _Model.TrackGPS(v);
            //v.TrackGPSControl.Tracking = t;
            //v.ScrollWidth = 2.17f;
            //List<GPSCoord> trackingAnother = new List<GPSCoord>(tracking);
            ////_Model.TrackGPS.SetOrigin(ref trackingAnother,origin);
            //t.SetTracking(trackingAnother, 0, 0);
            //t.Color = Color.Blue;
            //v.ID = 100;
            //v.Name = "test1";

            //////origin = origin.Offset(5, 2);
            //List<GPSCoord> trackingYetAnother = new List<GPSCoord>(tracking);
            //_Model.Roller v2 = new _Model.Roller(dk);
            //dk.VehicleControl.AddVehicle(v2);
            //_Model.TrackGPS t2 = new _Model.TrackGPS(v2);
            //v2.TrackGPSControl.Tracking = t2;
            //t2.SetTracking(trackingYetAnother, 5, 2);
            //t2.Color = Color.Orange;
            //v.ScrollWidth = 2.17f;
            //v2.ID = 101;
            //v2.Name = "test2";

            //List<GPSCoord> trackingYetAnother1 = new List<GPSCoord>(tracking);
            //_Model.Roller v3 = new _Model.Roller(dk);
            //dk.VehicleControl.AddVehicle(v3);
            //_Model.TrackGPS t3 = new _Model.TrackGPS(v3);
            //v3.TrackGPSControl.Tracking = t3;
            //t3.SetTracking(trackingYetAnother1, 10, 4);
            //t3.Color = Color.DarkBlue;
            //v3.ID = 102;
            //v3.Name = "test2";

            //List<GPSCoord> trackingYetAnother2 = new List<GPSCoord>(tracking);
            //_Model.Roller v4 = new _Model.Roller(dk);
            //dk.VehicleControl.AddVehicle(v4);
            //_Model.TrackGPS t4 = new _Model.TrackGPS(v4);
            //v4.TrackGPSControl.Tracking = t4;
            //t4.SetTracking(trackingYetAnother2, 15, 6);
            //t4.Color = Color.YellowGreen;
            //v4.ID = 103;
            //v4.Name = "test4";

            _Model.Roller vInstant = new _Model.Roller(dk);
            dk.VehicleControl.AddVehicle(vInstant);
            _Model.TrackGPS tInstant = new _Model.TrackGPS(vInstant);
            vInstant.TrackGPSControl.Tracking = tInstant;
            tInstant.Color = Color.Blue;
            vInstant.ID = 101;
            vInstant.Name = "实时测试";
            vInstant.ListenGPS();

            trackingCount = 0;
            timerTracking.Interval = 10;
            timerTracking.Tick -= OnTickTracking;
            timerTracking.Tick += OnTickTracking;
            timerTracking.Start();
            ExperimentAtOnce(zm);
        }

        private void ExperimentAtOnce(double zm)
        {
            if (_MyLayer.DeckControl.Decks.Count == 0)
                return;

            _Model.Deck dk = _MyLayer.DeckControl.Decks[0];
        }


        private void OnTickTracking(object sender, EventArgs e)
        {
            if (trackingCount >= tracking.Count)
            {
                timerTracking.Stop();
                trackingCount = 0;
                return;
            }
            _Model.Deck dk =_MyLayer.DeckControl.Decks[0];
            _Model.Roller veh = dk.VehicleControl.Rollers[0];
            veh.TrackGPSControl.Tracking.AddOnePoint(tracking[trackingCount],
                0,
                0);
 
            trackingCount++;
            MyRefresh();
        }
#endregion

        #region -------------------------------出实时轨迹图-----------------------------

        private void miDatTrackMap_Click(object sender, EventArgs e)
        {
            _IsMenuDeckOn = false;
            if (_MyLayer.VisibleDeck == null)
                return;

            if (!Utils.MB.OKCancelQ("您确定生成碾压轨迹图吗？"))
            {
                return;
            }
            dlg.Dispose();
            dlg = new Forms.Waiting();
            dlg.Start(this, "正在计算，请稍候……", ReportTrackMap, 1000);
        }

        private void ReportTrackMap()
        {
            if (_MyLayer.VisibleDeck == null)
                return;

            lock (_UpdateLock)
            {
                _MyLayer.VisibleDeck.CreateTrackMap();
            }
            dlg.Finished = true;

            System.IO.FileInfo fi = new System.IO.FileInfo(@"C:\output\" + this._MyLayer.CurrentDeck.Name + @"\" + this._MyLayer.CurrentDeck._Trackingaddress);

#if !DEBUG
                Utils.Sys.SysUtils.StartProgram(fi.FullName, null);
#else
                Utils.Sys.SysUtils.StartProgram(@"C:\output\" + this._MyLayer.CurrentDeck._Trackingaddress, null);
#endif
            
        }

        #endregion

       

    }
}
