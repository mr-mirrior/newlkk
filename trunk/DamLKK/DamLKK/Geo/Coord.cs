using System.Xml.Serialization;
using System.Drawing;
using System.Runtime.InteropServices;
using System;

namespace DamLKK.Geo
{
    /// <summary>
    /// 基本2D坐标点
    /// </summary>
    public struct Coord
    {
        double _x;
        double _y;

        public Coord(double xx, double yy) { _x = xx; _y = yy; }
        public Coord(Coord c) { _x = c._x; _y = c._y; }
        public Coord(Point pt) { _x = pt.X; _y = pt.Y; }
        public Coord(PointF pt) { _x = pt.X; _y = pt.Y; }

        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }

        [XmlIgnore]
        public float XF { get { return (float)_x; } set { _x = value; } }
        [XmlIgnore]
        public float YF { get { return (float)_y; } set { _y = value; } }

        /// <summary>
        /// 返回整形的坐标点
        /// </summary>
        [XmlIgnore]
        public Point PT { get { return new Point((int)_x, (int)_y); } set { _x = value.X; _y = value.Y; } }
        /// <summary>
        /// 返回浮点型的坐标点
        /// </summary>
        [XmlIgnore]
        public PointF PF { get { return new PointF((float)_x, (float)_y); } set { _x = value.X; _y = value.Y;} }

        
        public override string ToString()
        {
            return string.Format("X={0:0.00},Y={1:0.00};", _x, _y);
        }
        /// <summary>
        /// 完全相等判断，包含Z坐标
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>如果完全相等返回true，否则false</returns>
        public bool IsEqual(Coord c)
        {
            return c._x == _x && c._y == _y /*&& c.z == z*/;
        }

        /// <summary>
        /// 按照指定参数偏移
        /// </summary>
        public Coord Offset(double x1, double y1)
        {
            double xx = _x;
            double yy = _y;
            xx += x1;
            yy += y1;
            return new Coord(xx, yy);
        }

        /// <summary>
        /// 按照指定点偏移
        /// </summary>
        public Coord Offset(Coord c)
        {
            return Offset(c._x, c._y);
        }

        /// <summary>
        /// 返回坐标x,y相差的坐标
        /// </summary>
        public Coord Origin(Coord c)
        {
            double xx = _x;
            double yy = _y;
            xx -= c._x;
            yy -= c._y;
            return new Coord(xx, yy);
        }

        /// <summary>
        /// 将坐标的x,y都致负
        /// </summary>
        public Coord Negative()
        {
            return new Coord(-_x, -_y);
        }

        /// <summary>
        /// 返回按照指定比例放大的坐标
        /// </summary>
        /// <param name="zoom">比例系数</param>
        public Coord Scale(double zoom)
        {
            double xx = _x;
            double yy = _y;
            xx *= zoom;
            yy *= zoom;
            return new Coord(xx, yy);
        }

        /// <summary>
        /// 返回c1和c2的x坐标的差
        /// </summary>
        /// <param name="c1">起坐标</param>
        /// <param name="c2">止坐标</param>
        /// <returns>-1,c1.x小于c2.x;0,c1.x=c2.x;1,c1.x>c2.x</returns>
        public static int XCompare(Coord c1, Coord c2)
        {
            double delta = c1._x - c2._x;
            if (delta < 0) return -1;
            if (delta == 0) return 0;
            return 1;
        }
        
        /// <summary>
        /// 返回c1和c2的y坐标的差
        /// </summary>
        /// <param name="c1">起坐标</param>
        /// <param name="c2">止坐标</param>
        /// <returns>-1,c1.y小于c2.y;0,c1.y=c2.y;1,c1.y>c2.y</returns>
        public static int YCompare(Coord c1, Coord c2)
        {
            double delta = c1._y - c2._y;
            if (delta < 0) return -1;
            if (delta == 0) return 0;
            return 1;
        }

        /// <summary>
        /// 坐标的x,y分别差
        /// </summary>
        /// <returns>偏差坐标</returns>
        public static Coord operator -(Coord c1, Coord c2)
        {
            return new Coord(c1._x - c2._x, c1._y - c2._y);
        }

