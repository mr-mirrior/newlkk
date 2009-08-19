using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DamLKK.Geo;
using System.Xml.Serialization;

namespace DamLKK._Model
{
    /// < 多边形>
    /// 多边形
    /// </ 多边形>
    public class Polygon: IDisposable
    {
        public Polygon() { }
        public Polygon(List<Coord> vtx) { SetVertex(vtx); }
        public Polygon(DMRectangle rc)
        {
            Coord p1 = rc.LeftTop;
            Coord p2 = new Coord(rc.Right, rc.Top);
            Coord p3 = new Coord(rc.Right, rc.Bottom);
            Coord p4 = new Coord(rc.Left, rc.Bottom);
            List<Coord> lst = new List<Coord>();
            lst.Add(p1);
            lst.Add(p2);
            lst.Add(p3);
            lst.Add(p4);
            lst.Add(p1);
            SetVertex(lst);
        }

        int _BlockID = -1;
        /// <summary>
        /// 坝段标识
        /// </summary>
        public int BlockID
        {
            get { return _BlockID; }
            set { _BlockID = value; }
        }

        Unit _Unit;

        /// <summary>
        /// 多边形所画单元
        /// </summary>
        public Unit Unit
        {
            get { return _Unit; }
            set { _Unit = value; }
        }

