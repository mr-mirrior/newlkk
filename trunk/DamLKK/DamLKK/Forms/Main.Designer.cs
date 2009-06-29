namespace DamLKK.Forms
{
    partial class Main
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。

        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this._ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this._FATabStrip = new FarsiLibrary.Win.FATabStrip();
            this._FaTabStripItem = new FarsiLibrary.Win.FATabStripItem();
            ((System.ComponentModel.ISupportInitialize)(this._FATabStrip)).BeginInit();
            this.SuspendLayout();
            // 
            // _FATabStrip
            // 
            this._FATabStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this._FATabStrip.Location = new System.Drawing.Point(0, 0);
            this._FATabStrip.Name = "_FATabStrip";
            this._FATabStrip.SelectedItem = this._FaTabStripItem;
            this._FATabStrip.Size = new System.Drawing.Size(475, 304);
            this._FATabStrip.TabIndex = 0;
            this._FATabStrip.TabStripItemDBClicked += new System.EventHandler(this.tab_TabStripItemDBClicked);
            this._FATabStrip.TabCloseButtonClicked += new System.EventHandler(this.tab_TabCloseButtonClicked);
            this._FATabStrip.TabStripItemSelectionChanged += new FarsiLibrary.Win.TabStripItemChangedHandler(this._FATabStrip_TabStripItemSelectionChanged);
            this._FATabStrip.KeyDown += new System.Windows.Forms.KeyEventHandler(this._FATabStrip_KeyDown);
            // 
            // _FaTabStripItem
            // 
            this._FaTabStripItem.IsDrawn = true;
            this._FaTabStripItem.Name = "_FaTabStripItem";
            this._FaTabStripItem.Selected = true;
            this._FaTabStripItem.Size = new System.Drawing.Size(473, 277);
            this._FaTabStripItem.TabIndex = 0;
            this._FaTabStripItem.Title = "TabStrip Page 1";
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(475, 304);
            this.Controls.Add(this._FATabStrip);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "DamManager 2.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Main_Load);
            this.Leave += new System.EventHandler(this.Main_Leave);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this._FATabStrip)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip _ToolTip;
        public FarsiLibrary.Win.FATabStrip _FATabStrip;
        public FarsiLibrary.Win.FATabStripItem _FaTabStripItem;

    }
}