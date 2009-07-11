using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DamLKK._Model
{
    public class Unit
    {
        int _ID;

        /// <summary>
        /// 数据库id
        /// </summary>
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        string _Name, _Vertex;

        /// <summary>
        /// 获取边界点
        /// </summary>
        public string Vertex
        {
            get { return _Vertex; }
            set { _Vertex = value;
            ChangePolg();
            }
        }

        private void ChangePolg()
        {
            List<DamLKK.Geo.Coord> coords=new List<DamLKK.Geo.Coord>();
            string[] xy=_Vertex.Trim().Split(';');

            foreach (string s in xy)
            {
                string[] coord=s.Split(',');
                DamLKK.Geo.Coord cd=new DamLKK.Geo.Coord(double.Parse(coord[0]),double.Parse(coord[1]));
                coords.Add(cd);
            }

            _Polygon = new Polygon(coords);
        }


        // 仓面顶点坐标格式化字符串，准备入库

        public string VertexString()
        {
            string s = "";
            foreach (Geo.Coord c in _Polygon.Vertex)
            {
                s += string.Format("{0:0.00},{1:0.00};", c.X, c.Y);
            }
            return s;
        }

        Polygon _Polygon;
        /// <summary>
        /// 多边形形状
        /// </summary>
        public Polygon Polygon
        {
            get { return _Polygon; }
            set { _Polygon = value;
            _Vertex = VertexString();
            }
        }

        /// <summary>
        /// 获取单元名
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        float _StartZ, _EndZ;
        /// <summary>
        /// 获取起始高程
        /// </summary>
        public float EndZ
        {
            get { return _EndZ; }
            set { _EndZ= value; }
        }
        /// <summary>
        /// 获取开始高程
        /// </summary>
        public float StartZ
        {
            get { return _StartZ; }
            set { _StartZ = value; }
        }

        List<_Model.Block> _Blocks;
        /// <summary>
        /// 包含坝段
        /// </summary>
        public List<_Model.Block> Blocks
        {
            get { return _Blocks; }
            set { _Blocks= value; }
        }

        Layer _CurrentLayer;
        /// <summary>
        /// 当前打开的层
        /// </summary>
        public Layer CurrentLayer
        {
            get { return _CurrentLayer; }
            set { _CurrentLayer = value; }
        }

        public Unit(){}
        public Unit( string p_Name, List<_Model.Block> p_Blocks, string p_Vertex, float p_StartZ, float p_EndZ)
        {
            _Name = p_Name;
            _Blocks = p_Blocks;
            _Vertex = p_Vertex;
            _StartZ = p_StartZ;
            _EndZ = p_EndZ;
        }

        /// <summary>
        /// 转向一个层
        /// </summary>
        /// <param name="view"></param>
        public void GoLayer(Views.LayerView view)
        {
            FarsiLibrary.Win.FATabStripItem item = DamLKK.Forms.Main.GetInstance()._FATabStrip.Exist(view);
            if (item == null)
                return;
            DamLKK.Forms.Main.GetInstance()._FATabStrip.SelectedItem = item;
            DamLKK._Control.LayerControl.Instance.ChangeCurrentLayer(view);
        }

        /// <summary>
        /// 关闭层
        /// </summary>
        public bool CloseWnd(Type t, Views.LayerView view)
        {
            if (t != typeof(_Control.LayerControl))
            {
                return false;
            }
            if (!Utils.MB.OKCancelQ("您确定要关闭窗口吗？" + "\n\n" + view.MyLayer.Name))
                return false;
            DamLKK.Forms.Main.GetInstance()._FATabStrip.CloseWindow(view);
            return true;
        }

        /// <更新此单元的高程列表>
        /// 更新此单元的高程列表
        /// </更新此单元的高程列表>
        public void UpdateCbTags(int p_modeindex)
         {
             List<string> tags = new List<string>();
             List<double> decktags = new List<double>();
             if (p_modeindex==1)
             {
                 decktags = DB.UnitDAO.GetInstance().GetTagsInUnit(this.ID,false);

                 if (decktags == null)
                     return;

                 for(int i=0;i<decktags.Count;i++)
                 {
                     if (i >= 1 && decktags[i] == decktags[i - 1])
                         continue;

                     if (decktags[i] < 100)
                         tags.Add("斜-" + decktags[i].ToString("0"));
                     else
                         tags.Add(decktags[i].ToString());
                 }
             }
             else   //每0.1米一层
             {
                 for (float i = Convert.ToSingle(this.StartZ.ToString("0.0")); i < Convert.ToSingle(this.EndZ.ToString("0.0")); i += 0.1f)
                 {
                     tags.Add(i.ToString("0.0"));
                 }
             }

             DamLKK.Forms.ToolsWindow.GetInstance().cbUnitTag.Items.Clear();
             DamLKK.Forms.ToolsWindow.GetInstance().cbUnitTag.Text =string.Empty;
             if (tags.Count == 0)
                 return;

             DamLKK.Forms.ToolsWindow.GetInstance().cbUnitTag.Items.AddRange(tags.ToArray());
             DamLKK.Forms.ToolsWindow.GetInstance().cbUnitTag.SelectedIndex = 0;
         }
    }
}
