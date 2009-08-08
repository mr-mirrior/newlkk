using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DamLKK.Geo;
using DamLKK._Control;

namespace DamLKK._Model
{
    public class TrackGPS : ICloneable, IDisposable
    {
        /// <summary>
        /// 轨迹段状态标识
        /// </summary>
        private enum TrackingSatus
        {
            NoInfo=0,
            /// <summary>
            /// 静碾
            /// </summary>
            NoLibrated=1,
            /// <summary>
            /// 振碾
            /// </summary>
            Librated=2,
            NoInfoOverSpeed=3,
            NoLibratedOverSpeed = 4,
            LibratedOverSpeed=5
        }


        public TrackGPS(Roller v) { _OwnerRoller = v; origTP.Capacity = 100000; }
        public TrackGPS() { origTP.Capacity = 100000; }

        public object Clone()
        {
            TrackGPS gps = (TrackGPS)this.MemberwiseClone();
            gps.origTP = new List<GPSCoord>(origTP);
            gps.screenSeg = new List<List<GPSCoord>>(screenSeg);
            for (int i = 0; i < screenSeg.Count; i++)
            {
                gps.screenSeg[i] = new List<GPSCoord>(screenSeg[i]);
            }
            return gps;
        }
        public void Dispose()
        {
            lock (sync)
            {
                gpTrackSatus.Clear();
                gpOverspeed.Clear();
                gpTracking.Clear();
                gpTrackingLib.Clear();
                gpNoLib.Clear();
                gpBand.Clear();
                gpBandNolib.Clear();
                gpBandLib.Clear();
            }
            screenSeg.Clear();
            screenSegLib.Clear();
            screenSegFiltered.Clear();
            filteredSeg.Clear();
            filteredTP.Clear();
            segmentedTP.Clear();
            origTP.Clear();
            GC.SuppressFinalize(this);
        }
        // TP means TrackingPoints for short
        List<Geo.GPSCoord> origTP = new List<Geo.GPSCoord>();  // 原始轨迹点，未经任何筛选，施工坐标

        public List<Geo.GPSCoord> TrackPoints
        {
            get { return origTP; }
            set { origTP = value; }
        }
        List<Geo.GPSCoord> filteredTP = new List<Geo.GPSCoord>(); // 经过筛选的轨迹点，施工坐标
        List<List<Geo.GPSCoord>> segmentedTP = new List<List<Geo.GPSCoord>>();   // 分段的坐标，施工坐标，未段内筛选

        List<List<Geo.GPSCoord>> filteredSeg = new List<List<Geo.GPSCoord>>();    // 经过段内筛选的分段坐标，施工坐标

        List<List<Geo.GPSCoord>> screenSeg = new List<List<Geo.GPSCoord>>();     // 经过2次筛选的屏幕坐标

        List<List<Geo.GPSCoord>> screenSegLib = new List<List<Geo.GPSCoord>>();     // 经过2次筛选振动的屏幕坐标

        List<List<Geo.GPSCoord>> screenSegFiltered = new List<List<Geo.GPSCoord>>();     // 经过2次筛选的屏幕坐标

        bool inCurve = true;
        public bool InCurve { get { return inCurve; } set { inCurve = value; } }
        Color cl = Color.Navy;
        public Color Color { get { return cl; } set { cl = value; } }

        public int Count { get { return origTP.Count; } }
        // 归属关系
        Roller _OwnerRoller = null;
        public Roller OwnerRoller { get { return _OwnerRoller; } set { _OwnerRoller = value; } }
        public override string ToString()
        {
            return string.Format("Orig={0}, Segment={1}, Filtered={2}", origTP.Count, segmentedTP.Count, filteredSeg.Count);
        }

        //volatile bool singlePointAdded = false;
        public void AddOnePoint(Geo.GPSCoord pt, double offScrX, double offScrY)
        {
            lock (adding)
            {
                if (!_Control.TrackGPSControl.IsValid(origTP, pt, _OwnerRoller.Owner, _OwnerRoller))
                    return;
                origTP.Add(pt);
            }
            SetTracking(origTP, offScrX, offScrY);
        }
        object adding = new object();
        public void CreateScreen()
        {
            if (origTP.Count == 0)
                return;
            lock (adding)
                CreatePath(0, 0);
        }
        List<GraphicsPath> gpTracking = new List<GraphicsPath>();
        List<GraphicsPath> gpTrackingLib = new List<GraphicsPath>();
        List<GraphicsPath> gpBand = new List<GraphicsPath>();  //所有图形的GP
        List<GraphicsPath> gpBandNolib = new List<GraphicsPath>();  //所有静碾图形的GP
        List<GraphicsPath> gpBandLib = new List<GraphicsPath>();  //所有震碾图形的GP
      
        List<bool> gpOverspeed = new List<bool>();
        List<bool> gpNoLib = new List<bool>();            //是否是不振动 true 不振; false振动
        List<TrackingSatus> gpTrackSatus = new List<TrackingSatus>();

        public void SetTracking(List<Geo.GPSCoord> pts, double offScrX, double offScrY)
        {
            lock (adding)
            {
                /////////////////////////////////feiying 09.3.21 修改出数据图错误问题。待测试
                if (pts.Count == 0)
                    return;
                origTP = new List<GPSCoord>(pts);
                filteredTP = new List<GPSCoord>(origTP);
                PreFilter(ref filteredTP);
                Segment();
                FilterSegments();
                lock (sync)
                    CreatePath(offScrX, offScrY);
            }
        }
        bool IsFlexPoint(int idx, List<Geo.GPSCoord> lst)
        {
            if (idx == 0 || idx == (lst.Count - 1))
                return true;
            return false;
        }
        private double SegmentLength(List<Geo.GPSCoord> lst)
        {
            if (lst == null || lst.Count <= 1)
                return 0;
            double sum = 0;
            for (int i = 0; i < lst.Count - 1; i++)
            {
                Vector v = new Vector(lst[i].Plane, lst[i + 1].Plane);
                sum += v.Length();
            }
            return sum;
        }

