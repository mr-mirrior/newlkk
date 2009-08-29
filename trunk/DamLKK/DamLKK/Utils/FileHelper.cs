using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DamLKK.Utils
{
    public static class FileHelper
    {
        /// <读取层的控制点信息>
        /// 读取层的控制点信息
        /// </读取层的控制点信息>
        /// <param name="fullpath">完全的url</param>
        /// <param name="XNegative">是否将x致负</param>
        /// <param name="YNegative">是否将y致负</param>
        /// <returns>控制点列表</returns>
        public static List<Geo.Coord> ReadLayer(string fullpath, bool XNegative, bool YNegative)
        {
            try
            {
                List<Geo.Coord> pts = new List<Geo.Coord>();
                FileStream fs = new FileStream(fullpath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length != 0)
                    {
                        string[] s = line.Trim().Split(new char[] { ' ' });

                        Geo.Coord coord = new Geo.Coord(Convert.ToSingle(s[0].Trim()), -Convert.ToSingle(s[3].Trim()));
                        if (XNegative)
                            coord.X = -coord.X;
                        if (YNegative)
                            coord.Y = -coord.Y;

                        pts.Add(coord);
                    }
                    line = sr.ReadLine();
                }
                fs.Close();

                return pts;
            }
            catch (FileNotFoundException)
            {
                Utils.MB.Error(fullpath + " not found!");
            }
            return null;
        }


        public static List<DamLKK.Geo.Coord> ChangeToCoords(string p_strcoord)
        {
            List<DamLKK.Geo.Coord> cds = new List<DamLKK.Geo.Coord>();
            string[] coords = p_strcoord.Trim().Split(';');
            string[] xy;
            foreach (string s in coords)
            {
                if (s == string.Empty)
                    continue;
                xy = s.Split(',');
                DamLKK.Geo.Coord temp = new Geo.Coord(Convert.ToDouble(xy[0]), -Convert.ToDouble(xy[1]));
                cds.Add(temp.ToEarthCoord());
            }

            return cds;
        }

        public static List<Geo.GPSCoord> ReadTracking(string file)
            {
            List<Geo.GPSCoord> tracking = new List<Geo.GPSCoord>();
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string line = sr.ReadLine();
                double ZBase = double.PositiveInfinity;
                Random rand = new Random(0);
                while (line != null)
                {
                    if (line.Length != 0)
                    {
                        string[] s = line.Trim().Split(new char[] { ' ' });
                        if (s.Length != 4)
                        {
                            //MessageBox.Show("轨迹读取异常，请检查xyz.txt");
                            line = sr.ReadLine();
                            continue;
                        }

                        DamLKK.Geo.GPSCoord cd = new Geo.GPSCoord(Convert.ToSingle(s[0].Trim()), -Convert.ToSingle(s[3].Trim()), 1111);
                        cd.V = rand.Next(5);
                        cd.LibratedStatus = rand.Next(3);
                        tracking.Add(cd);
                        if (ZBase > tracking.Last().Z)
                            ZBase = tracking.Last().Z;
  ;
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
                fs.Close();

                for (int i = 0; i < tracking.Count; i++)
                {
                    DamLKK.Geo.GPSCoord cd = new Geo.GPSCoord(tracking[i].Plane, tracking[i].Z - ZBase);
                    cd.V = rand.Next(5);
                    cd.LibratedStatus = rand.Next(3);
                    tracking[i] = cd;
                }
                return tracking;
            }
            catch
            {
                return null;
            }
        }
    }
}

