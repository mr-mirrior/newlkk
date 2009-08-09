using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DamLKK._Control;
using System.Drawing;

namespace DamLKK._Model
{
    /// <summary>
    /// 击震力状态枚举
    /// </summary>
    public enum LibratedState 
    {
        No=0,
        Low=1,
        High=2,
        Nomarl=3
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 击震力信息结构体
    /// </summary>
    struct LibratedInfo 
    {
        int _CarID;
        /// <summary>
        /// 所属车辆id
        /// </summary>
        public int CarID
        {
            get { return _CarID; }
            set { _CarID = value; }
        }

        LibratedState _State;
        /// <summary>
        /// 击震力状态
        /// </summary>
        public LibratedState State
        {
            get { return _State; }
            set { _State = value; }
        }

        DateTime _DT;
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime DT
        {
            get { return _DT; }
            set { _DT = value; }
        }
    }

    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 碾压机
    /// </summary>
    public class Roller : IDisposable
    {
        #region 
        int _ID;
        /// <summary>
        /// 数据库id
        /// </summary>
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        string _Name;
        /// <summary>
        /// 车辆名字
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        double _ScrollWidth;
        /// <summary>
        /// 碾轮宽度
        /// </summary>
        public double ScrollWidth
        {
            get { return _ScrollWidth; }
            set { _ScrollWidth = value; }
        }

        double _GPSHeight;
        /// <summary>
        /// 天线高
        /// </summary>
        public double GPSHeight
        {
            get { return _GPSHeight; }
            set { _GPSHeight = value; }
        }

        double _Speed;
        /// <summary>
        /// 速度
        /// </summary>
        public double Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }

        LibratedState _LibratedState;

        public LibratedState LibratedState
        {
            get { return _LibratedState; }
            set { _LibratedState = value; }
        }

        #endregion

        public Roller(RollerDis car)
        {
            Init();
            this.Assignment = car;
        }


        public Roller(){  Init(); }
        
        public Roller(Deck o) { _OwnerDeck = o; Init(); }

        Deck _OwnerDeck = null;
        TrackGPSControl _TrackCtrl=new TrackGPSControl();
        RollerDis _RollDis = new RollerDis();
        Roller _Roll;

        [System.Xml.Serialization.XmlIgnore]
        public Deck Owner { get { return _OwnerDeck; } set { _OwnerDeck = value; } }

        public TrackGPSControl TrackGPSControl { get { return _TrackCtrl; } set { _TrackCtrl = value; } }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            TrackGPSControl.Owner = this;
            this.ScrollWidth = 2.17;
            _Timer.Interval = 1000 * 60 * 3;
            _Timer.Elapsed += new System.Timers.ElapsedEventHandler(_Timer_Elapsed);
        }

      
        public RollerDis Assignment
        {
            get { return _RollDis; }
            set { _RollDis = value; if (_RollDis != null) _Roll = _Control.VehicleControl.FindVechicle(_RollDis.RollerID); }
        }

        public Roller Roll
        {
            get { return _Roll; }
            set { _Roll = value; }
        }
        private Roller FindThis() { return _Control.VehicleControl.FindVechicle(ID); }


        public void Dispose()
        {
            lock (disposing)
            {
                TurnOffGPS();
                _TrackCtrl.Dispose();
                GC.SuppressFinalize(this);
                isDisposed = true;
            }
        }


        public int[] RollCount(PointF pt)
        {
            return TrackGPSControl.Tracking.RollCount(pt);
        }

        public int RollCountALL(PointF pt)
        {
            return TrackGPSControl.Tracking.RollCountALL(pt);
        }

        public void Draw(Graphics g, bool frameonly)
        {
            TrackGPSControl.Draw(g, frameonly);
        }


#region -----------------------接收gps点----------------------------


