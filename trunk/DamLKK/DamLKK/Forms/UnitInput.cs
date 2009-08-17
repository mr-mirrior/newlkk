using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DamLKK.Geo;

namespace DamLKK.Forms
{
    public partial class UnitInput : Form
    {
        public UnitInput()
        {
            InitializeComponent();
        }

        private void cbStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbEnd.SelectedIndex < cbStart.SelectedIndex)
            {
                cbEnd.SelectedIndex = cbStart.SelectedIndex;
            }
        }

        private void cbEnd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStart.SelectedIndex > cbEnd.SelectedIndex)
            {
                cbStart.SelectedIndex = cbEnd.SelectedIndex;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            float StartZ,EndZ;

            if(tbName.Text==string.Empty)
            {
                Utils.MB.Warning("仓面名称不能为空!");
                return;
            }


            if (cbStart.SelectedIndex==-1 || cbEnd.SelectedIndex==-1)
            {
                Utils.MB.Warning("选择坝段不能为空!");
                return;
            }

            if (!float.TryParse(tbStartZ.Text, out StartZ) && !float.TryParse(tbEndZ.Text, out EndZ))
            {
                Utils.MB.Warning("高程必须全是数字!");
                return;
            }

            if (!float.TryParse(tbStartZ.Text, out StartZ) && !float.TryParse(tbEndZ.Text, out EndZ))
            {
                Utils.MB.Warning("高程必须全是数字!");
                return;
            }

              List<int> Blocks=new List<int>();
            for(int i=cbStart.SelectedIndex;i<=cbEnd.SelectedIndex;i++)
                Blocks.Add(i);

            //////////////////////////////////控制输入坐标必须准确必须在指定坝段坐标范围之内/////////////////////////
            string coordtxt = string.Empty;
            List<DamLKK.Geo.Coord> coords;
            if (tbCoords.Text==string.Empty)
            {
                Utils.MB.Warning("坐标输入不能为空！");
                return;
            }
            else
            {
                try
                {
                   
                   coords= Utils.FileHelper.ChangeToCoords(tbCoords.Text);
                    if (coords.Count<3)
                    {
                        Utils.MB.Error("您输入的坐标不足4个，请重新输入！");
                        return;
                    }
                    coords.Add(coords.First());

                    //DamLKK.Geo.BorderShapeII b2=null;
                    foreach (DamLKK.Geo.Coord c in coords)
                    {
                        coordtxt += c.XF.ToString() + "," + c.YF.ToString()+";";
                    //    bool at = false;

                    //    if (Blocks.First() > 0)
                    //        b2 = new DamLKK.Geo.BorderShapeII(DamLKK._Model.Dam.GetInstance().Blocks[Blocks.First() - 1].Polygon.Vertex);
                    //    if (Blocks.Last() < DamLKK._Model.Dam.GetInstance().Blocks.Count - 1)
                    //        b2 = new DamLKK.Geo.BorderShapeII(DamLKK._Model.Dam.GetInstance().Blocks[Blocks.First() + 1].Polygon.Vertex);
                    //    if (b2 != null && b2.IsInsideIII(c))
                    //        at = true;

                    //    //341524.0398   2936662.6486
                    //    //342326.4075   2936734.8466
                    //    //342332.5773   2936664.6449
                    //    //341951.2937   2936443.1657
                    //    //341529.9343   2936582.5707
                    //    //341524.0398   2936662.6486
                    //    //List<DamLKK.Geo.Coord> ver = new List<DamLKK.Geo.Coord>();
                    //    //ver.Add(new Coord(341524.0398,2936662.6486));
                    //    //ver.Add(new Coord(342326.4075,2936734.8466));
                    //    //ver.Add(new Coord(342332.5773,2936664.6449));
                    //    //ver.Add(new Coord(341951.2937,2936443.1657));
                    //    //ver.Add(new Coord(341529.9343,2936582.5707));
                    //    //ver.Add(new Coord(341524.0398, 2936662.6486));
                    //    foreach (int i in Blocks)
                    //    {
                    //        b2 = new DamLKK.Geo.BorderShapeII(DamLKK._Model.Dam.GetInstance().Blocks[i].Polygon.Vertex);//DamLKK._Model.Dam.GetInstance().Blocks[i].Polygon.Vertex
                    //        if (b2.IsInsideIII(c))
                    //        {
                    //            at = true;
                    //            break;
                    //        }
                    //    }
                    //    if(!at)
                    //    {
                    //        Utils.MB.Warning(c.ToString() + "不再所选坝段范围之内,请检查重新输入!");
                    //        return;
                    //    }
                    }
                }
                catch
                {
                    Utils.MB.Error("输入的坐标有错，请检查！");
                    return;
                }
            }

            coordtxt = coordtxt.Substring(0,coordtxt.Length - 1);
            
          

            if (DamLKK._Model.Dam.GetInstance().NewOneUnit(Blocks,tbName.Text, coordtxt, float.Parse(tbStartZ.Text), float.Parse(tbEndZ.Text)))
            {
                Utils.MB.OK("添加仓面成功!");
                Forms.ToolsWindow.GetInstance().cbWorkUnit.Items.Add(tbName.Text);
                this.Close();
            }
            else
            {
                Utils.MB.Warning("添加仓面失败!");
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UnitInput_Load(object sender, EventArgs e)
        {
            cbStart.SelectedIndex = 0;
            cbEnd.SelectedIndex = 0;
        }

        private void UnitInput_FormClosed(object sender, FormClosedEventArgs e)
        {
            Forms.ToolsWindow.GetInstance().FrmUnitIn = null;  //制空可以再打开单元输入窗口
        }
    }
}
