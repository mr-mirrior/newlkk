using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using DamLKK._Control;

namespace DamLKK.Forms
{
    public partial class Warning : Form, IMessageFilter
    {
        private static Warning warningObj = new Warning();
        public static Warning WarningObj { get { return warningObj; } }

        MouseOffsetRecorder mr = new MouseOffsetRecorder();
        bool boolFormDrag;
        int i = 10;

        WarningType warningType=WarningType.OVERTHICKNESS;

        public enum SenseOrganState{None = 0, High = 1, Low = 2, Normal = 3};
/*
此枚举各值含义：

None：	不振
High：	高频低振
Low：	低频高振
Normal：震动	//此值只适用于只有两种状态的碾压机*/


        int librateState;

        public int LibrateState
        {
            get { return librateState; }
            set { librateState = value; }
        }
        public static string GetLibratedString(int state)
        {
            switch (state)
            {
            case 0:
                    return "未振动";
            case 1:
                    return "高频低振";
            case 2:
                    return "低频高振";
            case 3:
                    return "振动";
                default:
                    return string.Empty;
            }
        }

        public WarningType WarningType
        {
            get { return warningType; }
            set { warningType = value; }
        }
        string carName = "四号碾压机";

        public string CarName
        {
            get { return carName; }
            set { carName = value; }
        }
        string warningTime = "12:22:24";

        public string WarningTime
        {
            get { return warningTime; }
            set { warningTime = value; }
        }
        string warningDate = "2008-03-24";

        public string WarningDate
        {
            get { return warningDate; }
            set { warningDate = value; }
        }
        string _UnitName = "ED";

        public string UnitName
        {
            get { return _UnitName; }
            set { _UnitName = value; }
        }
        double designZ = 666.2;

        public double DesignZ
        {
            get { return designZ; }
            set { designZ = value; }
        }
        string deckName = "仓面1";

        public string DeckName
        {
            get { return deckName; }
            set { deckName = value; }
        }
        double maxSpeed = 2;
        /// <summary>
        /// 设置速度
        /// </summary>
        public double MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }
        double thisSpeed = 2.43;
        /// <summary>
        /// 超速时的即时速度
        /// </summary>
        public double ThisSpeed
        {
            get { return thisSpeed; }
            set { thisSpeed = value; }
        }

        //碾压遍数不足需要指标
        double totalAreaRatio=0;

        public double TotalAreaRatio
        {
            get { return totalAreaRatio; }
            set { totalAreaRatio = value; }
        }
        double shortRollerArea=0;

        public double ShortRollerArea
        {
            get { return shortRollerArea; }
            set { shortRollerArea = value; }
        }

        //碾压过厚指标

        double designDepth=0;

        public double DesignDepth
        {
            get { return designDepth; }
            set { designDepth = value; }
        }

        Geo.GPSCoord coord;

        public Geo.GPSCoord GPSCoord
        {
            get { return coord; }
            set { coord = value; }
        }

        string overMerter=string.Empty;

        public string OverMerter
        {
            get { return overMerter; }
            set { overMerter = value; }
        }

        string position="563.2";

        public string Position
        {
            get { return position; }
            set { position = value; }
        }

        double startZ=0;

        public double StartZ
        {
            get { return startZ; }
            set { startZ = value; }
        }
        double area=0;
        public double ActualArea { get { return area; } set { area = value; } }

