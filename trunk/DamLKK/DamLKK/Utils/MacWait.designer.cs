namespace DamLKK.Utils.MacWait
{
    partial class MacWait
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
            this.lbPrompt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbPrompt
            // 
            this.lbPrompt.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbPrompt.ForeColor = System.Drawing.Color.Black;
            this.lbPrompt.Location = new System.Drawing.Point(0, 92);
            this.lbPrompt.Name = "lbPrompt";
            this.lbPrompt.Size = new System.Drawing.Size(369, 23);
            this.lbPrompt.TabIndex = 0;
            this.lbPrompt.Text = "请稍候……";
            this.lbPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MacWait
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.lbPrompt);
            this.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.Color.DarkOrchid;
            this.Name = "MacWait";
            this.Size = new System.Drawing.Size(372, 201);
            this.Load += new System.EventHandler(this.MacWait_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbPrompt;
    }
}
