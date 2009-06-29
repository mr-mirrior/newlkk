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
    public partial class Main : Form
    {
        private Main()
        {
            InitializeComponent();
        }

        static Main _FrmMain=null;

        public static Main MyInstance()
        {
            if (_FrmMain==null)
            {
                _FrmMain = new Main();
            }
            return _FrmMain;
        }

        #region 最高优先级键盘响应
        public bool PreFilterMessage(ref Message msg)
        {
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_LBUTTONUP = 0x0202;
            const int WM_LBUTTONDBLCLK = 0x0203;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_RBUTTONUP = 0x0205;
            const int WM_RBUTTONDBLCLK = 0x0206;
            const int WM_MBUTTONDOWN = 0x0207;
            const int WM_MBUTTONUP = 0x0208;
            const int WM_MBUTTONDBLCLK = 0x0209;
            const int WM_KEYDOWN = 0x0100;
            const int WM_KEYUP = 0x0101;


            switch (msg.Msg)
            {
                case WM_LBUTTONDOWN:
                case WM_LBUTTONUP:
                case WM_LBUTTONDBLCLK:
                case WM_RBUTTONDOWN:
                case WM_RBUTTONUP:
                case WM_RBUTTONDBLCLK:
                case WM_MBUTTONDOWN:
                case WM_MBUTTONUP:
                case WM_MBUTTONDBLCLK:
                case WM_KEYDOWN:
                case WM_KEYUP:
                    break;
            }
            return false;
        }
        #endregion 

        ToolsWindow toolsWnd = ToolsWindow.GetInstance();

        private void Main_Load(object sender, EventArgs e)
        {
            ToggleFullScreen();
            PlaceToolsWindow();

            DamLKK._Model.Dam.GetInstance().ShowMini(this);   //显示鹰眼图
        }

        /// <设置全屏>
        /// 设置全屏
        /// </设置全屏>
        private void ToggleFullScreen()
        {
            if (IsFullScreen)
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            }
        }

        /// <判断是否是无边框样式>
        /// 判断是否是无边框样式
        /// </判断是否是无边框样式>
        private bool IsFullScreen
        {
            get
            {
                return this.FormBorderStyle == FormBorderStyle.None;
            }
        }

        /// <summary>
        /// 初始化toolswindows工具条
        /// </summary>
        private void PlaceToolsWindow()
        {
            toolsWnd.Show(this);
            int x = this.DesktopBounds.Right - toolsWnd.Width - 20;
            int y = this.DesktopBounds.Top + 40;
            toolsWnd.Location = new Point(x, y);
        }

        private void tab_TabCloseButtonClicked(object sender, EventArgs e)
        {
#if !DEBUG
            if(Utils.MB.OKCancelQ("您确定要退出系统吗？"))
#endif
            this.Close();
        }

        /// <双击tab窗口上的Item时发生，如果有item执行关闭>
        /// 双击tab窗口上的Item时发生，如果有item执行关闭
        /// </双击tab窗口上的Item时发生，如果有item执行关闭>
        private void tab_TabStripItemDBClicked(object sender, EventArgs e)
        {
            if (sender != null)
            {
                FarsiLibrary.Win.FATabStripItem item = (FarsiLibrary.Win.FATabStripItem)sender;
                _Control.LayerControl.Instance.CloseLayer((Views.LayerView)item.Tag);
            }
            else
            {
                ToggleFullScreen();
            }
        }

        private void Main_Leave(object sender, EventArgs e)
        {
            if (IsFullScreen)
                ToggleFullScreen();
        }

        /// <summary>
        /// 处理键盘消息，接受来自其他窗口的键盘消息
        /// </summary>
        /// <param name="e">按下的键</param>
        /// <returns>如果接受按键处理返回true（"抢夺"），否则返回false（不"抢夺"）</returns>
        public bool ProcessKeys(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            switch (e.KeyCode)
            {
                //case Keys.P:
                //    toolsWnd.FitScreen();
                //    break;
                //case Keys.H:
                //    toolsWnd.ResetRotate();
                //    break;
                //case Keys.M:
                //    toolsWnd.Operation = Views.Operations.SCROLL_FREE;
                //    break;
                //case Keys.X:
                //    toolsWnd.Operation = Views.Operations.ROTATE;
                //    break;
                //case Keys.F:
                //    toolsWnd.Operation = Views.Operations.ZOOM;
                //    break;
                //case Keys.W:
                //    if (e.Control)
                //    {
                //        if (tab.SelectedItem != null)
                //            DMControl.LayerControl.Instance.CloseLayer((Views.LayerView)tab.SelectedItem.Tag);
                //        return true;
                //    }
                //    break;
                case Keys.Escape:
                    System.Diagnostics.Debug.Print(sender.ToString());
                    Exit();
                    return true;
                case Keys.F11:
                    ToggleFullScreen();
                    return true;
                //case Keys.G:
                //    ToggleToolbar();
                //    return true;
                case Keys.Tab:
                    if (e.Control)
                    {
                        if (e.Shift)
                        {
                            _FATabStrip.PrevItem();  // CTRL+SHIFT+TAB
                            return true;
                        }
                        else
                        {
                            _FATabStrip.NextItem(); // CTRL+TAB
                            return true;
                        }
                    }
                    else
                    {
                    }
                    break;
                case Keys.F1:
                    toolsWnd.Visible = !toolsWnd.Visible;
                    DamLKK._Model.Dam.GetInstance().FrmEagleEye.Visible = toolsWnd.Visible;
                    if (toolsWnd.CurrentLayer != null)
                        toolsWnd.CurrentLayer.ShowLandscape(toolsWnd.Visible);
                    System.Diagnostics.Debug.Print("F1 pressed");
                    return true;
                //case Keys.F12:
                //    //Warning dlg = new Warning();
                //    //dlg.Show(this);
                //    DMControl.GPSServer.test();
                //    return true;
                default:
                    break;
            }
            e.SuppressKeyPress = false;
            return false;
        }

        private void Exit()
        {
#if !DEBUG
            if(Utils.MB.OKCancelQ("您确定要退出系统吗？"))
#endif
            this.Close();
        }

        public Views.LayerView OpenLayer(_Model.Unit p, DamLKK._Model.Elevation e)
        {
            FarsiLibrary.Win.FATabStripItem item;

            Views.LayerView lv = new Views.LayerView();
            item = new FarsiLibrary.Win.FATabStripItem("New Layer", lv);
            _FATabStrip.AddTab(item);
            lv.Dock = DockStyle.Fill;
            lv.Padding = new Padding(0, 0, 0, 0);
            lv.Margin = new Padding(0, 0, 0, 0);
            lv.BackColor = Color.White;

            if (!lv.OpenLayer(p, e))
            {
                _FATabStrip.RemoveTab(item);
                Utils.MB.Warning("打开层失败！");
                return null;
            }
            lv.Visible = true;
            item.Title = lv.MyLayer.Name;
            item.Tag = lv;
            lv.Init();

            _FATabStrip.SelectedItem = item;

            return lv;
        }

        private void _FATabStrip_TabStripItemSelectionChanged(FarsiLibrary.Win.TabStripItemChangedEventArgs e)
        {
            _Control.LayerControl.Instance.ChangeCurrentLayer((Views.LayerView)e.Item.Tag);
        }

        private void _FATabStrip_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeys(sender, e);
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeys(sender, e);
        }


#region  ---------------------------报警相关------------------------
        private void internalShowWarningDlg(Warning dlg)
        {
            dlg.Show(this);
        }

        public delegate void InvokeDelegate(Forms.Warning w);
        
        public void ShowWarningDlg(Warning dlg)
        {
            this.BeginInvoke(new InvokeDelegate(internalShowWarningDlg), dlg);
        }
#endregion
    }
}
