using System;
using System.Collections.Generic;
using System.Text;

/*
 * John, 2008-09-23
 * 使用方法：

 * GPSCoord gpsTrans = new GPSCoord();
 * gpsTrans.InitCoord();
 * BLH ellipseCoord = new BLH(...);
 * XYZ planeCoord = gpsTrans.WGS84To54(ellipseCoord);
 * 
 */
namespace DamLKK.Geo
{
    public struct BLH
    {
        public BLH(double b1, double l1, double h1) { b = b1; l = l1; h = h1; }
        public double b;
        public double l;
        public double h;
    }
    public struct XYZ
    {
        public double x;
        public double y;
        public double z;
    }

    public struct ElliParam
    {
        public double a;
        public double b;
        public double e;
        public double e2;
        public double sube;
        public double c;
    }
    public struct GPS
    {
        public double LaD;
        public double LaM;
        public double LaS;
        public double LoD;
        public double LoM;
        public double LoS;
        public double H;
    }
    public struct CoordParam
    {
        public double dx;
        public double dy;
        public double dz;
        public double ox;
        public double oy;
        public double oz;
        public double k;
    }

    public struct PARAM_54
    {
        public double OffsetX;
        public double OffsetY;
        public double CenterL;
    }
    public class Coord84To54
    {
        static Coord84To54 i = new Coord84To54();
        //public static Coord84To54 Instance { get { return i; } }
        public static XYZ Convert(BLH pt) { return i.WGS84To54(pt); }

        public Coord84To54() { InitCoord(); }
        //double zwx = 0;
        //double E54Height;
        CoordParam gCordParam = new CoordParam();
        ElliParam gEP84 = new ElliParam();
        ElliParam gEP54 = new ElliParam();
        //double ParamN = 0;
        PARAM_54 param54 = new PARAM_54();

        private void SetParam(double dx, double dy, double dz, double ex, double ey, double ez, double s)
        {
            gCordParam.dx = dx;
            gCordParam.dy = dy;
            gCordParam.dz = dz;
            gCordParam.ox = ex / 3600;
            gCordParam.oy = ey / 3600;
            gCordParam.oz = ez / 3600;
            gCordParam.k = s;
        }
        private void SetOffsetParam(double cl, double ex, double ey)
        {
            param54.CenterL = cl;
            param54.OffsetX = ex;
            param54.OffsetY = ey;
        }
        private void SetEP54(double a, double b)
        {
            InitEP(ref gEP54, a, b);
        }
        private void SetEP84(double a, double b)
        {
            InitEP(ref gEP84, a, b);
        }
        public void InitCoord()
        {

            /*
             gCordParam.dx =  -207.804;
             gCordParam.dy = 1852.735;
             gCordParam.dz =  125.612;
             gCordParam.ox =  10.4137 / 3600;
             gCordParam.oy =  5.5769 / 3600;
             gCordParam.oz =   4.7975/3600;
             gCordParam.k=0.16070057212*0.001;

              
 * */
            gCordParam.dx = -521.6289;
            gCordParam.dy = 931.4806;
            gCordParam.dz = 557.493;
            gCordParam.ox = 05.412 / 3600;
            gCordParam.oy = 08.837 / 3600;
            gCordParam.oz = 08.766 / 3600;
            gCordParam.k = -0.0001572451;

            // InitEP(ref gEP54,6378140,6356863.01877);
            InitEP(ref gEP54, 6378245, 6356863.01877);
            InitEP(ref gEP84, 6378137, 6356752.3142);
            param54.CenterL = 99;
        }
        private double R2A(double r)
        {
            return r * 180 / Math.PI;
        }
        private double A2R(double a)
        {
            return a * Math.PI / 180;
        }
        private void InitEP(ref ElliParam ep, double a, double b)
        {
            double sinb = Math.Sin(b);
            ep.a = a;
            ep.b = b;
            ep.e = Math.Sqrt((a * a - b * b) / (a * a));
            ep.e2 = ep.e * ep.e;// (a * a - b * b) / (a * a);
            ep.sube = (a * a - b * b) / (b * b);
            ep.c = a * a / b;
        }
        private void InitEPByAF(ref ElliParam ep, double a, double f)
        {

            ep.a = a;
            ep.b = (a * f - a) / f;
            ep.e = Math.Sqrt((a * a - ep.b * ep.b) / (a * a));
            ep.e2 = ep.e * ep.e;// (a * a - b * b) / (a * a);
            ep.sube = (a * a - ep.b * ep.b) / (ep.b * ep.b);
            ep.c = a * a / ep.b;
        }

        private double GetN(ElliParam ep, double b)
        {
            double sinb = Math.Sin(b);
            return ep.a / Math.Sqrt(1 - ep.e2 * sinb * sinb);
        }


        private XYZ BLH2XYZ(ElliParam ep, BLH s)
        {
            XYZ t = new XYZ();
            double N = GetN(ep, s.b);
            t.x = (N + s.h) * Math.Cos(s.b) * Math.Cos(s.l);
            t.y = (N + s.h) * Math.Cos(s.b) * Math.Sin(s.l);
            t.z = (N * (1 - ep.e2) + s.h) * Math.Sin(s.b);
            return t;
        }

