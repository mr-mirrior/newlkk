using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamLKK._Model
{
    /// <summary>
    /// //定义车辆工作状态枚举
    /// </summary>
    public enum CarDis_Status
    {
        
        FREE = 0,
        ASSIGNED = 1,
        ENDWORK = 2,
        WORK = 3
    }

    public class RollerDis
    {
        int _RollerID;
        /// <summary>
        /// 车辆id
        /// </summary>
        public int RollerID
        {
            get { return _RollerID; }
            set { _RollerID = value; }
        }
        int _UnitID;
        /// <summary>
        /// 单元id
        /// </summary>
        public int UnitID
        {
            get { return _UnitID; }
            set { _UnitID = value; }
        }
        double _Elevation;
        /// <summary>
        /// 高程或斜面id
        /// </summary>
        public double Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; }
        }
        int _SegmentID;
        /// <summary>
        /// 仓面id
        /// </summary>
        public int SegmentID
        {
            get { return _SegmentID; }
            set { _SegmentID = value; }
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

        List<DamLKK.Geo.GPSCoord> _Track;
        /// <summary>
        ///  轨迹
        /// </summary>
        internal List<DamLKK.Geo.GPSCoord> Track
        {
            get { return _Track; }
            set { _Track = value; }
        }

        int _LibratedState;
        /// <summary>
        /// 击震力状态
        /// </summary>
        public int LibratedState
        {
            get { return _LibratedState; }
            set { _LibratedState = value; }
        }

        double _DesignZ;
        /// <summary>
        /// 高程标识
        /// </summary>
        public double DesignZ
        {
            get { return _DesignZ; }
            set { _DesignZ = value; }
        }

        CarDis_Status _Status;
        /// <summary>
        /// 分配车的分配状态
        /// </summary>
        public CarDis_Status Status
        {
            get { return _Status; }
            set { _Status = value; }
        }


        public bool IsFinished()
        {
            return DTEnd != DateTime.MinValue && DTStart != DateTime.MinValue;
        }
        public bool IsWorking()
        {
            return DTEnd == DateTime.MinValue && DTStart != DateTime.MinValue;
        }
        public bool IsAssigned()
        {
            return DTEnd == DateTime.MinValue && DTStart == DateTime.MinValue;
        }
    }
}
