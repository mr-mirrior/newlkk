using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using DamLKK.DB;
using System.IO;
using System.Windows.Forms;
using DamLKK._Model;

namespace DamLKK._Control
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GPSDATA
    {
        public static int Size()
        {
            return Marshal.SizeOf(typeof(GPSDATA));
        }
        public override string ToString()
        {
            Geo.GPSCoord c3d = this.GPSCoord;
            return string.Format("GPSDATA {0} Car={1} Speed={2}, GPSCoord={3} LibratedStatues={4}", Time.ToString(), 
                CarID, 
                Speed,
                c3d.ToString(),
                LibratedStatus.ToString());
        }
        public DateTime Time { get { try { return new DateTime(Year + 2000, Month, Day, Hour, Minute, Second); } catch { return DateTime.MinValue; } } }
        public Geo.BLH BLH { get { return new DamLKK.Geo.BLH(Latitude, Longitude, Altitude); } }
        public Geo.XYZ XYZ { get { return Geo.Coord84To54.Convert(BLH); } }
        // XYZ 中，X和Y互换 dont know why
        public Geo.GPSCoord GPSCoord { get { Geo.XYZ xyz = this.XYZ; 
            Geo.GPSCoord c = new DamLKK.Geo.GPSCoord(xyz.y, xyz.x, xyz.z, Speed,0,this.Time,LibratedStatus);
            //c.When = this.Time;
            return c;
        }
        }

        [MarshalAs(UnmanagedType.U1)]
        public byte Len;
        [MarshalAs(UnmanagedType.U1)]
        public byte Type;       // 0x01

        [MarshalAs(UnmanagedType.U4)]
        public int CarID;   //车辆编号        

        [MarshalAs(UnmanagedType.U1)]
        public byte Year;
        [MarshalAs(UnmanagedType.U1)]
        public byte Month;
        [MarshalAs(UnmanagedType.U1)]
        public byte Day;
        [MarshalAs(UnmanagedType.U1)]
        public byte Hour;
        [MarshalAs(UnmanagedType.U1)]
        public byte Minute;
        [MarshalAs(UnmanagedType.U1)]
        public byte Second;

        [MarshalAs(UnmanagedType.R4)]
        public float Speed;        //速度
        [MarshalAs(UnmanagedType.R8)]
        public double Latitude;     //纬度
        [MarshalAs(UnmanagedType.R8)]
        public double Longitude;    //经度
        [MarshalAs(UnmanagedType.R4)]
        public float Altitude;     //海拔
        [MarshalAs(UnmanagedType.U1)]
        public byte LibratedStatus;

        [MarshalAs(UnmanagedType.U1)]
        public byte WorkFlag;   //高4位为工作状态 0xFx 表示正在碾压, 低四位为GPS定位状态
    }
    //超速报警结构 
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WarningSpeed
    {
        public static int Size()
        {
            return Marshal.SizeOf(typeof(WarningSpeed));
        }
        public override string ToString()
        {
            return string.Format("超速告警 Car={0}, Speed={1}", CarID, Speed);
        }
        [MarshalAs(UnmanagedType.U1)]
        public byte Len;
        [MarshalAs(UnmanagedType.U1)]
        public byte Type;       //0x02
        [MarshalAs(UnmanagedType.U4)]
        public int CarID;   //车辆编号
        [MarshalAs(UnmanagedType.R4)]
        public float Speed;        //速度

        public float RealSpeed() { return Speed / 100; /*千米/秒*/ }
    }

    //振动异常
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LibratedError
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte Len;
        [MarshalAs(UnmanagedType.U1)]
        public byte Type;   //振动异常 0xFF
        [MarshalAs(UnmanagedType.I4)]
        public int CarID;
        [MarshalAs(UnmanagedType.U1)]
        public byte SenseOrgan;   //状态

        public static int Size()
        {
            return Marshal.SizeOf(typeof(WorkErrorString));
        }
    }

    //施工出错
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WorkError
    {
        public static int Size()
        {
            return Marshal.SizeOf(typeof(WorkError));
        }
        [MarshalAs(UnmanagedType.U1)]
        public byte Len;
        [MarshalAs(UnmanagedType.U1)]
        public byte Type;   //=3
        [MarshalAs(UnmanagedType.U1)]
        public byte BlockID;   //分区号

        [MarshalAs(UnmanagedType.R4)]
        public float WorkLayer;  //层号
        [MarshalAs(UnmanagedType.U1)]
        public byte SegmentID;  //仓号
    }

    internal enum UpdateMessage
    {
        NONE = 0,
        WARNING = 0x04,
        OPEN_DECK = 0x11,
        CLOSE_DECK = 0x12,
        MOBILE_UPDATE = 0x13,
        HEARTBEAT = 0x20,
        UPDATE=0x10
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UpdateInfo
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte Len;
        [MarshalAs(UnmanagedType.U1)]
        public byte Type;       // 0x11 开仓，0x12 关仓，0x13 手机号列表更新，0x20  心跳包维持连接
    }

    public static class GPSServer
    {
        static int TOTAL_MSG_LEN = 0;

        public static Socket socket;
        static string serverIP = "172.23.225.215";
        static int serverPort=6666;
        public delegate void GPSEventHandler(object sender, GPSCoordEventArg ev);
        public static event GPSEventHandler OnResponseData;
        static IAsyncResult connection = null;
        static byte[] coords = null;
        static object lockRead = new object();
        static GPSServer()
        {
            TOTAL_MSG_LEN = Math.Max(TOTAL_MSG_LEN, GPSDATA.Size());
            TOTAL_MSG_LEN = Math.Max(TOTAL_MSG_LEN, WorkError.Size());
            TOTAL_MSG_LEN = Math.Max(TOTAL_MSG_LEN, WarningSpeed.Size());
            TOTAL_MSG_LEN = Math.Max(TOTAL_MSG_LEN, LibratedError.Size());
        }
        public static void StartReceiveGPSData(string ip, int port)
        {
            OnResponseData -= Dummy;
            OnResponseData += Dummy;
            serverIP = ip;
            serverPort = port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            AsyncCallback callback = new AsyncCallback(OnData);
            try
            {
                socket.Connect(serverIP, serverPort);
                socket.ReceiveTimeout = 30000;
            }
            catch (System.Exception e)
            {
                TRACE(e.ToString());
                return;
            }

            
            coords = new byte[TOTAL_MSG_LEN];
            connection = socket.BeginReceive(coords, 0, coords.Length, SocketFlags.None, callback, null);
            TRACE("GPSReceiver started.");

            StartHearbeat();
            TRACE("GPS Heartbeat started.");
        }
        static System.Timers.Timer tmHeartbeat = new System.Timers.Timer();
        private static void StartHearbeat()
        {
            OnHeartbeat(null, null);
            tmHeartbeat.Interval = 2 * 60 * 1000; // 2 minutes
            tmHeartbeat.Elapsed -= OnHeartbeat;
            tmHeartbeat.Elapsed += OnHeartbeat;
            tmHeartbeat.Enabled = true;
            tmHeartbeat.Start();
        }
        private static void OnHeartbeat(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendHeartbeat();
            TRACE("Sent heartbeat.");
        }
        private static void SendHeartbeat()
        {
            UpdateInfo ui = new UpdateInfo();
            ui.Len = (byte)Marshal.SizeOf(ui);
            ui.Type = (int)UpdateMessage.HEARTBEAT;
            SendStruct(ui, Marshal.SizeOf(ui));
        }
        private static void Send(byte[] sent)
        {
            if(IsConnected)
                socket.Send(sent);
        }
        public static void SendString(object x, int size, byte[] str)
        {

            byte[] sent = new byte[size + str.Length];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(x, ptr, false);
            Marshal.Copy(ptr, sent, 0, size);
            Marshal.FreeHGlobal(ptr);
            for (int i = 0; i < str.Length; i++ )
            {
                sent[i + size] = (byte)str[i];
            }
            Send(sent);
        }
        public static void OpenDeck()
        {
            UpdateInfo ui = new UpdateInfo();
            ui.Len = (byte)Marshal.SizeOf(ui);
            ui.Type = (byte)UpdateMessage.OPEN_DECK;
            SendStruct(ui, ui.Len);
        }
        public static void UpdateDeck()
        {
            UpdateInfo ui = new UpdateInfo();
            ui.Len = (byte)Marshal.SizeOf(ui);
            ui.Type = (byte)UpdateMessage.UPDATE;
            SendStruct(ui, ui.Len);
        }
        public static void CloseDeck()
        {
            UpdateInfo ui = new UpdateInfo();
            ui.Len = (byte)Marshal.SizeOf(ui);
            ui.Type = (byte)UpdateMessage.CLOSE_DECK;
            SendStruct(ui, ui.Len);
        }
        private static void SendStruct(object x, int size)
        {
            byte[] sent = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(x, ptr, false);
            Marshal.Copy(ptr, sent, 0, size);
            Marshal.FreeHGlobal(ptr);
            Send(sent);
        }
        private static void Dummy(object sender, GPSCoordEventArg e){}
        private static void TRACE(string fmt, params object[] param)
        {
            System.Diagnostics.Debug.Print(fmt, param);
        }
        private static void TRACE(string fmt)
        {
            System.Diagnostics.Debug.Print(fmt);
        }

        public static bool IsConnected { get { return  socket.Connected;} }            //此此为判断是否soket已经连接，测试阶段先置false；

        unsafe private static void OnGPSData()
        {
            GPSCoordEventArg e = new GPSCoordEventArg();
            fixed (byte* p = coords)
            {
                GPSDATA* gps = (GPSDATA*)p;
                TRACE(gps->ToString());
              
                e.msg = GPSMessage.GPSDATA;
                e.gps = *gps;
            }
            OnResponseData.Invoke(null, e);
        }

        #region  ----///////////临时数据测试，跳过tcp传输部分-----


        static System.Timers.Timer temptime = new System.Timers.Timer();
       
        public static void StartSendTestData()
        {
            OnResponseData -= Dummy;
            OnResponseData += Dummy;

            temptime.Interval = 1000;
            temptime.Elapsed += new System.Timers.ElapsedEventHandler(temptime_Elapsed);
            temptime.Start();
        }

        unsafe static void temptime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GPSCoordEventArg ex = new GPSCoordEventArg();
            GPSDATA gps = new GPSDATA();

            ex.msg = GPSMessage.GPSDATA;
            ex.gps = gps;

            OnResponseData.Invoke(null, ex);
        }


       #endregion

        unsafe private static void OnWarningSpeed()
        {
            WarningSpeed* warningSpeed = null;
            RollerDis cd = null;
            Roller ci = null;
            Deck dk = null;
            _Model.Unit unit = null;
            fixed(byte* p=coords)
            {
                try
                {
                    warningSpeed = (WarningSpeed*)p;
                    if (warningSpeed == null)
                        return;
                    TRACE(warningSpeed->ToString());
                    cd = DamLKK._Control.VehicleControl.FindVehicleInUse(warningSpeed->CarID);
                    ci = DamLKK._Control.VehicleControl.FindVechicle(warningSpeed->CarID);
                    if (cd == null || ci == null)
                    {
                        //Utils.MB.Warning(string.Format("接收到一个超速告警，但是车辆查询失败。车辆编号{0}", warningSpeed->CarID));
                        return;
                    }
                    dk = LayerControl.Instance.FindDeck(warningSpeed->CarID);
                    if (dk == null)
                    {
                        //Utils.MB.Warning(string.Format("接收到一个超速告警，但是仓面查询失败。车辆编号{0}", warningSpeed->CarID));
                        return;
                    }
                    //unit = DamLKK._Control.UnitControl.FromID(cd.Blockid);
                    //if (unit == null)
                    //{
                    //    //Utils.MB.Warning(string.Format("接收到一个超速告警，但是分区查询失败。分区编号{0}", cd.Blockid));
                    //    return;
                    //}


                    DateTime dt = DB.DateUtil.GetDate();
                    Forms.Warning dlg = new DamLKK.Forms.Warning();
                    dlg.WarningType = WarningType.OVERSPEED;
                    dlg.WarningDate = dt.Date.ToString("D");
                    dlg.WarningTime = dt.ToString("T");
                    dlg.ThisSpeed = warningSpeed->Speed;
                    dlg.MaxSpeed = dk.MaxSpeed;
                    dlg.DesignZ = cd.DesignZ;
                    dlg.DeckName = dk.Name;
                    dlg.CarName = ci.Name;
                    dlg.UnitName = unit.Name;
                    dlg.FillForms();
                    Forms.Main.GetInstance.ShowWarningDlg(dlg);
                }
                catch(Exception e)
                {
                    DamLKK.Utils.DebugUtil.log(e);
                    DateTime t = DateTime.Now;
                    string s = t.ToString("logs\\yy-MM-dd HH.mm.ss\\");
                    if (warningSpeed != null)
                    {
                        FileStream fs = new FileStream(s+"WarningSpeed.dat", FileMode.CreateNew);
                        BinaryWriter bw = new BinaryWriter(fs);
                        int size = Marshal.SizeOf(typeof(WarningSpeed));
                        byte[] buf = new byte[size];
                        Marshal.Copy((IntPtr)warningSpeed, buf, 0, size);
                        bw.Write(buf, 0, size);
                        bw.Close();
                        fs.Close();
                        
                        Utils.Xml.XMLUtil<RollerDis>.SaveXml(s+"CarDistribute.xml", cd);
                        Utils.Xml.XMLUtil<Roller>.SaveXml(s + "CarInfo.xml", ci);
                        Utils.Xml.XMLUtil<Deck>.SaveXml(s + "Segment.xml", dk);
                        //Utils.Xml.XMLUtil<DamLKK.Models.Unit>.SaveXml(s + "Unit.xml", part);
                    }
                }
            }
        }
        /// <summary>
        /// 击震力不合格报警处理
        /// </summary>
        unsafe private static void OnWarningLibrated()
        {
            LibratedError* warningLibrated = null;
            fixed (byte* p = coords)
            {
                try
                {

                    warningLibrated = (LibratedError*)p;
                    if (warningLibrated == null)
                        return;

                    DateTime dt = DB.DateUtil.GetDate();
                    Forms.Warning dlg = new DamLKK.Forms.Warning();
                    dlg.WarningType = WarningType.LIBRATED;
                    dlg.WarningDate = dt.Date.ToString("D");
                    dlg.WarningTime = dt.ToString("T");
                    dlg.LibrateState = (int)warningLibrated->SenseOrgan;
                    dlg.CarName = VehicleControl.FindVechicle(warningLibrated->CarID).Name;
                    dlg.FillForms();
                    Forms.Main.GetInstance.ShowWarningDlg(dlg);
                }
                catch (Exception e)
                {
                    DamLKK.Utils.DebugUtil.log(e);
                }
            }
        }

        unsafe private static void OnWorkError()
        {
            try
            {
                if ((int)coords[1] == 4)
                    return;
                WorkError* workError = null;
                WarningOverThickness wot = new WarningOverThickness();
                
                int msgLen = (int)(coords[0]) - WorkError.Size();
                byte[] errorString = new byte[msgLen];
                for (int i = WorkError.Size(), j = 0; i < coords.Length - WorkError.Size(); i++, j++)
                {
                    errorString[j] = coords[i];
                }
                socket.BeginReceive(errorString, WorkError.Size(), errorString.Length - WorkError.Size(), SocketFlags.None, null, null);
                string msg = System.Text.Encoding.Default.GetString(errorString);
                if (!msg.Substring(0, 6).Equals("碾压超厚告警"))
                    return;
                wot = _Control.WarningControl.ParseOverThickness(msg);
                DateTime dt = DB.DateUtil.GetDate();
                Forms.Warning dlg = new DamLKK.Forms.Warning();
                dlg.WarningType = WarningType.OVERTHICKNESS;
                dlg.DeckName = wot.DeckName;
                dlg.DesignZ = Convert.ToDouble(wot.Elevation);
                dlg.OverMerter = wot.OverThickness;
                dlg.Position = wot.Position;
                dlg.UnitName = wot.Unit;
                dlg.FillForms();
                Forms.Main.GetInstance.ShowWarningDlg(dlg);  
            }


            catch (System.Exception e)
            {
                DamLKK.Utils.DebugUtil.log(e);
            }
        }

        public static void OnData(IAsyncResult result)
        {

            if (!IsConnected)
                return;
            //int bytes = socket.EndReceive(result);
            //if (bytes <= 0)
            //    return;
            //TRACE("bytes={0}, type={1}", coords[0], coords[1]);

            switch ((GPSMessage)coords[1])
            {
                case GPSMessage.GPSDATA:
                    OnGPSData();
                    break;
                case GPSMessage.WARNINGSPEED:
                    OnWarningSpeed();
                    break;
                case GPSMessage.WORKERROR:
                    //OnWorkError();
                    break;
                case GPSMessage.WARNLIBRATED:
                    OnWarningLibrated();
                    break;
                case GPSMessage.COMMONLIBRATED:
                    OnSaveLibrated();
                    break;
            }

            try
            {
                connection = socket.BeginReceive(coords, 0, coords.Length, SocketFlags.None, OnData, null);
            }
            catch
            {
                Utils.MB.Warning("网络连接出现问题，窗口即将关闭。请重新运行客户端。");
                Application.Exit();
            }
        }

        unsafe private static void OnSaveLibrated()
        {
            //DM.Models.TrackGPS.hasReadCar.Clear();//清空列表重新查库
            //LibratedError* warningLibrated = null;
            //fixed (byte* p = coords)
            //{
            //    try
            //    {
            //        warningLibrated = (LibratedError*)p;
            //        if (warningLibrated == null)
            //            return;
            //        for (int i = 0; i < VehicleControl.carIDs.Length;i++ )
            //        {
            //           if (VehicleControl.carIDs[i]==warningLibrated->CarID)
            //           {
            //               VehicleControl.carLibratedStates[i] = warningLibrated->SenseOrgan;
            //               VehicleControl.carLibratedTimes[i] = DB.DateUtil.GetDate();
            //           }
            //        }
                   
            //    }
            //    catch (Exception e)
            //    {
            //        DamLKK.Utils.DebugUtil.log(e);
            //    }
            //}
        }
        public static void Stop()
        {
            lock(lockRead)
            {
                if (!IsConnected)
                    return;
                tmHeartbeat.Stop();
                TRACE("GPS Heartbeat stopped.");
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                TRACE("GPSReceiver stopped.");
            }
        }
        public static void test()
        {
            WarningSpeed ws = new WarningSpeed();
            ws.CarID = 993;
            ws.Speed = 400;
            GPSCoordEventArg e = new GPSCoordEventArg();
            e.msg = GPSMessage.WARNINGSPEED;
            e.speed = ws;
            OnResponseData.Invoke(null, e);
        }
    }

    public enum GPSMessage
    {
        GPSDATA = 1,
        WARNINGSPEED = 2,
        WORKERROR = 3,
        WARNLIBRATED=0xFF,
        COMMONLIBRATED=0xFE
    }
    public class GPSCoordEventArg : EventArgs
    {
        public GPSDATA gps;
        public WarningSpeed speed;
        public WorkError error;
        public GPSMessage msg;
    }

}
