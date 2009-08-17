namespace DamLKK.Forms
{
    partial class DeckInfo
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
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txDesignDepth = new System.Windows.Forms.TextBox();
            this.txStartZ = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txErrorParam = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbMaxSpeed = new System.Windows.Forms.TextBox();
            this.la = new System.Windows.Forms.Label();
            this.tbNLibCounts = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDeckName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbLibCounts = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cbSpeedUnit = new System.Windows.Forms.ComboBox();
            this.lbPastion = new System.Windows.Forms.Label();
            this.lbPastiondsa = new System.Windows.Forms.Label();
            this.lbBlockname = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(145, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "遍(及其以上)";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.Location = new System.Drawing.Point(155, 266);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(422, 187);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(24, 21);
            this.label11.TabIndex = 13;
            this.label11.Text = "米";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label10.Location = new System.Drawing.Point(200, 187);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(16, 21);
            this.label10.TabIndex = 10;
            this.label10.Text = "米";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txDesignDepth
            // 
            this.txDesignDepth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txDesignDepth.Location = new System.Drawing.Point(351, 187);
            this.txDesignDepth.Name = "txDesignDepth";
            this.txDesignDepth.Size = new System.Drawing.Size(65, 23);
            this.txDesignDepth.TabIndex = 12;
            this.txDesignDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txDesignDepth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SpreadZ_KeyPress);
            // 
            // txStartZ
            // 
            this.txStartZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txStartZ.Enabled = false;
            this.txStartZ.Location = new System.Drawing.Point(123, 187);
            this.txStartZ.Name = "txStartZ";
            this.txStartZ.Size = new System.Drawing.Size(71, 23);
            this.txStartZ.TabIndex = 9;
            this.txStartZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txStartZ.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.StratZ_KeyPress);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(49, 187);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 21);
            this.label8.TabIndex = 8;
            this.label8.Text = "铺前高程：";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(200, 216);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 21);
            this.label7.TabIndex = 16;
            this.label7.Text = "%";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txErrorParam
            // 
            this.txErrorParam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txErrorParam.Location = new System.Drawing.Point(123, 216);
            this.txErrorParam.Name = "txErrorParam";
            this.txErrorParam.Size = new System.Drawing.Size(71, 23);
            this.txErrorParam.TabIndex = 15;
            this.txErrorParam.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txErrorParam.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ErrorParam_KeyPress);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(22, 216);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 21);
            this.label6.TabIndex = 14;
            this.label6.Text = "容许误差：";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbMaxSpeed
            // 
            this.tbMaxSpeed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbMaxSpeed.Location = new System.Drawing.Point(95, 116);
            this.tbMaxSpeed.Name = "tbMaxSpeed";
            this.tbMaxSpeed.Size = new System.Drawing.Size(50, 23);
            this.tbMaxSpeed.TabIndex = 6;
            this.tbMaxSpeed.Text = "0";
            this.tbMaxSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMaxSpeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MaxSpeed_KeyPress);
            // 
            // la
            // 
            this.la.AutoSize = true;
            this.la.Location = new System.Drawing.Point(13, 57);
            this.la.Name = "la";
            this.la.Size = new System.Drawing.Size(68, 17);
            this.la.TabIndex = 5;
            this.la.Text = "最大限速：";
            // 
            // tbNLibCounts
            // 
            this.tbNLibCounts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbNLibCounts.Location = new System.Drawing.Point(95, 85);
            this.tbNLibCounts.MaxLength = 2;
            this.tbNLibCounts.Name = "tbNLibCounts";
            this.tbNLibCounts.Size = new System.Drawing.Size(50, 23);
            this.tbNLibCounts.TabIndex = 3;
            this.tbNLibCounts.Text = "2";
            this.tbNLibCounts.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbNLibCounts.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DesignRollCount_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "静碾遍数：";
            // 
            // tbDeckName
            // 
            this.tbDeckName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbDeckName.Location = new System.Drawing.Point(123, 31);
            this.tbDeckName.Name = "tbDeckName";
            this.tbDeckName.Size = new System.Drawing.Size(300, 23);
            this.tbDeckName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "碾压层名称：";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(245, 266);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 27);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 159);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(451, 91);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "碾压高程";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(237, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 21);
            this.label5.TabIndex = 44;
            this.label5.Text = "设计铺料厚度：";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.tbLibCounts);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.la);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(12, 62);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(451, 91);
            this.groupBox2.TabIndex = 39;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "合格判定";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(365, 26);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(76, 17);
            this.label13.TabIndex = 46;
            this.label13.Text = "遍(及其以上)";
            // 
            // tbLibCounts
            // 
            this.tbLibCounts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbLibCounts.Location = new System.Drawing.Point(308, 24);
            this.tbLibCounts.MaxLength = 2;
            this.tbLibCounts.Name = "tbLibCounts";
            this.tbLibCounts.Size = new System.Drawing.Size(51, 23);
            this.tbLibCounts.TabIndex = 44;
            this.tbLibCounts.Text = "6";
            this.tbLibCounts.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(237, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 17);
            this.label9.TabIndex = 44;
            this.label9.Text = "振碾遍数：";
            // 
            // cbSpeedUnit
            // 
            this.cbSpeedUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSpeedUnit.FormattingEnabled = true;
            this.cbSpeedUnit.Items.AddRange(new object[] {
            "米/秒",
            "千米/小时"});
            this.cbSpeedUnit.Location = new System.Drawing.Point(152, 116);
            this.cbSpeedUnit.Name = "cbSpeedUnit";
            this.cbSpeedUnit.Size = new System.Drawing.Size(91, 25);
            this.cbSpeedUnit.TabIndex = 7;
            this.cbSpeedUnit.TextChanged += new System.EventHandler(this.cbSpeedUnit_TextChanged);
            // 
            // lbPastion
            // 
            this.lbPastion.AutoSize = true;
            this.lbPastion.Location = new System.Drawing.Point(311, 6);
            this.lbPastion.Name = "lbPastion";
            this.lbPastion.Size = new System.Drawing.Size(61, 17);
            this.lbPastion.TabIndex = 43;
            this.lbPastion.Text = "lbPastion";
            // 
            // lbPastiondsa
            // 
            this.lbPastiondsa.AutoSize = true;
            this.lbPastiondsa.Location = new System.Drawing.Point(274, 6);
            this.lbPastiondsa.Name = "lbPastiondsa";
            this.lbPastiondsa.Size = new System.Drawing.Size(44, 17);
            this.lbPastiondsa.TabIndex = 42;
            this.lbPastiondsa.Text = "标识：";
            // 
            // lbBlockname
            // 
            this.lbBlockname.AutoSize = true;
            this.lbBlockname.Location = new System.Drawing.Point(157, 6);
            this.lbBlockname.Name = "lbBlockname";
            this.lbBlockname.Size = new System.Drawing.Size(64, 17);
            this.lbBlockname.TabIndex = 41;
            this.lbBlockname.Text = "lbBlockID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(113, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 17);
            this.label4.TabIndex = 40;
            this.label4.Text = "仓面：";
            // 
            // DeckInfo
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(474, 304);
            this.Controls.Add(this.lbPastion);
            this.Controls.Add(this.lbPastiondsa);
            this.Controls.Add(this.lbBlockname);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbSpeedUnit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txDesignDepth);
            this.Controls.Add(this.txStartZ);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txErrorParam);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbMaxSpeed);
            this.Controls.Add(this.tbNLibCounts);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbDeckName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeckInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "仓面设置";
            this.Load += new System.EventHandler(this.OpenDeckInfo_Load);
            this.Shown += new System.EventHandler(this.DeckInfo_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txDesignDepth;
        private System.Windows.Forms.TextBox txStartZ;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txErrorParam;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbMaxSpeed;
        private System.Windows.Forms.Label la;
        private System.Windows.Forms.TextBox tbNLibCounts;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDeckName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbSpeedUnit;
        private System.Windows.Forms.Label lbPastion;
        private System.Windows.Forms.Label lbPastiondsa;
        private System.Windows.Forms.Label lbBlockname;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbLibCounts;
        private System.Windows.Forms.Label label13;
    }
}