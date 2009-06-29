using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DamLKK.DB;

namespace DamLKK.Forms
{
    public partial class DeckInfo : Form
    {
        DamLKK._Model.Deck deck = new DamLKK._Model.Deck();
        DeckDAO Segmentdao = DeckDAO.GetInstance();
        string blockName;
        bool isWorking=false;

        public bool IsWorking
        {
            get { return isWorking; }
            set { isWorking = value; }
        }

        public string BlockName
        {
            get { return blockName; }
            set { blockName = value; }
        }

        public DamLKK._Model.Deck Deck
        {
            get { return deck; }
            set { deck = value; }
        }

        
        public DeckInfo()
        {
            InitializeComponent();
        }
       
        private void MaxSpeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == Convert.ToChar(".")))
            {
                e.Handled = true;
            }
        }

        private void ErrorParam_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == Convert.ToChar(".")))
            {
                e.Handled = true;
            }
        }

        private void StratZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == Convert.ToChar(".")))
            {
                e.Handled = true;
            }
        }

        private void SpreadZ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b' || e.KeyChar == Convert.ToChar(".")))
            {
                e.Handled = true;
            }
        }

        private void DesignRollCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b'))
            {
                e.Handled = true;
            }
        }

        private void OpenDeckInfo_Load(object sender, EventArgs e)
        {

#if !DEBUG
            if (isWorking)
            {
                btnOK.Enabled = false;
                cbSpeedUnit.Enabled = false;
                lbBlockname.Enabled = false;
                lbPastion.Enabled = false;
                tbDeckName.Enabled = false;
                tbNLibCounts.Enabled = false;
                tbLibCounts.Enabled = false;
                tbMaxSpeed.Enabled = false;
                txStartZ.Enabled = false;
                txDesignDepth.Enabled = false;
                txErrorParam.Enabled = false;
            }
#endif
            cbSpeedUnit.SelectedIndex = 1;
            lbBlockname.Text = BlockName;
            lbPastion.Text = deck.Elevation.Height.ToString();
            if (deck.Elevation.Height < 100)
            {
                lbPastiondsa.Text = "第" + deck.Elevation.Height.ToString() + "斜层";
                lbPastion.Visible = false;
            }
            this.tbDeckName.Text = deck.Name;
            this.tbNLibCounts.Text = deck.NOLibRollCount.ToString();
            this.tbLibCounts.Text = deck.LibRollCount.ToString();
            this.tbMaxSpeed.Text = deck.MaxSpeed.ToString("0.00");
            this.txStartZ.Text = deck.StartZ.ToString("0.00");
            this.txDesignDepth.Text = deck.DesignDepth.ToString("0.00");
            this.txErrorParam.Text = deck.ErrorParam.ToString("0.00");
            if (Convert.ToInt32(tbNLibCounts.Text)==0)
                this.tbNLibCounts.Text = "2";
            if (Convert.ToInt32(tbLibCounts.Text)==0)
                this.tbLibCounts.Text = "6";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private  int GetIdxValue(int idx)
        {
            switch (idx)
            {
            case 0:
                    return 0;
            case 1:
                    return 3;
            case 2:
                    return 2;
            case 3:
                    return 1;
                default:
                    return -1;
            }
        }

        private int GetValueIdx(int idx)
        {
            switch (idx)
            {
                case 0:
                    return 0;
                case 3:
                    return 1;
                case 2:
                    return 2;
                case 1:
                    return 3;
                default:
                    return -1;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (tbDeckName.Text.Equals(""))
            {
                MessageBox.Show("仓面名称不能为空！");
            }
            else if (Convert.ToInt32(tbNLibCounts.Text) == 0 || Convert.ToInt32(tbLibCounts.Text) == 0 || tbNLibCounts.Text == string.Empty || tbLibCounts.Text == string.Empty || tbNLibCounts.Text.Equals("") || Convert.ToInt32(tbNLibCounts.Text) == 0 || Convert.ToSingle(txErrorParam.Text) == 0 || txErrorParam.Text.Equals("") || Convert.ToSingle(tbMaxSpeed.Text) == 0 || tbMaxSpeed.Text.Equals("") || txDesignDepth.Text.Equals("") || Convert.ToSingle(txDesignDepth.Text) == 0)
            {
                MessageBox.Show("输入数值信息不能为0或为空！");
            }
            else
            {
                DialogResult dr = MessageBox.Show("您确定保存仓面信息？", "确认输入", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    if (cbSpeedUnit.SelectedIndex == 0)
                    {
                        deck.MaxSpeed = Convert.ToSingle(tbMaxSpeed.Text) * 3.6;
                    }
                    else
                    {
                        deck.MaxSpeed = Convert.ToDouble(tbMaxSpeed.Text);
                    }
                    this.DialogResult = DialogResult.OK;
                    deck.NOLibRollCount =Convert.ToInt32(tbNLibCounts.Text);
                    deck.LibRollCount = Convert.ToInt32(tbLibCounts.Text);
                    deck.ErrorParam = Convert.ToDouble(txErrorParam.Text);
                    deck.StartZ = (float)Convert.ToDouble(txStartZ.Text);
                    deck.DesignDepth = (float)Convert.ToDouble(txDesignDepth.Text);
                    deck.Name = tbDeckName.Text;
                }
                else
                {
                    this.DialogResult = DialogResult.No;
                }
            }
        }


        float meterPerSecond = 0.0f;
        float kmPerHour = 0.0f;
        private float ToKMPerHour(float meterPerSecond)
        {
            return kmPerHour = meterPerSecond * 3.6f;

        }
        private float ToMeterPerSecond(float kmPerHour)
        {
            return meterPerSecond = kmPerHour / 3.6f;
        }
        private void cbSpeedUnit_TextChanged(object sender, EventArgs e)
        {
            if (cbSpeedUnit.SelectedIndex == 1)
            {
                tbMaxSpeed.Text = ToKMPerHour(Convert.ToSingle(tbMaxSpeed.Text)).ToString();
            }
            else
            {
                tbMaxSpeed.Text = ToMeterPerSecond(Convert.ToSingle(tbMaxSpeed.Text)).ToString();
            }
        }

        private void DeckInfo_Shown(object sender, EventArgs e)
        {
#if !DEBUG
            if (isWorking)
            {
                MessageBox.Show("仓面正在运行或者已经结束，无法修改仓面属性！");
            }
#endif
        }
    }
}
