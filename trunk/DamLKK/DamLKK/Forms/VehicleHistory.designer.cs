namespace DamLKK.Forms
{
    partial class VehicleHistory
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
            this.lbDeckName = new System.Windows.Forms.Label();
            this.dada = new System.Windows.Forms.Label();
            this.lbPastion = new System.Windows.Forms.Label();
            this.lbPastiondsa = new System.Windows.Forms.Label();
            this.lbBlockname = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lstVehicle = new System.Windows.Forms.ListView();
            this.CarName = new System.Windows.Forms.ColumnHeader();
            this.DTStart = new System.Windows.Forms.ColumnHeader();
            this.DTEnd = new System.Windows.Forms.ColumnHeader();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbDeckName
            // 
            this.lbDeckName.AutoSize = true;
            this.lbDeckName.Location = new System.Drawing.Point(309, 14);
            this.lbDeckName.Name = "lbDeckName";
            this.lbDeckName.Size = new System.Drawing.Size(0, 17);
            this.lbDeckName.TabIndex = 21;
            // 
            // dada
            // 
            this.dada.AutoSize = true;
            this.dada.Location = new System.Drawing.Point(268, 14);
            this.dada.Name = "dada";
            this.dada.Size = new System.Drawing.Size(56, 17);
            this.dada.TabIndex = 20;
            this.dada.Text = "碾压层：";
            // 
            // lbPastion
            // 
            this.lbPastion.AutoSize = true;
            this.lbPastion.Location = new System.Drawing.Point(189, 14);
            this.lbPastion.Name = "lbPastion";
            this.lbPastion.Size = new System.Drawing.Size(0, 17);
            this.lbPastion.TabIndex = 19;
            // 
            // lbPastiondsa
            // 
            this.lbPastiondsa.AutoSize = true;
            this.lbPastiondsa.Location = new System.Drawing.Point(152, 14);
            this.lbPastiondsa.Name = "lbPastiondsa";
            this.lbPastiondsa.Size = new System.Drawing.Size(44, 17);
            this.lbPastiondsa.TabIndex = 18;
            this.lbPastiondsa.Text = "标识：";
            // 
            // lbBlockname
            // 
            this.lbBlockname.AutoSize = true;
            this.lbBlockname.Location = new System.Drawing.Point(79, 14);
            this.lbBlockname.Name = "lbBlockname";
            this.lbBlockname.Size = new System.Drawing.Size(0, 17);
            this.lbBlockname.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 16;
            this.label1.Text = "仓面：";
            // 
            // lstVehicle
            // 
            this.lstVehicle.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CarName,
            this.DTStart,
            this.DTEnd});
            this.lstVehicle.FullRowSelect = true;
            this.lstVehicle.GridLines = true;
            this.lstVehicle.HideSelection = false;
            this.lstVehicle.Location = new System.Drawing.Point(12, 43);
            this.lstVehicle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstVehicle.Name = "lstVehicle";
            this.lstVehicle.Size = new System.Drawing.Size(403, 342);
            this.lstVehicle.TabIndex = 22;
            this.lstVehicle.UseCompatibleStateImageBehavior = false;
            this.lstVehicle.View = System.Windows.Forms.View.Details;
            // 
            // CarName
            // 
            this.CarName.Text = "车辆";
            this.CarName.Width = 95;
            // 
            // DTStart
            // 
            this.DTStart.Text = "开始时间";
            this.DTStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DTStart.Width = 150;
            // 
            // DTEnd
            // 
            this.DTEnd.Text = "结束时间";
            this.DTEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DTEnd.Width = 150;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(168, 398);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(91, 27);
            this.btnOK.TabIndex = 23;
            this.btnOK.Text = "关闭(&O)";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // VehicleHistory
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(426, 436);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lstVehicle);
            this.Controls.Add(this.lbDeckName);
            this.Controls.Add(this.dada);
            this.Controls.Add(this.lbPastion);
            this.Controls.Add(this.lbPastiondsa);
            this.Controls.Add(this.lbBlockname);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VehicleHistory";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "车辆派遣历史";
            this.Load += new System.EventHandler(this.VehicleHistory_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbDeckName;
        private System.Windows.Forms.Label dada;
        private System.Windows.Forms.Label lbPastion;
        private System.Windows.Forms.Label lbPastiondsa;
        private System.Windows.Forms.Label lbBlockname;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lstVehicle;
        private System.Windows.Forms.ColumnHeader CarName;
        private System.Windows.Forms.ColumnHeader DTStart;
        private System.Windows.Forms.ColumnHeader DTEnd;
        private System.Windows.Forms.Button btnOK;
    }
}