        private double VibrantAngle1(List<Geo.GPSCoord> s1, List<Geo.GPSCoord> s2)
        {
            if (s1.Count <= 1 || s2.Count <= 2)
                return double.MaxValue;
            Vector v1 = new Vector(s1[s1.Count - 2].Plane, s1[s1.Count - 1].Plane);
            Vector v2 = new Vector(s2[1].Plane, s2[2].Plane);
            double angle = v1.DeltaAngleTo(v2);
            angle = Math.Abs(angle);
            return angle;
        }
        private double VibrantAngle2(List<Geo.GPSCoord> s1, List<Geo.GPSCoord> s2)
        {
            if (s1.Count <= 2 || s2.Count <= 1)
                return double.MaxValue;
            Vector v1 = new Vector(s1[s1.Count - 3].Plane, s1[s1.Count - 2].Plane);
            Vector v2 = new Vector(s2[0].Plane, s2[1].Plane);
            double angle = v1.DeltaAngleTo(v2);
            angle = Math.Abs(angle);
            return angle;
        }

        private void FilterVibrant()
        {
            for (int i = 1; i < filteredSeg.Count - 1; i++)
            {
                List<GPSCoord> s1 = filteredSeg[i - 1];
                List<GPSCoord> s2 = filteredSeg[i];
                List<GPSCoord> s3 = filteredSeg[i + 1];
                if (s2.Count != 2)
                    continue;

                double l = SegmentLength(s2);
                if (l > 3)
                    continue;

                Vector v = new Vector(s1[s1.Count - 2], s1[s1.Count - 1]);
                double angle = v.Angle();
                //System.Diagnostics.Debug.Print("angle={0:0.00}", angle);
                if (angle < 180)
                {
                    // 往下

                    if (s3.Count <= 2)
                    {
                        s2[1] = new GPSCoord(s2[1], 1);
                        continue;
                    }
                    Vector v2 = new Vector(s3[1], s3[2]);
                    double a2 = v.DeltaAngleTo(v2);
                    double a3 = Math.Abs(a2 - 180);
                    if (a2 > 30 && a3 > 30)
                        continue;
                    s2[1] = new GPSCoord(s2[1], 1);
                }
                else
                {
                    // 往上

                    if (s1.Count <= 2)
                    {
                        s2[0] = new GPSCoord(s2[0], 2);
                        continue;
                    }
                    Vector v1 = new Vector(s1[s1.Count - 3], s1[s1.Count - 2]);
                    Vector v2 = new Vector(s3[0], s3[1]);
                    double a2 = v1.DeltaAngleTo(v2);
                    double a3 = Math.Abs(a2 - 180);
                    if (a2 > 30 && a3 > 30)
                        continue;
                    s2[0] = new GPSCoord(s2[0], 2);
                }
            }
        }
        // 删除距离过近的点（包括相邻坐标相同的点）
        const double THRESHOLD = .2;
        public static void PreFilter(ref List<GPSCoord> lst)
        {
            if (lst.Count == 0)
                return;
            List<GPSCoord> newlst = new List<GPSCoord>();
            newlst.Capacity = lst.Count;
            newlst.Add(lst.First());
            double distance = 0;
            for (int i = 1; i < lst.Count - 1; i++)
            {
                Vector v = new Vector(newlst.Last().Plane, lst[i].Plane);
                //distance += v.Length();
                distance = v.Length();
                if (distance >= _Model.Config.I.BASE_FILTER_THRES)
                {
                    newlst.Add(lst[i]);
                    distance = 0;
                    //                     lst.RemoveAt(i);
                    //                     i--;
                }
            }
            lst = newlst;
        }
        // 按方向选段
        private void Segment()
        {
            //             const int START = 40;
            //             const int END = 717;
            segmentedTP.Clear();
            if (filteredTP.Count < 2)
                return;
            List<GPSCoord> single = new List<GPSCoord>(new GPSCoord[] { filteredTP[0], filteredTP[1] });
            int count = 0;
            for (int i = 1; i < filteredTP.Count - 1; i++)
            {
                Vector v = new Vector(filteredTP[i - 1].Plane, filteredTP[i].Plane);
                GPSCoord pt = filteredTP[i + 1];

                // 丢包点筛选

                bool newseg = isDrawingElevation;
                if (newseg)
                {
                    TimeSpan ts = filteredTP[i].When - filteredTP[i - 1].When;
                    if (ts.TotalSeconds > Config.I.ELEV_FILTER_SECONDS)
                    {
                        newseg = true;
                        if (single.Count != 0)
                            single.RemoveAt(single.Count - 1);
                        count++;
                    }
                    else
                        newseg = false;
                }

                if ((v.DotProductTo(pt.Plane) >= 0) || newseg)
                {
                    // 新段
                    segmentedTP.Add(single);
                    single = new List<GPSCoord>();
                    single.Add(filteredTP[i]);
                }
                single.Add(pt);
            }
            if (single.Count != 0)
                segmentedTP.Add(single);

            if (isDrawingElevation)
            {
                System.Diagnostics.Debug.Print("丢包点: {0}，总共{1}", count, filteredTP.Count);
            }
            //System.Diagnostics.Debug.Print("found segments: {0}", segmentedTP.Count);
        }
        // 段内筛选
        private void FilterSegments()
        {
            if (segmentedTP.Count == 0)
                return;
            filteredSeg = new List<List<GPSCoord>>(segmentedTP);
            for (int i = 0; i < segmentedTP.Count; i++)
            {
                filteredSeg[i] = new List<GPSCoord>(segmentedTP[i]);
            }
            const double THRES_HEADTAIL = 0.6;
            foreach (List<GPSCoord> lst in filteredSeg)
            {
                if (lst.Count <= 3)
                    continue;

                for (int i = 0; i < lst.Count - 1; i++)
                {
                    GPSCoord h1 = lst[i];
                    GPSCoord h2 = lst[i + 1];
                    Vector header = new Vector(h1.Plane, h2.Plane);
                    if (header.Length() < THRES_HEADTAIL)
                    {
                        if (i != (lst.Count - 2)) // 是否末尾
                        {
                            lst.RemoveAt(i + 1);
                            i--;
                        }
                        else
                        {
                            //TRACE("found to end of list {0}", i);
                            lst.RemoveAt(i);
                        }
                    }

                    if (lst.Count <= 2)
                        break;
                }
            }
        }
        DMRectangle scrBoundary = new DMRectangle();
        private void ResetBoundary()
        {
            scrBoundary.Left = scrBoundary.Top = float.MaxValue;
            scrBoundary.Right = scrBoundary.Bottom = float.MinValue;
        }
        private void FilterBoundary(Coord c)
        {
            scrBoundary.Left = Math.Min(scrBoundary.Left, c.X);
            scrBoundary.Top = Math.Min(scrBoundary.Top, c.Y);
            scrBoundary.Right = Math.Max(scrBoundary.Right, c.X);
            scrBoundary.Bottom = Math.Max(scrBoundary.Bottom, c.Y);
        }


#region  -------------------------仅供出实时轨迹图计算用---------------------------
        //public void CreatTracking(double offScrX, double offScrY)
        //{
           
