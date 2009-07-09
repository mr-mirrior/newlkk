using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DamLKK._Model
{
    /// <大坝模型>
    /// 大坝模型
    /// </大坝模型>
    public class Dam
    {
        static Dam _MyInstance=null;

        /// <获取大坝静态实例>
        /// 获取大坝静态实例
        /// </获取大坝静态实例>
        public static Dam GetInstance()
        {
            if (_MyInstance == null)
            {
                _MyInstance = new Dam();
            }
            return _MyInstance;
        }

        Unit _CurrentUnit;
        /// <summary>
        /// 当前打开的单元
        /// </summary>
        public Unit CurrentUnit
        {
            get { return _CurrentUnit; }
            set { _CurrentUnit = value; }
        }

        List<Block> _Blocks;
        List<Elevation> _Elevations;
        List<List<Geo.Coord>> _MiniData;
        /// <summary>
        /// 文本坐标
        /// </summary>
        public List<List<Geo.Coord>> MiniData
        {
            get { return _MiniData; }
            set { _MiniData = value; }
        }
        List<System.Drawing.Color> _MiniColor;

        /// <summary>
        /// 坝段颜色
        /// </summary>
        public List<System.Drawing.Color> MiniColor
        {
            get { return _MiniColor; }
            set { _MiniColor = value; }
        }

        Forms.EagleEye _FrmEagleEye;
        /// <鹰眼试图窗口>
        /// 鹰眼试图窗口
        /// </鹰眼试图窗口>
        public Forms.EagleEye FrmEagleEye
        {
            get { return _FrmEagleEye; }
            set { _FrmEagleEye = value; }
        }

        /// <summary>
        /// 所有坝段
        /// </summary>
        public List<Block> Blocks
        {
            get { return _Blocks; }
            set { _Blocks = value; }
        }

        private Dam()
        {
            _MiniData = new List<List<DamLKK.Geo.Coord>>();
            #region -初始化鸟瞰颜色和鸟瞰迷你控制点-
            _MiniColor = new List<Color>{
                   Color.AliceBlue,
                   Color.AntiqueWhite,
                   Color.Beige,
                   Color.DarkKhaki,
                   Color.DarkSeaGreen,
                   Color.Gainsboro,
                   Color.Khaki,
                   Color.Lavender,
                   Color.LemonChiffon,
                   Color.LightBlue,
                   Color.LightCyan,
                   Color.LightGreen,
                   Color.LightGray,
                   Color.LightPink,
                   Color.Coral,
                   Color.LightSteelBlue,
                   Color.Linen,
                   Color.MistyRose,
                   Color.Moccasin,
                   Color.OldLace,
                   Color.PaleGoldenrod,
                   Color.PaleTurquoise,
                   Color.RosyBrown,
                   Color.Silver,
                   Color.SkyBlue,
                   Color.SlateGray,
                   Color.PaleGreen,
                   Color.SteelBlue,
                   Color.Tan,
                   Color.Thistle
            };


            //读取鸟瞰的所有点信息
            for (int i = 1; i < 31; i++)
            {
                _MiniData.Add(Utils.FileHelper.ReadLayer(Config._MiniData + i.ToString() + "号坝段.txt", true,true));
            }
            #endregion

            _Blocks =DB.BlockDAO.GetInstance().GetBlocks();
            foreach (Block b in Blocks)
            {
                b.Polygon = new Polygon(Utils.FileHelper.ReadLayer(Config.BLOCK_VERTEX + "\\" + b.BlockID.ToString()+"号坝段" + "\\" + b.BlockID.ToString() + "号坝段.txt", false,true));
            }
            _Elevations = new List<Elevation>();
            
        }


        /// <创建一个新单元>
        /// 创建一个新单元
        /// </创建一个新单元>
        public bool NewOneUnit(List<int> p_BlockIdxs,string p_Name,string p_Vertex,float p_StartZ,float p_EndZ)
        {
            List<Block> blocks=new List<Block>();

            foreach (int idx in p_BlockIdxs)
            {
                blocks.Add(_Blocks[idx]);
            }

            Unit unit = new Unit(p_Name,blocks,p_Vertex,p_StartZ,p_EndZ);

            return DB.UnitDAO.GetInstance().AddUnit(unit); 
        }

        /// <显示鹰眼>
        /// 显示鹰眼
        /// </显示鹰眼>
        public void ShowMini(System.Windows.Forms.Form p_Main)
        {
            List<Unit> Units=DB.UnitDAO.GetInstance().GetWorkingUnits();
            
            _FrmEagleEye = new DamLKK.Forms.EagleEye(_MiniData,_MiniColor,Units);
            _FrmEagleEye.Show(p_Main);
        }

        /// <更新工具条上units列表>
        /// 更新工具条上units列表 1为查询模式 0为设计模式
        /// </更新工具条上units列表>
        public void UpdateCbUnits(int p_modeindex)
        {
            List<string> UnitNames=new List<string>();
            List<Unit> Units;

            Units = DB.UnitDAO.GetInstance().GetUnits();

            if (Units== null)
                return;

            foreach (Unit u in Units)
            {
                UnitNames.Add(u.Name);
            }
           
            DamLKK.Forms.ToolsWindow.GetInstance().cbWorkUnit.Items.Clear();
            DamLKK.Forms.ToolsWindow.GetInstance().cbWorkUnit.Items.AddRange(UnitNames.ToArray());
            if (DamLKK.Forms.ToolsWindow.GetInstance().cbWorkUnit.Items.Count!=0)
            {
                DamLKK.Forms.ToolsWindow.GetInstance().cbWorkUnit.SelectedIndex = 0;
                Units.First().UpdateCbTags(p_modeindex);
            }
        }

        public Unit WorkUnitFromName(int blockid, float p)
        {
            foreach (Unit u in _FrmEagleEye.WorkUntis)
            {
                bool HasBlock=false;
                foreach (Block b in u.Blocks)
                {
                    if (blockid == b.BlockID)
                    {
                        HasBlock = true;
                        break;
                    }
                }

                if (HasBlock && (p <u.StartZ+5 && p>u.StartZ-5))
                {
                    return u;
                }

            }
            return null;
        }
    }
}