        private BLH XYZ2BLH(ElliParam ep, XYZ s)
        {
            int i;
            double N;
            double tanv;
            BLH t = new BLH();
            tanv = s.y / s.x;
            t.l = Math.Atan(tanv);
            N = ep.a;
            t.h = Math.Sqrt(s.x * s.x + s.y * s.y + s.z * s.z) - Math.Sqrt(ep.a * ep.b);

            tanv = s.z / Math.Sqrt(s.x * s.x + s.y * s.y) / (1 - (ep.e2 * N / (N + t.h)));
            if (tanv < 0)
                t.b = Math.PI - Math.Atan(tanv);
            else
                t.b = Math.Atan(tanv);

            for (i = 0; i < 3; i++)
            {
                N = ep.a / Math.Sqrt(1 - ep.e2 * Math.Sin(t.b) * Math.Sin(t.b));

                t.h = Math.Sqrt(s.x * s.x + s.y * s.y) / Math.Cos(t.b) - N;

                tanv = s.z / Math.Sqrt(s.x * s.x + s.y * s.y) / (1 - ep.e2 * N / (N + t.h));
                if (tanv < 0)
                    t.b = Math.PI - Math.Atan(tanv);
                else
                    t.b = Math.Atan(tanv);
            }
            return t;
        }

        private XYZ XYZ2XYZ(XYZ O, CoordParam Param)
        {
            XYZ N = new XYZ();
            N.x = Param.dx + (1 + Param.k) * O.x + Param.oz * O.y - Param.oy * O.z;
            N.y = Param.dy + (1 + Param.k) * O.y - Param.oz * O.x + Param.ox * O.z;
            N.z = Param.dz + (1 + Param.k) * O.z + Param.oy * O.x - Param.ox * O.y;
            return N;
        }
        private double GetX(ElliParam ep, BLH s)
        {
            double d;
            double r0;
            double r2;
            double r4;
            double r6;
            double r8;

            double cosb;


            cosb = Math.Cos(s.b);

            r0 = 1 - 3 * (ep.sube) / 4;
            r0 = r0 + 45 * (Math.Pow(ep.sube, 2)) / 64;
            r0 = r0 - 175 * (Math.Pow(ep.sube, 3)) / 256;
            r0 = r0 + 11025 * (Math.Pow(ep.sube, 4)) / 16384;

            r2 = r0 - 1;

            r4 = 15 * (Math.Pow(ep.sube, 2)) / 32 - 175 * (Math.Pow(ep.sube, 3)) / 384 + 3675 * (Math.Pow(ep.sube, 4)) / 8192;

            r6 = -35 * (Math.Pow(ep.sube, 3)) / 96 + 735 * (Math.Pow(ep.sube, 4)) / 2048;

            r8 = 315 * (Math.Pow(ep.sube, 4)) / 1024;

            d = r2 * cosb + r4 * Math.Pow(cosb, 3) + r6 * Math.Pow(cosb, 5) + r8 * Math.Pow(cosb, 7);
            d = d * Math.Sin(s.b) + r0 * s.b;
            d = d * ep.c;
            return d;

        }

        private XYZ BLHto54(ElliParam ep, BLH s)
        {
            XYZ Target = new XYZ();
            double row;
            double x;
            double t;
            double l;
            double N;
            x = GetX(ep, s);
            N = GetN(ep, s.b);
            t = Math.Tan(s.b);
            row = ep.sube * Math.Pow(Math.Cos(s.b), 2);

            l = s.l;

            Target.x = x
                        + Math.Pow(l, 2) * N * Math.Sin(s.b) * Math.Cos(s.b) / 2
                        + Math.Pow(l, 4) * N * Math.Sin(s.b) * Math.Pow(Math.Cos(s.b), 3) * (5 - Math.Pow(t, 2) + 9 * row + 4 * Math.Pow(row, 2)) / 24
                        + Math.Pow(l, 6) * N * Math.Sin(s.b) * Math.Pow(Math.Cos(s.b), 5) * (61 - 58 * Math.Pow(t, 2) + Math.Pow(t, 4)) / 720;

            Target.y = l * N * Math.Cos(s.b)
                + Math.Pow(l, 3) * N * Math.Pow(Math.Cos(s.b), 3) * (1 - Math.Pow(t, 2) + row) / 6
                + Math.Pow(l, 5) * N * Math.Pow(Math.Cos(s.b), 5) * (5 - 18 * Math.Pow(t, 2) + Math.Pow(t, 4) + 14 * row - 58 * row * Math.Pow(t, 2)) / 120;
            return Target;

        }

        public XYZ WGS84To54(BLH s)
        {
            double d;
            s.l -= 99;
            s.l = A2R(s.l);
            s.b = A2R(s.b);
            XYZ xyzBuff = BLH2XYZ(gEP84, s);
            XYZ t;// = new XYZ();
            t = xyzBuff;
            
            BLH blhBuff = XYZ2BLH(gEP54, t);

            d = blhBuff.h;
            t = BLHto54(gEP54, blhBuff);
            t.y +=(503000-20.5-600000);
            t.x += (3000 + 12.6 - 2500000);
            t.z = d + 144.4;
            return t;
        }
    }
}