        //    if (filteredSeg.Count == 0)
        //        return;

        //    gpTrackingLib.Clear();
        //    gpTracking.Clear();
        //    gpBand.Clear();
        //    gpBandNolib.Clear();
        //    gpBandLib.Clear();
        //    gpOverspeed.Clear();
        //    gpTrackSatus.Clear();
        //    screenSegFiltered.Clear();


        //    screenSeg = new List<List<GPSCoord>>(filteredSeg);
        //    for (int i = 0; i < filteredSeg.Count; i++)
        //    {
        //        screenSeg[i] = new List<GPSCoord>(filteredSeg[i]);
        //    }
        //    Layer _OwnerLayer = _OwnerRoller.Owner.MyLayer;
        //    Deck MyDeck = this.OwnerRoller.Owner;

        //    for (int i = 0; i < screenSeg.Count; i++)
        //    {
        //        for (int j = 0; j < screenSeg[i].Count; j++)
        //        {
        //            Coord c = new Coord(_OwnerLayer.DamToScreen(screenSeg[i][j].Plane));
        //            c = c.Offset(offScrX, offScrY);
        //            screenSeg[i][j] = new GPSCoord(c.X, c.Y, screenSeg[i][j].Z, screenSeg[i][j].V, screenSeg[i][j].Tag, filteredSeg[i][j].When, filteredSeg[i][j].LibratedStatus);
        //        }
        //    }

        //    RectangleF rc = new RectangleF();


        //    foreach (List<GPSCoord> lst in screenSeg)
        //    {
        //        // 筛选超速点

        //        List<GPSCoord> onelist = new List<GPSCoord>();
        //        List<List<GPSCoord>> lstoflst = new List<List<GPSCoord>>();

        //        TrackingSatus satus = TrackingSatus.Right;
        //        int count = MyDeck.RollCountALL(lst[0].Plane.PF);

        //        if (count < MyDeck.NOLibRollCount && lst[0].LibratedStatus != 0)
        //            satus = TrackingSatus.NoLibWarn;
        //        else if (count > MyDeck.NOLibRollCount + Config.I.NOLIBRITEDALLOWNUM && lst[0].LibratedStatus == 0)
        //            satus = TrackingSatus.LibratedWarn;
        //        else if (lst[0].V >= _OwnerRoller.Owner.MaxSpeed)
        //            satus = TrackingSatus.OverSpeed;

        //        bool overspeeding = (lst[0].V >= _OwnerRoller.Owner.MaxSpeed);

        //        onelist.Add(lst[0]);
        //        lstoflst.Add(onelist);

        //        GPSCoord previous = lst[0];

        //        for (int i = 1; i < lst.Count; i++)
        //        {
        //            count = MyDeck.RollCountALL(lst[i].Plane.PF);
        //            if (count < MyDeck.NOLibRollCount && lst[i].LibratedStatus != 0)
        //            {
        //                if (satus != TrackingSatus.NoLibWarn)
        //                {
        //                    onelist = new List<GPSCoord>();
        //                    onelist.Add(previous);
        //                    lstoflst.Add(onelist);
        //                }
        //                satus = TrackingSatus.NoLibWarn;
        //            }
        //            else if (count > MyDeck.NOLibRollCount + Config.I.NOLIBRITEDALLOWNUM && lst[i].LibratedStatus == 0)
        //            {
        //                if (satus != TrackingSatus.LibratedWarn)
        //                {
        //                    onelist = new List<GPSCoord>();
        //                    onelist.Add(previous);
        //                    lstoflst.Add(onelist);
        //                }
        //                satus = TrackingSatus.LibratedWarn;
        //            }
        //            else if (lst[i].V >= _OwnerRoller.Owner.MaxSpeed)
        //            {
        //                if (satus != TrackingSatus.OverSpeed)
        //                {
        //                    onelist = new List<GPSCoord>();
        //                    onelist.Add(previous);
        //                    lstoflst.Add(onelist);
        //                }
        //                satus = TrackingSatus.OverSpeed;
        //            }
        //            else
        //            {
        //                if (satus != TrackingSatus.Right)
        //                {
        //                    onelist = new List<GPSCoord>();
        //                    onelist.Add(previous);
        //                    lstoflst.Add(onelist);
        //                }
        //                satus = TrackingSatus.Right;
        //            }
        //            onelist.Add(lst[i]);
        //            previous = lst[i];
        //        }
        //            //System.Diagnostics.Debug.Print("舍弃超速点{0}个", count);
        //            using (Pen p = WidthPen(Color.Black))
        //                for (int i = 0; i < lstoflst.Count; i++)
        //                {
        //                    if (lstoflst[i].Count < 2)
        //                        continue;
        //                    GraphicsPath gp = new GraphicsPath();
        //                    PointF[] plane = Geo.DamUtils.Translate(lstoflst[i]);

        //                    screenSegFiltered.Add(lstoflst[i]);
        //                    gp.AddLines(plane);
        //                    rc = RectangleF.Union(rc, gp.GetBounds(new Matrix(), p));
        //                    gpTracking.Add(gp);

