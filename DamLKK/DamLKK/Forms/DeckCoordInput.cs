using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DamLKK.Forms
{
    public partial class DeckCoordInput : Form
    {
        static Forms.DeckCoordInput _FrmCoordInput;
        private DeckCoordInput()
        {
            InitializeComponent();
        }

        public static DeckCoordInput GetInstance()
        {
            if (_FrmCoordInput == null)
                _FrmCoordInput = new DeckCoordInput();
           
            return _FrmCoordInput;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _FrmCoordInput = null;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tbCoords.Text.TrimEnd().Equals(string.Empty))
            {
                Utils.MB.Warning("输入坐标不能为空！");
                return;
            }
        
            List<DamLKK.Geo.Coord> deckcoords = new List<DamLKK.Geo.Coord>();
            string[] coords = tbCoords.Text.Split(';');
            for (int i = 0; i < coords.Length;i++ )
            {
                string coord = coords[i].Trim();
                string[] cdxy=coord.Split(',');
                if (cdxy.Length < 2)
                {
                    Utils.MB.Warning("您输入的坐标不正确，请检查后重新输入！");
                    return;
                }
                DamLKK.Geo.Coord cd = new DamLKK.Geo.Coord(Convert.ToDouble(cdxy[0]),-Convert.ToDouble(cdxy[1]));
                //if (cd.XF>700||cd.YF>500||cd.YF<-500)
                //{
                //    Utils.MB.Warning("输入坐标超越坝轴坐标界限，请检查后重新输入！");
                //}
                deckcoords.Add(cd.ToEarthCoord());
            }
            deckcoords.Add(deckcoords.First());

            Forms.ToolsWindow.GetInstance().CurrentLayer._DeckSelectPolygon = deckcoords;
            Forms.ToolsWindow.GetInstance().CurrentLayer.IsDeckInput = true;
            Forms.ToolsWindow.GetInstance().CurrentLayer.MyLayer.IsDeckInput = true;
            this.Close();
            Forms.ToolsWindow.GetInstance().CurrentLayer.IsPolySelecting = false;
        }

        private void DeckCoordInput_Load(object sender, EventArgs e)
        {
            tbCoords.Focus();
        }

        private void tbCoords_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!(Char.IsNumber(e.KeyChar) ||
            //    e.KeyChar == '\b' ||
            //    e.KeyChar == Convert.ToChar(".") ||
            //    e.KeyChar == Convert.ToChar(",") ||
            //    e.KeyChar == Convert.ToChar(";")||
            //    e.KeyChar == Convert.ToChar("-")))
            //{
            //    e.Handled = true;
            //}
        }

        private void DeckCoordInput_FormClosed(object sender, FormClosedEventArgs e)
        {
            _FrmCoordInput = null;
        }
    }
}
