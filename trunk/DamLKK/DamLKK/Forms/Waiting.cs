using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DamLKK.Forms
{
    public partial class Waiting : Form
    {
        public Waiting()
        {
            InitializeComponent();
        }

        public string Prompt { get { return wait.Prompt; } set { wait.Prompt = value; } }
        volatile bool finished = false;
        public bool Finished { get { return finished; } set { finished = value; } }
        public void Start(System.Windows.Forms.IWin32Window owner, string caption, ThreadStart proc, int minMS)
        {
            Application.UseWaitCursor = true;
            this.Prompt = caption;
            this.Show(owner);
            Application.DoEvents();
            Thread thrd = new Thread(proc);
            thrd.Start();
            int count = minMS;
            DateTime justnow = DateTime.Now;
            while (true)
            {
                if (finished)
                    break;
                TimeSpan ts = DateTime.Now - justnow;
                if (!thrd.IsAlive && ts.TotalMilliseconds > minMS)
                    break;

                Application.DoEvents();
                Thread.Sleep(1);
                minMS--;
            }
            this.Hide();

            Application.UseWaitCursor = false;
        }
    }
}