        //                    TrackingSatus sat = TrackingSatus.Right;
        //                    if (count < MyDeck.NOLibRollCount && lstoflst[i].Last().LibratedStatus != 0)
        //                        sat = TrackingSatus.NoLibWarn;
        //                    else if (count > MyDeck.NOLibRollCount + Config.I.NOLIBRITEDALLOWNUM && lstoflst[i].Last().LibratedStatus == 0)
        //                        sat = TrackingSatus.LibratedWarn;
        //                    else if (lst[0].V >= _OwnerRoller.Owner.MaxSpeed)
        //                        sat = TrackingSatus.OverSpeed;

        //                    gpTrackSatus.Add(sat);
        //                    gpOverspeed.Add(lstoflst[i].Last().V >= _OwnerRoller.Owner.MaxSpeed);
        //                }
        //            }

        //    //// John, 2009-1-19
        //    if (Config.I.IS_OVERSPEED_VALID)
        //    {
        //        foreach (List<GPSCoord> elem in screenSeg)
        //        {
        //            GraphicsPath gp = new GraphicsPath();
        //            PointF[] lines = Geo.DamUtils.Translate(elem);
        //            gp.AddLines(lines);
        //            gpBand.Add(gp);
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < gpTracking.Count; i++)
        //        {
        //            GraphicsPath gp = gpTracking[i];

        //            if (gpTrackSatus[i] != TrackingSatus.OverSpeed)
        //                gpBand.Add(gp.Clone() as GraphicsPath);

        //        }
        //    }


        //    // John, 2009-1-19

        //    scrBoundary = new DMRectangle(rc);
        //    //             }
        //}


#endregion


