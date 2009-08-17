using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DamLKK.DB;
using System.Data.SqlClient;
using DamLKK._Model;

namespace DamLKK.Forms
{
    public partial class AssignVehicle : Form
    {
        bool Dresult;

        //引用数据库类的实例
        CarDistributeDAO cardisDAO = CarDistributeDAO.GetInstance();
        RollerDAO carInfoDAO = RollerDAO.GetInstance();
        DamLKK._Model.RollerDis carDis = new DamLKK._Model.RollerDis();
        List<int> Cars = new List<int>();
        List<DamLKK._Model.Roller> _AllCars = _Control.VehicleControl.vehiclesInfo;


        DamLKK._Model.Deck _Deck = new DamLKK._Model.Deck();

        public DamLKK._Model.Deck Deck
        {
            get { return _Deck; }
            set { _Deck = value; }
        }
        string blockName;

        public string BlockName
        {
            get { return blockName; }
            set { blockName = value; }
        }

        Button[] buttons;
        public AssignVehicle()
        {
            InitializeComponent();
            buttons = new Button[] { btnStop0, btnStop1, btnStop2, btnStop3, btnStop4, 
            btnStop5, btnStop6, btnStop7,btnStop8,btnStop9,btnStop10,btnStop11,btnStop12,btnStop13,btnStop14,btnStop15 };
            //添加所有按钮的事件
            btnStop1.Click += new EventHandler(btnStop_Click);
            btnStop2.Click += new EventHandler(btnStop_Click);
            btnStop3.Click += new EventHandler(btnStop_Click);
            btnStop4.Click += new EventHandler(btnStop_Click);
            btnStop5.Click += new EventHandler(btnStop_Click);
            btnStop6.Click += new EventHandler(btnStop_Click);
            btnStop7.Click += new EventHandler(btnStop_Click);
            btnStop8.Click += new EventHandler(btnStop_Click);
            btnStop9.Click += new EventHandler(btnStop_Click);
            btnStop10.Click += new EventHandler(btnStop_Click);
            btnStop11.Click += new EventHandler(btnStop_Click);
            btnStop12.Click += new EventHandler(btnStop_Click);
            btnStop13.Click += new EventHandler(btnStop_Click);
            btnStop14.Click += new EventHandler(btnStop_Click);
            btnStop15.Click += new EventHandler(btnStop_Click);
        }
        public AssignVehicle(DamLKK._Model.Unit unit, double designZ, int segmentid)
        {
            InitializeComponent();
            _Deck.Unit = unit;
            _Deck.Elevation = new DamLKK._Model.Elevation(designZ);
            _Deck.ID = segmentid;
        }

        private void SendVehicle_Load(object sender, EventArgs e)
        {
            //初始化button
            for (int i = 0; i < buttons.Length; i++)
            {
                lstVehicle.Items.Add("");
                buttons[i].Location = new Point(buttons[i].Location.X, lstVehicle.Items[i].Position.Y + lstVehicle.Location.Y);
            }

            //初始化信息条
            lbBlockname.Text = blockName;
            lbPastion.Text = _Deck.Elevation.Height.ToString();
            lbDeckName.Text = _Deck.Name;
            //在Vehical listView 中显示闲置车辆
            DeckDAO segmentDAO = DeckDAO.GetInstance();
            
            _Deck = DeckDAO.GetInstance().GetDeck(_Deck.Unit.ID, _Deck.Elevation.Height, _Deck.ID);
            if (_Deck==null)
                return;
          

            //根据舱面状态来选择显示内容
            if (_Deck.WorkState == DeckWorkState.WORK)
            {
                UpdateData();
            }
            else if (_Deck.WorkState == DeckWorkState.WAIT)
            {
                UpdateAssignData();
            }
            else if (_Deck.WorkState == DeckWorkState.END)
            {
                UpdateAssignData();
            }
        }


