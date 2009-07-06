using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using DamLKK.Geo;
using System.Drawing;

namespace DamLKK._Model
{
    /// <summary>
    /// 层
    /// </summary>
    public class Layer : IDisposable
    {
        public event EventHandler OnMouseEnter;
        public event EventHandler OnMouseLeave;
        public event EventHandler OnMouseEnterDeck;
        public event EventHandler OnMouseLeaveDeck;

        Unit _MyUnit;

        /// <summary>
        /// 从属单元
        /// </summary>
        public Unit MyUnit
        {
            get { return _MyUnit; }
            set { _MyUnit = value; }
        }

        Elevation _MyElevation;

        /// <summary>
        ///  起始规划高程
        /// </summary>
        public Elevation MyElevation
        {
            get { return _MyElevation; }
            set { _MyElevation = value; }
        }

        string _Name;

        public string Name
        {
            get { return _MyUnit.Name+" " + ChangeTag(_MyElevation.Height); }
            set { _Name = value; }
        }

        private string ChangeTag(double t)
        {
            if (t < 100)
                return "斜-" + t.ToString("0");
            else
               return t.ToString();
        }

        Views.LayerView _OwnerView = null;

        /// <summary>
        /// 获得显示界面对象
        /// </summary>
        public Views.LayerView OwnerView
        {
            get { return _OwnerView; }
            set { _OwnerView = value; }
        }

        /// <summary>
        /// 放大率
        /// </summary>
        public double Zoom
        {
            get { return _Mtx.Zoom; }
            set { if (AlwaysFitScreen) return; _Mtx.Zoom = value; }
        }

        List<Deck> _Decks;     //该层的仓面
        Deck _FocusedDeck ;    //交点仓面
        Deck _CurrentDeck ;    //现在的仓面
        Deck _VisibleDeck;     //正在显示的仓面

        double _FitScreenZoom = 1.0;  //合适的屏幕比率

        List<Polygon> _Polygons = new List<Polygon>();

        /// <summary>
        /// 包涵的所有坝段多边形
        /// </summary>
        public List<Polygon> Polygons
        {
            get { return _Polygons; }
            set { _Polygons = value; }
        }

        Polygon _MyPolygon = new Polygon();
        /// <summary>
        /// 本身的多边形
        /// </summary>
        public Polygon MyPolygon
        {
            get { return _MyPolygon; }
            set { _MyPolygon = value; }
        }

        bool _IsMultiLayer = false;
        /// <summary>
        /// 是否有多个多边形组成
        /// </summary>
        public bool IsMultiLayer { get { return _IsMultiLayer; } }

        /// <summary>
        /// 焦点仓面
        /// </summary>
        [XmlIgnore]
        public Deck FocusedDeck { get { return _FocusedDeck; } set { _FocusedDeck = value; } }
        /// <summary>
        /// 现在的仓面
        /// </summary>
        [XmlIgnore]
        public Deck CurrentDeck { get { return _CurrentDeck; } set { _CurrentDeck = value; } }
        /// <summary>
        /// 正在显示的仓面
        /// </summary>

        public Deck VisibleDeck { get { return DeckControl.GetVisibleDeck(); } }

     

        bool _IsDeckInput = false;
        /// <summary>
        /// 是否是输入坐标
        /// </summary>
        public bool IsDeckInput
        {
            get { return _IsDeckInput; }
            set { _IsDeckInput = value; }
        }

        public Layer() { Init(); }
        public Layer(_Model.Unit p_Unit,float p_tag)
        {
            _MyUnit = p_Unit;
            _MyElevation = new Elevation(p_tag);
            _OwnerView = new DamLKK.Views.LayerView(this);
            _Decks = new List<Deck>();

            _MyPolygon.Unit = _MyUnit;
            _MyPolygon.Elevation = _MyElevation;

            string tag;
            if (p_tag < 100)
                tag = "斜-" + ((int)p_tag).ToString();
            else
                tag = p_tag.ToString("0.0");

            _Name=_MyUnit.Name + "-" +tag;

            Init();
        }