        public void CreatePath(double offScrX, double offScrY)
        {
             if (filteredSeg.Count == 0)
                return;

            gpNoLib.Clear();
            gpBandNolib.Clear();
            gpBandLib.Clear();
            gpTracking.Clear();
            gpTrackingLib.Clear();
            gpBand.Clear();
            gpOverspeed.Clear();
            gpTrackSatus.Clear();
            screenSegFiltered.Clear();
            

            screenSeg = new List<List<GPSCoord>>(filteredSeg);
           
            for (int i = 0; i < filteredSeg.Count; i++)
            {
                screenSeg[i] = new List<GPSCoord>(filteredSeg[i]);
                
            }
            Layer _OwnerLayer = _OwnerRoller.Owner.MyLayer;
            Deck MyDeck=this.OwnerRoller.Owner;

            for (int i = 0; i < screenSeg.Count; i++)
            {
                for (int j = 0; j < screenSeg[i].Count; j++)
                {
                    Coord c = new Coord(_OwnerLayer.DamToScreen(screenSeg[i][j].Plane));
                    c = c.Offset(offScrX, offScrY);
                    screenSeg[i][j] = new GPSCoord(c.X, c.Y, screenSeg[i][j].Z, screenSeg[i][j].V, screenSeg[i][j].Tag, filteredSeg[i][j].When, filteredSeg[i][j].LibratedStatus);

                }
            }

            RectangleF rc = new RectangleF();
            screenSegLib = screenSeg;
            
            foreach (List<GPSCoord> lst in screenSeg)
            {
                // 筛选超速点
           
                List<GPSCoord> onelist = new List<GPSCoord>();
                List<List<GPSCoord>> lstoflst = new List<List<GPSCoord>>();

                TrackingSatus overspeeding;

                 if (lst[0].LibratedStatus == -1 && lst[0].V >= this._OwnerRoller.Owner.MaxSpeed)
                    overspeeding = TrackingSatus.NoInfoOverSpeed;
                else if (lst[0].LibratedStatus == 0 && lst[0].V >= this._OwnerRoller.Owner.MaxSpeed)
                    overspeeding = TrackingSatus.NoLibratedOverSpeed;
                else if (lst[0].LibratedStatus >0 && lst[0].V >= this._OwnerRoller.Owner.MaxSpeed)
                     overspeeding = TrackingSatus.LibratedOverSpeed;
                else if (lst[0].LibratedStatus == 0)
                    overspeeding = TrackingSatus.NoLibrated;
                else if (lst[0].LibratedStatus > 0)
                    overspeeding = TrackingSatus.Librated;
                 else
                     overspeeding = TrackingSatus.NoInfo;
               

                onelist.Add(lst[0]);
                lstoflst.Add(onelist);

                GPSCoord previous = lst[0];

                 for (int i = 1; i < lst.Count; i++)
                {
                    if (lst[i].LibratedStatus == -1 && lst[i].V >= this._OwnerRoller.Owner.MaxSpeed)
                    {
                        if (overspeeding!= TrackingSatus.NoInfoOverSpeed)
                        {
                            onelist = new List<GPSCoord>();
                            onelist.Add(previous);
                            lstoflst.Add(onelist);
                        }
                        overspeeding= TrackingSatus.NoInfoOverSpeed;
                    }
                    else if (lst[i].LibratedStatus == 0 && lst[i].V >= this._OwnerRoller.Owner.MaxSpeed)
                    {
                        if (overspeeding != TrackingSatus.NoLibratedOverSpeed)
                        {
                            onelist = new List<GPSCoord>();
                            onelist.Add(previous);
                            lstoflst.Add(onelist);
                        }
                        overspeeding = TrackingSatus.NoLibratedOverSpeed;
                    }
                    else if (lst[i].LibratedStatus > 0 && lst[i].V >= this._OwnerRoller.Owner.MaxSpeed)
                    {
                        if (overspeeding != TrackingSatus.LibratedOverSpeed)
                        {
                            onelist = new List<GPSCoord>();
                            onelist.Add(previous);
                            lstoflst.Add(onelist);
                        }
                        overspeeding = TrackingSatus.LibratedOverSpeed;
                    }
                    else if (lst[i].LibratedStatus == 0)
                    {
                        if (overspeeding != TrackingSatus.NoLibrated)
                        {
                            onelist = new List<GPSCoord>();
                            onelist.Add(previous);
                            lstoflst.Add(onelist);
                        }
                        overspeeding = TrackingSatus.NoLibrated;
                    }
                    else if (lst[i].LibratedStatus > 0)
                    {
                        if (overspeeding != TrackingSatus.Librated)
                        {
                            onelist = new List<GPSCoord>();
                            onelist.Add(previous);
                            lstoflst.Add(onelist);
                        }
                        overspeeding = TrackingSatus.Librated;
                    }
                    else
                    {
                        if (overspeeding != TrackingSatus.NoInfo)
                        {
                            onelist = new List<GPSCoord>();
                            onelist.Add(previous);
                            lstoflst.Add(onelist);
                        }
                        overspeeding = TrackingSatus.NoInfo;
                    }

                    //if (lst[i].V >= MyDeck.MaxSpeed)
                    //{
                    //    if (!overspeeding)
                    //    {
                    //        onelist = new List<GPSCoord>();
                    //        onelist.Add(previous);
                    //        lstoflst.Add(onelist);
                    //        //System.Diagnostics.Debug.Print("未超速->超速");
                    //        //                             gpOverspeed.SetVertex(true);
                    //    }
                    //    overspeeding = true;
                    //}
                    //else
                    //{
                    //    if (overspeeding)
                    //    {
                    //        onelist = new List<GPSCoord>();
                    //        onelist.Add(previous);
                    //        lstoflst.Add(onelist);
                    //        //System.Diagnostics.Debug.Print("超速->未超速");
                    //        //                             gpOverspeed.SetVertex(false);
                    //    }
                    //    overspeeding = false;
                    //}
                    onelist.Add(lst[i]);
                    previous = lst[i];

                 }


                using (Pen p = WidthPen(Color.Black))
                    for (int i = 0; i < lstoflst.Count; i++)
                    {
                        if (lstoflst[i].Count < 2)
                            continue;
                        GraphicsPath gp = new GraphicsPath();
                        PointF[] plane = Geo.DamUtils.Translate(lstoflst[i]);

                        screenSegFiltered.Add(lstoflst[i]);
                        gp.AddLines(plane);
                        rc = RectangleF.Union(rc, gp.GetBounds(new Matrix(), p));
                        gpTracking.Add(gp);

                        if (lstoflst[i].Last().LibratedStatus == -1 && lstoflst[i].Last().V >= this._OwnerRoller.Owner.MaxSpeed)
                        {
                            gpTrackSatus.Add(TrackingSatus.NoInfoOverSpeed);
                            gpOverspeed.Add(true);
                        }
                        else if (lstoflst[i].Last().LibratedStatus == 0 && lstoflst[i].Last().V >= this._OwnerRoller.Owner.MaxSpeed)
                        {
                            gpTrackSatus.Add(TrackingSatus.NoLibratedOverSpeed);
                            gpOverspeed.Add(true);
                        }
                        else if (lstoflst[i].Last().LibratedStatus > 0 && lstoflst[i].Last().V >= this._OwnerRoller.Owner.MaxSpeed)
                        {
                            gpTrackSatus.Add(TrackingSatus.LibratedOverSpeed);
                            gpOverspeed.Add(true);
                        }
                        else if (lstoflst[i].Last().LibratedStatus == 0)
                        {
                            gpTrackSatus.Add(TrackingSatus.NoLibrated);
                            gpOverspeed.Add(false);
                        }
                        else if (lstoflst[i].Last().LibratedStatus > 0)
                        {
                            gpTrackSatus.Add(TrackingSatus.Librated);
                            gpOverspeed.Add(false);
                        }
                        else
                        {
                            gpTrackSatus.Add(TrackingSatus.NoInfo);
                            gpOverspeed.Add(false);
                        }
                    }
                
                }


            //////////////////////////////筛选振动点

            foreach (List<GPSCoord> lst in screenSegLib)
            {
                // 筛选超速点

                List<GPSCoord> onelist = new List<GPSCoord>();
                List<List<GPSCoord>> lstoflstLib = new List<List<GPSCoord>>();

                bool nolib = (lst[0].LibratedStatus == 0);


                onelist.Add(lst[0]);
                lstoflstLib.Add(onelist);

                GPSCoord previous = lst[0];

                for (int i = 1; i < lst.Count; i++)
                {
                    if (lst[i].LibratedStatus == 0)
                    {
                        if (!nolib)
                        {
                            onelist = new List<GPSCoord>();
                            onelist.Add(previous);
                            lstoflstLib.Add(onelist);
                        }
                        nolib = true;
                    }
                    else
                    {
                        if (nolib)
                        {
                            onelist = new List<GPSCoord>();
                            onelist.Add(previous);
                            lstoflstLib.Add(onelist);
                        }
                        nolib = false;
                    }
                    onelist.Add(lst[i]);
                    previous = lst[i];

                }

                using (Pen p = WidthPen(Color.Black))
                    for (int i = 0; i < lstoflstLib.Count; i++)
                    {
                        if (lstoflstLib[i].Count < 2)
                            continue;
                        GraphicsPath gp = new GraphicsPath();
                        PointF[] plane = Geo.DamUtils.Translate(lstoflstLib[i]);
                        gp.AddLines(plane);
                        rc = RectangleF.Union(rc, gp.GetBounds(new Matrix(), p));
                        gpTrackingLib.Add(gp);
                        gpNoLib.Add(lstoflstLib[i].Last().LibratedStatus==0);
                    }

            }

          

                //把静碾和振碾的轨迹区分开
                for(int i=0;i<gpTrackingLib.Count;i++)
                {
                    GraphicsPath gp = gpTrackingLib[i];

                    if (gpNoLib[i])
                        gpBandNolib.Add(gp);
                    else
                        gpBandLib.Add(gp);
                }

            //// John, 2009-1-19
            if (Config.I.IS_OVERSPEED_VALID)
            {
                foreach (List<GPSCoord> elem in screenSeg)
                {
                    GraphicsPath gp = new GraphicsPath();
                    PointF[] lines = Geo.DamUtils.Translate(elem);
                    gp.AddLines(lines);
                    gpBand.Add(gp);
                }
            }
            else
            {
                for (int i = 0; i < gpTracking.Count; i++)
                {
                    GraphicsPath gp = gpTracking[i];

                    if (!gpOverspeed[i])
                        gpBand.Add(gp.Clone() as GraphicsPath);
                }
            }


            // John, 2009-1-19

            scrBoundary = new DMRectangle(rc);
            //             }
            //System.Diagnostics.Debug.Print(libratedTracking.Count.ToString());
        }