        Elevation _Elevation;
        /// <summary>
        /// 多边形的高程
        /// </summary>
        public Elevation Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; }
        }


        // 施工坐标（真实坐标）
        List<Coord> _Vertex = new List<Coord>();
        /// <summary>
        /// 施工坐标（真实坐标）
        /// </summary>
        public List<Coord> Vertex { get { return _Vertex; } set { _Vertex = value; } }

        DMRectangle _Boundary = new DMRectangle();

        /// <summary>
        /// 边界线
        /// </summary>
        public DMRectangle Boundary
        {
            get { return _Boundary; }
            set { _Boundary = value; }
        }
        /// <设置边界点坐标>
        /// 设置边界点坐标
        /// </设置边界点坐标>
        /// <param name="vtx"></param>
        public void SetVertex(List<Coord> vtx)
        {
            _Vertex = vtx;
            UpdateBoundary();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        /// <更新边界线>
        /// 更新边界线
        /// </更新边界线>
        public void UpdateBoundary()
        {
            List<Coord> copy = new List<Coord>(_Vertex);
            if (copy.Count == 0) return;
            copy.Sort(Coord.XCompare);

            float xmin = copy.First().XF;
            float xmax = copy.Last().XF;

            copy.Sort(Coord.YCompare);
            float ymin = copy.First().YF;
            float ymax = copy.Last().YF;
            _Boundary = new DMRectangle(xmin, ymin, xmax - xmin, ymax - ymin);
        }

        //
        /// <summary>
        ///  实际面积 平方米
        /// </summary>
        public double ActualArea{get{return Geo.PolygonUtils.AreaOfPolygon(_Vertex);}}
        // 
        /// <summary>
        /// 屏幕面积 像素数
        /// </summary>
        public double ScreenArea { get { return Geo.PolygonUtils.AreaOfPolygon(_ScreenVertex); } }
        // 
        /// <summary>
        /// 实际质心
        /// </summary>
        public Coord Centroid { get { return Geo.PolygonUtils.CentroidOfPolygon(_Vertex); } }
        //
        /// <summary>
        ///  屏幕质心
        /// </summary>
        public Coord ScreenCentroid { get { return Geo.PolygonUtils.CentroidOfPolygon(_ScreenVertex); } }


        #region 画图相关
        Pen _PenLine = new Pen(Color.BlueViolet);
        SolidBrush _BrFill = new SolidBrush(Color.Lavender);

        bool _Antialias = true;   //平滑
        bool _InCurve = false;    //内弯曲
        bool _Token = false;       //象征
        bool _NeedClose = true;
        object _SyncLock = new object();//线程同步锁

        Geo.DMMatrix _DMMatrix;

        public Geo.DMMatrix DMMatrix
        {
            get { return _DMMatrix; }
            set { _DMMatrix = value; }
        }
        /// <summary>
        /// 边界线的画笔颜色
        /// </summary>
        public Color LineColor { get { return _PenLine.Color; } set { lock (_SyncLock) _PenLine.Color = value; } }
        /// <summary>
        /// 线宽度
        /// </summary>
        public float LineWidth { set { lock (_SyncLock) _PenLine.Width = value; } }
        /// <summary>
        /// 虚线样式
        /// </summary>
        public DashStyle LineDashStyle { set { lock (_SyncLock) _PenLine.DashStyle = value; } }
        /// <summary>
        /// 自定义划断线和白线的数组
        /// </summary>
        public float[] LineDashPattern { set { lock (_SyncLock) _PenLine.DashPattern = value; } }

        /// <summary>
        /// 图形的填充色
        /// </summary>
        public Color FillColor { get { return _BrFill.Color; } set { lock (_SyncLock) _BrFill.Color = value; } }
        /// <summary>
        /// 是否反锯齿
        /// </summary>
        public bool AntiAlias { get { return _Antialias; } set { _Antialias = value; } }
        /// <summary>
        /// 是否内斜角
        /// </summary>
        public bool InCurve { get { return _InCurve; } set { _InCurve = value; } }

        [XmlIgnore]
        public bool NeedClose { get { return _NeedClose; } set { _NeedClose = value; } }

        [XmlIgnore]
        public bool Token { get { return _Token; } set { _Token = value; } }

        
        GraphicsPath _GraphicsPath = new GraphicsPath();
        List<Coord> _ScreenVertex = new List<Coord>();

        /// <屏幕坐标>
        /// 屏幕坐标
        /// </屏幕坐标>
        public List<Coord> ScreenVertex
        {
            get { return _ScreenVertex; }
            set { _ScreenVertex = value; }
        }
        
        DMRectangle _ScreenBoundary;    //屏幕矩形
        /// <获取屏幕矩形>
        /// 获取屏幕矩形
        /// </获取屏幕矩形>
        [XmlIgnore]
        public DMRectangle ScreenBoundary { get { return _ScreenBoundary; } }

       /// <summary>
        /// 创建在大坝模型上的图形(屏幕上)
       /// </summary>
       /// <param name="matrix"></param>
        public void CreateScreen(Geo.DMMatrix matrix)
        {
            _DMMatrix = matrix;
            CopyCoords();
            Relative();
            Scale();
            Rotate();
            CalcVisible();
            CreatePath();
        }
        /// <summary>
        /// 填充坐标
        /// </summary>
        private void CopyCoords()
        {
            _ScreenVertex = new List<Coord>(_Vertex);
        }

        private void Rotate()
        {
            _ScreenVertex = Geo.DamUtils.RotateDegree(_ScreenVertex, new Coord(0,0), _DMMatrix.Degrees);
        }

        /// <summary>
        /// 将坐标转换为屏幕坐标,将大坝模型的偏移坐标赋值
        /// </summary>
        private void Relative()
        {
            for (int i = 0; i < _ScreenVertex.Count; i++ )
            {
                _ScreenVertex[i] = _ScreenVertex[i].Origin(_DMMatrix.At);
            }
            _DMMatrix.Offset = _DMMatrix.At;
        }

        /// <summary>
        /// 按照大坝比较缩放坐标并讲屏幕坐标列表更新
        /// </summary>
        private void Scale()
        {
            for (int i = 0; i < _ScreenVertex.Count; i++)
            {
                _ScreenVertex[i] = _ScreenVertex[i].Scale(_DMMatrix.Zoom);
            }
        }

        /// <summary>
        /// 将图形按照指定坐标偏移
        /// </summary>
        public void OffsetGraphics(Coord c)
        {
            _DMMatrix.Offset = c;

            for (int i = 0; i < _ScreenVertex.Count; i++ )
            {
                _ScreenVertex[i] = _ScreenVertex[i].Offset(c);
            }
            CalcVisible();
            CreatePath();
        }

        /// <summary>
        ///将屏幕坐标点列表转换为一个屏幕矩形（最大都包容）
        /// </summary>
        private void CalcVisible()
        {
            _ScreenBoundary = Geo.DamUtils.MinBoundary(_ScreenVertex);
        }

        /// <summary>
        /// 将屏幕图形添加到_GraphicsPath中。
        /// </summary>
        private void CreatePath()
        {
            lock(ooxx)
            {
                _GraphicsPath.Reset();
                if (_ScreenVertex.Count < 3 && NeedClose)
                    return;

                List<PointF> lst = Geo.DamUtils.TranslatePoints(_ScreenVertex);

                if (_InCurve)
                    _GraphicsPath.AddCurve(lst.ToArray());
                else
                {
                    if (_NeedClose)
                        _GraphicsPath.AddPolygon(lst.ToArray());
                    else
                        _GraphicsPath.AddLines(lst.ToArray());
                }
            }
           
        }

        /// <画控制点(橙色)>
        /// 画控制点(橙色)
        /// </画控制点(橙色)>
        private void DrawTokenPoint(Graphics g, PointF pt)
        {
            g.FillEllipse(Brushes.OrangeRed, pt.X-2, pt.Y-2, 4, 4);
        }

        /// <画控制点(橙色)>
        /// 画控制点(橙色)
        /// </画控制点(橙色)>
        private void DrawToken(Graphics g)
        {
            foreach (Coord c in _ScreenVertex)
            {
                DrawTokenPoint(g, c.PF);
            }
        }
        // 直接添加屏幕坐标，无转换
        /// <直接添加屏幕坐标，无转换>
        /// 直接添加屏幕坐标，无转换
        /// </直接添加屏幕坐标，无转换>
        public void SetScreenVertex(List<Coord> lc)
        {
            _ScreenVertex = lc;
            CalcVisible();
            CreatePath();
        }
        /// < 直接偏移屏幕坐标，无转换>
        /// 直接偏移屏幕坐标，无转换
        /// </ 直接偏移屏幕坐标，无转换>
        /// <param name="c"></param>
        public void OffsetScreen(Coord c)
        {
            for (int i = 0; i < _ScreenVertex.Count; i++ )
            {
                _ScreenVertex[i] = _ScreenVertex[i].Offset(c);
            }
            CreatePath();
        }
        /// <summary>
        /// 返回 g的.Clip中Region 并将g的Clip 设置为 盖多边形的Clip
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public Region SetDrawClip(Graphics g)
        {
            lock(ooxx)
            {
                Region r = new Region(_GraphicsPath);
                Region old = g.Clip;
                g.Clip = r;
                return old;
            }
            
        }

        /// <summary>
        /// 将图形信息都绘制到g中(图形，边线，控制点)
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            lock(ooxx)
            {
                // 填充多边形
                g.FillPath(_BrFill, _GraphicsPath);//_BrFill
                g.DrawPath(_PenLine, _GraphicsPath);
            }
            

            if(_BlockID!=-1)
                g.DrawString(_BlockID.ToString(), new Font("微软雅黑", 12), Brushes.Black, this.ScreenCentroid.PF);
            if (Token)
                DrawToken(g);
        }

        /// < 大地坐标相切，用this切scrcut并用相切的数据覆盖创建一个新的polygon返回>
        /// 大地坐标相切，用this切scrcut并用相切的数据覆盖创建一个新的polygon返回
        /// </ 大地坐标相切，用this切scrcut并用相切的数据覆盖创建一个新的polygon返回>
        public Polygon CutByOfEarth(Polygon scrCut)
        {
            List<Coord> copy = new List<Coord>(this.Vertex);
            BorderShapeII shape = new BorderShapeII(copy);
            BorderShapeII cutShape = new BorderShapeII(scrCut.Vertex);
            shape.Intersect(cutShape);
            Polygon result = new Polygon();
            result.Vertex = shape.Data;

            return result;
        }
        /// <大地坐标相切，用scrcut多边形切this并用相切的数据覆盖创建一个新的polygon返回>
        /// 大地坐标相切，用scrcut多边形切this并用相切的数据覆盖创建一个新的polygon返回
        /// </大地坐标相切，用scrcut多边形切this并用相切的数据覆盖创建一个新的polygon返回>
        public Polygon CutBy(Polygon scrCut)
        {
            List<Coord> copy = new List<Coord>(_ScreenVertex);
            BorderShapeII shape = new BorderShapeII(copy);
            BorderShapeII cutShape = new BorderShapeII(scrCut.Vertex);
            shape.Intersect(cutShape);
            if (shape.IsEmpty)
                return null;
            Polygon result = new Polygon();
            result.Vertex = shape.Data;

            return result;
        }
        private object ooxx = new object();

        /// <summary>
        /// 返回点是否在屏幕图形内
        /// </summary>
        public bool IsScreenVisible(Coord pt)
        {
            lock(ooxx)
            {
                return _GraphicsPath.IsVisible(pt.PF);
            }
            
        }
        #endregion
    }
}
