namespace DamLKK.Forms
{
    partial class Warning
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbMessage = new System.Windows.Forms.Label();
            this.lbTimer = new System.Windows.Forms.Label();
            this.roundPanel1 = new DamLKK.RoundPanel();
            this.lbWarningType = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lb = new System.Windows.Forms.Label();
            this.lbWarningTime = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbProportion = new System.Windows.Forms.Label();
            this.lbVehicleName = new System.Windows.Forms.Label();
            this.roundPanel2 = new DamLKK.RoundPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.roundPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.roundPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(14)))), ((int)(((byte)(32)))));
            this.panel1.Controls.Add(this.lbMessage);
            this.panel1.Controls.Add(this.lbTimer);
            this.panel1.Controls.Add(this.roundPanel1);
            this.panel1.Controls.Add(this.roundPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(14)))), ((int)(((byte)(32)))));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(664, 399);
            this.panel1.TabIndex = 0;
            this.toolTip1.SetToolTip(this.panel1, "双击关闭");
            this.panel1.DoubleClick += new System.EventHandler(this.panel1_DoubleClick);
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // lbMessage
            // 
            this.lbMessage.BackColor = System.Drawing.Color.Transparent;
            this.lbMessage.ForeColor = System.Drawing.Color.LightGray;
            this.lbMessage.Location = new System.Drawing.Point(316, 365);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(307, 34);
            this.lbMessage.TabIndex = 3;
            this.lbMessage.Text = "窗口将自动关闭，单击这里停止";
            this.lbMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lbMessage.Click += new System.EventHandler(this.lbMessage_Click);
            // 
            // lbTimer
            // 
            this.lbTimer.BackColor = System.Drawing.Color.Transparent;
            this.lbTimer.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbTimer.ForeColor = System.Drawing.Color.LightGray;
            this.lbTimer.Location = new System.Drawing.Point(606, 365);
            this.lbTimer.Name = "lbTimer";
            this.lbTimer.Size = new System.Drawing.Size(58, 34);
            this.lbTimer.TabIndex = 2;
            this.lbTimer.Text = "10";
            this.lbTimer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // roundPanel1
            // 
            this.roundPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.roundPanel1.Back = System.Drawing.Color.Empty;
            this.roundPanel1.BackColor = System.Drawing.Color.Transparent;
            this.roundPanel1.Controls.Add(this.lbWarningType);
            this.roundPanel1.Controls.Add(this.pictureBox1);
            this.roundPanel1.Controls.Add(this.lb);
            this.roundPanel1.Controls.Add(this.lbWarningTime);
            this.roundPanel1.Controls.Add(this.label6);
            this.roundPanel1.Controls.Add(this.lbProportion);
            this.roundPanel1.Controls.Add(this.lbVehicleName);
            this.roundPanel1.ForeColor = System.Drawing.Color.White;
            this.roundPanel1.Location = new System.Drawing.Point(73, 62);
            this.roundPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel1.MatrixRound = 10;
            this.roundPanel1.Name = "roundPanel1";
            this.roundPanel1.Size = new System.Drawing.Size(512, 143);
            this.roundPanel1.TabIndex = 0;
            // 
            // lbWarningType
            // 
            this.lbWarningType.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbWarningType.ForeColor = System.Drawing.Color.Black;
            this.lbWarningType.Location = new System.Drawing.Point(241, 13);
            this.lbWarningType.Name = "lbWarningType";
            this.lbWarningType.Size = new System.Drawing.Size(177, 27);
            this.lbWarningType.TabIndex = 2;
            this.lbWarningType.Text = "标准遍数百分比为：";
            this.lbWarningType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DamLKK.Properties.Resources.warning;
            this.pictureBox1.Location = new System.Drawing.Point(7, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lb
            // 
            this.lb.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(14)))), ((int)(((byte)(32)))));
            this.lb.Location = new System.Drawing.Point(90, 13);
            this.lb.Name = "lb";
            this.lb.Size = new System.Drawing.Size(156, 27);
            this.lb.TabIndex = 1;
            this.lb.Text = "碾压简报: ";
            this.lb.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbWarningTime
            // 
            this.lbWarningTime.ForeColor = System.Drawing.Color.Black;
            this.lbWarningTime.Location = new System.Drawing.Point(413, 46);
            this.lbWarningTime.Name = "lbWarningTime";
            this.lbWarningTime.Size = new System.Drawing.Size(93, 15);
            this.lbWarningTime.TabIndex = 13;
            this.lbWarningTime.Text = "12:12,12";
            this.lbWarningTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Location = new System.Drawing.Point(143, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(360, 2);
            this.label6.TabIndex = 12;
            this.label6.Text = "label6";
            // 
            // lbProportion
            // 
            this.lbProportion.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbProportion.ForeColor = System.Drawing.Color.Black;
            this.lbProportion.Location = new System.Drawing.Point(160, 57);
            this.lbProportion.Name = "lbProportion";
            this.lbProportion.Size = new System.Drawing.Size(276, 58);
            this.lbProportion.TabIndex = 11;
            this.lbProportion.Text = "23.5%";
            this.lbProportion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbVehicleName
            // 
            this.lbVehicleName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbVehicleName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbVehicleName.ForeColor = System.Drawing.Color.Black;
            this.lbVehicleName.Location = new System.Drawing.Point(416, 22);
            this.lbVehicleName.Name = "lbVehicleName";
            this.lbVehicleName.Size = new System.Drawing.Size(90, 15);
            this.lbVehicleName.TabIndex = 3;
            this.lbVehicleName.Text = "四号碾压机";
            this.lbVehicleName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.lbVehicleName, "分区：ED，高程：569.2米，仓面1");
            // 
            // roundPanel2
            // 
            this.roundPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.roundPanel2.Back = System.Drawing.Color.Empty;
            this.roundPanel2.BackColor = System.Drawing.Color.Transparent;
            this.roundPanel2.Controls.Add(this.label1);
            this.roundPanel2.Controls.Add(this.pictureBox2);
            this.roundPanel2.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roundPanel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.roundPanel2.Location = new System.Drawing.Point(76, 69);
            this.roundPanel2.MatrixRound = 10;
            this.roundPanel2.Name = "roundPanel2";
            this.roundPanel2.Size = new System.Drawing.Size(516, 147);
            this.roundPanel2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(139)))), ((int)(((byte)(14)))), ((int)(((byte)(32)))));
            this.label1.Location = new System.Drawing.Point(88, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(215, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "警告:   车辆超速警报！";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(78, 72);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 0;
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ShowAlways = true;
            this.toolTip1.UseAnimation = false;
            this.toolTip1.UseFading = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Warning
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(664, 399);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Warning";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Warning_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Warning_KeyDown);
            this.panel1.ResumeLayout(false);
            this.roundPanel1.ResumeLayout(false);
            this.roundPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.roundPanel2.ResumeLayout(false);
            this.roundPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private RoundPanel roundPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private RoundPanel roundPanel1;
        private System.Windows.Forms.Label lbProportion;
        private System.Windows.Forms.Label lbVehicleName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbWarningTime;
        private System.Windows.Forms.Label lbTimer;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbMessage;
        private System.Windows.Forms.Label lb;
        public System.Windows.Forms.Label lbWarningType;
    }
}