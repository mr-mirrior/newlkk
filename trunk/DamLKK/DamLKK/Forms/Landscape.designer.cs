namespace DamLKK.Forms
{
    partial class Landscape
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
            this.SuspendLayout();
            // 
            // Landscape
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(108, 138);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Landscape";
            this.Opacity = 0.5;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "鸟瞰";
            this.Deactivate += new System.EventHandler(this.Landscape_Deactivate);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Landscape_MouseUp);
            this.Activated += new System.EventHandler(this.Landscape_Activated);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Landscape_MouseDown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Landscape_FormClosing);
            this.Resize += new System.EventHandler(this.Landscape_Resize);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Landscape_MouseMove);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Landscape_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}