        public void TurnOffGPS()
        {
            _Control.GPSServer.OnResponseData -= OnGPSData;
        }
        public void ListenGPS()
        {
            _Control.GPSServer.OnResponseData -= OnGPSData;
            _Control.GPSServer.OnResponseData += OnGPSData;
        }
        object disposing = new object();
        bool isDisposed = false;
        System.Timers.Timer _Timer = new System.Timers.Timer();
        int[] Count; //每个点新点碾压边数
        bool IsNoLib=false;//报警类型
        void _Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            WaringLibrated();
        }

        private void OnGPSData(object sender, _Control.GPSCoordEventArg e)
        {
            lock (disposing)
            {
                if (isDisposed)
                    return;
                if (e.msg != _Control.GPSMessage.GPSDATA)
                    return;
                if (e.gps.CarID != this.ID)
                    return;
                if (!_RollDis.IsWorking())
                    return;

                // 这个点属于该车段
                if (_OwnerDeck.IsVisible)
                {
                    if (!_OwnerDeck.MyLayer.RectContains(e.gps.GPSCoord.Plane))
                        return;
                    SoundControl.BeepIII();

                    this._LibratedState = (LibratedState)e.gps.LibratedStatus;
                    // John, 2009-1-20
                    if (TrackGPSControl.Tracking.Count != 0)
                    {
                        // 判断超厚
                        // 先要看是否停在某个固定点
                        Geo.Vector v = new Geo.Vector(e.gps.GPSCoord.Plane, TrackGPSControl.Tracking.TrackPoints.Last().Plane);
                        if (v.Length() > Config.I.OVERTHICKNESS_DISTANCE)
                            _OwnerDeck.CheckOverThickness(e.gps.GPSCoord);
                    }
                    //feiying  击震力报警
                    //1.是否在仓面内
                    //2.边数和击震力状态是否符合要求
                    if (Owner.IsInThisDeck(e.gps.GPSCoord.Plane))
                    {
                        Count=Owner.RollCount(Owner.MyLayer.DamToScreen(e.gps.GPSCoord.Plane));
                        
                        if(Count!=null&&(Count[0]+Count[1])<this.Owner.NOLibRollCount&&(int)e.gps.LibratedStatus!=0)
                        {
                            if(!_Timer.Enabled)
                            {
                                _Timer.Start();
                                IsNoLib = true;
                            }
                            
                        }
                        else if(Count!=null&&(Count[0]+Count[1])>(this.Owner.NOLibRollCount+Config.I.NOLIBRITEDALLOWNUM)&&(int)e.gps.LibratedStatus==0)
                        {
                            if (!_Timer.Enabled)
                            {
                                _Timer.Start();
                                IsNoLib = false;
                            }
                        }
                        else
                        {
                            if (_Timer.Enabled)
                            _Timer.Stop();
                        }
                    }


                    TrackGPSControl.Tracking.AddOnePoint(e.gps.GPSCoord, 0, 0);
                    _OwnerDeck.MyLayer.OwnerView.RequestPaint();
                }
            }
        }

        /// <summary>
        /// 击震力不合格报警,
        /// </summary>
        private void WaringLibrated()
        {
            string warning;
            Forms.Warning warndlg = new DamLKK.Forms.Warning();
            if (IsNoLib)
            {
                warning = string.Format("振动不合格报警：碾压机：{0},当前地点静碾了{1}遍,振碾了{2}边,该车当前振动状态为振动,设计应为不振。)",
                                this.Name, Count[0].ToString(), Count[1].ToString());
                warndlg.LibrateState = 1;
            }
            else
            {
                warning = string.Format("振动不合格报警：碾压机：{0},当前地点静碾了{1}遍,振碾了{2}边,该车当前振动状态为不振,设计应为振动。)",
                               this.Name, Count[0].ToString(), Count[1].ToString());
                warndlg.LibrateState = 0;
            }

            WarningControl.SendMessage(WarningType.LIBRATED, Owner.Unit.ID, warning);

           
            warndlg.UnitName = this.Owner.Unit.Name;
            warndlg.DeckName = this.Owner.Name;
            warndlg.DesignZ = this.Owner.Elevation.Height;
            warndlg.WarningDate = DB.DateUtil.GetDate().Date.ToString("D");
            warndlg.WarningTime = DB.DateUtil.GetDate().ToString("T");
            warndlg.WarningType = WarningType.LIBRATED;
            warndlg.FillForms();
            Forms.Main.GetInstance.ShowWarningDlg(warndlg);
        }
#endregion
    }
}
