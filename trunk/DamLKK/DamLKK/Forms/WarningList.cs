using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DamLKK._Control;

namespace DamLKK.Forms
{
    public partial class WarningList : Form
    {
        Forms.Warning warnForm;
        public WarningList()
        {
            InitializeComponent();
           warnForm = new Warning();
        }

        private void WarningList_Load(object sender, EventArgs e)
        {
            UpdateWarnList();
        }

        void UpdateWarnList()
        {
            //ListViewItem item = new ListViewItem();
            if (WarningControl.listWarinInfo.Count==0)
            {
                return;
            }

            lstWarnList.Items.Clear();

            int i = 0;
            foreach (WarningInfo wi in WarningControl.listWarinInfo)
            {
                ListViewItem item = new ListViewItem();
                item.Text = wi.warningTime + " " + wi.warningDate;
                lstWarnList.Items.Add(item);
                lstWarnList.Items[i].SubItems.Add((getWarningType(wi.warnType)));
                i++;
            }
        }

        public string getWarningType(WarningType wt)
        {
            if (wt == WarningType.OVERSPEED)
            {
                return "碾压超速！";
            }
            else if (wt == WarningType.OVERTHICKNESS)
            {
                return "碾压过厚！";
            }
            else if (wt == WarningType.ROLLINGLESS)
            {
                return "碾压简报！";
            }
            else if (wt == WarningType.LIBRATED)
            {
                return "振动警报！";
            }
            return null;
        }

        private void lstWarnList_DoubleClick(object sender, EventArgs e)
        {
            int index = lstWarnList.SelectedIndices[0];
            WarningInfo wi=WarningControl.listWarinInfo[index];


            warnForm.ThisSpeed = wi.thisSpeed;
            warnForm.DesignDepth = wi.designDepth;
            warnForm.LibrateState = wi.libratedState;
            warnForm.GPSCoord = wi.coord3d;
            warnForm.CarName = wi.carName;
            warnForm.MaxSpeed = wi.maxSpeed;
            warnForm.StartZ = wi.startZ;
            warnForm.Position = wi.position;
            warnForm.OverMerter = wi.overMeter;
            warnForm.UnitName = wi._UnitName;
            warnForm.DeckName = wi.deckName;
            warnForm.DesignZ = wi.designZ;
            warnForm.ShortRollerArea = wi.shortRollerArea;
            warnForm.TotalAreaRatio = wi.totalArea;
            warnForm.ActualArea = wi.ActualArea;
            warnForm.WarningDate = wi.warningDate;
            warnForm.WarningTime = wi.warningTime;
            warnForm.WarningType = wi.warnType;
            warnForm.FillLabelsValue();
            Forms.Main.GetInstance().ShowWarningDlg(warnForm);
        }
    }
}
