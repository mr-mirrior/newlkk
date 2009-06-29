namespace DamLKK.Forms
{
    partial class EagleEye
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
            this.components = new System.ComponentModel.Container();
            this.tpp = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // tpp
            // 
            this.tpp.AutoPopDelay = 5000;
            this.tpp.InitialDelay = 500;
            this.tpp.ReshowDelay = 500;
            this.tpp.ShowAlways = true;
            // 
            // EagleEye
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(276, 123);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "EagleEye";
            this.Opacity = 0.75;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "鹰眼";
            this.Deactivate += new System.EventHandler(this.EagleEye_Deactivate);
            this.Load += new System.EventHandler(this.EagleEye_Load);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.EagleEye_MouseDoubleClick);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.EagleEye_MouseClick);
            this.Activated += new System.EventHandler(this.EagleEye_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EagleEye_FormClosing);
            this.Resize += new System.EventHandler(this.EagleEye_Resize);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EagleEye_MouseMove);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EagleEye_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip tpp;
    }
}