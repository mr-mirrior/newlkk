using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DamLKK._Model
{
    /// <summary>
    /// 坝段
    /// </summary>
    public class Block
    {
        private int _BlockID;

        public int BlockID
        {
            get { return _BlockID; }
            set { _BlockID = value; }
        }

        private string _BlockName;

        public string BlockName
        {
            get { return _BlockName; }
            set { _BlockName = value; }
        }

        Polygon _Polygon;
        /// <summary>
        /// 真是大地坐标
        /// </summary>
        public Polygon Polygon
        {
            get { return _Polygon; }
            set { _Polygon = value; }
        }

        public Block() { }
        public Block(int id, string name) { BlockID = id;  BlockName = name; }

        public override string ToString()
        {
            return string.Format("ID={0}, Code={2}, Name={1}", BlockID, BlockName);
        }

        public static string GetName(int p_BlockID)
        {
            return p_BlockID.ToString("0号坝段");
        }
    }
}
