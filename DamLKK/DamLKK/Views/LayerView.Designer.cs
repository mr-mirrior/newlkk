namespace DamLKK.Views
{
    partial class LayerView
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripMenuItem 仓面操作ToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem 视图VToolStripMenuItem;
            System.Windows.Forms.ToolStripMenuItem 设置SToolStripMenuItem;
            this.miStartDeck = new System.Windows.Forms.ToolStripMenuItem();
            this.miEndDeck = new System.Windows.Forms.ToolStripMenuItem();
            this.miDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.tmiNotRolling = new System.Windows.Forms.ToolStripMenuItem();
            this.miSkeleton = new System.Windows.Forms.ToolStripMenuItem();
            this.miRollingCount = new System.Windows.Forms.ToolStripMenuItem();
            this.miOverspeed = new System.Windows.Forms.ToolStripMenuItem();
            this.miVehicleInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.miArrows = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsReport = new System.Windows.Forms.ToolStripMenuItem();
            this.miDatTrackMap = new System.Windows.Forms.ToolStripMenuItem();
            this.生成压实厚度图TToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.miAssignment = new System.Windows.Forms.ToolStripMenuItem();
            this.miLookHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.tpp = new System.Windows.Forms.ToolTip(this.components);
            this.menuDeck = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miDeckName = new System.Windows.Forms.ToolStripMenuItem();
            this.miActive = new System.Windows.Forms.ToolStripMenuItem();
            this.btnChange = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.测试TToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            仓面操作ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            视图VToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            设置SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeck.SuspendLayout();
            this.SuspendLayout();
            // 
            // 仓面操作ToolStripMenuItem
            // 
            仓面操作ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miStartDeck,
            this.miEndDeck,
            this.miDelete,
            this.toolStripMenuItem4,
            this.tmiNotRolling});
            仓面操作ToolStripMenuItem.Name = "仓面操作ToolStripMenuItem";
            仓面操作ToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            仓面操作ToolStripMenuItem.Text = "仓面操作(&O)";
            // 
            // miStartDeck
            // 
            this.miStartDeck.Name = "miStartDeck";
            this.miStartDeck.ShortcutKeyDisplayString = "Ctrl+Alt+Shift+D";
            this.miStartDeck.Size = new System.Drawing.Size(213, 22);
            this.miStartDeck.Text = "开仓(&O)";
            this.miStartDeck.Click += new System.EventHandler(this.miStartDeck_Click);
            // 
            // miEndDeck
            // 
            this.miEndDeck.Name = "miEndDeck";
            this.miEndDeck.ShortcutKeyDisplayString = "Ctrl+Alt+Shift+D";
            this.miEndDeck.Size = new System.Drawing.Size(213, 22);
            this.miEndDeck.Text = "关仓(&S)";
            this.miEndDeck.Click += new System.EventHandler(this.miEndDeck_Click);
            // 
            // miDelete
            // 
            this.miDelete.Name = "miDelete";
            this.miDelete.Size = new System.Drawing.Size(213, 22);
            this.miDelete.Text = "删除仓面(&D)";
            this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(210, 6);
            this.toolStripMenuItem4.Visible = false;
            // 
            // tmiNotRolling
            // 
            this.tmiNotRolling.Name = "tmiNotRolling";
            this.tmiNotRolling.ShortcutKeyDisplayString = "Ctrl+F10";
            this.tmiNotRolling.Size = new System.Drawing.Size(213, 22);
            this.tmiNotRolling.Text = "排除碾压区域(&E)";
            this.tmiNotRolling.Visible = false;
            // 
            // 视图VToolStripMenuItem
            // 
            视图VToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSkeleton,
            this.miRollingCount,
            this.miOverspeed,
            this.miVehicleInfo,
            this.miArrows,
            this.toolStripMenuItem3,
            this.tsReport,
            this.miDatTrackMap,
            this.生成压实厚度图TToolStripMenuItem});
            视图VToolStripMenuItem.Name = "视图VToolStripMenuItem";
            视图VToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            视图VToolStripMenuItem.Text = "视图(&V)";
            // 
            // miSkeleton
            // 
            this.miSkeleton.Name = "miSkeleton";
            this.miSkeleton.ShortcutKeyDisplayString = "F5";
            this.miSkeleton.Size = new System.Drawing.Size(177, 22);
            this.miSkeleton.Text = "轨迹(&S)";
            this.miSkeleton.Click += new System.EventHandler(this.miSkeleton_Click_1);
            // 
            // miRollingCount
            // 
            this.miRollingCount.Name = "miRollingCount";
            this.miRollingCount.ShortcutKeyDisplayString = "F6";
            this.miRollingCount.Size = new System.Drawing.Size(177, 22);
            this.miRollingCount.Text = "碾压遍数(&R)";
            this.miRollingCount.Click += new System.EventHandler(this.miRollingCount_Click);
            // 
            // miOverspeed
            // 
            this.miOverspeed.Name = "miOverspeed";
            this.miOverspeed.ShortcutKeyDisplayString = "F7";
            this.miOverspeed.Size = new System.Drawing.Size(177, 22);
            this.miOverspeed.Text = "超速指示(&O)";
            this.miOverspeed.Click += new System.EventHandler(this.miOverspeed_Click_1);
            // 
            // miVehicleInfo
            // 
            this.miVehicleInfo.Name = "miVehicleInfo";
            this.miVehicleInfo.ShortcutKeyDisplayString = "F8";
            this.miVehicleInfo.Size = new System.Drawing.Size(177, 22);
            this.miVehicleInfo.Text = "碾压机信息(&V)";
            this.miVehicleInfo.Click += new System.EventHandler(this.miVehicleInfo_Click_1);
            // 
            // miArrows
            // 
            this.miArrows.Name = "miArrows";
            this.miArrows.Size = new System.Drawing.Size(177, 22);
            this.miArrows.Text = "轨迹箭头(&W)";
            this.miArrows.Click += new System.EventHandler(this.miArrows_Click_1);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(174, 6);
            // 
            // tsReport
            // 
            this.tsReport.Name = "tsReport";
            this.tsReport.ShortcutKeyDisplayString = "F9";
            this.tsReport.Size = new System.Drawing.Size(177, 22);
            this.tsReport.Text = "生成图形报告(&R)";
            this.tsReport.Click += new System.EventHandler(this.tsReport_Click);
            // 
            // miDatTrackMap
            // 
            this.miDatTrackMap.Name = "miDatTrackMap";
            this.miDatTrackMap.ShortcutKeyDisplayString = "F10";
            this.miDatTrackMap.Size = new System.Drawing.Size(177, 22);
            this.miDatTrackMap.Text = "生成轨迹图(&D)";
            this.miDatTrackMap.Click += new System.EventHandler(this.miDatTrackMap_Click);
            // 
            // 生成压实厚度图TToolStripMenuItem
            // 
            this.生成压实厚度图TToolStripMenuItem.Name = "生成压实厚度图TToolStripMenuItem";
            this.生成压实厚度图TToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.生成压实厚度图TToolStripMenuItem.Text = "生成压实厚度图(&T)";
            this.生成压实厚度图TToolStripMenuItem.Click += new System.EventHandler(this.生成压实厚度图TToolStripMenuItem_Click);
            // 
            // 设置SToolStripMenuItem
            // 
            设置SToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miProperties,
            this.miAssignment,
            this.miLookHistory});
            设置SToolStripMenuItem.Name = "设置SToolStripMenuItem";
            设置SToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            设置SToolStripMenuItem.Text = "设置(&S)";
            // 
            // miProperties
            // 
            this.miProperties.Name = "miProperties";
            this.miProperties.ShortcutKeyDisplayString = "";
            this.miProperties.Size = new System.Drawing.Size(160, 22);
            this.miProperties.Text = "修改属性(&P)";
            this.miProperties.Click += new System.EventHandler(this.miProperties_Click);
            // 
            // miAssignment
            // 
            this.miAssignment.Name = "miAssignment";
            this.miAssignment.ShortcutKeyDisplayString = "";
            this.miAssignment.Size = new System.Drawing.Size(160, 22);
            this.miAssignment.Text = "车辆派遣(&V)";
            this.miAssignment.Click += new System.EventHandler(this.miAssignment_Click);
            // 
            // miLookHistory
            // 
            this.miLookHistory.Name = "miLookHistory";
            this.miLookHistory.Size = new System.Drawing.Size(160, 22);
            this.miLookHistory.Text = "车辆派遣历史(&L)";
            this.miLookHistory.Click += new System.EventHandler(this.miLookHistory_Click);
            // 
            // tpp
            // 
            this.tpp.AutoPopDelay = 5000;
            this.tpp.InitialDelay = 1000;
            this.tpp.ReshowDelay = 1000;
            this.tpp.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.tpp.UseAnimation = false;
            this.tpp.UseFading = false;
            // 
            // menuDeck
            // 
            this.menuDeck.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDeckName,
            this.miActive,
            this.btnChange,
            this.toolStripSeparator4,
            仓面操作ToolStripMenuItem,
            视图VToolStripMenuItem,
            this.toolStripSeparator2,
            设置SToolStripMenuItem,
            this.miCancel,
            this.测试TToolStripMenuItem});
            this.menuDeck.Name = "menuDeck";
            this.menuDeck.Size = new System.Drawing.Size(179, 214);
            // 
            // miDeckName
            // 
            this.miDeckName.Enabled = false;
            this.miDeckName.Name = "miDeckName";
            this.miDeckName.Size = new System.Drawing.Size(178, 22);
            this.miDeckName.Text = "\"仓面1 ED-569.2米\"";
            // 
            // miActive
            // 
            this.miActive.Name = "miActive";
            this.miActive.ShortcutKeyDisplayString = "";
            this.miActive.Size = new System.Drawing.Size(178, 22);
            this.miActive.Text = "设为可见仓面(&D)";
            this.miActive.Click += new System.EventHandler(this.miActive_Click);
            // 
            // btnChange
            // 
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(178, 22);
            this.btnChange.Text = "修改仓面范围(&F)";
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(175, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(175, 6);
            // 
            // miCancel
            // 
            this.miCancel.Name = "miCancel";
            this.miCancel.Size = new System.Drawing.Size(178, 22);
            this.miCancel.Text = "取消(&C)";
            this.miCancel.Visible = false;
            // 
            // 测试TToolStripMenuItem
            // 
            this.测试TToolStripMenuItem.Name = "测试TToolStripMenuItem";
            this.测试TToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.测试TToolStripMenuItem.Text = "测试(&T)";
            this.测试TToolStripMenuItem.Visible = false;
            // 
            // LayerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "LayerView";
            this.Load += new System.EventHandler(this.OnLoad);
            this.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseDBClick);
            this.Leave += new System.EventHandler(this.LayerView_Leave);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.LayerView_Scroll);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.Resize += new System.EventHandler(this.OnResize);
            this.Enter += new System.EventHandler(this.LayerView_Enter);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.menuDeck.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip tpp;
        private System.Windows.Forms.ContextMenuStrip menuDeck;
        private System.Windows.Forms.ToolStripMenuItem miDeckName;
        private System.Windows.Forms.ToolStripMenuItem miActive;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem miStartDeck;
        private System.Windows.Forms.ToolStripMenuItem miEndDeck;
        private System.Windows.Forms.ToolStripMenuItem miDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem tmiNotRolling;
        private System.Windows.Forms.ToolStripMenuItem miSkeleton;
        private System.Windows.Forms.ToolStripMenuItem miRollingCount;
        private System.Windows.Forms.ToolStripMenuItem miOverspeed;
        private System.Windows.Forms.ToolStripMenuItem miVehicleInfo;
        private System.Windows.Forms.ToolStripMenuItem miArrows;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem tsReport;
        private System.Windows.Forms.ToolStripMenuItem miDatTrackMap;
        private System.Windows.Forms.ToolStripMenuItem 生成压实厚度图TToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miProperties;
        private System.Windows.Forms.ToolStripMenuItem miAssignment;
        private System.Windows.Forms.ToolStripMenuItem miLookHistory;
        private System.Windows.Forms.ToolStripMenuItem miCancel;
        private System.Windows.Forms.ToolStripMenuItem 测试TToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnChange;
    }
}
