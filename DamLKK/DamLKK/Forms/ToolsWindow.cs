using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DamLKK._Model;

namespace DamLKK.Forms
{
    public partial class ToolsWindow : Form
    {
        private ToolsWindow()
        {
            InitializeComponent();
        }

        private static ToolsWindow _MyInstance;

        public static ToolsWindow GetInstance()
        {
            if (_MyInstance==null)
            {
                _MyInstance = new ToolsWindow();
            }

            return _MyInstance;
        }

        Waiting _WaitDlg = new Waiting();
        System.Threading.Thread _Thrd;
        LayerPreview _FrmPreview = new LayerPreview();
        Forms.UnitInput _FrmUnitIn;

        public Forms.UnitInput FrmUnitIn
        {
            set { _FrmUnitIn = value; }
        }

        public _Model.ElevationFile CurrentFile { get { if (cbUnitTag.Items.Count > 0 && float.Parse(cbUnitTag.SelectedItem.ToString()) > 100) return (_Model.ElevationFile)cbUnitTag.SelectedItem; return null; } set { cbUnitTag.SelectedItem = value; } }

        Views.LayerView _CurrentLayer = null;
        /// <summary>
        /// 当前层
        /// </summary>
        public Views.LayerView CurrentLayer 
        {
            get { return _CurrentLayer; } 
            set { if (value == CurrentLayer) return; _CurrentLayer = value; UpdateLayer(); } 
        }

        /// <summary>
        /// 有待观察
        /// </summary>
        private void UpdateLayer()
        {
            if (_CurrentLayer == null)
            {
                this.Text = "无";
                return;
            }
        }

        /// <summary>
        /// 操作是鼠标的图形
        /// </summary>
        public Views.Operations Operation
        {
            get
            {
                if (CurrentLayer != null)
                    return CurrentLayer.Operation;
                return Views.Operations.NONE;
            }
            set
            {
                if (CurrentLayer != null)
                    CurrentLayer.Operation = value;
                UpdateLayer();
            }
        }

        /// <summary>
        /// 隐藏预览
        /// </summary>
        public void HidePreview()
        {
            _FrmPreview.Hide();
            //ckPreview.CheckState = CheckState.Unchecked;
        }

        /// <summary>
        /// 找屏幕最适合位置
        /// </summary>
        public void FitScreen()
        {
            if (CurrentLayer != null)
            {
                CurrentLayer.FitScreenOnce();
            }

        }


        #region - 初始化 -
        private void Init()
        {
            InitButtons();
        }
        private void InitButtons()
        {
            foreach (Control c in this.Controls)
            {
                if (c is Utils.VistaButton)
                {
                    c.Click += OnClickBtn;
                }
            }
        }

        private void OnClickBtn(object sender, EventArgs e)
        {
            if (sender == null)
                return;
            if (sender is Utils.VistaButton)
            {
                Utils.VistaButton btn = (Utils.VistaButton)sender;
                this.Text = btn.ButtonText;
            }
        }
        #endregion
        
        private void ToolsWindow_Load(object sender, EventArgs e)
        {

            if (_Control.LoginControl.User.Authority == _Control.LoginResult.VIEW)
            {
                btnDeckPoly.Enabled = false;
                btnInputCoord.Enabled = false;
                btnDeckPoly.ForeColor = Color.Gray;
            }
            foreach (Control c in this.Controls)
            {
                if (c is Utils.VistaButton)
                {
                    Utils.VistaButton btn = (Utils.VistaButton)c;
                    if (btn.Checked)
                        OnClickBtn(btn, null);
                }
            }

            DamLKK._Model.Dam.GetInstance().UpdateCbUnits(cbMode.SelectedIndex);

            ShowPreview();

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            _WaitDlg.Prompt = "正在读取数据……";
            _WaitDlg.Show(this);   
            _Thrd = new System.Threading.Thread(OnTickOnLoad);
            _Thrd.Start();
        }