        private void btnSend_Click(object sender, EventArgs e)
        {
            if (lstVehicle.SelectedItems.Count == 0)
            {
                MessageBox.Show("您没选中空闲车辆！");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lstVehicle.CheckedItems.Count < 1)
            {
                MessageBox.Show("请勾选要派遣的车辆！");
                return;
            }

            if (_Deck.WorkState == DeckWorkState.WORK)
            {
                DialogResult result = MessageBox.Show(
              "仓面正在运行中，\n" +
              "添加到此仓面的车辆将立刻开始工作！\n\n" +
              "按\"是\"确认添加该车，按\"否\"取消操作",
              "添加车辆", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach (ListViewItem item in lstVehicle.CheckedItems)
                    {
                        int carid = carInfoDAO.GetCarNameByCarID(_AllCars, item.Text);
                        Cars.Add(carid);
                        RollerDis thisCardis = new RollerDis();
                        thisCardis.UnitID = _Deck.Unit.ID;
                        thisCardis.Elevation = _Deck.Elevation.Height;
                        thisCardis.SegmentID = _Deck.ID;
                        thisCardis.RollerID = carid;

                        Dresult = cardisDAO.StartCar(thisCardis, _Deck.MaxSpeed, 0, _Deck.Elevation.Height);
                        _Control.GPSServer.UpdateDeck();
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else if (_Deck.WorkState == DeckWorkState.WAIT)
            {
                DialogResult dr = MessageBox.Show("请确认操作", "确认派遣", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    foreach (ListViewItem item in lstVehicle.CheckedItems)
                    {
                        int carid = carInfoDAO.GetCarNameByCarID(_AllCars, item.Text);
                        Cars.Add(carid);
                        item.Tag = CarDis_Status.ASSIGNED;
                    }
                    Dresult = cardisDAO.DistributeCars(_Deck.Unit.ID, _Deck.Elevation.Height, _Deck.ID, Cars);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else if (_Deck.WorkState == DeckWorkState.END)
            {
                DialogResult dr = MessageBox.Show("请确认操作", "确认派遣", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    foreach (ListViewItem item in lstVehicle.CheckedItems)
                    {
                        int carid = carInfoDAO.GetCarNameByCarID(_AllCars, item.Text);
                        Cars.Add(carid);
                        item.Tag = CarDis_Status.ASSIGNED;
                    }
                    Dresult = cardisDAO.DistributeCars(_Deck.Unit.ID, _Deck.Elevation.Height, _Deck.ID, Cars);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }


        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Vehical_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if ((CarDis_Status)lstVehicle.Items[e.Index].Tag == CarDis_Status.WORK)
            {
                e.NewValue = e.CurrentValue;
            }
            else if (((CarDis_Status)lstVehicle.Items[e.Index].Tag) != CarDis_Status.FREE)
            {
                e.NewValue = e.CurrentValue;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            DeckDAO segmentDAO = DeckDAO.GetInstance();
            Deck segment = new Deck();
    
            segment = segmentDAO.GetDeck(_Deck.Unit.ID, _Deck.Elevation.Height, _Deck.ID);

            int i;
            if (lstVehicle.Items.Count < 1)
            {
                return;
            }
            for (i = 0; i < buttons.Length; i++)
            {
                if (((Button)sender) == buttons[i])
                {
                    lstVehicle.Items[i].Selected = true;
                }
            }

            RollerDis endCardis = new RollerDis();
            Control c = (sender as Control);
            if ((CarDis_Status)c.Tag == CarDis_Status.WORK)
            {
                DialogResult result = MessageBox.Show(
                "车辆正在运行，\n" +
                "强制停止可能影响计算结果！\n\n" +
                "按\"是\"确认强制停止该车，按\"否\"取消操作",
                "强制停止车辆", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (CarDistributeDAO.GetInstance().GetCarDisInDeck_Inuse(_Deck.Unit.ID, _Deck.Elevation.Height, _Deck.ID).Count == 1)
                    {
                        Utils.MB.Warning("不能在工作仓面中结束唯一工作的车辆，请结束碾压监控！");
                        return;
                    }
                    ListViewItem item = lstVehicle.SelectedItems[0];
                    item.Tag = CarDis_Status.FREE;
                    c.Tag = CarDis_Status.FREE;
                    endCardis.RollerID= carInfoDAO.GetCarNameByCarID(_AllCars, item.Text);
                    endCardis.UnitID= _Deck.Unit.ID;
                    endCardis.Elevation = _Deck.Elevation.Height;
                    endCardis.SegmentID= _Deck.ID;
                    cardisDAO.EndCar(endCardis);
                    

                    if (segment==null)
                    {
                        MessageBox.Show("无此仓面!");
                        return;
                    }

                    _Control.GPSServer.UpdateDeck();
                    this.Close();

                }
            }
            else if ((CarDis_Status)c.Tag == CarDis_Status.ASSIGNED)
            {
                DialogResult result = MessageBox.Show(
               "车辆已经在之前分配过，\n" +
               "取消分配可以去掉之前的分配记录！\n\n" +
               "按\"是\"确认取消分配，按\"否\"取消操作",
               "取消分配操作车辆", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    endCardis.RollerID= carInfoDAO.GetCarNameByCarID(_AllCars, lstVehicle.SelectedItems[0].Text);
                    endCardis.UnitID = _Deck.Unit.ID;
                    endCardis.Elevation = _Deck.Elevation.Height;
                    endCardis.SegmentID = _Deck.ID;
                    cardisDAO.RemoveCar(endCardis);
                }
                

                if (segment == null)
                {
                    MessageBox.Show("无此仓面!");
                    return;
                }

                if (segment.WorkState == DeckWorkState.WORK)
                {
                    UpdateData();
                }
                else if (segment.WorkState == DeckWorkState.WAIT)
                {
                    UpdateAssignData();
                }
                else if (segment.WorkState == DeckWorkState.END)
                {
                    UpdateAssignData();
                }
            }

        }

        /// <summary>
        /// 返回是否被占有
        /// </summary>
        private bool IsOccupied(int idx)
        {
            if (idx < 0 || idx >= lstVehicle.Items.Count)
                return false;
            return lstVehicle.Items[idx].Tag != null;
        }

        /// <summary>
        /// 仓面在工作状态时更新列表
        /// </summary>
        private void UpdateData()
        {
            lstVehicle.Items.Clear();
            for (int j = 0; j < buttons.Length; j++)
            {
                buttons[j].Visible = false;
            }

            List<Roller> inUsedAtThisDeck = cardisDAO.GetCarsInDeck_Inuse(_Deck.Unit.ID, _Deck.Elevation.Height, _Deck.ID);
            List<RollerDis> cds = cardisDAO.GetCarDisInDeck(_Deck.Unit.ID, _Deck.Elevation.Height, _Deck.ID);
            List<Roller> unUsedCars = new List<Roller>();
            List<Roller> InUsedNotAtThisDeck = new List<Roller>();
            List<Roller> allInUsedCars = CarDistributeDAO.GetInstance().GetInusedCars();

            //获取没在工作的所有车辆

            foreach (Roller ci in _AllCars)
            {
                int j = 0;
                for (int k = 0; k < allInUsedCars.Count; k++)
                {
                    if (ci.ID != allInUsedCars[k].ID)
                    {
                        j++;
                    }
                }
                if (j == allInUsedCars.Count)
                {
                    unUsedCars.Add(ci);
                }

            }
            //获取没有工作在此仓面的车辆信息

            foreach (Roller ci in allInUsedCars)
            {
                int j = 0;
                for (int k = 0; k < inUsedAtThisDeck.Count; k++)
                {
                    if (ci.ID != inUsedAtThisDeck[k].ID)
                    {
                        j++;
                    }
                }
                if (j == inUsedAtThisDeck.Count)
                {
                    InUsedNotAtThisDeck.Add(ci);
                }
            }

            //添加车辆信息
            int i = 0;
            foreach (Roller ci in unUsedCars)
            {
                lstVehicle.Items.Add(ci.Name);
                buttons[i].Tag = CarDis_Status.FREE;
                string status = "可分配";
                ListViewItem item = lstVehicle.Items[i];
                item.Tag = CarDis_Status.FREE;
                string info = ci.ScrollWidth.ToString();
#if DEBUG
                info = ci.ID.ToString();
#endif
                item.SubItems.AddRange(new string[] { info, ci.GPSHeight.ToString(), status });
                i++;
            }

            foreach (Roller ci in inUsedAtThisDeck)
            {

                lstVehicle.Items.Add(ci.Name);
                ListViewItem item = lstVehicle.Items[i];
                buttons[i].Tag = CarDis_Status.WORK;
                item.Tag = CarDis_Status.WORK;
                buttons[i].Visible = true;
                string status = "此占用";
                string info = ci.ScrollWidth.ToString();
#if DEBUG
                info = ci.ID.ToString();
#endif
                item.SubItems.AddRange(new string[] { info, ci.GPSHeight.ToString(), status });
                item.Checked = true;
                item.ForeColor = Color.DarkGray;
                item.BackColor = Color.WhiteSmoke;
                item.Font = new Font(this.Font, FontStyle.Bold);
                i++;
            }

            foreach (Roller ci in InUsedNotAtThisDeck)
            {
                lstVehicle.Items.Add(ci.Name);
                ListViewItem item = lstVehicle.Items[i];
                buttons[i].Tag = CarDis_Status.WORK;
                buttons[i].Visible = true;
                buttons[i].Enabled = false;
                string status = "已占用";
                item.Tag = CarDis_Status.WORK;
                string info = ci.ScrollWidth.ToString();
#if DEBUG
                info = ci.ID.ToString();
#endif
                item.SubItems.AddRange(new string[] { info, ci.GPSHeight.ToString(), status });
                item.ForeColor = Color.DarkGray;
                item.BackColor = Color.WhiteSmoke;
                item.Font = new Font(this.Font, FontStyle.Bold);
                i++;
            }
            lstVehicle.Items[0].Selected = true;
        }
        /// <summary>
        /// 舱面在wait&end状态更新显示列表
        /// </summary>
        private void UpdateAssignData()
        {
            List<Roller> inUsedAtThisDeck = cardisDAO.GetCarsInDeck_Inuse(_Deck.Unit.ID, _Deck.Elevation.Height, _Deck.ID);
            List<RollerDis> cds = cardisDAO.GetCarDisInDeck(_Deck.Unit.ID, _Deck.Elevation.Height, _Deck.ID);
            List<Roller> unUsedCars = new List<Roller>();
            List<Roller> InUsedNotAtThisDeck = new List<Roller>();
            List<Roller> allInUsedCars = CarDistributeDAO.GetInstance().GetInusedCars();
            List<RollerDis> allDistributeCars = cardisDAO.GetCarDisInDeck_all_except_end(_Deck.Unit.ID, _Deck.Elevation.Height, _Deck.ID);
            List<int> other = new List<int>();

            lstVehicle.Items.Clear();
            for (int j = 0; j < buttons.Length; j++)
            {
                buttons[j].Visible = false;
            }

            //获取没有工作在此仓面的车辆信息

            foreach (Roller ci in allInUsedCars)
            {
                int j = 0;
                for (int k = 0; k < inUsedAtThisDeck.Count; k++)
                {
                    if (ci.ID != inUsedAtThisDeck[k].ID)
                    {
                        j++;
                    }
                }
                if (j == inUsedAtThisDeck.Count)
                {
                    InUsedNotAtThisDeck.Add(ci);
                }
            }



            string status = "可分配";
            int i = 0;
            foreach (RollerDis cd in allDistributeCars)
            {
                foreach (Roller ci in _AllCars)
                {
                    if (ci.ID == cd.RollerID && cd.IsAssigned())
                    {
                        lstVehicle.Items.Add(ci.Name);
                        ListViewItem item = lstVehicle.Items[i];
                        item.Tag = CarDis_Status.ASSIGNED;
                        status = "已分配";
                        buttons[i].Text = "取消分配(&S)";
                        buttons[i].Tag = CarDis_Status.ASSIGNED;
                        buttons[i].Visible = true;
                        string info = ci.ScrollWidth.ToString();
#if DEBUG
                        info = ci.ID.ToString();
#endif
                        item.SubItems.AddRange(new string[] { info, ci.GPSHeight.ToString(), status });
                        i++;
                        break;
                    }

                }
                other.Add(cd.RollerID);
            }

            bool add = false;
            foreach (Roller ci in allInUsedCars)
            {
                add = false;
                if (ci == null)
                    continue;
                foreach (RollerDis cI in allDistributeCars)
                {
                    if (ci.ID == (cI.RollerID))
                    {
                        add = true;
                        break;
                    }
                }

                if (!add)
                {
                    lstVehicle.Items.Add(ci.Name);
                    ListViewItem item = lstVehicle.Items[i];
                    status = "正在工作，可分配";
                    buttons[i].Tag = CarDis_Status.FREE;
                    item.Tag = CarDis_Status.FREE;
                    buttons[i].Visible = false;
                    item.ForeColor = Color.DarkGray;
                    item.BackColor = Color.WhiteSmoke;
                    item.Font = new Font(this.Font, FontStyle.Bold);
                    string info = ci.ScrollWidth.ToString();
                    item.SubItems.AddRange(new string[] { info, ci.GPSHeight.ToString(), status });
                    other.Add(ci.ID);
                    i++;
                }
            }

            List<Roller> getOther = cardisDAO.GetOthers(other);
            foreach (Roller ci in getOther)
            {
                lstVehicle.Items.Add(ci.Name);
                ListViewItem item = lstVehicle.Items[i];
                status = "可分配";
                buttons[i].Tag = CarDis_Status.FREE;
                item.Tag = CarDis_Status.FREE;
                buttons[i].Visible = false;
                string info = ci.ScrollWidth.ToString();
#if DEBUG
                info = ci.ID.ToString();
#endif
                item.SubItems.AddRange(new string[] { info, ci.GPSHeight.ToString(), status });
                i++;
            }
            lstVehicle.Items[0].Selected = true;
        }
        private void AssignVehicle_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Dresult == true && _Deck.WorkState == DeckWorkState.WORK)
            {
                MessageBox.Show("添加车辆成功，车辆已经投入碾压中！");
            }
            else if (Dresult == true && _Deck.WorkState == DeckWorkState.WAIT)
            {
                MessageBox.Show("分配车辆成功！");
            }
            else if (Dresult == true && _Deck.WorkState == DeckWorkState.END)
            {
                MessageBox.Show("分配车辆成功！");
            }
        }
    }
}