        string _FullPath;

        public string FullPath
        {
            get { return _FullPath; }
            set { _FullPath = value; }
        }

        DamLKK.Geo.DMMatrix _Mtx = new DamLKK.Geo.DMMatrix();
        /// <summary>
        /// 初始化模型
        /// </summary>
        private void Init()
        {
            _Mtx.Boundary = new DMRectangle();
            ResetBoundary();
            _Mtx.Zoom = 1;
        }

        /// <summary>
        /// 重置边框
        /// </summary>
        private void ResetBoundary()
        {
            _ScreenBoundary.Left = _ScreenBoundary.Top = float.MaxValue;
            _ScreenBoundary.Right = _ScreenBoundary.Bottom = float.MinValue;
        }

        /// <summary>
        /// 给边框赋值
        /// </summary>
        private void FilterBoundary(DMRectangle rc)
        {
            // 计算变换后的边框
            _ScreenBoundary.Left=Math.Min(_ScreenBoundary.Left, rc.Left);
            _ScreenBoundary.Top=Math.Min(_ScreenBoundary.Top, rc.Top);
            _ScreenBoundary.Right=Math.Max(_ScreenBoundary.Right, rc.Right);
            _ScreenBoundary.Bottom=Math.Max(_ScreenBoundary.Bottom, rc.Bottom);
        }

        /// <summary>
        /// 查找最适合的屏幕比率
        /// </summary>
        private void CheckFitScreen()
        {
            double cwidth = _Canvas.Width;
            double cheight = _Canvas.Height;
            double swidth = _ScreenBoundary.Width;
            double sheight = _ScreenBoundary.Height;
            if (swidth == 0 || sheight == 0)
                return;
            double xzoom = cwidth / swidth;
            double yzoom = cheight / sheight;
            double zoom = Math.Min(xzoom, yzoom);
            _FitScreenZoom = _Mtx.Zoom * zoom;
        }

        /// <summary>
        /// 检查图形可视
        /// </summary>
        private void CheckVisible()
        {
            Coord offset = new Coord(0, 0);

            offset.X = -_ScreenBoundary.Left;
            offset.Y = -_ScreenBoundary.Top;

            if (_ScreenBoundary.Width < _Canvas.Width)
            {
                offset.X += (_Canvas.Width - _ScreenBoundary.Width) / 2;
            }
            if (_ScreenBoundary.Height < _Canvas.Height)
                offset.Y += (_Canvas.Height - _ScreenBoundary.Height) / 2;

            _Mtx.Offset = offset;
            ResetBoundary();
            foreach (Polygon pl in _Polygons)
            {
                pl.OffsetGraphics(_Mtx.Offset);
                FilterBoundary(pl.ScreenBoundary);
            }

            _MyPolygon.OffsetGraphics(_Mtx.Offset);
            FilterBoundary(_MyPolygon.ScreenBoundary);
        }

        /// <summary>
        /// 如果不合适重新赋值
        /// </summary>
        public void DoFitScreen()
        {
            ResetBoundary();
            _Mtx.Zoom = _FitScreenZoom;
            foreach (Polygon pl in _Polygons)
            {
                pl.CreateScreen(_Mtx);
                FilterBoundary(pl.ScreenBoundary);
            }
            _MyPolygon.CreateScreen(_Mtx);
            FilterBoundary(_MyPolygon.ScreenBoundary);
            CheckVisible();
        }


        /// <summary>
        /// 模型
        /// </summary>
        public DamLKK.Geo.DMMatrix DMMatrix
        {
            get { return _Mtx; }
            set { _Mtx = value; }
        }


        bool _AlwaysFitScreen;   //屏幕最佳位置
        DamLKK.Geo.DMRectangle _ScreenBoundary = new DamLKK.Geo.DMRectangle();
        Rectangle _Canvas;