        private float WidthPen()
        {
            return (float)OwnerRoller.Owner.MyLayer.ScreenSize(_OwnerRoller.ScrollWidth);
        }
        private Pen WidthPen(Color cl)
        {
            return new Pen(cl, WidthPen());
        }
        object sync = new object();

        /// <summary>
        /// 画图  0:所有轨迹,1:静碾轨迹;2:振碾轨迹
        /// </summary>
        public void Draw(Graphics g, bool frameonly,int mapindex)
        {
            lock (sync)
            {

                if (frameonly)
                {
                    for (int i = 0; i < gpTracking.Count; i++)
                    {
                        g.DrawPath(Pens.Black, gpTracking[i]);
                    }
                    return;
                }
                Layer _OwnerLayer = _OwnerRoller.Owner.MyLayer;
                //double screenMeters = ownerLayer.ScreenSize(owner.WheelWidth);
                using (Pen p = WidthPen(Color.FromArgb(0x20, cl)))
                {
                    p.LineJoin = LineJoin.Round;
                    // 画碾压带
                    switch (mapindex)
                    {
                        case 0:
                                for (int i = 0; i < gpBand.Count; i++)
                                {
                                    g.DrawPath(p, gpBand[i]);
                                }
                	        break;
                        case 1:
                            for (int i = 0; i < gpBandNolib.Count; i++)
                            {
                                g.DrawPath(p, gpBandNolib[i]);
                            }
                            break;
                        case 2:
                            for (int i = 0; i < gpBandLib.Count; i++)
                            {
                                g.DrawPath(p, gpBandLib[i]);
                            }
                            break;
                    }
                  
                }
            }
        }
        private void DrawToken(Graphics g, GPSCoord c, Color cl1, Color cl2, Color cl3)
        {
            float radius = (float)_OwnerRoller.Owner.MyLayer.ScreenSize(0.1);
            RectangleF rc = new RectangleF(c.Plane.XF - radius, c.Plane.YF - radius, radius * 2, radius * 2);
            using (Brush b = new SolidBrush(cl1))
            {
                g.FillEllipse(b, rc);
            }
            using (Pen p = new Pen(cl2, radius * .3f))
                g.DrawEllipse(p, rc);
            rc.Inflate(-radius * .6f, -radius * .6f);
            using (Pen p = new Pen(cl3, radius * .3f))
            {
                g.DrawLine(p, new PointF(rc.Left, rc.Top), new PointF(rc.Right, rc.Bottom));
                g.DrawLine(p, new PointF(rc.Left, rc.Bottom), new PointF(rc.Right, rc.Top));
            }
        }
        private void DrawPointOK(Graphics g, GPSCoord c)
        {
            DrawToken(g, c, Color.Gray, Color.Black, Color.Transparent);
        }
        bool drawOverspeed = true;
        public bool DrawOverSpeed { get { return drawOverspeed; } set { drawOverspeed = value; } }
        public void DrawSkeleton(Graphics g, bool drawingArrows)
        {
            lock (sync)
            {
                float scrSize = (float)_OwnerRoller.Owner.MyLayer.ScreenSize(0.05);
                scrSize = Math.Max(scrSize, 0.1f);
                scrSize = Math.Min(scrSize, 0.8f);
                float Bsize = 2.5f,size=1.5f;// (float)owner.Owner.Owner.ScreenSize(.15f);



                //if (_OwnerRoller.Owner._IsOutTrackingMap)
                //{
                //   using (Pen p = new Pen(Color.FromArgb(0xFF, this.Color), size),
                //   p1 = new Pen(Color.Yellow, 1.8f),
                //   pblack = new Pen(Color.Black, p.Width),
                //   pred = new Pen(Color.Red, p.Width),
                //   p2 = new Pen(Color.Black, 2.7f))

                //    for (int i = 0; i < gpTracking.Count; i++)
                //    {

                //        if (g.SmoothingMode == SmoothingMode.AntiAlias && drawingArrows)
                //            p.CustomEndCap = new AdjustableArrowCap(scrSize * 3, scrSize * 12, true);


                //        if (gpTrackSatus[i] == TrackingSatus.NoLibWarn)
                //        {
                //            g.DrawPath(pred, gpTracking[i]);
                //        }
                //        else if (gpTrackSatus[i] == TrackingSatus.LibratedWarn)
                //        {
                //            g.DrawPath(pblack, gpTracking[i]);
                //        }
                //        else if (gpTrackSatus[i] == TrackingSatus.OverSpeed)//gpOverspeed[i]
                //        {
                //            if (drawOverspeed)
                //            {
                //                g.DrawPath(p2, gpTracking[i]);
                //                g.DrawPath(p1, gpTracking[i]);
                //            }
                //        }
                //        else
                //            g.DrawPath(p, gpTracking[i]);
                //    }
                //}
                //else
                {
                    using (Pen pBthis = new Pen(Color.FromArgb(0xFF, this.Color), Bsize),
                        pMthis = new Pen(Color.FromArgb(0xFF, this.Color), 1.5f),
                         pthis = new Pen(Color.FromArgb(0xFF, this.Color), size),
                        pByellow = new Pen(Color.Yellow, Bsize),
                        pMyellow = new Pen(Color.Yellow, 1.5f),
                        pyellow = new Pen(Color.Yellow, size))

                        for (int i = 0; i < gpTracking.Count; i++)
                        {

                            if (g.SmoothingMode == SmoothingMode.AntiAlias && drawingArrows)
                            {
                                pBthis.CustomEndCap = new AdjustableArrowCap(scrSize * 3, scrSize * 12, true);
                                //pMthis.CustomEndCap = new AdjustableArrowCap(scrSize * 3, scrSize * 12, true);
                                //pthis.CustomEndCap = new AdjustableArrowCap(scrSize * 3, scrSize * 12, true);
                            }


                            if (gpTrackSatus[i] == TrackingSatus.NoInfoOverSpeed)
                            {
                                if (this.DrawOverSpeed)
                                    g.DrawPath(pMyellow, gpTracking[i]);
                                else
                                    g.DrawPath(pMthis, gpTracking[i]);
                            }
                            else if (gpTrackSatus[i] == TrackingSatus.NoLibratedOverSpeed)
                            {
                                if (this.DrawOverSpeed)
                                    g.DrawPath(pyellow, gpTracking[i]);
                                else
                                    g.DrawPath(pthis, gpTracking[i]);
                            }
                            else if (gpTrackSatus[i] == TrackingSatus.LibratedOverSpeed)
                            {
                                //g.DrawPath(pByellow, gpTracking[i]);
                                if (this.DrawOverSpeed)
                                    g.DrawPath(pByellow, gpTracking[i]);
                                else
                                    g.DrawPath(pBthis, gpTracking[i]);
                            }
                            else if (gpTrackSatus[i] == TrackingSatus.NoLibrated)
                            {
                                g.DrawPath(pthis, gpTracking[i]);
                            }
                            else if (gpTrackSatus[i] == TrackingSatus.Librated)
                            {
                                g.DrawPath(pBthis, gpTracking[i]);
                              
                            }
                            else
                            {
                                g.DrawPath(pMthis, gpTracking[i]);
                            }

                            //if (gpOverspeed[i])
                            //{
                            //    if (drawOverspeed)
                            //    {
                            //        g.DrawPath(p2, gpTracking[i]);
                            //        g.DrawPath(p1, gpTracking[i]);
                            //    }
                            //}
                            //else
                            //    g.DrawPath(p, gpTracking[i]);
                        }
                }

            }
        }
        public void DrawAnchor(Graphics g)
        {
            lock (sync)
            {
                // 画最后轨迹点箭头
                //             if (!anchor)
                //             {
                //                 isPainting = false;
                //                 return;
                //             }
                if (screenSeg.Count == 0 || filteredSeg.Count == 0)
                {
                    return;
                }
                List<GPSCoord> lst = screenSeg.Last(); // 最后一段
                List<GPSCoord> filteredLst = filteredSeg.Last();
                if (lst.Count < 2 || filteredLst.Count < 2)
                {
                    return;
                }
                Coord c1 = lst[lst.Count - 2].Plane;  // 倒数第2点

                Coord c2 = lst[lst.Count - 1].Plane;  // 最后1点

                using (Pen p = new Pen(Color.Black, (float)_OwnerRoller.Owner.MyLayer.ScreenSize(0.5)))
                {
                    p.EndCap = LineCap.ArrowAnchor;
                    g.DrawLine(p, c1.PF, c2.PF);
                }
                //DrawAnchor(g, lst[lst.Count - 1], filteredSeg.Last().Last());

                GPSCoord lastptScr = screenSeg.Last().Last();
                GPSCoord lastpt = filteredSeg.Last().Last();
                Coord cp = lastptScr.Plane;
                RectangleF rc = new RectangleF((float)cp.X, (float)cp.Y, 1000, 500);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Near;
                sf.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
                rc.Offset(10, 3);
                //写实时点的击震力状态。feiying 09.3.20
                string libratedstring = string.Empty;
                if (!_OwnerRoller.Assignment.IsWorking())
                    libratedstring = string.Empty;
                else
                    libratedstring = "（" + Forms.Warning.GetLibratedString((int)OwnerRoller.LibratedState) + "）";

                string strInfo = string.Format("{0}", OwnerRoller.Name + libratedstring, lastpt.Plane.ToString());
                string strVelocity = string.Format("{0:0.00} km/h", lastpt.V);

                FontStyle fs = FontStyle.Regular;
                float emsize = .8f;
                float emsize1 = (float)_OwnerRoller.Owner.MyLayer.ScreenSize(1);
                Color cl = Color.Navy;
                float maxspeed = (float)_OwnerRoller.Owner.MaxSpeed;
                if (lastpt.V >= maxspeed)
                {
                    fs = FontStyle.Bold;
                    emsize *= 1 + ((float)lastpt.V - maxspeed) / maxspeed;
                    emsize = Math.Min(emsize, 48);
                    cl = Color.OrangeRed;
                }
                emsize = (float)_OwnerRoller.Owner.MyLayer.ScreenSize(emsize);
                Font orig = new Font(_OwnerRoller.Owner.MyLayer.OwnerView.Font.FontFamily, emsize1, FontStyle.Bold);
                Font ft = new Font(orig.FontFamily, emsize, fs);
                SizeF size1 = g.MeasureString(strInfo, orig);
                SizeF size2 = g.MeasureString(strVelocity, ft);
                rc.Width = Math.Max(size1.Width, size2.Width);
                float height1 = size1.Height;
                float height2 = size2.Height;
                RectangleF rc1 = rc;
                rc1.Offset(0, height1);
                //             orig = new Font(orig, FontStyle.Bold);
                using (Brush b1 = new SolidBrush(Color.FromArgb(0xff, this.Color)), b2 = new SolidBrush(Color.FromArgb(0x50, Color.WhiteSmoke)))
                    Utils.Graph.OutGlow.DrawOutglowText(g, strInfo, orig, rc, sf, b1, b2);

                if (_OwnerRoller.Assignment.IsWorking())
                    using (Brush b1 = new SolidBrush(Color.FromArgb(0xff, cl)), b2 = new SolidBrush(Color.FromArgb(0x50, Color.WhiteSmoke)))
                        Utils.Graph.OutGlow.DrawOutglowText(g, strVelocity, ft, rc1, sf, b1, b2);
            }

        }
        public void SetOrigin(Coord neworig)
        {
            for (int i = 0; i < this.screenSeg.Count; i++)
            {
                List<GPSCoord> s = this.screenSeg[i];
                SetOrigin(ref s, neworig);
                this.screenSeg[i] = s;
            }
        }
        