        private delegate void OnLoadOp();
        private void OnTickOnLoad()
        {
            cbMode.SelectedIndex = 0;
            OnLoadUpdateLayer();
            if (this.IsHandleCreated)
                this.BeginInvoke(new OnLoadOp(EndThread));
        }

        private void EndThread()
        {
            _WaitDlg.Hide();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = true;

            Operation = Views.Operations.SCROLL_FREE;
        }

        private void OnLoadUpdateLayer()
        {
            Cursor = Main.MyInstance().Cursor = Cursors.WaitCursor;

            SearchUnits(false);

            Cursor = Main.MyInstance().Cursor = Cursors.Default;
        }

        private void SearchUnits(bool review)
        {
            Dam.GetInstance().UpdateCbUnits(0);
        }


        private void ShowPreview()
        {
            if (_FrmPreview.Visible)
                return;
            _FrmPreview.Show(this);

            Rectangle rc = this.DesktopBounds;
            int x, y;
            x = rc.Left - _FrmPreview.Width;
            y = rc.Bottom - _FrmPreview.Height;
            if (x < 0 || y < 0)
            {
                _FrmPreview.Hide();
                return;
            }
            _FrmPreview.Location = new Point(x, y);
        }

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DamLKK._Model.Dam.GetInstance().CurrentUnit == null)
                return;

