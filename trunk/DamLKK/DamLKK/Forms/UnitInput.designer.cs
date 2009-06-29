namespace DamLKK.Forms
{
    partial class UnitInput
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
            this.cbStart = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbEnd = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbStartZ = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbEndZ = new System.Windows.Forms.TextBox();
            this.tbCoords = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbStart
            // 
            this.cbStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStart.FormattingEnabled = true;
            this.cbStart.Items.AddRange(new object[] {
            "1号坝段",
            "2号坝段",
            "3号坝段",
            "4号坝段",
            "5号坝段",
            "6号坝段",
            "7号坝段",
            "8号坝段",
            "9号坝段",
            "10号坝段",
            "11号坝段",
            "12号坝段",
            "13号坝段",
            "14号坝段",
            "15号坝段",
            "16号坝段",
            "17号坝段",
            "18号坝段",
            "19号坝段",
            "20号坝段",
            "21号坝段",
            "22号坝段",
            "23号坝段",
            "24号坝段",
            "25号坝段",
            "26号坝段",
            "27号坝段",
            "28号坝段",
            "29号坝段",
            "30号坝段                                               "});
            this.cbStart.Location = new System.Drawing.Point(127, 60);
            this.cbStart.Name = "cbStart";
            this.cbStart.Size = new System.Drawing.Size(126, 25);
            this.cbStart.TabIndex = 0;
            this.cbStart.SelectedIndexChanged += new System.EventHandler(this.cbStart_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "起始坝段：";
            // 
            // cbEnd
            // 
            this.cbEnd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEnd.FormattingEnabled = true;
            this.cbEnd.Items.AddRange(new object[] {
            "1号坝段",
            "2号坝段",
            "3号坝段",
            "4号坝段",
            "5号坝段",
            "6号坝段",
            "7号坝段",
            "8号坝段",
            "9号坝段",
            "10号坝段",
            "11号坝段",
            "12号坝段",
            "13号坝段",
            "14号坝段",
            "15号坝段",
            "16号坝段",
            "17号坝段",
            "18号坝段",
            "19号坝段",
            "20号坝段",
            "21号坝段",
            "22号坝段",
            "23号坝段",
            "24号坝段",
            "25号坝段",
            "26号坝段",
            "27号坝段",
            "28号坝段",
            "29号坝段",
            "30号坝段                                            "});
            this.cbEnd.Location = new System.Drawing.Point(373, 57);
            this.cbEnd.Name = "cbEnd";
            this.cbEnd.Size = new System.Drawing.Size(126, 25);
            this.cbEnd.TabIndex = 3;
            this.cbEnd.SelectedIndexChanged += new System.EventHandler(this.cbEnd_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(44, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "起始高程：";
            // 
            // tbStartZ
            // 
            this.tbStartZ.Location = new System.Drawing.Point(127, 118);
            this.tbStartZ.Name = "tbStartZ";
            this.tbStartZ.Size = new System.Drawing.Size(99, 23);
            this.tbStartZ.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(233, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "米";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(290, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "结束坝段：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(290, 121);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "终止高程：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(479, 121);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "米";
            // 
            // tbEndZ
            // 
            this.tbEndZ.Location = new System.Drawing.Point(373, 116);
            this.tbEndZ.Name = "tbEndZ";
            this.tbEndZ.Size = new System.Drawing.Size(99, 23);
            this.tbEndZ.TabIndex = 10;
            // 
            // tbCoords
            // 
            this.tbCoords.AcceptsReturn = true;
            this.tbCoords.Location = new System.Drawing.Point(113, 169);
            this.tbCoords.Multiline = true;
            this.tbCoords.Name = "tbCoords";
            this.tbCoords.Size = new System.Drawing.Size(386, 85);
            this.tbCoords.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(28, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 17);
            this.label7.TabIndex = 13;
            this.label7.Text = "坐标点(&C)：";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.Location = new System.Drawing.Point(124, 357);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(91, 27);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "确认(&O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(328, 357);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(91, 27);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(195, 11);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(235, 23);
            this.tbName.TabIndex = 20;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(112, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 17);
            this.label9.TabIndex = 19;
            this.label9.Text = "单元名称：";
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(5, 264);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(533, 79);
            this.label10.TabIndex = 21;
            this.label10.Text = "排除碾压区域边界输入法：形如x11,y11;x12,y12;x13,y13|x21,y21;x22,y22;x23,y23;x24,y24 ，即逗号隔离某一点的坝" +
                "横坐标与坝纵坐标，分号隔离两个控制点，竖杠隔离两个排除碾压区域（坐标点请按照顺时针或逆时针依次输入）。排除碾压区域备注输入法：形如 监测仪器埋设|修路 标点请使" +
                "用英文半角输入法！";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UnitInput
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(543, 395);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbCoords);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbEndZ);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbStartZ);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbEnd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbStart);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UnitInput";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "施工单元规划";
            this.Load += new System.EventHandler(this.UnitInput_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.UnitInput_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbEnd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbStartZ;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbEndZ;
        private System.Windows.Forms.TextBox tbCoords;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}