        /// <summary>
        /// // 重置原点
        /// </summary>
        public static void SetOrigin(ref List<GPSCoord> lst, Coord neworig)
        {
            Coord origin = new Coord(double.MaxValue, double.MaxValue);
            for (int i = 0; i < lst.Count; i++)
            {
                GPSCoord c3d = lst[i];
                Coord c = c3d.Plane;
                if (origin.X > c.X)
                    origin.X = c.X;
                if (origin.Y > c.Y)
                    origin.Y = c.Y;
            }

            for (int i = 0; i < lst.Count; i++)
            {
                GPSCoord c3d = lst[i];
                Coord c = c3d.Plane;
                c = c.Offset(origin.Negative());
                c = c.Offset(neworig);
                lst[i] = new GPSCoord(c, c3d.Z);
            }
        }
        public int[] RollCount(PointF scrPoint)
        {
            lock (sync)
            {
                if (!this.scrBoundary.Contains(scrPoint))
                    return null;
                int count = 0,countNo=0;
                using (Pen p = WidthPen(Color.Black))
                {
                    for (int i = 0; i < gpBandNolib.Count; i++)
                    {
                        if (gpBandNolib[i].IsOutlineVisible(scrPoint, p))
                            countNo++;
                    }

                    for (int i = 0; i < gpBandLib.Count; i++)
                    {
                        if (gpBandLib[i].IsOutlineVisible(scrPoint, p))
                            count++;
                    }
                }
               
                return new int[]{countNo,count};
            }
        }