        [XmlIgnore]
        public bool AlwaysFitScreen { get { return _AlwaysFitScreen; } set { _AlwaysFitScreen = value; } }
        [XmlIgnore]
        public double RotateDegree { get { return _Mtx.Degrees; } set { _Mtx.Degrees = value; } }
        [XmlIgnore]
        public System.Drawing.Size VisibleSize { get { return new System.Drawing.Size((int)_ScreenBoundary.Width, (int)_ScreenBoundary.Height); } }

        public bool IsEqual(Layer l) { return IsEqual(l.Name); }
        public bool IsEqual(string n) { return Name.Equals(n, StringComparison.OrdinalIgnoreCase); }

        _Control.DeckControl _Dkcontrol = new _Control.DeckControl(null);
        public _Control.DeckControl DeckControl
        {
            get { return _Dkcontrol; }
            //             set { dkcontrol = value; }
        }

        /// <summary>
        /// 调整面
        /// </summary>
        public void CreateScreen()
        {
            CreateScreen(_Canvas);
        }


        // 之前必须保证AddLayer已经调用
        public void CreateScreen(System.Drawing.Rectangle rcClient)
        {
            _Canvas = rcClient;
            ResetBoundary();
            foreach (Polygon pl in _Polygons)
            {
                pl.Token = true;
                pl.CreateScreen(_Mtx);
                FilterBoundary(pl.ScreenBoundary);
            }

            _MyPolygon.Token = true;
            _MyPolygon.CreateScreen(_Mtx);
            FilterBoundary(_MyPolygon.ScreenBoundary);

            _Mtx.Offset = new Coord(0, 0);
            CheckFitScreen();
            CheckVisible();
            if (AlwaysFitScreen)
                DoFitScreen();

            //仓面操作=====//////////////////////////////////////////////////
            for (int i = 0; i < _Dkcontrol.Decks.Count; i++)
            {
                Polygon pl = _Dkcontrol.Decks[i].Polygon;
                CreateDeckScreen(ref pl);
                foreach (Roller v in _Dkcontrol.Decks[i].VehicleControl.Rollers)
                {
                    v.TrackGPSControl.Tracking.CreateScreen();
                }
            }
        }

        public void Dispose()
        {
            _MyUnit = null;
            _MyElevation = null;
        }
      
        /// <大坝转屏幕>
        /// 大坝转屏幕
        /// </大坝转屏幕>
        public PointF DamToScreen(Coord c)
        {
            Coord pt = new Coord(c);

            pt.X -= _Mtx.At.X;
            pt.Y -= _Mtx.At.Y;
            pt.X *= Zoom;
            pt.Y *= Zoom;
            pt = Geo.DamUtils.RotateDegree(pt, new Coord(0, 0), _Mtx.Degrees);
            pt.X += _Mtx.Offset.X;
            pt.Y += _Mtx.Offset.Y;

            return pt.PF;
        }

        /// <屏幕大小>
        /// 屏幕大小
        /// </屏幕大小>
        public double ScreenSize(double realSize)
        {
            return realSize * _Mtx.Zoom;
        }

        /// <屏幕转大坝>
        /// 屏幕转大坝
        /// </屏幕转大坝>
        public Coord ScreenToDam(PointF pt)
        {
            Coord c = new Coord(pt);

            c.X -= _Mtx.Offset.X;
            c.Y -= _Mtx.Offset.Y;
            c = Geo.DamUtils.RotateRadian(c, new Coord(0, 0), Geo.DamUtils.Degree2Radian(-_Mtx.Degrees));
            c.X /= Zoom;
            c.Y /= Zoom;
            c.X += _Mtx.At.X;
            c.Y += _Mtx.At.Y;

            return c;
        }


