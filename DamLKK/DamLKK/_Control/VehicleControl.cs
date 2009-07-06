using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DamLKK._Model;

namespace DamLKK._Control
{
    // 车辆控制，隶属于一某仓面

    public class VehicleControl: IDisposable
    {
        #region - 静态 -
        static List<Roller> vehiclesInfo = new List<Roller>();
        //最后一条击震力数据的字典 feiying 09.3.20
        public static int[] carIDs ;
        public static int[] carLibratedStates;
        public static DateTime[] carLibratedTimes;
        public static void ReadVehicleInfo()
        {
            DB.RollerDAO dao = DB.RollerDAO.GetInstance();
            vehiclesInfo = dao.GetAllCarInfo();

            carIDs=new int[vehiclesInfo.Count];
            carLibratedStates= new int[carIDs.Length];
            carLibratedTimes = new DateTime[carIDs.Length];
            for (int i = 0; i < vehiclesInfo.Count; i++)
            {
                carIDs[i] = vehiclesInfo[i].ID;
                carLibratedStates[i] = -1;
                carLibratedTimes[i] = DateTime.MinValue;
            }
        }
        public static Roller FindVechicle(int id)
        {
            foreach (Roller v in vehiclesInfo)
            {
                if (v.ID == id)
                    return v;
            }
            return null;
        }
        static List<RollerDis> vehiclesWorking = new List<RollerDis>();
        public static void LoadCarDistribute()
        {
            vehiclesWorking.Clear();
            try
            {
                DamLKK.DB.CarDistributeDAO dao = DamLKK.DB.CarDistributeDAO.GetInstance();
                vehiclesWorking = dao.GetInusedCarDis();
            }
            catch
            {
                Utils.MB.Warning("数据库访问错误！");
            }
        }
        public static RollerDis FindVehicleInUse(int carid)
        {
            // test
            if( carid == 993 )
            {
                RollerDis cd = new RollerDis();
                cd.RollerID = 3;
                cd.SegmentID = 0;
                cd.UnitID =3;
                cd.DesignZ = 615;
                cd.DTStart = DateTime.MinValue;
                cd.DTEnd = DateTime.MinValue;
                cd.Status= CarDis_Status.ENDWORK;
                return cd;
            }
            //test


            foreach (RollerDis cd in vehiclesWorking)
            {
                if (cd.RollerID == carid)
                    return cd;
            }
            return null;
        }
        #endregion

        /*
        None：	不振
        High：	高频低振
        Low：	低频高振
        Normal：震动	//此值只适用于只有两种状态的碾压机
         */
        public enum SenseOrganState { None = 0, High = 1, Low = 2, Normal = 3 };

        List<Roller> vehicles = new List<Roller>();
        Deck _Owner = null;
        public void Dispose()
        {
            foreach (Roller v in vehicles)
            {
                v.Dispose();
            }
            GC.SuppressFinalize(this);
        }
        public Deck Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }

        public List<Roller> Rollers
        {
            get { return vehicles; }
        }

        public VehicleControl(){}

        public void AssignVehicle(Deck deck)
        {
            _Owner = deck;
           
            if (deck.Unit == null||deck.Elevation==null)
                return;

            Forms.AssignVehicle dlg = new Forms.AssignVehicle();
            dlg.Deck = deck;
            dlg.BlockName = deck.Unit.Name;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //this.DeleteAll();
            }
            LoadDB();
        }

        public bool AddVehicle(Roller v)
        {
            vehicles.Add(v);
            return true;
        }
//         public bool DeleteVehicle(Vehicle v)
//         {
//             return true;
//         }
        public void DeleteAll()
        {
            DamLKK.DB.CarDistributeDAO dao = DamLKK.DB.CarDistributeDAO.GetInstance();
            //int 在这里实现_删除某仓面全部安排车辆;
            foreach (Roller v in this.Rollers)
            {
                //dao.removeCar(v.Assignment);
            }
            this.Clear();
        }
        public void Clear()
        {
            foreach (Roller vk in Rollers)
            {
                vk.Dispose();
            }
            vehicles.Clear();
        }
        public int[] RollCount(PointF pt)
        {
            int count = 0,countNo=0;
            foreach (Roller vk in vehicles)
            {
                int[] ct=vk.RollCount(pt);
                if(ct==null)
                   continue;
                count +=ct[1];
                countNo += ct[0];
            }
            return new int[]{countNo,count};
        }
        private List<Roller> Translate(List<RollerDis> lst)
        {
            Color[] cls = new Color[]{
                Color.Blue,
                Color.DarkSlateGray,
                Color.Navy,
                Color.DarkTurquoise,
                Color.SlateBlue,
                Color.DarkOliveGreen,
                Color.Green,
                Color.YellowGreen,
                Color.DodgerBlue,
                Color.CornflowerBlue,
                Color.Olive,
                Color.Teal,
                Color.Goldenrod,
                Color.Indigo,
                Color.SteelBlue,
                Color.LimeGreen

            };

            List<Roller> vs = new List<Roller>();
            if (lst == null)
                return vs;
            foreach (RollerDis cd in lst)
            {
                int color = 0;
                for (int i = 0; i < VehicleControl.vehiclesInfo.Count; i++)
                {
                    if (VehicleControl.vehiclesInfo[i].ID == cd.RollerID)
                    {
                        color = i % 16;
                        break;
                    }
                }

                Roller vk = new Roller(cd);
                vk.Owner = this.Owner;
                vk.TrackGPSControl.LoadDB();
                vk.TrackGPSControl.Tracking.Color = cls[color];
                if (cd.IsWorking())
                    vk.ListenGPS();
                vs.Add(vk);
            }
            return vs;
        }
       
        private static int VechilePriority(Roller v1, Roller v2)
        {
            if (v1.Assignment.DTStart < v2.Assignment.DTStart)
                return -1;
            if (v1.Assignment.DTStart == v2.Assignment.DTStart)
                return 0;
            return 1;
        }
        private void Sort()
        {
            vehicles.Sort(VechilePriority);
        }

        public void MaxMin(out double lo, out double hi)
        {
            lo = -1;
            hi = -1;
            if (vehicles.Count == 0)
                return;
            double max = double.MinValue;
            double min = double.MaxValue;
            foreach (Roller v in vehicles)
            {
                //double l, h;
                //v.TrackGPSControl.Tracking.MaxMin(out l, out h);
                //if (h != -1)
                //    max = Math.Max(max, h);
                //if( l != -1 )
                //    min = Math.Min(min, l);
            }
            hi = max;
            lo = min;
        }
        public void LoadDB()
        {
            this.Clear();

            //int 在这里实现_从数据库读取某仓面的车辆安排情况;
            DamLKK.DB.CarDistributeDAO dao = DamLKK.DB.CarDistributeDAO.GetInstance();
            if (_Owner == null)
                return;
            List<RollerDis> lst = dao.GetCarDisInDeck(_Owner.Unit.ID, _Owner.Elevation.Height, _Owner.ID);
            this.vehicles = Translate(lst);

            Sort();
        }
    }
}