        public Warning()
        {
            InitializeComponent();
            Application.AddMessageFilter(this);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Color c1 = Color.FromArgb(179, 14, 32);
            Color c2 = Color.FromArgb(92, 0, 0);
            Point pt1 = new Point(panel1.Left, panel1.Top);
            Point pt2 = new Point(panel1.Left, panel1.Bottom);
            using (Brush b = new LinearGradientBrush(pt1, pt2, c1, c2))
            {
                g.FillRectangle(b, 0, 0, panel1.Width, panel1.Height);
            }

            Rectangle rc = panel1.ClientRectangle;
            rc.Inflate(-1, -1);
            rc.Width--;
            rc.Height--;
            using (Pen p = new Pen(Color.White, 3))
                g.DrawRectangle(p, rc);
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void panel1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = false;
        }


        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.boolFormDrag = true;
                mr.Record();
            }

        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.boolFormDrag == true)
            {
                this.Location = new Point(this.Location.X + mr.offsetX, this.Location.Y + mr.offsetY);
                mr.Record();
            }

        }

        public string getWarningType(WarningType warningType)
        {
            if (warningType == WarningType.OVERSPEED)
            {
                return "当前速度为：！";
            }
            else if (warningType == WarningType.OVERTHICKNESS)
            {
                return "碾压过厚！";
            }
            else if (warningType == WarningType.ROLLINGLESS)
            {
                return "标准遍数百分比为：";
            }
            else if (warningType== WarningType.LIBRATED)
            {
                return "当前振动状态为：";
            }
            return null;
        }

        private void Warning_Load(object sender, EventArgs e)
        {
            timer1.Start();
            FillForms();
        }

        public void FillForms()
        {

            //记录下错误信息保存到结构体列表
            WarningInfo warningInfo = new WarningInfo();

            warningInfo.warnType = this.warningType;
            warningInfo._UnitName = this._UnitName;
            warningInfo.carName = this.carName;
            warningInfo.deckName = this.deckName;
            warningInfo.designZ = this.designZ;
            warningInfo.warningDate = this.warningDate;
            warningInfo.warningTime = this.warningTime;
            warningInfo.libratedState = this.librateState;
            warningInfo.maxSpeed = this.maxSpeed;
            warningInfo.thisSpeed = this.thisSpeed;
            warningInfo.ActualArea = this.ActualArea;

            warningInfo.totalArea = this.totalAreaRatio;
            warningInfo.shortRollerArea = this.shortRollerArea;

            warningInfo.designDepth = this.designDepth;
            warningInfo.startZ = this.startZ;
            warningInfo.coord3d = this.coord;
            warningInfo.overMeter = this.overMerter;
            warningInfo.position = this.position;

            bool has = false;
            foreach (WarningInfo wi in WarningControl.listWarinInfo)
            {
                if
                (wi.warnType == this.warningType &&
                 wi._UnitName == this._UnitName &&
                 wi.carName == this.carName &&
                 wi.deckName == this.deckName &&
                 wi.designZ == this.designZ &&
                 wi.maxSpeed == this.maxSpeed &&
                 wi.thisSpeed == this.thisSpeed &&
                 wi.totalArea == this.totalAreaRatio &&
                 wi.shortRollerArea == this.shortRollerArea &&
                 wi.designDepth == this.designDepth &&
                 wi.startZ == this.startZ &&
                 wi.warningDate.Equals(this.warningDate) &&
                 wi.libratedState==this.librateState &&
                 wi.warningTime.Equals(this.warningTime))
                    has = true;
            }
            if (!has)
            {
                WarningControl.listWarinInfo.Add(warningInfo);
            }


            FillLabelsValue();
        }

        public void FillLabelsValue()
        {
            lbVehicleName.Text = this.carName;
            lbWarningTime.Text = this.warningTime;
            lbWarningType.Text = getWarningType(this.warningType);
            double proportion;
            if (this.warningType == WarningType.OVERSPEED)
            {
                if (maxSpeed == 0 || thisSpeed < maxSpeed)
                {
                    return;
                }
                lb.Text = "超速警报：";
                //proportion = (this.thisSpeed - this.maxSpeed) / this.maxSpeed * 100;
                lbProportion.Text = this.thisSpeed.ToString("0.00") + "Km/h"; /*proportion.ToString("0.00") + "%";*/

                string toolVehicle = "单元：" + this._UnitName + ",高程：" + this.designZ.ToString() + ",仓面：" + this.deckName;
                string toolProportion = "限速：" + this.maxSpeed.ToString("0.00") + "千米/小时,实际：" + this.thisSpeed.ToString("0.00") + "千米/小时";
                toolTip1.SetToolTip(lbVehicleName, toolVehicle);
                toolTip1.SetToolTip(lbProportion, toolProportion);
            }
            else if (warningType == WarningType.ROLLINGLESS)
            {
                if (totalAreaRatio == 0)
                {
                    return;
                }

                lb.Text = "碾压简报：";
                proportion = (this.totalAreaRatio - this.shortRollerArea) /*/ totalAreaRatio*/ * 100;
                lbProportion.Text = proportion.ToString("0.00") + "%";

                string toolVehicle = "单元：" + this._UnitName + ",高程：" + this.designZ.ToString();
                string toolProportion = "碾压合格面积：" + ((this.totalAreaRatio - this.shortRollerArea) * this.ActualArea).ToString("0.00") +
                    "平方千米，总面积：" + this.ActualArea.ToString("0.00") + "平方千米";
                lbVehicleName.Text = this.deckName;
                toolTip1.SetToolTip(lbVehicleName, toolVehicle);
                toolTip1.SetToolTip(lbProportion, toolProportion);
            }
            //else if (warningObj.warningType == WarningType.OVERTHICKNESS)
            //{
            //    return;
            //    if (warningObj.designDepth == 0)
            //    {
            //        return;
            //    }
            //    warningObj.lb.Text = "告警：";
            //    warningObj.lbWarningType.Text = "碾压过厚！";
            //    //proportion = (coord.Z - startZ - designDepth) / designDepth * 100;
            //    warningObj.lbProportion.Text = warningObj.overMerter;

            //    string toolVehicle = "分区：" + warningObj.blockName + ",高程：" + this.designZ.ToString();
            //    string toolProportion = "碾压过厚位置：" + warningObj.position;
            //    lbVehicleName.Text = warningObj.deckName;
            //    toolTip1.SetToolTip(warningObj.lbVehicleName, toolVehicle);
            //    toolTip1.SetToolTip(warningObj.lbProportion, toolProportion);
            //}
            else if (warningType == WarningType.LIBRATED)
            {
                lb.Text = "振动警报：";
                lbWarningType.Text = "当前振动状态为：";
                lbProportion.Text = GetLibratedString(librateState);

                lbVehicleName.Text = this.carName;
                lbWarningTime.Text = this.warningTime;
            }

            toolTip1.SetToolTip(lbWarningTime, warningDate);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.boolFormDrag == true)
            {
                this.boolFormDrag = false;
                mr.Record();
            }
        }

        private void Warning_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Visible=false;
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (i == 0)
            {
                timer1.Stop();
                this.Visible = false;
                //this.Dispose();
            }

            lbTimer.Text = i.ToString();
            i--;

        }
        //public new void Show()
        //{
        //    this.Show(Main.MainWindow);
        //}
        public bool PreFilterMessage(ref Message msg)
        {
            if (IsDisposed)
                return false;

            const int WM_LBUTTONDOWN = 0x0201;
            //const int WM_LBUTTONUP = 0x0202;
            //const int WM_LBUTTONDBLCLK = 0x0203;
            //const int WM_RBUTTONDOWN = 0x0204;
            //const int WM_RBUTTONUP = 0x0205;
            //const int WM_RBUTTONDBLCLK = 0x0206;
            //const int WM_MBUTTONDOWN = 0x0207;
            //const int WM_MBUTTONUP = 0x0208;
            //const int WM_MBUTTONDBLCLK = 0x0209;
            //const int WM_KEYDOWN = 0x0100;
            //const int WM_KEYUP = 0x0101;
            //const int WM_MOUSEWHEEL = 0x020A;
            //const int WM_HSCROLL = 0x0114;
            //const int WM_VSCROLL = 0x0115;
            //const int WM_CHAR = 0x0102;
            int x = (int)msg.LParam;
            Point pt = new Point(x & 0xFFFF, x >> 16);
            Rectangle rc = this.RectangleToScreen(this.ClientRectangle);

            if (msg.Msg == WM_LBUTTONDOWN && rc.Contains(pt))
            {
                timer1.Stop();
                lbTimer.Visible = false;
                lbMessage.Visible = false;
            }
            return false;
        }

        private void lbMessage_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            lbTimer.Visible = false;
            lbMessage.Visible = false;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            lbTimer.Visible = false;
            lbMessage.Visible = false;
        }



    }
    ///   <summary>   
    ///   记录鼠标在两个动作间的位移   
    ///   </summary>   
    public class MouseOffsetRecorder
    {
        private Point StartPosition = new Point();

        //   水平位置上的位移   
        public int offsetX
        {
            get
            {
                return this.GetCurrentPosition().X - this.StartPosition.X;
            }
        }
        //   垂直位置上的位移   
        public int offsetY
        {
            get
            {
                return this.GetCurrentPosition().Y - this.StartPosition.Y;
            }
        }

        public void Record()
        {
            this.StartPosition = this.GetCurrentPosition();
        }
        protected Point GetCurrentPosition()
        {
            return new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
        }
    }


}