        /// <summary>
        /// 将仓面画在屏幕上
        /// </summary>
        /// <param name="dk"></param>
        private void CreateDeckScreen(ref Polygon dk)
        {
            dk.CreateScreen(_Mtx);
            dk.OffsetGraphics(_Mtx.Offset);
        }

        /// <summary>
        /// 批量转换为大坝坐标5
        /// </summary>
        /// <param name="pts"></param>
        public void ScreenToDam(ref List<Coord> pts)
        {
            for (int i = 0; i < pts.Count; i++)
            {
                pts[i] = ScreenToDam(pts[i].PF);
            }
        }

        /// <summary>
        /// 过滤相近的相同的对象
        /// </summary>
        /// <param name="lst"></param>
        private void FilterIdentical(ref List<Coord> lst)
        {
            List<Coord> newlst = new List<Coord>();
            newlst.Capacity = lst.Count;
            for (int i = 0; i < lst.Count - 1; i++)
            {
                if (lst[i].IsEqual(lst[i + 1]))
                {
                    // 这里千万不能用Remove(object)
                    // 否则会删错对象，期望删除第71，却删除了第一个

                    // 估计和Coord.Equals有关？（未重写）
                    lst.RemoveAt(i);
                    i--;
                }
            }
        }


        /// <summary>
        /// 绘制跨越的坝段，仅添加数据，刷新图形，需要执行ConstructGraphics   
        /// </summary>
        /// <param name="vertex"></param>
        public void DrawBlocks(List<Coord> vertex, Unit p, Elevation e, bool loadDB)
        {
            if (vertex == null)
                return;

            FilterIdentical(ref vertex);

            if (vertex.Count <= 3)  // 最少4点才能成为一个多边形，首尾点相同
                return;
            

           
            List<Coord> batch = new List<Coord>();
            bool first = true;
            int id = p.Blocks.First().BlockID;
            for (int i = 0; i < vertex.Count; i++)
            {
                Coord pt = vertex[i];

                // 计算边框
                _Mtx.Boundary.Left=Math.Min(_Mtx.Boundary.Left, pt.X);
                _Mtx.Boundary.Right =Math.Max(_Mtx.Boundary.Right, pt.X);
                _Mtx.Boundary.Top =Math.Min(_Mtx.Boundary.Top, pt.Y);
                _Mtx.Boundary.Bottom=Math.Max(_Mtx.Boundary.Bottom, pt.Y);

                batch.Add(pt);
                if (first)
                {
                    first = false;
                    continue;
                }
                // 如果发现和第一个点相同的点，说明图形封闭
               
                // 添加前面已经统计的点，并开启新统计
                if (pt.IsEqual(batch.First()))
                {
                    AddPolygon(batch, p, e,id++);
                    batch = new List<Coord>();
                    first = true;
                }
            }
            if (batch.Count != 0)
            {
                AddPolygon(batch, p, e,id);
            }

            _Mtx.At = _Mtx.Boundary.LeftTop;//Transform.DamUtils.CenterPoint(mtx.boundary);
        }


