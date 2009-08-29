using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace DamLKK._Model
{
    public enum DrawingComponent
    {
        NONE = 0,
        SKELETON = 1,
        BAND = 2,
        OVERSPEED = 4,
        VEHICLE = 8,
        ARROWS = 16,
        ALL = SKELETON/*|BAND*/| OVERSPEED | VEHICLE/*|ARROWS*/
    }

    //仓面工作状态

    public enum DeckWorkState
    {
        WAIT = 0,//等待状态

        WORK = 1,//工作状态

        END = 2//结束状态

    }

    public class Deck:IDisposable
    {

        public Deck()
        {
            _Unit = new Unit();
            _Elevation = new Elevation();
        }

        public Deck(Deck p_Deck)
        {
            _Unit = p_Deck.Unit;
            _Elevation = p_Deck._Elevation;
            _DesignDepth = p_Deck.DesignDepth;
            _NOLibRollCount = p_Deck.NOLibRollCount;
            _LibRollCount = p_Deck.LibRollCount;
            _ErrorParam = p_Deck.ErrorParam;
            _MaxSpeed = p_Deck.MaxSpeed;
            _Name = p_Deck.Name;
            _SpreadZ = p_Deck.SpreadZ;
            _StartZ = p_Deck.StartZ;
            _WorkState = p_Deck._WorkState;
            Init();
        }

        public void Dispose()
        {
            this.VehicleControl.Dispose();
            GC.SuppressFinalize(this);
        }

        private void Init()
        {
            vCtrl.Owner = this;
        }


        DrawingComponent drawingComponent = DrawingComponent.ALL;
        public DrawingComponent DrawingComponent { get { return drawingComponent; } set { drawingComponent = value; } }

        #region -属性-

        Unit _Unit=new Unit();
        /// <summary>
        /// 所属单元
        /// </summary>
        public Unit Unit
        {
            get { return _Unit; }
            set { _Unit = value; }
        }

        Elevation _Elevation=new Elevation();
        /// <summary>
        /// 所在高程
        /// </summary>
        public Elevation Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; }
        }

        int _ID;
        /// <summary>
        /// 仓面id
        /// </summary>
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        DeckWorkState _WorkState;
        /// <summary>
        /// 工作状态
        /// </summary>
        public DeckWorkState WorkState
        {
            get { return _WorkState; }
            set { _WorkState = value; }
        }

        string _Vertex;
        /// <summary>
        /// 边界点
        /// </summary>
        public string Vertex
        {
            get { return _Vertex; }
            set { _Vertex = value; }
        }

        DateTime _DTStart;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime DTStart
        {
            get { return _DTStart; }
            set { _DTStart = value; }
        }

        DateTime _DTEnd;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime DTEnd
        {
            get { return _DTEnd; }
            set { _DTEnd = value; }
        }

        int _NOLibRollCount;
        /// <summary>
        /// 设计静碾边数
        /// </summary>
        public int NOLibRollCount
        {
            get { return _NOLibRollCount; }
            set { _NOLibRollCount = value; }
        }

        int _LibRollCount;
        /// <summary>
        /// 设计静碾边数
        /// </summary>
        public int LibRollCount
        {
            get { return _LibRollCount; }
            set { _LibRollCount = value; }
        }


        double _ErrorParam;
        /// <summary>
        /// 容错率
        /// </summary>
        public double ErrorParam
        {
            get { return _ErrorParam; }
            set { _ErrorParam = value; }
        }

        double _DesignDepth;
        /// <summary>
        /// 设计铺料厚度
        /// </summary>
        public double DesignDepth
        {
            get { return _DesignDepth; }
            set { _DesignDepth = value; }
        }

        double _StartZ;
        /// <summary>
        /// 铺前高程
        /// </summary>
        public double StartZ
        {
            get { return _StartZ; }
            set { _StartZ = value; }
        }

        string _Name;
        /// <summary>
        /// 仓面名称
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        double _MaxSpeed;

        /// <summary>
        /// 最大限速
        /// </summary>
        public double MaxSpeed
        {
            get { return _MaxSpeed; }
            set { _MaxSpeed = value; }
        }

        Layer _MyLayer;
        /// <summary>
        /// 从属层
        /// </summary>
        public Layer MyLayer
        {
            get { return _MyLayer; }
            set { _MyLayer = value; }
        }

        List<RollerDis> _RollerDis;
        /// <summary>
        /// 车辆分配列表
        /// </summary>
        public List<RollerDis> RollerDis
        {
            get { return _RollerDis; }
            set { _RollerDis = value; }
        }

        double _POP;
        /// <summary>
        /// 标准边数百分比
        /// </summary>
        public double POP
        {
            get { return _POP; }
            set { _POP = value; }
        }

        Polygon _Polygon;
        /// <summary>
        /// 仓面形状
        /// </summary>
        public Polygon Polygon
        {
            get { return _Polygon; }
            set
            {
                _Polygon = value;
                ResetStyles();
                _Vertex = VertexString();
            }
        }

        double _SpreadZ;

        /// <summary>
        /// 摊铺高程
        /// </summary>
        public double SpreadZ
        {
            get { return _SpreadZ; }
            set { _SpreadZ = value; }
        }

        bool _IsVisible;
        /// <summary>
        /// 是否显示状态
        /// </summary>
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { _IsVisible = value; ResetStyles(); }
        }
        #endregion


        public bool _IsDatamap = false; //是否是数据图

        // 安排了多个车辆

        _Control.VehicleControl vCtrl = new _Control.VehicleControl();
        public _Control.VehicleControl VehicleControl { get { return vCtrl; } set { vCtrl = value; } }

        Font _Font;
        // 仓面顶点坐标格式化字符串，准备入库

        public string VertexString()
        {
            string s = "";
            foreach (Geo.Coord c in Polygon.Vertex)
            {
                s += string.Format("{0:0.00},{1:0.00};", c.X, c.Y);
            }
            if (s.Length < 1)
                return string.Empty;
            s = s.Substring(0, s.Length - 1);
            return s;
        }


        Color _FollColor = Color.Silver;
        Color _LineColor = Color.Black;

        /// <summary>
        /// 重置样式
        /// </summary>
        public void ResetStyles()
        {
            _Polygon.AntiAlias = true;
            _Polygon.FillColor = Color.FromArgb(0x80, _FollColor);
            _Polygon.LineColor = _LineColor;
            _Polygon.LineColor = Color.FromArgb(0xFF, Color.Black);
            _Polygon.LineWidth = 2f;
            _Polygon.LineDashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            _Polygon.LineDashPattern = new float[] { 6, 4 };

            if (this.WorkState== DeckWorkState.END)
            {
                _Polygon.LineColor = Color.DimGray;
                _Polygon.LineDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            }
            if (this.WorkState == DeckWorkState.WORK)
            {
                _Polygon.LineColor = Color.BlueViolet;
                _Polygon.LineDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
            }
            if (this.IsVisible)
                _Polygon.FillColor = Color.FromArgb(0xa0, Color.White);
        }

        /// <summary>
        /// 是否是同一个仓面
        /// </summary>
        public bool IsEqual(Deck dk)
        {
            return this.Unit.Equals(dk.Unit) && this.Elevation.Height == dk.Elevation.Height && this.ID == dk.ID;
        }

        /// <summary>
        /// 画自己
        /// </summary>
        public void Draw(Graphics g, bool frameonly, Font ft)
        {
            this._Font = ft;
            this.Polygon.Draw(g);
         
            if (!IsVisible)
                return;
            if (frameonly && IsDrawing(DrawingComponent.SKELETON))
            {
                foreach (Roller v in vCtrl.Rollers)
                {
                    v.Draw(g, frameonly);
                }
                return;
            }
            foreach (Roller v in vCtrl.Rollers)
            {
                if (IsDrawing(DrawingComponent.OVERSPEED))
                    v.TrackGPSControl.Tracking.DrawOverSpeed = true;
                else
                    v.TrackGPSControl.Tracking.DrawOverSpeed = false;

                if (IsDrawing(DrawingComponent.SKELETON))
                    v.TrackGPSControl.Tracking.DrawSkeleton(g, IsDrawing(DrawingComponent.ARROWS));
                if (IsDrawing(DrawingComponent.BAND))
                    v.Draw(g, false);
            }
            if (IsDrawing(DrawingComponent.VEHICLE))
                foreach (Roller v in vCtrl.Rollers)
                {
                    v.TrackGPSControl.Tracking.DrawAnchor(g);
                }
        }

        public bool IsDrawing(DrawingComponent dc) { return 0 != (drawingComponent & dc); }

        /// <summary>
        /// 点是否在方块内
        /// </summary>
        public bool RectContains(Geo.Coord pt)
        {
            return Polygon.Boundary.Contains(pt.PF);
        }

        public void ShowDrawingComponent(DrawingComponent dc, bool show)
        {
            if (show)
                ShowDrawingComponent(dc);
            else
                HideDrawComponent(dc);
        }

        public void ShowDrawingComponent(DrawingComponent dc)
        {
            drawingComponent |= dc;
        }
        public void HideDrawComponent(DrawingComponent dc)
        {
            drawingComponent &= ~dc;
        }

        /// <summary>
        /// 边数颜色0-15
        /// </summary>
        Color[] _LayersColor = new Color[] {
                            Color.LightYellow,
                            Color.Thistle,
                            Color.Pink,
                            Color.Gray,
                            Color.Orange,
                            Color.CornflowerBlue,
                            Color.Cyan,
                            Color.Chocolate,
                            Color.Green,
                            Color.Red,
                            Color.Purple,
                            Color.Blue,
                            Color.SlateGray,
                            Color.Indigo,
                            Color.Aqua 
                        };


        Font _FtScale;
        Font _FtString;
        Font ft;
        Geo.Coord _DamOrignCoord;
        string _OrignCoordString;
        public string _Rolladdress;
        public string _Trackingaddress;

        #region ------------------------实时轨迹图--------------------------        
        public bool _IsOutTrackingMap = false;

        public void CreateTrackMap()
        {
            //_IsOutTrackingMap = true;
            DrawPathMap();
            //_IsOutTrackingMap = false;
        }
