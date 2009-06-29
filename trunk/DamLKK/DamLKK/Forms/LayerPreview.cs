using System;
using System.Drawing;
using System.Windows.Forms;

namespace DamLKK.Forms
{
    public partial class LayerPreview : Form
    {
        public event EventHandler OnHide;
        Views.LayerView _Preview = new Views.LayerView();
       
        public LayerPreview()
        {
            _Preview.IsPreview = true;
            InitializeComponent();
            OnHide += Dummy;
        }
        private void Dummy(object sender, EventArgs e) {}
      
        //public void OpenLayer(_Model.UnitDirectory p_Unit)
        //{
        //    if (p == null || e == null)
        //        return;
        //    if( p.Unit == null )
        //    {
        //        Utils.MB.Warning("数据库中未找到该分区信息："+p.Name);
        //        return;
        //    }
        //    preview.OpenLayer(p, e);
        //    preview.UpdateGraphics();

        //    this.Text = "预览 " + preview.ToString();
        //}


        private void HideThis()
        {
            Hide();
            OnHide.Invoke(this, null);
        }
        private void LayerPreview_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Escape )
            {
                HideThis();
                e.SuppressKeyPress = true;
                return;
            }
            /////////////////////////////////////feiying 主窗口捕捉按键
            //Main.MainWindow.ProcessKeys(this, e);
        }

        private void LayerPreview_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            HideThis();
        }

        private void LayerPreview_Load(object sender, EventArgs e)
        {
            pl.Controls.Add(_Preview);
            _Preview.Dock = DockStyle.Fill;
            _Preview.Padding = new Padding(0, 0, 0, 0);
            _Preview.Margin = new Padding(0, 0, 0, 0);
            _Preview.BackColor = Color.White;
            _Preview.Visible = true;
            //_Preview.AlwaysFitScreen = true;
        }

        private void LayerPreview_Resize(object sender, EventArgs e)
        {
            //_Preview.UpdateGraphics();
        }
    }
}