            if (cbMode.SelectedIndex == 0)
            {
                cbSyncline.Enabled = true;
                
                DamLKK._Model.Dam.GetInstance().CurrentUnit.UpdateCbTags(0);
            }
            else
            {
                cbSyncline.Enabled = false;
                cbSyncline.Checked = false;
                DamLKK._Model.Dam.GetInstance().CurrentUnit.UpdateCbTags(1);
            }
        }

        private void btnUnit_Click(object sender, EventArgs e)
        {
            if (_FrmUnitIn==null)
            {
                _FrmUnitIn = new UnitInput();
                _FrmUnitIn.Show(this.Owner);
            }
        }

        /// <summary>
        /// 是否还在初始化
        /// </summary>
        /// <returns></returns>
        private bool IsInitializing()
        {
            if (_Thrd == null) return false;
            return _Thrd.IsAlive;
        }


        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (IsInitializing())
            {
                Utils.MB.Warning("初始化尚未完成，请稍候再试一次。");
                return;
            }

            if ((!cbSyncline.Checked&&cbUnitTag.SelectedItem == null) || cbWorkUnit.SelectedItem == null)
                return;
            
            Unit unit=GetCurrentUnit();

            if(unit==null)
                return;

            
             double Tag=-1f;
            //斜层取最小标识，平层取高程
             if (cbSyncline.Checked)
             {
                 Tag = DB.UnitDAO.GetInstance().GetMinTag(unit);

                 if (Tag == -1)
                     Tag = 1;
                 else if (Tag == -2)
                     return;
             }
            else
             {
                 if (!double.TryParse(cbUnitTag.SelectedItem.ToString(), out Tag))
                     Tag = double.Parse(cbUnitTag.SelectedItem.ToString().Trim().Substring(2));

             }
           
            Views.LayerView layerview = _Control.LayerControl.Instance.OpenLayer(unit, new Elevation(Tag));

            if (layerview != null)
                CurrentLayer = layerview;
            else
                return;

            layerview.UpdateGraphics();
            layerview.FitScreenOnce();
        }


        /// <summary>
        /// 根据单元和高程打开一个层
        /// </summary>
        public void OpenLayer(Unit p_unit, double elev)
        {
            Views.LayerView layerview = _Control.LayerControl.Instance.OpenLayer(p_unit, new Elevation(elev));

            if (layerview != null)
                CurrentLayer = layerview;
            else
                return;

            layerview.UpdateGraphics();
            layerview.FitScreenOnce();
        }


        private void cbWorkUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            Unit unit = GetCurrentUnit();
            if (unit == null)
                return;
            unit.UpdateCbTags(cbMode.SelectedIndex);
        }

        /// <summary>
        /// 获取当前选中单元并更新dam的currentunit
        /// </summary>
        /// <returns></returns>
        private Unit GetCurrentUnit()
        {
            Unit unit = DB.UnitDAO.GetInstance().GetOneUnit(cbWorkUnit.SelectedItem.ToString());
            Dam.GetInstance().CurrentUnit = unit;

            return unit;
        }

        /// <summary>
        /// 拖动
        /// </summary>
        private void btn3_Click(object sender, EventArgs e)
        {
            Operation = Views.Operations.SCROLL_FREE;
        }


        /// <summary>
        /// 缩放
        /// </summary>
        private void btn11_Click(object sender, EventArgs e)
        {
            Operation = Views.Operations.ZOOM;
        }

        /// <summary>
        /// 屏幕
        /// </summary>
        private void btnFitscreen_Click(object sender, EventArgs e)
        {
            if (_CurrentLayer != null)
            {
                _CurrentLayer.FitScreenOnce();
            }
        }

        /// <summary>
        /// 旋转
        /// </summary>
        private void btn7_Click(object sender, EventArgs e)
        {
            Operation = Views.Operations.ROTATE;
        }

        /// <summary>
        /// 0度
        /// </summary>
        private void btnRestore_Click(object sender, EventArgs e)
        {
            RotateLayer(0);
        }

        const double SHANGYOU = -175.790047519344;
        const double XIAYOU = SHANGYOU - 180;
        const double ZUOAN = SHANGYOU - 90;
        const double YOUAN = XIAYOU - 90;

        /// <summary>
        /// 上游
        /// </summary>
        private void btnShangyou_Click(object sender, EventArgs e)
        {
            RotateLayer(SHANGYOU);
        }


        private void btnXiayou_Click(object sender, EventArgs e)
        {
            RotateLayer(XIAYOU);
        }

        private void btnZuoan_Click(object sender, EventArgs e)
        {
            RotateLayer(ZUOAN);
        }

        private void btnYouan_Click(object sender, EventArgs e)
        {
            RotateLayer(YOUAN);
        }


        /// <summary>
        /// 按指定角度旋转
        /// </summary>
        private void RotateLayer(double angle)
        {
            if (_CurrentLayer != null)
            {
                _CurrentLayer.RotateDegrees = angle;
                _CurrentLayer.UpdateGraphics();
            }
        }

        Forms.DeckCoordInput _FrmCoordInput;
        private void btnInputCoord_Click(object sender, EventArgs e)
        {
            if (_FrmCoordInput != null)
                return;
            _FrmCoordInput = new DeckCoordInput();
            if (CurrentLayer == null)
                return;
            _FrmCoordInput.Show(this.Owner);
            _FrmCoordInput.tbCoords.Text = string.Empty;
        }

        /// <summary>
        /// 是否为临时取仓面边界
        /// </summary>
        public bool _IsTempSelectDeck = false;

        public void btnDeckPoly_Click(object sender, EventArgs e)
        {
            Operation = Views.Operations.DECK_POLYGON;
        }

        private void cbSyncline_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSyncline.Checked)
            {
                cbUnitTag.SelectedIndex = -1;
                cbUnitTag.Enabled = false;
            }
            else
            {
                cbUnitTag.SelectedIndex =0;
                cbUnitTag.Enabled =true;
            }
        }

        public void UpdateMode()
        {
            cbMode_SelectedIndexChanged(null, null);
        }

        Forms.WarningList FrmWarnlst;
        private void vistaButton1_Click(object sender, EventArgs e)
        {
            if (FrmWarnlst != null)
            {
                FrmWarnlst.Close();
            }

            FrmWarnlst = new WarningList();
            if (FrmWarnlst.Visible)
                return;

            Rectangle rc = this.DesktopBounds;
            int x, y;
            x = rc.Left - FrmWarnlst.Width;
            y = rc.Bottom - FrmWarnlst.Height;
            FrmWarnlst.Location = new Point(x, y);
            FrmWarnlst.Show();
        }

        //单击边数查询
        private void btnRollCount_Click(object sender, EventArgs e)
        {
            Operation = Views.Operations.ROOL_COUNT;
        }
        
    }
}