        /// <summary>
        /// 绘制自己，仅添加数据，刷新图形，需要执行ConstructGraphics   
        /// </summary>
        /// <param name="vertex"></param>
        public void DrawMe(List<Coord> vertex, Unit p, Elevation e, bool loadDB)
        {
            if (vertex == null)
                return;

            FilterIdentical(ref vertex);

            if (vertex.Count <= 3)  // 最少4点才能成为一个多边形，首尾点相同
                return;
            //layers.Clear();

            if (_MyPolygon.Vertex.Count!= 0)
                _IsMultiLayer = true;
            else
            {
                // 第一次添加层信息
                // 尝试从数据库读取该层的仓面信息    读仓面仓面操作

                if (_Dkcontrol.Decks.Count == 0)
                {
                    _Dkcontrol.Owner = this;
                    if (loadDB)
                        _Dkcontrol.LoadDB(null);
                }
            }

            List<Coord> batch = new List<Coord>();
            bool first = true;
            int id = p.Blocks.First().BlockID;
            for (int i = 0; i < vertex.Count; i++)
            {
                Coord pt = vertex[i];

                // 计算边框
                _Mtx.Boundary.Left = Math.Min(_Mtx.Boundary.Left, pt.X);
                _Mtx.Boundary.Right = Math.Max(_Mtx.Boundary.Right, pt.X);
                _Mtx.Boundary.Top = Math.Min(_Mtx.Boundary.Top, pt.Y);
                _Mtx.Boundary.Bottom = Math.Max(_Mtx.Boundary.Bottom, pt.Y);

                batch.Add(pt);
                if (first)
                {
                    first = false;
                    continue;
                }
                // 如果发现和第一个点相同的点，说明图形封闭

                // 添加前面已经统计的点，并开启新统计
                if (pt.IsEqual(batch.First()))
                {
                    AddPolygon(batch,p,e);
                    batch = new List<Coord>();
                    first = true;
                }
            }
            if (batch.Count != 0)
            {
                AddPolygon(batch,p,e);
            }

            _Mtx.At = _Mtx.Boundary.LeftTop;//Transform.DamUtils.CenterPoint(mtx.boundary);
        }

        /// <summary>
        /// 将仓面设置为激活
        /// </summary>
        public void SetActiveDeck()
        {
            if (CurrentDeck == null)
                return;
            if (CurrentDeck.IsVisible)
                return;
            _Dkcontrol.SetVisibleDeck(CurrentDeck);
        }
        /// <summary>
        /// 将仓面设置为不激活
        /// </summary>
        public void HideActiveDeck()
        {
            if (CurrentDeck == null)
                return;
            if (!CurrentDeck.IsVisible)
                return;
            DamLKK._Control.DeckControl.UnvisibleDeck(CurrentDeck);
        }

        /// <summary>
        /// 加一个图形
        /// </summary>
        private void AddPolygon(List<Coord> batch,Unit p, Elevation e,int blockid)
        {
            Polygon pl = new Polygon(batch);
            pl.Unit = p;
            pl.Elevation = e;
            Color line=Color.BlueViolet, fill=Dam.GetInstance().MiniColor[blockid-1];
            pl.BlockID = blockid;
            pl.LineColor = line;
            pl.FillColor = fill;
            _Polygons.Add(pl);
        }

        private void AddPolygon(List<Coord> batch,Unit p, Elevation e)
        {
            Polygon pl = new Polygon(batch);
            Color line = Color.Black, fill = Color.FromArgb(0x80,204,204,153);
            pl.LineColor = line;
            pl.FillColor = fill;
            _MyPolygon = pl;
            _MyPolygon.Unit = p;
            _MyPolygon.Elevation = e;   
        }

        public bool RectContains(Coord pt)
        {
            return _Mtx.Boundary.Contains(pt.PF);
        }


