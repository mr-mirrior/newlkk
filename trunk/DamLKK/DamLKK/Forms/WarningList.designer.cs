namespace DamLKK.Forms
{
    partial class WarningList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panellist = new System.Windows.Forms.Panel();
            this.lstWarnList = new System.Windows.Forms.ListView();
            this.colWarnTime = new System.Windows.Forms.ColumnHeader();
            this.colWarningType = new System.Windows.Forms.ColumnHeader();
            this.panellist.SuspendLayout();
            this.SuspendLayout();
            // 
            // panellist
            // 
            this.panellist.Controls.Add(this.lstWarnList);
            this.panellist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panellist.Location = new System.Drawing.Point(0, 0);
            this.panellist.Name = "panellist";
            this.panellist.Size = new System.Drawing.Size(189, 573);
            this.panellist.TabIndex = 0;
            // 
            // lstWarnList
            // 
            this.lstWarnList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colWarnTime,
            this.colWarningType});
            this.lstWarnList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstWarnList.GridLines = true;
            this.lstWarnList.Location = new System.Drawing.Point(0, 0);
            this.lstWarnList.Name = "lstWarnList";
            this.lstWarnList.Size = new System.Drawing.Size(189, 573);
            this.lstWarnList.TabIndex = 0;
            this.lstWarnList.UseCompatibleStateImageBehavior = false;
            this.lstWarnList.View = System.Windows.Forms.View.Details;
            this.lstWarnList.DoubleClick += new System.EventHandler(this.lstWarnList_DoubleClick);
            // 
            // colWarnTime
            // 
            this.colWarnTime.Text = "报警时间";
            this.colWarnTime.Width = 90;
            // 
            // colWarningType
            // 
            this.colWarningType.Text = "报警种类";
            this.colWarningType.Width = 90;
            // 
            // WarningList
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(189, 573);
            this.Controls.Add(this.panellist);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WarningList";
            this.Opacity = 0.5;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "报警查询";
            this.Load += new System.EventHandler(this.WarningList_Load);
            this.panellist.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panellist;
        private System.Windows.Forms.ListView lstWarnList;
        private System.Windows.Forms.ColumnHeader colWarnTime;
        private System.Windows.Forms.ColumnHeader colWarningType;
    }
}