        /*
        X0 = -COS *X - SIN *Y + 46557.7811830799932563179112397188
        Y0 =  SIN *X - COS *Y - 20616.2311146461071871455578251375
        式中，X、Y为大地坐标，X0、Y0为坝轴线坐标。


        反算公式：

        X = － COS *X0 ＋ SIN *Y0 ＋ 50212.59
        Y = － SIN *X0 － COS *Y0 ＋ 8447

        SIN=0.5509670120356448784912018921605
        COS=0.83452702271916488948079272306091
         */
        /// <summary>
        /// 大地坐标转大坝坐标
        /// </summary>
        /// <returns>大坝坐标</returns>
        public Geo.Coord ToDamAxisCoord()
        {
            //double SIN = 0.5509670120356448784912018921605;
            //double COS = 0.83452702271916488948079272306091;

            Geo.Coord cod0 = new Geo.Coord();
            cod0.X = this._x;//(-COS * this.X + SIN * this.Y + 46557.7811830799932563179112397188);
            cod0.Y = this._y; //(SIN * this.X + COS * this.Y - 20616.2311146461071871455578251375);

            return cod0;
        }
        
        /// <summary>
        /// 大坝坐标转大地坐标
        /// </summary>
        /// <returns>大地坐标</returns>
        public Geo.Coord ToEarthCoord()
        {
            //double SIN = 0.5509670120356448784912018921605;
            //double COS = 0.83452702271916488948079272306091;

            Geo.Coord c = new Geo.Coord();
            c.X = this._x;//(-COS * this.X + SIN * this.Y + 50212.59);
            c.Y = this._y;//(-SIN * this.X - COS * this.Y + 8447);

            c.Y = -c.Y;
            return c;
        }
    }

    /// <summary>
    /// GPS坐标
    /// </summary>
    public struct GPSCoord
    {
        byte _Tag;
        /// <summary>
        /// 状态字节
        /// </summary>
        public byte Tag { get { return _Tag; } set { _Tag = value; } }
          
        double _V;
        /// <summary>
        /// GPS点速度
        /// </summary>
        public double V { get { return _V; } set { _V = value; } }

        DamLKK.Geo.Coord _Plane;
        /// <summary>
        /// 平面坐标
        /// </summary>
        public DamLKK.Geo.Coord Plane { get { return _Plane; } set { _Plane = value; } }

        double _Z;
        /// <summary>
        /// 高程Z坐标
        /// </summary>
        public double Z { get { return _Z; } set { _Z = value; } }

        DateTime _When;
        /// <summary>
        /// GPS点时间
        /// </summary>
        public DateTime When { get { return _When; } set { _When = value; } }

        int _LibratedStatus;
        /// <summary>
        /// 振动状态
        /// </summary>
        public int LibratedStatus
        {
            get { return _LibratedStatus; }
            set { _LibratedStatus = value; }
        }

        public GPSCoord(double xx, double yy, double zz)
        {
            _When = DateTime.MinValue;
            _Tag = 0;

            _V = 0;
            _Plane = new DamLKK.Geo.Coord(xx, yy);
            _Z = zz;
            _LibratedStatus = 0;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="c">gps点</param>
        /// <param name="t1">gps点状态字符</param>
        public GPSCoord(GPSCoord c, byte t1)
        {
            _When = DateTime.MinValue;
            this._Plane = c.Plane;
            this._Z = c._Z;
            this._V = c._V;
            this._Tag = t1;
            this._LibratedStatus = c._LibratedStatus;
        }

        public GPSCoord(double xx, double yy, double zz, double vv, byte t1)
        {
            _When = DateTime.MinValue;
            _Tag = t1;

            _Plane = new DamLKK.Geo.Coord(xx, yy);
            _Z = zz;
            _V = vv;
            _LibratedStatus = 0;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="vv">速度</param>
        /// <param name="t1">gps状态字节</param>
        /// <param name="dt">gps时间</param>
        public GPSCoord(double xx, double yy, double zz, double vv, byte t1, DateTime dt,int librated)
        {
            _When = dt;
            _Tag = t1;
            _Plane = new DamLKK.Geo.Coord(xx, yy);
            _Z = zz;
            _V = vv;
            _LibratedStatus = librated;
        }
        public GPSCoord(DamLKK.Geo.Coord c, double zz)
        {
            _When = DateTime.MinValue;
            _Tag = 0;

            _V = 0;
            _Plane = c;
            _Z = zz;
            _LibratedStatus = 0;
        }

        public override string ToString()
        {
            return string.Format("{{X={0:0.00},Y={1:0.00},Z={2:0.00},V={3:0.00}}}", _Plane.X, _Plane.Y, _Z, _V);
        }
        
    }

    /// <summary>
    /// 振动状态枚举
    /// </summary>
    public enum LibratedSatus 
    {
        NoLibrated=0,
        LowLibrated,
        HighLibrated
    }
}
