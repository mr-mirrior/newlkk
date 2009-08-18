using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;

namespace DamLKK._Control
{
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WorkErrorString
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte Len;
        [MarshalAs(UnmanagedType.U1)]
        public byte Type;   //=4
        [MarshalAs(UnmanagedType.U1)]
        public byte BlockID;   //分区号


        public static int Size()
        {
            return Marshal.SizeOf(typeof(WorkErrorString));
        }
    }




    public enum WarningType
    {
        OVERSPEED = 0,
        ROLLINGLESS = 1,
        OVERTHICKNESS = 2,
        LIBRATED = 3
    }

    public struct WarningInfo
    {

        //通用指标
        public WarningType warnType;
        public string carName;
        public string warningTime;
        public string warningDate;
        public string _UnitName;
        public double designZ;
        public string deckName;
        public double ActualArea;
        public int libratedState;

        //超速需要指标
        public double maxSpeed;
        public double thisSpeed;

        //碾压遍数不足需要指标
        public double totalArea;
        public double shortRollerArea;

        //碾压过厚指标
        public double designDepth;
        public double startZ;
        public Geo.GPSCoord coord3d;
        public string overMeter;
        public string position;
    }

    // 碾压超厚告警！分区 上游围堰，高程 565米，仓面 10-2-32，超厚 0.5米，桩号 {100.00, 200.00} 
    public class WarningOverThickness
    {
        public string Unit;
        public string Elevation;
        public string DeckName;
        public string OverThickness;
        public string Position;
    }

    public static class WarningControl
    {

        //存储所有出错信息的list
        public static List<WarningInfo> listWarinInfo = new List<WarningInfo>();


        public static string warningString;
        public static WorkErrorString workErrorString = new WorkErrorString();

        public static void SendMessage(WarningType type, int unitid, string warningString)
        {
            if (LoginControl.User.Authority == LoginResult.DISWARNING)
                return;
            //链接服务端

            byte[] warningString2byte = Encoding.Default.GetBytes(warningString);
            int l;

            l = workErrorString.Len = (byte)(Marshal.SizeOf(workErrorString));
            workErrorString.Len = (byte)(Marshal.SizeOf(workErrorString) + warningString2byte.Length);

            if (type.Equals(""))
            {
                workErrorString.Type = 4;
            }
            else if (type == WarningType.ROLLINGLESS)
            {
                workErrorString.Type = 4;
            }
            else if (type == WarningType.OVERTHICKNESS)
            {
                workErrorString.Type = 0x05;
            }
            else if (type == WarningType.LIBRATED)
            {
                workErrorString.Type = 0x4;
            }
            workErrorString.BlockID = (byte)unitid;

            //发送错误信息

            byte[] workErrorString2byte = ToBytes(workErrorString);
            byte[] clientString = new byte[l + warningString2byte.Length];


            int i = 0;

            clientString[i++] = Convert.ToByte(workErrorString.Len);
            clientString[i++] = Convert.ToByte(workErrorString.Type);
            clientString[i++] = Convert.ToByte(workErrorString.BlockID);

            foreach (byte b in warningString2byte)
            {
                clientString[i] = b;
                i++;
            }
            //GPSServer.SendString(workErrorString, Marshal.SizeOf(workErrorString), warningString2byte);
        }
        private static byte[] ToBytes(WorkErrorString workErrorString)
        {
            unsafe
            {
                WorkErrorString* p;
                p = &workErrorString;
                //Marshal.StructureToPtr(workErrorString, (IntPtr)p, true);
                byte[] workErrorString2String = new byte[sizeof(WorkErrorString)];

                workErrorString2String[0] = (*p).Len;
                workErrorString2String[1] = (*p).Type;
                workErrorString2String[2] = (*p).BlockID;

                return workErrorString2String;
            }
        }
        public static void Init()
        {
            //GPSServer.OnResponseData -= OnWarning;
            //GPSServer.OnResponseData += OnWarning;
        }

        public static WarningOverThickness ParseOverThickness(string msg)
        {
            // 碾压超厚告警！分区 上游围堰，高程 565米，仓面 10-2-32，超厚 0.5米，桩号 {100.00, 200.00} 
            WarningOverThickness ot = new WarningOverThickness();
            string[] p1 = msg.Split(new string[] { "，" }, StringSplitOptions.RemoveEmptyEntries);
            if (p1.Length != 5)
                return null;
            List<string> content = new List<string>();
            foreach (string p in p1)
            {
                string[] p2 = p.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (p2.Length != 2)
                    return null;
                content.Add(p2[1]);
            }

            ot.Unit = content[0];
            ot.Elevation = content[1];
            ot.DeckName = content[2];
            ot.OverThickness = content[3];
            ot.Position = content[4];

            return ot;
        }

        private static void OnWarning(object sender, EventArgs e)
        {
            //GPSCoordEventArg f = (GPSCoordEventArg)e;

            //if (f.msg == GPSMessage.WARNINGSPEED)
            //{
            //    RollerDis cd = VehicleControl.FindVehicleInUse(f.speed.CarID);
            //    if (cd == null) return;
            //    Roller ci = VehicleControl.FindVechicle(cd.Carid);
            //    if (cd == null || ci == null)
            //        return;
            //    Models.Unit part = UnitControl.FromID(cd.Blockid);
            //    if (part == null) return;
            //    Models.Elevation elev = new DM.Models.Elevation(cd.DesignZ);
            //    Models.Layer layer = LayerControl.Instance.FindLayerByPE(part, elev);
            //    if (layer == null) return;
            //    Models.Deck deck = layer.DeckControl.FindDeckByIndex(cd.Segmentid);
            //    if (deck == null) return;
                
            //}

        }
    }
}