        // 主界面每次MouseMove都会调用Refresh
        // 所以每次OnPaint都会到这里
        Polygon _LastHoverLayer = null;
        /// <summary>
        /// 画层
        /// </summary>
        public void Draw(Graphics g, Coord scrCursor, bool autoCenter, bool frameonly, Font ft)
        {
            if (Polygons.Count == 0)
                return;

            Polygon thisHoverLayer = null;

            if (!autoCenter)
            {
                PointF offset = this._ScreenBoundary.LeftTop.PF;
                g.TranslateTransform(-offset.X, -offset.Y, System.Drawing.Drawing2D.MatrixOrder.Prepend);
            }
            foreach (Polygon pl in _Polygons)
            {
                pl.Draw(g);
            }

            _MyPolygon.Draw(g);
            if (_MyPolygon.IsScreenVisible(scrCursor))
            {
                thisHoverLayer = _MyPolygon;
            }
            else
            {
                //取消了多边形移动触发操作
                //if (_LastHoverLayer == pl)
                //    OnMouseLeave.Invoke(pl, null);
            }

            // 倒序查找，保证最晚的仓面优先被选中
            Deck lastSelect = FocusedDeck;
            _FocusedDeck = null;
            //////////////////////////////////////////////////// 
            for (int i = _Dkcontrol.Decks.Count - 1; i >= 0; i--)
            {
                Deck deck = _Dkcontrol.Decks[i];
                Polygon pl = deck.Polygon;
                if (pl.IsScreenVisible(scrCursor) && _FocusedDeck == null)
                {
                    deck.Polygon.LineDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    deck.Polygon.LineWidth = 4;
                    if (!deck.IsVisible)
                        deck.Polygon.FillColor = Color.FromArgb(0x80, Color.White);
                    _FocusedDeck = deck;
                    _CurrentDeck = deck;
                }
                else
                {
                    deck.ResetStyles();
                    //if (deck == lastSelect)
                        //OnMouseLeaveDeck.Invoke(deck, null);
                }
            }

             //顺序画仓面，保证最晚的仓面在最上

            foreach (Deck deck in _Dkcontrol.Decks)
            {
                deck.Draw(g, frameonly, ft);
            }

            // 层激活：thisHoverLayer != null
            // 仓激活：selectedDeck != null
            // 仓优先

            if (_FocusedDeck != null)
            {
                //OnMouseLeave(_LastHoverLayer, null);
                OnMouseEnterDeck(_FocusedDeck, null);
            }
            else
            {
                OnMouseEnter(thisHoverLayer, null);
            }
            _LastHoverLayer = thisHoverLayer;
        }



        /// <summary>
        /// 仓面裁剪
        /// </summary>
        /// <param name="scrCut"></param>
        public void CutBy(Polygon scrCut)
        {
            //bool shownWarning = false;
 
           
                // 目前只允许对当前层面进行分仓处理
                // 其他层面只做视觉参考

                Polygon scrDeck = _MyPolygon.CutBy(scrCut);

              
                if (scrDeck != null)
                {
                    List<Coord> lc = scrDeck.Vertex;
                    ScreenToDam(ref lc);
                    scrDeck.Vertex = lc;
                    Deck dk = new Deck();
                    scrDeck.UpdateBoundary();
                    dk.Polygon = scrDeck;
                    dk.Unit = _MyPolygon.Unit;
                    dk.Elevation = _MyPolygon.Elevation;
                    dk.MyLayer= this;
                    _Dkcontrol.AddDeck(dk);
                    //cutby.SetVertex(cut);
                }
                if (_IsDeckInput)
                {
                    Polygon DamDeck = _MyPolygon.CutByOfEarth(scrCut);
                    if (DamDeck != null)
                    {
                        Deck dk = new Deck();
                        DamDeck.UpdateBoundary();
                        dk.Polygon = DamDeck;
                        dk.Unit = _MyPolygon.Unit;
                        dk.Elevation = _MyPolygon.Elevation;
                        dk.MyLayer = this;
                        _Dkcontrol.AddDeck(dk);
                        _IsDeckInput = false;
                    }
                }
            
            CreateScreen(_Canvas);
        }

        /// <算边数>
        /// 算边数入口
        /// </算边数>
        public int RollCount(PointF p_CursorPos)
        {
            return 0;
        }

        /// <summary>
        /// 删除选中仓面
        /// </summary>
        public void DeleteCurrentDeck()
        {
            if (VisibleDeck == null)
                return;
            _Dkcontrol.DeleteDeck(VisibleDeck);
            _FocusedDeck = null;
            _CurrentDeck = null;
        }

        public void ModifyCurrentDeck()
        {
            if (VisibleDeck != null)
                _Dkcontrol.ModifyDeck(VisibleDeck);
        }

        public void AssignVehicle()
        {
            if (VisibleDeck != null)
                VisibleDeck.VehicleControl.AssignVehicle(VisibleDeck);
        }
    }
}
