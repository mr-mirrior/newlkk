using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace DamLKK
{
    public delegate void InvokeDelegate();
    static class Program
    {
        static volatile bool exiting = false;
        public static bool Exiting
        {
            get { return Program.exiting; }
            set { Program.exiting = value; _Control.GPSServer.Stop(); }
        }

        static void SetWorkingDirectory()
        {
            // exe文件所在目录
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
        }

        static void Init()
        {
            _Control.VehicleControl.ReadVehicleInfo();
            _Control.VehicleControl.LoadCarDistribute();
            _Control.LayerControl.Instance.LoadWorkingLayer();
            _Control.WarningControl.Init();
            DamLKK._Model.Dam.GetInstance();
            DB.DBconfig.GetInstance();
            dlg.Finished = true;
            // 开始接受GPS线程
            _Control.GPSServer.StartReceiveGPSData(DB.DBconfig.GetInstance().Damserver/*"125.38.51.59"*/, 6666);
            System.Diagnostics.Debug.Print("Init finished");
            if (!System.IO.File.Exists(_Model.Config.CONFIG_FILE))
                _Model.Config.Save();
            _Model.Config.I = DamLKK.Utils.Xml.XMLUtil<_Model.Config>.LoadXml(_Model.Config.CONFIG_FILE);
        }

        /// <summary>
        /// 应用程序的主入口点。
        static Forms.Waiting dlg = null;
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SetWorkingDirectory();

            if (!DamLKK.DB.DBconfig.GetInstance().Init())
            {
                Utils.MB.Error("数据库配置无法读取，请检查db.config文件！");
                return;
            }


#if !DEBUG
            dlg = new DamLKK.Forms.Waiting();
            dlg.Start(null, "请稍候，正在读取数据库……", Init, 2000);

            Forms.Login ldlg = new DamLKK.Forms.Login();
            ldlg.ShowDialog();
            if (ldlg.DialogResult==DialogResult.OK)
            {
                Utils.MB.OKI("登录成功！");

                Application.Run(new Forms.Main());
            }
#else
            dlg = new DamLKK.Forms.Waiting();
            dlg.Start(null, "请稍候，正在读取数据库……", Init, 2000);
#endif
            //DamLKK.Geo.Coord coord = new DamLKK.Geo.Coord(50229.8203125, 8426.7900390625);
            //DamLKK.Geo.Coord temp;
            //temp = coord.ToDamAxisCoord();
            //coord = temp.ToEarthCoord();
#if DEBUG
            Application.Run(new Forms.Main());
#endif
            
        }
    }
}
