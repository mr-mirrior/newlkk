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
        #endregion

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
            _Roll.ScrollWidth = 2.17;
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
            //lock (disposing)
            //{
            //    TurnOffGPS();
            //    trackCtrl.Dispose();
            //    GC.SuppressFinalize(this);
            //    isDisposed = true;
            //}
        }


        public int RollCount(PointF pt)
        {
            return TrackGPSControl.Tracking.RollCount(pt);
        }

        public void Draw(Graphics g, bool frameonly)
        {
            TrackGPSControl.Draw(g, frameonly);
        }
    }
}
