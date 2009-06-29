namespace DamLKK.Forms
{
    partial class LayerPreview
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
            this.pl = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pl
            // 
            this.pl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pl.Location = new System.Drawing.Point(0, 0);
            this.pl.Margin = new System.Windows.Forms.Padding(0);
            this.pl.Name = "pl";
            this.pl.Size = new System.Drawing.Size(331, 281);
            this.pl.TabIndex = 0;
            // 
            // LayerPreview
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(331, 281);
            this.Controls.Add(this.pl);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LayerPreview";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "预览";
            this.Load += new System.EventHandler(this.LayerPreview_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LayerPreview_FormClosing);
            this.Resize += new System.EventHandler(this.LayerPreview_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LayerPreview_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pl;

    }
}