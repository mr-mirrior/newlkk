using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DamLKK._Model;
using DamLKK.DB;

namespace DamLKK.Forms
{
    public partial class VehicleHistory : Form
    {
        public VehicleHistory()
        {
            InitializeComponent();
        }

        DamLKK._Model.Deck deck;

        public DamLKK._Model.Deck Deck
        {
            get { return deck; }
            set { deck = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VehicleHistory_Load(object sender, EventArgs e)
        {
            lstVehicle.Items.Clear();
            if (deck == null || deck.Name==string.Empty)
                return;
            lbBlockname.Text = UnitDAO.GetInstance().GetName(deck.Unit.ID);
            lbDeckName.Text = deck.Name;
            lbPastion.Text = deck.Elevation.Height.ToString("0.0");
            List<RollerDis> carsDistributed = CarDistributeDAO.GetInstance().GetCarsInDeck_Distributed(deck.Unit.ID, deck.Elevation.Height, deck.ID);
            if (carsDistributed.Count == 0)
                return;
            string start, end;
            foreach (RollerDis info in carsDistributed)
            {
                ListViewItem item = lstVehicle.Items.Add(_Control.VehicleControl.FindVechicle(info.RollerID).Name);
                if (info.DTStart == DateTime.MinValue)
                    start = "尚未开始";
                else
                    start = info.DTStart.ToString();
                if (info.DTEnd == DateTime.MinValue)
                    end = "尚未结束";
                else
                    end = info.DTEnd.ToString();
                item.SubItems.AddRange(new string[] { start, end });
            }
        }
    }
}