#endregion

#region  --------------------------------------碾压边数原图-------------------------------------
        /// <summary>
        /// 绘制碾压编数效果图，放大率等数据取屏幕当前设置值   0:所有轨迹,1:静碾轨迹;2:振碾轨迹
        /// </summary>
        /// <param name="areas">返回不同碾压编数的总和点（面积）</param>
        /// <returns>返回图形，用完后必须bmp.Dispose();</returns>
        public Bitmap CreateRollCountImage(out int[] areas,int mapindex)
        {
            Layer layer = this.MyLayer;
            Polygon pl = this.Polygon;
            Bitmap learning = new Bitmap(_LayersColor.Length * 2, 1, PixelFormat.Format32bppPArgb);
            Graphics gLearning = Graphics.FromImage(learning);
            gLearning.Clear(_LayersColor[0]);
            for (int i = 0; i < learning.Width; i++)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddLine(learning.Width, 0, i + 1, 0);
                    using (Pen p = new Pen(Color.FromArgb(0x20, Color.Navy)))
                        gLearning.DrawPath(p, gp);
                }
            }
            int[] colorDict = new int[learning.Width];
            for (int i = 0; i < learning.Width; i++)
            {
                colorDict[i] = learning.GetPixel(i, 0).ToArgb();
            }

            Bitmap bmp = new Bitmap((int)Math.Ceiling(pl.ScreenBoundary.Width), (int)Math.Ceiling(pl.ScreenBoundary.Height), PixelFormat.Format32bppPArgb);
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.White);

            g.TranslateTransform((float)-pl.ScreenBoundary.Left, (float)-pl.ScreenBoundary.Top);
            pl.AntiAlias = false;
            pl.LineColor = Color.Transparent;
            pl.FillColor = _LayersColor[0];
            pl.Draw(g);
            pl.SetDrawClip(g);

            for (int k = 0; k < VehicleControl.Rollers.Count; k++)
            {
                Roller v = VehicleControl.Rollers[k];
                if (_Control.VehicleControl.FindVechicle(v.Assignment.RollerID).Name == null)
                    continue;
                TrackGPS gps = v.TrackGPSControl.Tracking;
                Color oldcl = gps.Color;
                gps.Color = Color.Navy;
                gps.Draw(g, false,mapindex);
                gps.Color = oldcl;
            }

         
            gLearning.Dispose();
            learning.Dispose();

            Bitmap output = (Bitmap)bmp.Clone();
            bmp.Dispose();

            BitmapData bd = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.ReadWrite, output.PixelFormat);
            areas = new int[_LayersColor.Length];

            unsafe
            {
                int* pp = (int*)bd.Scan0.ToPointer();
                pp = (int*)bd.Scan0.ToPointer();
                for (int i = 0; i < bd.Height; i++)
                {
                    for (int j = 0; j < bd.Width; j++)
                    {
                        Int32 cl = *(pp + j);
                        int cl_idx = -1; //color_dict.IndexOf(cl);
                        for (int k = 0; k < colorDict.Length; k++) { if (colorDict[k] == cl) { cl_idx = k; break; } }
                        if (cl_idx == 0)
                        {
                            *(pp + j) = 0;
                            areas[cl_idx]++;
                            continue;
                        }
                        else if (cl_idx == -1) // 碾压超过限度的
                        {
                            if (cl == -1) // 该点不在仓面内
                            {
                                *(pp + j) = 0;
                                continue;
                            }
                            else
                                cl_idx = colorDict.Length - 1;
                        }

                        cl_idx = Math.Min(cl_idx, _LayersColor.Length - 1);
                        int last_idx;
                        if(mapindex==0)
                        last_idx = Math.Min(cl_idx, this.LibRollCount+this.NOLibRollCount);
                        else if(mapindex==1)
                            last_idx = Math.Min(cl_idx,this.NOLibRollCount);
                        else
                            last_idx = Math.Min(cl_idx, this.LibRollCount);

                        areas[cl_idx]++;
                        if (_IsDatamap)
                            *(pp + j) = _LayersColor[cl_idx].ToArgb();
                        else
                            *(pp + j) = _LayersColor[last_idx].ToArgb();
                   
                    }
                    pp += bd.Stride / 4;
                }
            }
            //填充数据库总面积和碾压遍数百分比字段
            double[] areasPercents = Deck.AreaRatio(areas, this);
            string percentages, one;
            percentages = string.Empty;
            for (int i = 0; i < 15; i++)
            {
                if (i < areasPercents.Length)
                    one = areasPercents[i].ToString("0.0000");
                else
                    one = ((double)0).ToString("0.0000");
                if (i != 14)
                    percentages = percentages + one + ",";
                else
                    percentages += one;
            }

            DamLKK.DB.DeckDAO.GetInstance().UpdateDeckAreaAndRollingPercentages(Unit.ID, Elevation.Height, ID, Polygon.ActualArea, percentages);

            //// 超过部分统一
            if (!_IsDatamap)
            {
                if(mapindex==0)
                {
                    for (int idx = this.NOLibRollCount + this.LibRollCount + 1; idx < areas.Length; idx++)
                    {
                        areas[this.NOLibRollCount + this.LibRollCount] += areas[idx];
                        areas[idx] = 0;
                    }
                }
                else if (mapindex == 1)
                {
                    for (int idx = this.NOLibRollCount+1; idx < areas.Length; idx++)
                    {
                        areas[this.NOLibRollCount] += areas[idx];
                        areas[idx] = 0;
                    }
                }
                else
                {
                    for (int idx =  this.LibRollCount + 1; idx < areas.Length; idx++)
                    {
                        areas[this.LibRollCount] += areas[idx];
                        areas[idx] = 0;
                    }
                }
                
            }
            output.UnlockBits(bd);
            g.Dispose();

            g = Graphics.FromImage(output);
            g.TranslateTransform((float)-pl.ScreenBoundary.Left, (float)-pl.ScreenBoundary.Top);
            pl.SetDrawClip(g);
            Font fft;
            try
            {
                fft = new Font(new FontFamily("微软雅黑"), 3 * (float)_MyLayer.ScreenSize(0.356), GraphicsUnit.Pixel);
            }
            catch
            {
                fft = new Font(new FontFamily("宋体"), 3 * (float)_MyLayer.ScreenSize(0.356), GraphicsUnit.Pixel);
            }
            return output;

        }

        public float GetMultipleFactor(Layer layer, double multiple)
        {
            return (float)layer.ScreenSize(0.1 * multiple);
        }
