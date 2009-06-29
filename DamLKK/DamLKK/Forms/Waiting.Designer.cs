namespace DamLKK.Forms
{
    partial class Waiting
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
            this.wait = new DamLKK.Utils.MacWait.MacWait();
            this.SuspendLayout();
            // 
            // wait
            // 
            this.wait.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wait.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.wait.ForeColor = System.Drawing.Color.Navy;
            this.wait.Location = new System.Drawing.Point(0, 0);
            this.wait.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.wait.Name = "wait";
            this.wait.Prompt = "请稍候……";
            this.wait.Size = new System.Drawing.Size(380, 105);
            this.wait.TabIndex = 0;
            // 
            // Waiting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(380, 105);
            this.ControlBox = false;
            this.Controls.Add(this.wait);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Waiting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "请稍候";
            this.ResumeLayout(false);

        }

        #endregion

        private Utils.MacWait.MacWait wait;
    }
}