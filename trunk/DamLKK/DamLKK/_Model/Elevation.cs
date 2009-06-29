using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamLKK._Model
{
    public class Elevation
    {
        double _Height;
        /// <summary>
        /// 高度
        /// </summary>
        public double Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        public Elevation(double p_Height)
        {
            _Height = p_Height;
        }

        public Elevation(){}

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Elevation))
                return false;
            Elevation e = (Elevation)obj;
            return _Height == e.Height;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    //////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////
    public class ElevationFile
    {
        public ElevationFile() { }
        public ElevationFile(string full) { name = full; }
        string name = "";
        public string FullName { get { return name; } set { name = value; } }
        public string FileName { get { return System.IO.Path.GetFileName(name); } }
        public Elevation Elevation { get { return new Elevation(HeightF); } }
        public string Height
        {
            get
            {
                string height = System.IO.Path.GetFileNameWithoutExtension(name);
                height = height.Replace('_', '.');
                return height;
            }
        }
        public double HeightF
        {
            get
            {
                string height = Height;
                double x = 0;
                if (double.TryParse(height, out x))
                    return x;
                return double.NaN;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ElevationFile))
                return false;
            ElevationFile ef = (ElevationFile)obj;
            return ef.Elevation.Height == this.Elevation.Height;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool IsEqual(ElevationFile ef1, ElevationFile ef2)
        {
            return ef2.Elevation.Height == ef1.Elevation.Height;
        }
        public static int Greater(ElevationFile ef1, ElevationFile ef2)
        {
            if (ef1.Elevation.Height < ef2.Elevation.Height)
                return -1;
            if (ef1.Elevation.Height == ef2.Elevation.Height)
                return 0;
            return 1;
        }
    }
    public class ElevationFiles
    {
        public ElevationFiles() { }

        List<ElevationFile> files = new List<ElevationFile>();
        public List<ElevationFile> Files { get { return files; } set { files = value; } }

        public bool Search(string dir)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(dir);
            if (!di.Exists)
                return false;
            System.IO.FileInfo[] fis = di.GetFiles("*.txt");
            if (fis == null)
                return false;
            if (fis.Length == 0)
                return false;
            foreach (System.IO.FileInfo fi in fis)
            {
                files.Add(new ElevationFile(fi.FullName));
            }
            return true;
        }
    }
}