#endregion


#region -----------------------------------------碾压边数图------------------------------------
        public bool CreateRollCountReport(double zoom, bool datamap)
        {
            Layer layer = _MyLayer;
            double oldZoom = layer.Zoom;
            double oldRotate = layer.RotateDegree;

#if !DEBUG
            if (WorkState== DeckWorkState.WAIT)//this.IsWorking||或结束结束碾压监控后
            {
                Utils.MB.Warning("该碾压层没有开启监控，无法生成图形报告。请再试一次。");
                return false;
            }
#endif

            //try
            {
                Brush[] bs = new SolidBrush[_LayersColor.Length];
                for (int i = 0; i < bs.Length; i++)
                {
                    bs[i] = new SolidBrush(_LayersColor[i]);
                }
                const double SHANGYOU = -175.790047519344;
                if (!datamap)
                {
                    layer.RotateDegree = SHANGYOU - 180;
                }
                else
                {
                    layer.Zoom = 2;
                    layer.RotateDegree = 0;
                }
                layer.Zoom = 5;
                layer.CreateScreen();
                Polygon pl = this.Polygon;
                if (pl.ScreenBoundary.Width > 5000 || pl.ScreenBoundary.Height > 5000)
                {
                    layer.RotateDegree = oldRotate;
                    layer.Zoom = oldZoom;
                    layer.CreateScreen();
                    Utils.MB.Warning("放大率过大，请缩小图形后再试一次");
                    return false;
                }
                DirectoryInfo di = new DirectoryInfo(@"C:\OUTPUT\" + this._Name);
                if (!di.Exists)
                {
                    di.Create();
                }

                if (pl.ScreenBoundary.Width == 0 || pl.ScreenBoundary.Height == 0)
                    return false;

                int[] areas;
                _Rolladdress = @"C:\OUTPUT\" + this._MyLayer.CurrentDeck.Name + @"\"  +this.Elevation.Height.ToString("0.0") + this.ID.ToString();
                Bitmap output = CreateRollCountImage(out areas,0);

                DB.DeckDAO.GetInstance().UpdateRollBitMap(this.Unit.ID, this.Elevation.Height, this._ID, DB.DeckDAO.GetInstance().ToByte(output));
#if DEBUG
                output.Save(@"C:\OUTPUT\" + this.Unit.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "OrignRoll.png", System.Drawing.Imaging.ImageFormat.Png);
#endif
                layer.Zoom = oldZoom;

                layer.CreateScreen();

                for (int i = 0; i < 3;i++ )
                {
                    output = CreateRollCountImage(out areas, i);
                    MachRollMap(output, areas, i,bs);
                }
                
                
            }



            this.DrawPathMap();
            if (this.WorkState == DeckWorkState.END)
            {
               
                Bitmap bmp = CreateElevationImage();
                if (bmp != null)
                    bmp.Dispose();
            }

            layer.RotateDegree = oldRotate;
            layer.Zoom = oldZoom;
            layer.CreateScreen();
            return true;
        }


        /// <summary>
        /// 加工边数图
        /// </summary>
        private void MachRollMap(Bitmap output,int[] areas,int mapindex,Brush[] bs)
        {
            
            _AreaScale = new double[areas.Length];

            float factor;
            factor = GetMultipleFactor(_MyLayer, 3.56);


            //求原点坐标
            Geo.Coord screenOriginCoord = this.Polygon.ScreenBoundary.LeftBottom;
            Geo.Coord earthOriginCoord = _MyLayer.ScreenToEarth(screenOriginCoord.PF);
            _DamOrignCoord = earthOriginCoord.ToDamAxisCoord();
            _OrignCoordString = "(" + _DamOrignCoord.X.ToString("0.00") + ", " + _DamOrignCoord.Y.ToString("0.00") + ")";

            float offsetx;
            float offsety;
            float offset;

            offsetx = output.Width * 1.2f;
            offsety = output.Width / 6 * 0.5f * 0.5f * 7f + output.Height; //output.Height + 120 * (int)Math.Ceiling(factor);
            offset = (offsetx - output.Width) / 2;


            Bitmap newBmp = new Bitmap((int)offsetx, (int)offsety);//output.Height + 120 * (int)Math.Ceiling(factor));//60,80                
            Graphics newG = Graphics.FromImage(newBmp);

            float newH = output.Width / 6 * 0.5f * 0.5f;

            newG.Clear(Color.White);

            newG.SmoothingMode = SmoothingMode.AntiAlias;
            newG.InterpolationMode = InterpolationMode.HighQualityBicubic;
            newG.TranslateTransform((float)-this.Polygon.ScreenBoundary.Left + (offsetx - output.Width) / 2, (float)-this.Polygon.ScreenBoundary.Top + newH);
            this.ResetStyles();
            this.Polygon.Draw(newG);
            newG.ResetTransform();
            newG.DrawImageUnscaled(output, (int)offset, (int)newH);

            Pen newPen = new Pen(Brushes.Black, 1);
            newPen.CustomEndCap = new AdjustableArrowCap(2, 8, true);
            ft = new Font("微软雅黑", 7.5f * factor, GraphicsUnit.Pixel);
            _FtScale = new Font("微软雅黑", 5.5f * factor);

            _FtString = new Font("微软雅黑", 7.5f * factor, FontStyle.Bold, GraphicsUnit.Pixel);


            float multiple = output.Width / 6;
            float w0 = multiple * 0.5f;
            float fa = 10f;

            SizeF s = newG.MeasureString("100.00%", _FtScale);

            while (s.Width > (multiple - w0) * 0.9f)
            {
                if (fa * factor < 0)
                    return;

                _FtScale = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("100.99%", _FtScale);
                fa = fa - 0.1f;
            }
            s = newG.MeasureString("未碾压", ft);
            fa = 10f;
            while (s.Width > (multiple - w0) * 0.9f)
            {
                ft = new Font("微软雅黑", fa * factor);
                _FtString = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("未碾压", _FtString);
                fa = fa - 0.1f;
            }

            //横轴
            newG.DrawLine(newPen, new PointF(offset - 4, (float)this.Polygon.ScreenBoundary.Height + newH), new PointF((float)this.Polygon.ScreenBoundary.Width + offset, (float)this.Polygon.ScreenBoundary.Height + newH));
            newG.DrawString("坝(m)", _FtScale, Brushes.Black, (float)this.Polygon.ScreenBoundary.Width + offset * 1f, (float)this.Polygon.ScreenBoundary.Height + newH);

            //纵轴
            newG.DrawLine(newPen, new PointF(offset, (float)this.Polygon.ScreenBoundary.Height + newH + 4), new PointF(offset, 2 * factor));
            //newG.DrawString("轴(m)", ftScale, Brushes.Black, offset * 0.9f, 0 * factor);

            //原点坐标
            newG.DrawString(_OrignCoordString, _FtScale, Brushes.Black, offset * 0.8f, (float)this.Polygon.ScreenBoundary.Height + newH + 2);
            newPen.Dispose();
            int okcount;
            if (mapindex==0)
                okcount = this.NOLibRollCount + this.LibRollCount;
            else if (mapindex == 1)
                okcount = this.NOLibRollCount;
            else
                okcount = this.LibRollCount;

            double[] area_ratio = AreaRatio(areas, this);

            for (int i = 0; i < 6; i++)
            {
                if (i > okcount)
                    continue;
                newG.FillRectangle(bs[i], offset + i * multiple + w0 - w0 / 2.2f, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2, w0 / 2.2f, w0 / 2.2f);
                newG.DrawRectangle(Pens.Black, offset + i * multiple + w0 - w0 / 2.2f, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2, w0 / 2.2f, w0 / 2.2f);
                if (i == 0)
                {
                    newG.DrawString("未碾压", _FtString, /*bs[i]*/Brushes.Black, offset * 1.05f + w0, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2);
                    newG.DrawString(area_ratio[i].ToString("0.00%"), _FtScale, /*bs[i]*/Brushes.Black, offset * 1.05f + w0, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2 + w0 / 3.5f);
                    continue;
                }

                newG.DrawString(i.ToString() + "遍" + ((i == okcount) ? "及以上" : ""), _FtString, /*bs[i]*/Brushes.Black, offset * 1.05f + ((i + 1) * multiple - w0), output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2);
                newG.DrawString(area_ratio[i].ToString("0.00%"), _FtScale, /*bs[i]*/Brushes.Black, offset * 1.05f + ((i + 1) * multiple - w0), output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2 + w0 / 3.5f);
            }

            for (int i = 0; i < 6; i++)
            {
                if (i + 6 > okcount)
                    continue;
                newG.FillRectangle(bs[6 + i], offset + i * multiple + w0 - w0 / 2.2f, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 / 2.2f, w0 / 2.2f);
                newG.DrawRectangle(Pens.Black, offset + i * multiple + w0 - w0 / 2.2f, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 / 2.2f, w0 / 2.2f);

                newG.DrawString((6 + i).ToString() + "遍" + (((i + 6) == okcount) ? "及以上" : ""), _FtString, /*bs[6 + i]*/Brushes.Black, offset * 1.05f + ((i + 1) * multiple - w0), output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 3.5f);
                newG.DrawString(area_ratio[6 + i].ToString("0.00%"), _FtScale, /*bs[6 + i]*/Brushes.Black, offset * 1.05f + ((i + 1) * multiple - w0), output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 3.5f + w0 / 3.5f);
            }

            //备注
            RectangleF remarkPf = new RectangleF(offset - 4, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f + w0 / 3.5f + s.Height, newBmp.Width - 2 * (offset - 4), newBmp.Height - (output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f + w0 / 3.5f) - s.Height);
            StringFormat remarkSf = new StringFormat();
            remarkSf.LineAlignment = StringAlignment.Near;
            remarkSf.Alignment = StringAlignment.Near;

            string remark = DB.DeckDAO.GetInstance().ReadSegmentRemark(this.Unit.ID, this.Elevation.Height, this.ID);

            if (remark != String.Empty)
            {
                remark = "(" + remark + ")";
                s = newG.MeasureString(remark, _FtString);
                float height = s.Height;
                fa = 10f;
                while (s.Width > (newBmp.Width - 2 * (offset - 4)) || s.Height >= height)
                {
                    _FtString = new Font("微软雅黑", fa * factor);
                    s = newG.MeasureString(remark, _FtString);
                    fa = fa - 0.1f;
                }

            }
            newG.DrawString(remark, _FtString, Brushes.Black, remarkPf, remarkSf);

            //刻度
            float meterPrePoint = GetMultipleFactor(_MyLayer, 50);
            PointF pf;
            PointF pf5;
            PointF pfWord;
            Pen p2p = new Pen(Brushes.Black, 2);
            RectangleF rf;
            PointF pf1;
            SizeF sz;
            StringFormat sf = new StringFormat(); ;
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;


            newG.SmoothingMode = SmoothingMode.None;

            float width = (float)Math.Min(this.Polygon.ScreenBoundary.Width, this.Polygon.ScreenBoundary.Height) / 5;
            if(this.Polygon.ScreenBoundary.Width<this.Polygon.ScreenBoundary.Height)
            {
                double max = Math.Abs(this.Polygon.Vertex[0].ToDamAxisCoord().X - _DamOrignCoord.X);
                foreach (DamLKK.Geo.Coord c in this.Polygon.Vertex)
                {
                    if (Math.Abs(c.ToDamAxisCoord().X - _DamOrignCoord.X) > max)
                        max = Math.Abs(c.ToDamAxisCoord().X - _DamOrignCoord.X);
                }
                meterPrePoint = (float)max / 5;
            }
            else
            {
                double max = Math.Abs(this.Polygon.Vertex[0].ToDamAxisCoord().Y- _DamOrignCoord.Y);
                foreach (DamLKK.Geo.Coord c in this.Polygon.Vertex)
                {
                    if (Math.Abs(c.ToDamAxisCoord().Y - _DamOrignCoord.Y) > max)
                        max = Math.Abs(c.ToDamAxisCoord().X - _DamOrignCoord.Y);
                }
                meterPrePoint = (float)max / 5;
            }
          

            //横轴刻度
            for (float i = width,j=1; i < this.Polygon.ScreenBoundary.Width; i+=width,j++)//this.Polygon.ScreenBoundary.Width-40//(float)(this.Polygon.ScreenBoundary.Width - 10) / meterPrePoint
            {
                pf = new PointF(offset + j *width/* meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH - 6);
                pf5 = new PointF(offset + j * width/*meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH);
                //if (i % 5 == 0)
                //{
                pf = new PointF(offset + j * width/*meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH - 10);
                pf5 = new PointF(offset + j * width/*meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH);
                    pfWord = new PointF(offset - 8 + j * width /*meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH + 5);
                    newG.DrawLine(Pens.Black, pf, pf5);

                    pf1 = new PointF(offset + (j - 1) *width/* meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH);
                    sz = new SizeF(2 * width/*meterPrePoint*/, offset * 0.4f);
                    rf = new RectangleF(pf1, sz);
                    newG.DrawString((_DamOrignCoord.X + j * meterPrePoint/*5*/).ToString("0"), _FtScale, Brushes.Black, rf, sf);
                    //continue;
                //}
                newG.DrawLine(Pens.Gray, pf, pf5);
            }
            //纵轴刻度

            for (float i = width,j=1; i < this.Polygon.ScreenBoundary.Height; i+=width,j++)//this.Polygon.ScreenBoundary.Width-40//(float)(this.Polygon.ScreenBoundary.Height - 2) / meterPrePoint
            {
                pf = new PointF(offset, -j * width /*meterPrePoint*/ + (float)this.Polygon.ScreenBoundary.Height + newH);
                pf5 = new PointF(offset + 5, -j * width/*meterPrePoint*/ + (float)this.Polygon.ScreenBoundary.Height + newH);
                //if (i % 5 == 0)
                ////{
                pf = new PointF(offset, -j * width/* meterPrePoint*/ + (float)this.Polygon.ScreenBoundary.Height + newH);
                pf5 = new PointF(offset + 10, -j * width/*meterPrePoint*/ + (float)this.Polygon.ScreenBoundary.Height + newH);
                    newG.DrawLine(Pens.Black, pf, pf5);
                    pf1 = new PointF(offset * 0.2f, -(j + 1) * width/*meterPrePoint*/ + (float)this.Polygon.ScreenBoundary.Height + newH);
                    sz = new SizeF(offset * 0.8f, 2 * width/*meterPrePoint*/);
                    rf = new RectangleF(pf1, sz);

                    newG.DrawString((_DamOrignCoord.Y + j *  meterPrePoint/*5*/).ToString("0"), _FtScale, Brushes.Black, rf, sf);

                    //continue;
                //}
                newG.DrawLine(Pens.Gray, pf, pf5);
            }
            newG.SmoothingMode = SmoothingMode.AntiAlias;
            //输出放大倍数和面积
            RectangleF thisPf = new RectangleF(0, offset * 0.1f, newBmp.Width - offset * 0.1f, newBmp.Height);
            StringFormat thisSf = new StringFormat();
            thisSf.Alignment = StringAlignment.Far;


            string dateStartString = "开始：" + this._DTStart.Year.ToString("00-") + this._DTStart.Month.ToString("00-") + this._DTStart.Day.ToString("00 ")
                + this._DTStart.Hour.ToString("00:") + this._DTStart.Minute.ToString("00:") + this._DTStart.Second.ToString("00");
            string dateEndString = "结束：" + this._DTEnd.Year.ToString("00-") + this._DTEnd.Month.ToString("00-") + this._DTEnd.Day.ToString("00 ")
                + this._DTEnd.Hour.ToString("00:") + this._DTEnd.Minute.ToString("00:") + this._DTEnd.Second.ToString("00");
            if (this.WorkState == DeckWorkState.WORK)
                dateEndString = "结束：" + "尚未收仓";
            string dateNow = DB.DateUtil.GetDate().Year.ToString("00-") + DB.DateUtil.GetDate().Month.ToString("00-") + DB.DateUtil.GetDate().Day.ToString("00 ")
                + DB.DateUtil.GetDate().Hour.ToString("00:") + DB.DateUtil.GetDate().Minute.ToString("00:") + DB.DateUtil.GetDate().Second.ToString("00");

            //
            float topBlank = newBmp.Height * 0.1f;
            Font ftTime = new Font("微软雅黑", 7.5f * factor, GraphicsUnit.Pixel);
            s = newG.MeasureString("出图时间", ftTime);
            fa = 10f;
            while (s.Height > topBlank * 0.2f)
            {
                ftTime = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("出图时间", ftTime);
                fa = fa - 0.1f;
            }

            Bitmap bitMp = new Bitmap((int)newBmp.Width, (int)(newBmp.Height + topBlank));
            Graphics endG = Graphics.FromImage(bitMp);
            thisPf = new RectangleF(0, 0, bitMp.Width * 0.98f, bitMp.Height);
            thisSf.LineAlignment = StringAlignment.Far;
            thisSf.Alignment = StringAlignment.Far;
            fa = 50f;
            Font ftWord = new Font("微软雅黑", ft.Size, GraphicsUnit.Pixel);
            s = newG.MeasureString("出图时间：" + dateNow, ftWord);
            while (s.Height * 2 > topBlank * 0.29f)
            {
                fa = fa - 0.1f;
                ftWord = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("出图时间：" + dateNow, ftWord);
            }
            endG.Clear(Color.White);
            endG.DrawImageUnscaled(newBmp, 0, (int)topBlank);
            endG.DrawString("出图时间：" + dateNow, ftWord, Brushes.Black, thisPf, thisSf);
            Font ftTitle = new Font("微软雅黑", 15 * factor);

            thisPf = new RectangleF(0, topBlank * 0.7f, bitMp.Width * 0.98f, topBlank);
            thisSf.LineAlignment = StringAlignment.Near;
            thisSf.Alignment = StringAlignment.Far;
            endG.DrawString(dateStartString, ftWord, Brushes.Black, thisPf, thisSf);
            thisPf = new RectangleF(0, topBlank * 0.7f + s.Height, bitMp.Width * 0.98f, topBlank);
            endG.DrawString(dateEndString, ftWord, Brushes.Black, thisPf, thisSf);
            //输出分区，高程，名称，时间
            string allString =  allString = this.MyLayer.MyUnit.Blocks.First().BlockID.ToString() + "-" + this.MyLayer.MyUnit.Blocks.Last().BlockID.ToString() + "坝段   " + this.MyLayer.MyUnit.StartZ.ToString("0.0米-") + this.MyLayer.MyUnit.EndZ.ToString("0.0米")+"     " + this._Name + "碾压层" +this.Polygon.ActualArea.ToString("（0.00 米²）");
            fa = 20f;
            ftTime = new Font("微软雅黑", fa * factor);
            s = newG.MeasureString(allString, ftTime);
            while (s.Height > topBlank * 0.29f || s.Width > (bitMp.Width * 0.98f - newG.MeasureString(dateEndString, ftWord).Width - offset * 3))
            {
                fa = fa - 0.1f;
                ftTime = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString(allString, ftTime);
            }
            endG.DrawString(allString, ftTime, Brushes.Black, offset * 2, topBlank * 0.71f);

            thisPf = new RectangleF(0f, topBlank * 0.25f, bitMp.Width, topBlank * 0.4f);
            thisSf.LineAlignment = StringAlignment.Center;
            thisSf.Alignment = StringAlignment.Center;

            endG.DrawLine(Pens.Black, offset * 0.6f, topBlank * 0.7f, newBmp.Width - offset * 0.6f, topBlank * 0.7f);

        


            s = newG.MeasureString("碾压遍数图形报告", ftTitle);
            fa = 10f;
            while (s.Height > topBlank * 0.4f || s.Width > newBmp.Width)
            {
                ftTitle = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("碾压遍数图形报告", ftTitle);
                fa = fa - 0.1f;
            }
            s = newG.MeasureString("轴(m)", _FtScale);

            endG.DrawString("轴(m)", _FtScale, Brushes.Black, offset * 0.9f, topBlank - s.Height * 0.9f + 2 * factor);

            string mapname=string.Empty;
            switch (mapindex)
            {
                case 0:
                    endG.DrawString("碾压遍数图形报告", ftTitle, Brushes.Black, thisPf, thisSf);
                    mapname=this.Unit.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "roll.png";
                    break;
                case 1:
                    endG.DrawString("静碾遍数图形报告", ftTitle, Brushes.Black, thisPf, thisSf);
                    mapname=this.Unit.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "NoLibroll.png";
                    break;
                case 2:
                    endG.DrawString("振碾遍数图形报告", ftTitle, Brushes.Black, thisPf, thisSf);
                    mapname = this.Unit.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "Libroll.png";
                    break;
            }       
            
#if DEBUG
            bitMp.Save(@"C:\OUTPUT\" + mapname);
#else
                DirectoryInfo dd = new DirectoryInfo(@"C:\OUTPUT\" + this.Name);
                if (!dd.Exists)
                {
                    dd.Create();
                }
            if (mapindex==0)
                bitMp.Save(_Rolladdress + "roll.png");
            else if (mapindex==1)
                bitMp.Save(_Rolladdress + "rollNoLib.png");
            else
                bitMp.Save(_Rolladdress + "rollLib.png");
#endif
            output.Dispose();
            endG.Dispose();
            newG.Dispose();
        }
#endregion


        #region -------------------------------------碾压轨迹图---------------------------------------

        public void DrawPathMap()
        
        {
            TrackGPS gps;
            Polygon pl = this.Polygon;
            Bitmap output = new Bitmap((int)Math.Ceiling(pl.ScreenBoundary.Width), (int)Math.Ceiling(pl.ScreenBoundary.Height), PixelFormat.Format32bppPArgb);
            Graphics g = Graphics.FromImage(output);

            g.Clear(Color.White);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TranslateTransform((float)-pl.ScreenBoundary.Left, (float)-pl.ScreenBoundary.Top);
            pl.AntiAlias = false;
            pl.LineColor = Color.Gray;
            pl.FillColor = Color.White;
            pl.Draw(g);
            pl.SetDrawClip(g);

            foreach (Roller v in vCtrl.Rollers)
            {
                gps = v.TrackGPSControl.Tracking;

                v.TrackGPSControl.Tracking.CreatePath(0,0);
                v.TrackGPSControl.Tracking.DrawSkeleton(g, false);
            }
            DirectoryInfo di = new DirectoryInfo(@"C:\OUTPUT");
            if (!di.Exists)
            {
                di.Create();
            }

            Layer layer = _MyLayer;
            float factor;
            factor = GetMultipleFactor(layer, 3.56);


            //求原点坐标
            Geo.Coord screenOriginCoord = pl.ScreenBoundary.LeftBottom;
            Geo.Coord earthOriginCoord = layer.ScreenToEarth(screenOriginCoord.PF);
            _DamOrignCoord = earthOriginCoord.ToDamAxisCoord();
            _OrignCoordString = "(" + _DamOrignCoord.X.ToString("0.00") + ", " + _DamOrignCoord.Y.ToString("0.00") + ")";

            float offsetx;
            float offsety;
            float offset;

            offsetx = output.Width * 1.2f;
            offsety = output.Width / 6 * 0.5f * 0.5f * 7f + output.Height;
            offset = (offsetx - output.Width) / 2;


            Bitmap newBmp = new Bitmap((int)offsetx, (int)offsety);         
            Graphics newG = Graphics.FromImage(newBmp);

            float newH = output.Width / 6 * 0.5f * 0.5f;

            newG.Clear(Color.White);

            newG.SmoothingMode = SmoothingMode.AntiAlias;
            newG.InterpolationMode = InterpolationMode.HighQualityBicubic;
            newG.TranslateTransform((float)-pl.ScreenBoundary.Left + (offsetx - output.Width) / 2, (float)-pl.ScreenBoundary.Top + newH);
            this.ResetStyles();
            pl.Draw(newG);
            newG.ResetTransform();
            newG.DrawImageUnscaled(output, (int)offset, (int)newH);

            Pen newPen = new Pen(Brushes.Black, 1);
            newPen.CustomEndCap = new AdjustableArrowCap(2, 8, true);
            ft = new Font("微软雅黑", 7.5f * factor, GraphicsUnit.Pixel);
            _FtScale = new Font("微软雅黑", 5.5f * factor);

            _FtString = new Font("微软雅黑", 7.5f * factor, FontStyle.Bold, GraphicsUnit.Pixel);


            float multiple = output.Width / 6;
            float w0 = multiple * 0.5f;
            float fa = 10f;

            SizeF s = newG.MeasureString("100.00%", _FtScale);

            while (s.Width > (multiple - w0) * 0.9f)
            {
                if (fa * factor < 0)
                    return;

                _FtScale = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("100.99%", _FtScale);
                fa = fa - 0.1f;
            }
            s = newG.MeasureString("未碾压", ft);
            fa = 10f;
            while (s.Width > (multiple - w0) * 0.9f)
            {
                ft = new Font("微软雅黑", fa * factor);
                _FtString = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("未碾压", _FtString);
                fa = fa - 0.1f;
            }

            //横轴
            newG.DrawLine(newPen, new PointF(offset - 4, (float)this.Polygon.ScreenBoundary.Height + newH), new PointF((float)this.Polygon.ScreenBoundary.Width + offset, (float)this.Polygon.ScreenBoundary.Height + newH));
            newG.DrawString("坝(m)", _FtScale, Brushes.Black, (float)this.Polygon.ScreenBoundary.Width + offset * 1f, (float)this.Polygon.ScreenBoundary.Height + newH);

            //纵轴
            newG.DrawLine(newPen, new PointF(offset, (float)this.Polygon.ScreenBoundary.Height + newH + 4), new PointF(offset, 2 * factor));
            //newG.DrawString("轴(m)", ftScale, Brushes.Black, offset * 0.9f, 0 * factor);

            //原点坐标
            newG.DrawString(_OrignCoordString, _FtScale, Brushes.Black, offset * 0.8f, (float)this.Polygon.ScreenBoundary.Height + newH + 2);
            newPen.Dispose();



            //刻度
            float meterPrePoint = GetMultipleFactor(layer, 50);
            PointF pf;
            PointF pf5;
            PointF pfWord;
            Pen p2p = new Pen(Brushes.Black, 2);
            RectangleF rf;
            PointF pf1;
            SizeF sz;
            StringFormat sf = new StringFormat(); ;
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;


            newG.SmoothingMode = SmoothingMode.None;
            float width = (float)Math.Min(this.Polygon.ScreenBoundary.Width, this.Polygon.ScreenBoundary.Height) / 5;
            if (this.Polygon.ScreenBoundary.Width < this.Polygon.ScreenBoundary.Height)
            {
                double max = Math.Abs(this.Polygon.Vertex[0].ToDamAxisCoord().X - _DamOrignCoord.X);
                foreach (DamLKK.Geo.Coord c in this.Polygon.Vertex)
                {
                    if (Math.Abs(c.ToDamAxisCoord().X - _DamOrignCoord.X) > max)
                        max = Math.Abs(c.ToDamAxisCoord().X - _DamOrignCoord.X);
                }
                meterPrePoint = (float)max / 5;
            }
            else
            {
                double max = Math.Abs(this.Polygon.Vertex[0].ToDamAxisCoord().Y - _DamOrignCoord.Y);
                foreach (DamLKK.Geo.Coord c in this.Polygon.Vertex)
                {
                    if (Math.Abs(c.ToDamAxisCoord().Y - _DamOrignCoord.Y) > max)
                        max = Math.Abs(c.ToDamAxisCoord().X - _DamOrignCoord.Y);
                }
                meterPrePoint = (float)max / 5;
            }


            //横轴刻度
            for (float i = width, j = 1; i < this.Polygon.ScreenBoundary.Width; i += width, j++)//this.Polygon.ScreenBoundary.Width-40//(float)(this.Polygon.ScreenBoundary.Width - 10) / meterPrePoint
            {
                pf = new PointF(offset + j * width/* meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH - 6);
                pf5 = new PointF(offset + j * width/*meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH);
                //if (i % 5 == 0)
                //{
                pf = new PointF(offset + j * width/*meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH - 10);
                pf5 = new PointF(offset + j * width/*meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH);
                pfWord = new PointF(offset - 8 + j * width /*meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH + 5);
                newG.DrawLine(Pens.Black, pf, pf5);

                pf1 = new PointF(offset + (j - 1) * width/* meterPrePoint*/, (float)this.Polygon.ScreenBoundary.Height + newH);
                sz = new SizeF(2 * width/*meterPrePoint*/, offset * 0.4f);
                rf = new RectangleF(pf1, sz);
                newG.DrawString((_DamOrignCoord.X + j * meterPrePoint/*5*/).ToString("0"), _FtScale, Brushes.Black, rf, sf);
                //continue;
                //}
                newG.DrawLine(Pens.Gray, pf, pf5);
            }
            //纵轴刻度

            for (float i = width, j = 1; i < this.Polygon.ScreenBoundary.Height; i += width, j++)//this.Polygon.ScreenBoundary.Width-40//(float)(this.Polygon.ScreenBoundary.Height - 2) / meterPrePoint
            {
                pf = new PointF(offset, -j * width /*meterPrePoint*/ + (float)this.Polygon.ScreenBoundary.Height + newH);
                pf5 = new PointF(offset + 5, -j * width/*meterPrePoint*/ + (float)this.Polygon.ScreenBoundary.Height + newH);
                //if (i % 5 == 0)
                ////{
                pf = new PointF(offset, -j * width/* meterPrePoint*/ + (float)this.Polygon.ScreenBoundary.Height + newH);
                pf5 = new PointF(offset + 10, -j * width/*meterPrePoint*/ + (float)this.Polygon.ScreenBoundary.Height + newH);
                newG.DrawLine(Pens.Black, pf, pf5);
                pf1 = new PointF(offset * 0.2f, -(j + 1) * width/*meterPrePoint*/ + (float)this.Polygon.ScreenBoundary.Height + newH);
                sz = new SizeF(offset * 0.8f, 2 * width/*meterPrePoint*/);
                rf = new RectangleF(pf1, sz);

                newG.DrawString((_DamOrignCoord.Y + j * meterPrePoint/*5*/).ToString("0"), _FtScale, Brushes.Black, rf, sf);

                //continue;
                //}
                newG.DrawLine(Pens.Gray, pf, pf5);
            }
            ////横轴刻度
            //for (float i = 1; i < (float)(pl.ScreenBoundary.Width - 10) / meterPrePoint; i++)//this.Polygon.ScreenBoundary.Width-40
            //{
            //    pf = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH - 6);
            //    pf5 = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
            //    if (i % 5 == 0)
            //    {
            //        pf = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH - 10);
            //        pf5 = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
            //        pfWord = new PointF(offset - 8 + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH + 5);
            //        newG.DrawLine(Pens.Black, pf, pf5);

            //        pf1 = new PointF(offset + (i - 1) * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
            //        sz = new SizeF(2 * meterPrePoint, offset * 0.4f);
            //        rf = new RectangleF(pf1, sz);
            //        newG.DrawString((_DamOrignCoord.X + i * 5).ToString("0"), _FtScale, Brushes.Black, rf, sf);
            //        continue;
            //    }
            //    newG.DrawLine(Pens.Gray, pf, pf5);
            //}
            ////纵轴刻度

            //for (float i = 1; i < (float)(pl.ScreenBoundary.Height - 10) / meterPrePoint; i++)//this.Polygon.ScreenBoundary.Width-40
            //{
            //    pf = new PointF(offset, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
            //    pf5 = new PointF(offset + 5, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
            //    if (i % 5 == 0)
            //    {
            //        pf = new PointF(offset, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
            //        pf5 = new PointF(offset + 10, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
            //        newG.DrawLine(Pens.Black, pf, pf5);
            //        pf1 = new PointF(offset * 0.2f, -(i + 1) * meterPrePoint + (float)pl.ScreenBoundary.Height + newH);
            //        sz = new SizeF(offset * 0.8f, 2 * meterPrePoint);
            //        rf = new RectangleF(pf1, sz);

            //        newG.DrawString((_DamOrignCoord.Y + i * 5).ToString("0"), _FtScale, Brushes.Black, rf, sf);

            //        continue;
            //    }
            //    newG.DrawLine(Pens.Gray, pf, pf5);
            //}
            newG.SmoothingMode = SmoothingMode.AntiAlias;


            //分析车辆
            List<string> vehicleName = new List<string>();
            List<Color> vehicleColor = new List<Color>();
            bool has = false;
            if (vCtrl.Rollers.Count <= 0)
            {
                return;
            }
            vehicleName.Add(_Control.VehicleControl.FindVechicle(vCtrl.Rollers[0].Assignment.RollerID).Name);
            vehicleColor.Add(vCtrl.Rollers[0].TrackGPSControl.Tracking.Color);
            foreach (Roller v in vCtrl.Rollers)
            {
                has = false;
                for (int i = 0; i < vehicleName.Count; i++)
                {
                    if (_Control.VehicleControl.FindVechicle(v.Assignment.RollerID).Name.Equals(vehicleName[i]))
                    {
                        has = true;
                        break;
                    }
                }
                if (!has)
                {
                    vehicleName.Add(_Control.VehicleControl.FindVechicle(v.Assignment.RollerID).Name);
                    vehicleColor.Add(v.TrackGPSControl.Tracking.Color);
                }
            }

            s = g.MeasureString("三号碾压机", _FtString);
            fa = 10f;
            while (s.Width > (multiple - w0 - 2))
            {
                _FtString = new Font("微软雅黑", fa * factor);
                s = g.MeasureString("三号碾压机", _FtString);
                fa = fa - 0.1f;
            }
            float cutline = (newBmp.Width - offset * 1.05f + w0 * 0.3f - s.Width * 2.6f) / 9;
            //图例
            newG.FillRectangle(Brushes.Black, offset, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2, w0 * 0.3f + 2, w0 / 6f + 2);
            newG.FillRectangle(Brushes.Yellow, offset + 1, 1 + output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2, w0 * 0.3f, w0 / 6f);
            newG.DrawString("超速", _FtString, Brushes.Black, offset * 1.05f + w0 * 0.3f, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2);
            //振动不合格
            newG.FillRectangle(Brushes.Black, offset, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 * 0.3f + 2, w0 / 6f + 2);
            newG.FillRectangle(Brushes.Red, offset + 1, 1 + output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 * 0.3f, w0 / 6f);
            newG.DrawString("静碾不合格", _FtString, Brushes.Black, offset * 1.05f + w0 * 0.3f, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f);

            newG.FillRectangle(Brushes.Red, offset*3, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 * 0.3f + 2, w0 / 6f + 2);
            newG.FillRectangle(Brushes.Black, offset * 3 + 1, 1 + output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 * 0.3f, w0 / 6f);
            newG.DrawString("振碾不合格", _FtString, Brushes.Black, offset*3 * 1.05f + w0 * 0.3f, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f);

            Brush bs;
            s = g.MeasureString("超速", _FtString);
            for (int i = 0; i < vehicleName.Count && i < 8; i++)
            {
                bs = new SolidBrush(vehicleColor[i]);
                newG.FillRectangle(Brushes.Black, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + i * cutline, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2, w0 * 0.2f + 2, w0 / 6f + 2);
                newG.FillRectangle(bs, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + 1 + i * cutline, 1 + output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2, w0 * 0.2f, w0 / 6f);
                newG.DrawString(vehicleName[i], _FtString, Brushes.Black, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + w0 * 0.2f + 2 + i * cutline, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2);
            }
            if (vehicleName.Count > 8)
            {
                for (int i = 0; i < vehicleName.Count - 8; i++)
                {
                    bs = new SolidBrush(vehicleColor[i + 8]);
                    newG.FillRectangle(Brushes.Black, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + i * cutline, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 * 0.2f + 2, w0 / 6f + 2);
                    newG.FillRectangle(bs, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + 1 + i * cutline, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 * 0.2f, w0 / 6f);
                    newG.DrawString(vehicleName[i + 8], _FtString, Brushes.Black, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + w0 * 0.2f + 2 + i * cutline, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f);
                }
            }

            //输出放大倍数和面积
            RectangleF thisPf = new RectangleF(0, offset * 0.1f, newBmp.Width - offset * 0.1f, newBmp.Height);
            StringFormat thisSf = new StringFormat();
            thisSf.Alignment = StringAlignment.Far;


            string dateStartString = "开始：" + this._DTStart.Year.ToString("00-") + this._DTStart.Month.ToString("00-") + this._DTStart.Day.ToString("00 ")
                  + this._DTStart.Hour.ToString("00:") + this._DTStart.Minute.ToString("00:") + this._DTStart.Second.ToString("00");
            string dateEndString = "结束：" + this._DTEnd.Year.ToString("00-") + this._DTEnd.Month.ToString("00-") + this._DTEnd.Day.ToString("00 ")
                + this._DTEnd.Hour.ToString("00:") + this._DTEnd.Minute.ToString("00:") + this._DTEnd.Second.ToString("00");
            if (this.WorkState == DeckWorkState.WORK)
                dateEndString = "结束：" + "尚未收仓";
            string dateNow = DB.DateUtil.GetDate().Year.ToString("00-") + DB.DateUtil.GetDate().Month.ToString("00-") + DB.DateUtil.GetDate().Day.ToString("00 ")
                + DB.DateUtil.GetDate().Hour.ToString("00:") + DB.DateUtil.GetDate().Minute.ToString("00:") + DB.DateUtil.GetDate().Second.ToString("00");

            float topBlank = newBmp.Height * 0.1f;

            Font ftTime = new Font("微软雅黑", 7.5f * factor, GraphicsUnit.Pixel);
            s = newG.MeasureString("出图时间", ftTime);
            fa = 10f;
            while (s.Height > topBlank * 0.2f)
            {
                ftTime = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("出图时间", ftTime);
                fa = fa - 0.1f;
            }

            Bitmap bitMp = new Bitmap((int)newBmp.Width, (int)(newBmp.Height + topBlank));
            Graphics endG = Graphics.FromImage(bitMp);
            thisPf = new RectangleF(0, 0, bitMp.Width * 0.98f, bitMp.Height);
            thisSf.LineAlignment = StringAlignment.Far;
            thisSf.Alignment = StringAlignment.Far;
            endG.Clear(Color.White);
            endG.DrawImageUnscaled(newBmp, 0, (int)topBlank);

            fa = 50f;
            Font ftWord = new Font("微软雅黑", ft.Size, GraphicsUnit.Pixel);
            s = newG.MeasureString("出图时间：" + dateNow, ftWord);
            while (s.Height * 2 > topBlank * 0.29f)
            {
                fa = fa - 0.1f;
                ftWord = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("出图时间：" + dateNow, ftWord);
            }

            endG.DrawString("出图时间：" + dateNow, ftWord, Brushes.Black, thisPf, thisSf);
            Font ftTitle = new Font("微软雅黑", 15 * factor);

            thisPf = new RectangleF(0, topBlank * 0.7f, bitMp.Width * 0.98f, topBlank);
            thisSf.LineAlignment = StringAlignment.Near;
            thisSf.Alignment = StringAlignment.Far;
            endG.DrawString(dateStartString, ftWord, Brushes.Black, thisPf, thisSf);
            thisPf = new RectangleF(0, topBlank * 0.7f + s.Height, bitMp.Width * 0.98f, topBlank);
            endG.DrawString(dateEndString, ftWord, Brushes.Black, thisPf, thisSf);
            //输出分区，高程，名称，时间


            string allString = this.MyLayer.MyUnit.Blocks.First().BlockID.ToString() + "-" + this.MyLayer.MyUnit.Blocks.Last().BlockID.ToString() + "坝段   " + this.MyLayer.MyUnit.StartZ.ToString("0.0米-") + this.MyLayer.MyUnit.EndZ.ToString("0.0米")+"     " + this._Name + "碾压层" + pl.ActualArea.ToString("（0.00 米²）");
            fa = 50f;
            ftTime = new Font("微软雅黑", fa * factor);
            s = newG.MeasureString(allString, ftTime);
            while (s.Height > topBlank * 0.29f || s.Width > (bitMp.Width * 0.98f - newG.MeasureString(dateEndString, ftWord).Width - offset * 3))
            {
                fa = fa - 0.1f;
                ftTime = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString(allString, ftTime);
            }
            endG.DrawString(allString, ftTime, Brushes.Black, offset * 2, topBlank * 0.71f);

            thisPf = new RectangleF(0f, topBlank * 0.25f, bitMp.Width, topBlank * 0.4f);
            thisSf.LineAlignment = StringAlignment.Center;
            thisSf.Alignment = StringAlignment.Center;

            endG.DrawLine(Pens.Black, offset * 0.6f, topBlank * 0.7f, newBmp.Width - offset * 0.6f, topBlank * 0.7f);

            s = newG.MeasureString("碾压轨迹图形报告", ftTitle);
            fa = 10f;
            while (s.Height > topBlank * 0.4f || s.Width > newBmp.Width)
            {
                ftTitle = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("碾压轨迹图形报告", ftTitle);
                fa = fa - 0.1f;
            }


            s = endG.MeasureString("轴", _FtScale);
            endG.DrawString("轴(m)", _FtScale, Brushes.Black, offset * 0.9f, topBlank - s.Height * 0.9f + 2 * factor);
            endG.DrawString("碾压轨迹图形报告", ftTitle, Brushes.Black, thisPf, thisSf);
            _Trackingaddress = this.Unit.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "tracing.png";

#if DEBUG
            bitMp.Save(@"C:\OUTPUT\" + this.Unit.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "tracing.png");
#else
            bitMp.Save(_Rolladdress+"Elevetion.png", System.Drawing.Imaging.ImageFormat.Png);
#endif

            output.Dispose();
            g.Dispose();
            endG.Dispose();
            newG.Dispose();
        }
#endregion


#region ----------------------------------------高程原图--------------------------------------
        /// <summary>
        /// 高程原图，返回最低和最高的高程
        /// </summary>
        public Bitmap ElevationImage(out double lo, out double hi)
        {
            Polygon pl = this.Polygon;
            Bitmap bmp = new Bitmap((int)pl.ScreenBoundary.Width + 1, (int)pl.ScreenBoundary.Height + 1);

            foreach (Roller v in this.VehicleControl.Rollers)
            {
                v.TrackGPSControl.Tracking.FilterForOutput();
            }

            // 1、决定车辆轨迹时间上的先后次序
            // 2、计算相对高度
            // 3、渐变画图
            this.VehicleControl.MaxMin(out lo, out hi);
            if (lo < 100 || hi < 100 || lo == double.MaxValue || hi == double.MinValue)
                return null;
       
            Graphics g = Graphics.FromImage(bmp);

            g.TranslateTransform((float)-pl.ScreenBoundary.Left, (float)-pl.ScreenBoundary.Top);

            pl.SetDrawClip(g);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            foreach (Roller v in this.VehicleControl.Rollers)
            {
                v.TrackGPSControl.Tracking.DrawElevation(g, lo, hi);
            }
            foreach (Roller v in this.VehicleControl.Rollers)
            {
                v.TrackGPSControl.Tracking.Reset();
            }
          
            return bmp;
        }


        public Bitmap CreateElevationImage()
        {
            Polygon pl = this.Polygon;
            double lo, hi;
            double zoomold = this.MyLayer.Zoom;
            this.MyLayer.Zoom = 5;
            this.MyLayer.CreateScreen();
            Bitmap bmp = ElevationImage(out lo, out hi);
            //无高程原图
            if (bmp == null)
                return null;
#if DEBUG
            bmp.Save(@"C:\OUTPUT\" + this.Unit.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "OrignElevation.png", System.Drawing.Imaging.ImageFormat.Png);
#endif
            DB.DeckDAO.GetInstance().UpdateElevationBitMap(this._Unit.ID, this._Elevation.Height, this._ID,DamLKK.DB.DeckDAO.GetInstance().ToByte(bmp), lo.ToString("0.00") + "," + hi.ToString("0.00"));
            this.MyLayer.Zoom = zoomold;
            bmp = ElevationImage(out lo, out hi);
            this.MyLayer.CreateScreen();
            // 1、决定车辆轨迹时间上的先后次序
            // 2、计算相对高度
            // 3、渐变画图
            if (lo < 100 || hi < 100 || lo == double.MaxValue || hi == double.MinValue)
                return null;
            double delta = hi - lo;
            Graphics g = Graphics.FromImage(bmp);

            DirectoryInfo di = new DirectoryInfo(@"C:\OUTPUT");
            if (!di.Exists)
            {
                di.Create();
            }
            return bmp;
        }
#endregion

        double _Thicknes=0;
        // 厚度监控，-1表示不监控
        public double Thickness { get { return _Thicknes; } set { _Thicknes = value; } }

        public void CheckOverThickness(Geo.GPSCoord c3d)
        {
            if (_Thicknes == -1)
                return;
            if (!this.RectContains(c3d.Plane))
                return;

            double distance = c3d.Z - (Thickness + (1 + this.ErrorParam / 100) * this.DesignDepth);
            if (distance > 0)
            {
                Geo.Coord c = c3d.Plane.ToDamAxisCoord();
                string position = string.Format("{{{0:0.00},{1:0.00}}}", c.X, c.Y);
                string warning = string.Format("碾压超厚告警！仓面 {0}，高程 {1}米，碾压层 {2}，超厚 {3:0.00}米，桩号 {4}",
                    this.Unit.Name,
                    this.Elevation.Height,
                    this.Name,
                    distance,
                    position
                    );
            }
        }


        //返回相应面积比
        double[] _AreaScale = null;
        /// <summary>
        /// 各个边数百分比
        /// </summary>
        public static double[] AreaRatio(int[] areas, Deck dk)
        {
            if (areas == null || dk == null)
                return null;
            int okcount = areas.Length;
            double[] area_ratio = new double[okcount + 1];
            double total = 0;
            for (int i = 0; i < areas.Length; i++)
            {
                total += areas[i];
            }
            if (total == 0)
                return null;
            for (int i = 0; i < okcount; i++)
            {
                area_ratio[i] = areas[i] / total;
            }
            for (int i = okcount; i < areas.Length; i++)
            {
                area_ratio[okcount] += areas[i] / total;
            }
            return area_ratio;
        }


        /// <summary>
        /// /返回原始图中点的坐标
        /// </summary>
        public PointF GetOrigin(PointF p, double offsetX, double offsetY)
        {
            PointF origP = new PointF();
            origP.X = (float)(p.X - offsetX);
            origP.Y = (float)(p.Y - offsetY);
            return origP;
        }

        /// <summary>
        /// 
        /// 改仓面上的该点静碾和振碾压边数
        /// </summary>
        public int[] RollCount(PointF pt)
        {
            if (!this.Polygon.IsScreenVisible(new Geo.Coord(pt)))
                return null;
            return VehicleControl.RollCount(pt);
        }

        /// <summary>
        /// 改仓面上的该点总碾压边数
        /// </summary>
        public int RollCountALL(PointF pt)
        {
            if (!this.Polygon.IsScreenVisible(new Geo.Coord(pt)))
                return 0;
            return VehicleControl.RollCountALL(pt);
        }

       
        Geo.BorderShapeII bs;
        /// <summary>
        /// 点是否在仓面范围内
        /// </summary>
        public bool IsInThisDeck(DamLKK.Geo.Coord cd)
        {
             bs= new DamLKK.Geo.BorderShapeII(this._Polygon.Vertex);
             return bs.IsInsideIII(cd);
        }

        //private string getSubTitle(Segment segment)
        //{
            //几到几坝段 多少到多少高程 仓面名称 平层就显示多少多少米 斜层就显示“斜层-几”

            //String unitblocks = DAO.getInstance().getUnitBlocks(segment.UnitID);
            //String startzendz = DAO.getInstance().getStartZEndZ(segment.UnitID);
            //string[] blocks = unitblocks.Split(',');
            //unitblocks = blocks[1] + "-" + blocks[blocks.Length - 2];
            //if (segment.DesignZ > 1000)
            //{//斜层
            //    return unitblocks + "坝段 " + startzendz +/*" 碾压层名称  "+segment.SegmentName+" "+*/segment.DesignZ + "米";
            //}
            //else
            //{
            //    return unitblocks + "坝段 " + startzendz /*+ " 碾压层名称  " + segment.SegmentName  */+ "  第" + segment.DesignZ + "斜层";
            //}

        //}
    }

}