        public int RollCountALL(PointF scrPoint)
        {
            lock (sync)
            {
                if (!this.scrBoundary.Contains(scrPoint))
                    return 0;
                int count = 0;
                using (Pen p = WidthPen(Color.Black))
                {
                    for (int i = 0; i < gpBand.Count; i++)
                    {
                        if (gpBand[i].IsOutlineVisible(scrPoint, p))
                            count++;
                    }
                }

                return count;
            }
        }

        public void MaxMin(out double lo, out double hi)
        {
            lo = -1;
            hi = -1;
            if (this.filteredTP.Count == 0)
                return;
            double max = filteredTP[0].Z;
            double min = max;
            foreach (GPSCoord c in filteredTP)
            {
                if (_OwnerRoller.Owner.RectContains(c.Plane))
                {
                    max = Math.Max(max, c.Z);
                    min = Math.Min(min, c.Z);
                }
            }
            lo = min;
            hi = max;
        }
        bool isDrawingElevation = false;
        List<GPSCoord> old;
        public void FilterForOutput()
        {
            List<GPSCoord> lst = new List<GPSCoord>();
            Deck dkinfo = _OwnerRoller.Owner;

            double lo = dkinfo.StartZ + dkinfo.DesignDepth * Config.I.ELEV_FILTER_ELEV_LOWER;
            double hi = dkinfo.StartZ + dkinfo.DesignDepth * Config.I.ELEV_FILTER_ELVE_UPPER;
            double speedmax = dkinfo.MaxSpeed * Config.I.ELEV_FILTER_SPEED;

            for (int i = 0; i < origTP.Count; i++)
            {
                double z = origTP[i].Z;
                double v = origTP[i].V;
                if (z >= lo && z <= hi && (v < speedmax))
                    lst.Add(origTP[i]);
            }

            old = origTP;
            isDrawingElevation = true;
            SetTracking(lst, 0, 0);
            isDrawingElevation = false;
            //origTP = old;
        }
        public void Reset()
        {
            SetTracking(old, 0, 0);
        }

        public void DrawElevation(Graphics g, double lo, double hi)
        {
            float delta = (float)(hi - lo);
            if (delta <= 0)
                return;

            for (int idx = 0; idx < screenSeg.Count; idx++)
            {
                //                 if (gpOverspeed[idx])
                //                     continue;
                List<GPSCoord> seg = screenSeg[idx];
                Vector test = new Vector(seg.First(), seg.Last());
                if (test.Length() < 0.01)
                    continue;

                float distance = 0;
                List<float> distances = new List<float>(); // 距离
                List<float> factors = new List<float>();    // 因子
                List<float> subdistances = new List<float>();    // 子距离, 0~1
                distances.Add(0);
                double f = (seg.First().Z - lo) / delta;
                f = Math.Max(0, f);
                f = Math.Min(1, f);
                factors.Add((float)f);
                for (int i = 1; i < seg.Count; i++)
                {
                    Vector v = new Vector(seg[i - 1], seg[i]);
                    float length = (float)v.Length();
                    distance += length;
                    distances.Add(distance);
                    f = (seg[i].Z - lo) / (delta);
                    f = Math.Max(0, f);
                    f = Math.Min(1, f);
                    factors.Add((float)f);
                }
                List<Color> colors = new List<Color>();
                for (int i = 0; i < distances.Count; i++)
                {
                    float subdistance = distances[i] / distance;
                    int gray = (int)(factors[i] * 255);
                    colors.Add(Color.FromArgb(gray, gray, gray));
                    subdistances.Add(subdistance);
                }

                GraphicsPath gp = new GraphicsPath();
                gp.AddLines(Geo.DamUtils.Translate(seg).ToArray());
                LinearGradientBrush br = new LinearGradientBrush(seg.First().Plane.PF, seg.Last().Plane.PF, Color.Black, Color.White);
                Blend bl = new Blend();
                bl.Factors = factors.ToArray();
                bl.Positions = subdistances.ToArray();
                br.Blend = bl;

                br.WrapMode = WrapMode.TileFlipXY;
                Pen p = new Pen(br);
                p.Width = WidthPen();
                g.DrawPath(p, gp);

                p.Dispose();
                br.Dispose();
                gp.Dispose();
                //                 GraphicsPath gp = (GraphicsPath)gpTracking[idx].Clone();
                //                 using (Pen p = WidthPen(Color.White))
                //                 {
                //                     gp.Widen(p);
                //                     using (PathGradientBrush b = new PathGradientBrush(gp))
                //                     {
                //                         //b.CenterColor = Color.Black;
                //                         ColorBlend cb = new ColorBlend();
                //                         cb.Colors = colors.ToArray();
                //                         cb.Positions = subdistances.ToArray();
                //                         b.InterpolationColors = cb;
                // 
                //                         //b.WrapMode = WrapMode.TileFlipXY;
                //                         g.FillPath(b, gp);
                //                     }
                //                     gp.Dispose();
                //                 }
            }
        }
    }
}
