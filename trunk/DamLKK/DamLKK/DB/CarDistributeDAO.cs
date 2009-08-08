using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DamLKK.Utils;
using DamLKK._Model;

namespace DamLKK.DB
{
    public class CarDistributeDAO
    {
        public const int NOMATCH = 0;//没有相对应的车辆
        public const int SUCCESS = 1;//成功
        public const int MATCHMORE = 2;//匹配太多车辆

        private CarDistributeDAO() { }

        private static CarDistributeDAO _MyInstance = null;

        public static CarDistributeDAO GetInstance()
        {
            if (_MyInstance == null)
            {
                _MyInstance = new CarDistributeDAO();
            }
            return _MyInstance;
        }


       
        /// <summary>
        ///  //结束该仓面上的车辆
        /// </summary>
        public int EndCars(_Model.RollerDis carDistribute)
        {
            String sqlTxt = "update cardistribute set DTEnd=getDate() where unitid=" + carDistribute.UnitID + " and designz=" + carDistribute.DesignZ + " and segmentid=" + carDistribute.SegmentID + " and DTEnd is null";
            try
            {
                int updateRowsNumber = DBConnection.executeUpdate(sqlTxt);

                //更新车辆状态
                sqlTxt = "update carinfo set unitid=0,maxspeed='0',segmentid=0,SenseOrganState=0,DesignZ='0' where carid in (select carid from cardistribute  where" +
                       " unitid=" + carDistribute.UnitID +
                       " and segmentid=" + carDistribute.RollerID +
                       " and designz= " + carDistribute.DesignZ + " and  dtend is not null)";

                DBConnection.executeUpdate(sqlTxt);

                return updateRowsNumber;

            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
        }

        //结束车辆
        //返回类型参照上面
         public int EndCar(_Model.RollerDis carDistribute)
         {
             string sqlTxt = "update cardistribute set DTEnd=getDate() where carid=" + carDistribute.RollerID + " and unitid=" + carDistribute.UnitID + " and designz=" + carDistribute.DesignZ + " and segmentid=" + carDistribute.RollerID + " and DTEnd is null";
             try
             {
                 int updateRowsNumber = DBConnection.executeUpdate(sqlTxt);
                 if (updateRowsNumber > 1)//更新了太多数据
                 {
                     return MATCHMORE;
                 }
                 else
                 {
                     //更新车辆状态
                     sqlTxt = "update carinfo set unitid=0,maxspeed='0',segmentid=0,SenseOrganState=0,DesignZ='0' where carid=" + carDistribute.RollerID;

                     DBConnection.executeUpdate(sqlTxt);

                     return updateRowsNumber;
                 }
             }
             catch (Exception exp)
             {
                 DebugUtil.log(exp);
                 throw exp;
             }
         }

         /// <summary>
         ///添加一部车辆
         ///分配未使用的car给一个舱面
         ///分配之前检查一下是否正在使用中.
         /// </summary>
         public bool StartCar(_Model.RollerDis carDistribute, double maxSpeed, int librate, double designz)
         {

             //检查是否在使用中
             String sqlTxt = "select * from cardistribute where carid=" + carDistribute.RollerID + " and ( dtstart is not null and DTEnd is null)";
             SqlConnection conn = null;
             SqlDataReader reader = null;
             try
             {
                 conn = DBConnection.getSqlConnection();
                 reader = DBConnection.executeQuery(conn, sqlTxt);
                 if (reader.Read())
                 {
                     return false;
                 }
                 DBConnection.closeDataReader(reader);
                 //分配车辆
                 sqlTxt = "insert cardistribute (carid,unitid,segmentid,designz,DTStart) values(" + carDistribute.RollerID + "," + carDistribute.UnitID + "," + carDistribute.SegmentID + "," + carDistribute.Elevation + "," + "getDate())";
                 int updateCount = DBConnection.executeUpdate(sqlTxt);

                 if (updateCount <= 0)
                 {
                     return false;
                 }


                 sqlTxt = "update carinfo set unitid=" + carDistribute.UnitID + ",maxspeed='" + maxSpeed + "',segmentid=" + carDistribute.SegmentID + ",SenseOrganState=" + librate + ",DesignZ='"+designz+"' where carid = " + carDistribute.RollerID;

                 updateCount = DBConnection.executeUpdate(sqlTxt);

                 if (updateCount <= 0)
                 {
                     return false;
                 }

                 return true;
             }
             catch (Exception exp)
             {
                 DebugUtil.log(exp);
                 return false;

             }
             finally
             {
                 DBConnection.closeSqlConnection(conn);
             }
         }


        
        /// <summary>
         /// //预先分配一些车辆到某仓面
        /// </summary>
        public bool DistributeCars(int unitid, double designZ, int segmentid, List<int> carids)
        {
            DamLKK._Model.RollerDis cd = new DamLKK._Model.RollerDis();
            cd.UnitID = unitid;
            cd.DesignZ = designZ;
            cd.SegmentID = segmentid;
            //匹配车辆.
            for (int i = 0; i < carids.Count; i++)
            {
                cd.RollerID = carids[i];
                try
                {
                    DistributeCar(cd);
                }
                catch (System.Exception e)
                {
                    DebugUtil.log(e);
                    return false;
                }
            }
            return true;
        }

        
        /// <summary>
        /// //预分配一部车辆
        /// </summary>
        public  bool DistributeCar(_Model.RollerDis carDistribute)
        {
            //检查是否在使用中            
            SqlConnection conn = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                //分配车辆
                String sqlTxt = "insert cardistribute (carid,unitid,segmentid,designz) values(" + carDistribute.RollerID + "," + carDistribute.UnitID + "," + carDistribute.SegmentID + "," + carDistribute.DesignZ + ")";
                int updateCount = DBConnection.executeUpdate(sqlTxt);

                if (updateCount <= 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return false;

            }
            finally
            {
                DBConnection.closeSqlConnection(conn);
            }
        }


        /// <summary>
        /// /删除某一仓面预定义的某辆车
        /// </summary>
        /// <param name="carDistribute"></param>
        /// <returns></returns>
        public bool RemoveCar(_Model.RollerDis carDistribute)
        {
            //检查是否在使用中            
            SqlConnection conn = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                //分配车辆
                string sqlTxt = "delete cardistribute where carid= " + carDistribute.RollerID +
                    " and unitid=" + carDistribute.UnitID +
                    " and segmentid=" + carDistribute.SegmentID +
                    " and designz= " + carDistribute.DesignZ;

                int updateCount = DBConnection.executeUpdate(sqlTxt);

                if (updateCount <= 0)
                {
                    return false;
                }

           
                if (updateCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return false;

            }
            finally
            {
                DBConnection.closeSqlConnection(conn);
            }
        }



        
        /// <summary>
        /// //启动当前仓面上的车辆
        /// </summary>
        public bool StartCars(_Model.RollerDis carDistribute,double maxspeed, string librate, double designz)
        {
            //检查是否在使用中            
            SqlConnection conn = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                //分配车辆
                string sqlTxt = "update cardistribute set dtstart = getdate() where  unitid=" + carDistribute.UnitID +
                    " and segmentid=" + carDistribute.SegmentID +
                    " and designz= '" + carDistribute.DesignZ +
                    "' and  dtstart is null and dtend is null";


                int updateCount = DBConnection.executeUpdate(sqlTxt);

                if (updateCount <= 0)
                {
                    return false;
                }

                sqlTxt = "update carinfo set unitid=" + carDistribute.UnitID + ",maxspeed='" + maxspeed + "',segmentid=" + carDistribute.SegmentID + ",SenseOrganState='" + "0" + "',DesignZ='" + designz + "' where carid in (select carid from cardistribute  where  unitid=" + carDistribute.UnitID +
                    " and segmentid=" + carDistribute.SegmentID+
                    " and designz= '" + carDistribute.DesignZ +
                    "' and  dtstart is not null and dtend is null)";

                updateCount = DBConnection.executeUpdate(sqlTxt);

                if (updateCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return false;

            }
            finally
            {
                DBConnection.closeSqlConnection(conn);
            }

        }


        
        /// <summary>
        /// //取得当前舱面已经分配的car
        /// </summary>
        public List<_Model.RollerDis> GetCarsInDeck_Distributed(int unitid, double designZ, int segmentid)
        {
            List<DamLKK._Model.RollerDis> carinfos = new List<DamLKK._Model.RollerDis>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where unitid=" + unitid +
                " and segmentid=" + segmentid +
                " and designZ=" + designZ;

            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    DamLKK._Model.RollerDis dis = new DamLKK._Model.RollerDis();
                    dis.RollerID = (int)reader["CarID"];
                    if (reader["DTStart"] == DBNull.Value)
                        dis.DTStart = DateTime.MinValue;
                    else
                        dis.DTStart = (DateTime)reader["DTStart"];

                    if (reader["DTEnd"] == DBNull.Value)
                        dis.DTEnd = DateTime.MinValue;
                    else
                        dis.DTEnd = (DateTime)reader["DTEnd"];

                    carinfos.Add(dis);
                }
                return carinfos;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }


        
        /// <summary>
        /// //取得当前舱面正在使用的car
        /// </summary>
        public List<DamLKK._Model.Roller> GetCarsInDeck_Inuse(int unitid, double designZ, int segmentid)
        {
            List<DamLKK._Model.Roller> carinfos = new List<DamLKK._Model.Roller>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where unitid=" +unitid + " and segmentid=" + segmentid + " and designZ=" + designZ + " and DTEnd is null and DTStart is not null";
            List<DamLKK._Model.Roller> all =_Control.VehicleControl.vehiclesInfo;
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    int carID = (Convert.ToInt32(reader["carid"]));
                    DamLKK._Model.Roller carinfo =RollerDAO.GetInstance().GetCarInfo(all, carID);
                    carinfos.Add(carinfo);
                }
                return carinfos;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

        private DamLKK._Model.CarDis_Status CheckStatus(DateTime start, DateTime end)
        {
            if (!start.Equals(DateTime.MinValue) && end.Equals(DateTime.MinValue))
            {
                return DamLKK._Model.CarDis_Status.WORK;
            }
            else if (!start.Equals(DateTime.MinValue) && !end.Equals(DateTime.MinValue))
            {
                return DamLKK._Model.CarDis_Status.ENDWORK;
            }
            return DamLKK._Model.CarDis_Status.ASSIGNED;
        }

        /// <summary>
        /// 取得某一个仓面所有分配过得车辆信息   
        /// </summary>
        public List<_Model.RollerDis> GetCarDisInDeck(int unitid, double designZ, int segmentid)
        {
            List<_Model.RollerDis> cardistributes = new List<_Model.RollerDis>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where unitid=" +unitid + " and segmentid=" + segmentid + " and designZ=" + designZ;
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    int carid = Convert.ToInt32(reader["carid"]);
                    DateTime dTEnd = DateTime.MinValue;
                    DateTime dTStart = DateTime.MinValue;
                    if (!reader["dtend"].Equals(DBNull.Value))
                    {
                        dTEnd = Convert.ToDateTime(reader["dtend"]);
                    }
                    if (!reader["dtstart"].Equals(DBNull.Value))
                    {
                        dTStart = Convert.ToDateTime(reader["dtstart"]);
                    }
                    DamLKK._Model.CarDis_Status status = CheckStatus(dTStart, dTEnd);
                    _Model.RollerDis cardistribute = new _Model.RollerDis();
                    cardistribute.RollerID = carid;
                    cardistribute.UnitID = unitid;
                    cardistribute.DTEnd = dTEnd;
                    cardistribute.DTStart = dTStart;
                    cardistribute.DesignZ = designZ;
                    cardistribute.SegmentID = segmentid;
                    cardistribute.Status = status;
                    cardistributes.Add(cardistribute);
                }
               
                return cardistributes;
            }
            catch (Exception exp)
            {
                DamLKK.Utils.DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

       
        /// <summary>
        ///  //取得某一个仓面所有分配过得车辆信息
        /// </summary>
        public List<_Model.RollerDis> GetCarDisInDeck_Inuse(int unitid, double designZ, int segmentid)
        {
            List<_Model.RollerDis> cardistributes = new List<_Model.RollerDis>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where unitid=" + unitid + " and segmentid=" + segmentid + " and designZ=" + designZ + " and DTEnd is null and DTStart is not null";
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    Int32 carid = Convert.ToInt32(reader["carid"]);
                    DateTime dTEnd = DateTime.MinValue;
                    DateTime dTStart = DateTime.MinValue;
                    if (!reader["dtend"].Equals(DBNull.Value))
                    {
                        dTEnd = Convert.ToDateTime(reader["dtend"]);
                    }
                    if (!reader["dtstart"].Equals(DBNull.Value))
                    {
                        dTStart = Convert.ToDateTime(reader["dtstart"]);
                    }
                    CarDis_Status status = CheckStatus(dTEnd, dTStart);
                    RollerDis cardistribute = new RollerDis();
                    cardistribute.RollerID = carid;
                    cardistribute.UnitID = unitid;
                    cardistribute.DTEnd = dTEnd;
                    cardistribute.DTStart = dTStart;
                    cardistribute.DesignZ = designZ;
                    cardistribute.SegmentID = segmentid;
                    cardistribute.Status = status;
                    cardistributes.Add(cardistribute);
                }
                return cardistributes;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }


        
        /// <summary>
        /// ////得到全部正在使用中的car
        /// </summary>
        public List<Roller> GetInusedCars()
        {
            List<Roller> carinfos = new List<Roller>();
            SqlConnection connection = null;
            SqlDataReader reader = null;
            List<Roller> all = _Control.VehicleControl.vehiclesInfo;
            String sqlTxt = "select * from cardistribute where DTEnd is null and DTStart is not null";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                while (reader.Read())
                {
                    int carID = (Convert.ToInt32(reader["carid"]));
                    Roller carinfo = RollerDAO.GetInstance().GetCarInfo(all, carID);
                    carinfos.Add(carinfo);
                }
                return carinfos;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(connection);
            }
        }

       
        /// <summary>
        ///  //得到全部正在使用中的car
        /// </summary>
        /// <returns></returns>
        public List<_Model.RollerDis> GetInusedCarDis()
        {
            List<RollerDis> cardistributes = new List<RollerDis>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where DTEnd is null and DTStart is not null";
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    int carid = Convert.ToInt32(reader["carid"]);
                    int unitid = Convert.ToInt32(reader["unitid"]);
                    Int32 segmentid = Convert.ToInt32(reader["segmentid"]);
                    Double designZ = Convert.ToDouble(reader["designz"]);
                    DateTime dTEnd = DateTime.MinValue;
                    DateTime dTStart = DateTime.MinValue;
                    if (!reader["dtend"].Equals(DBNull.Value))
                    {
                        dTEnd = Convert.ToDateTime(reader["dtend"]);
                    }
                    if (!reader["dtstart"].Equals(DBNull.Value))
                    {
                        dTStart = Convert.ToDateTime(reader["dtstart"]);
                    }
                    CarDis_Status status = CheckStatus(dTEnd, dTStart);
                    RollerDis cardistribute = new RollerDis();
                    cardistribute.RollerID = carid;
                    cardistribute.UnitID= unitid;
                    cardistribute.DTEnd = dTEnd;
                    cardistribute.DTStart = dTStart;
                    cardistribute.DesignZ = designZ;
                    cardistribute.SegmentID = segmentid;
                    cardistribute.Status = status;
                    cardistributes.Add(cardistribute);
                }
                return cardistributes;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }


       
        /// <summary>
        ///  //取得全部未使用的car()..
        /// //先得取出所有car信息,然后查询出正在使用中的car信息.在所有car信息中去掉正在使用中的car信息.	
        /// </summary>
        /// <returns></returns>
        public List<Roller> GetUnusedCars()
        {
            List<Roller> unusedCars = new List<Roller>();

            List<Roller> carinfos = null;
            List<Roller> inusedCars = null;
            try
            {
                carinfos = _Control.VehicleControl.vehiclesInfo;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                throw e;
            }

            try
            {
                inusedCars = GetInusedCars();
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                throw e;
            }

            foreach (Roller carinfo in carinfos)
            {
                Boolean inused = false;
                foreach (Roller inusedcar in inusedCars)
                {
                    if (inusedcar.ID == (carinfo.ID))
                    {
                        inused = true;
                        break;
                    }
                }
                if (!inused)
                {
                    unusedCars.Add(carinfo);
                }
            }
            return unusedCars;
        }
        
        /// <summary>
        /// //取得某一个仓面所有分配过得车辆信息,不包含已经结束的
        /// </summary>
        public List<_Model.RollerDis> GetCarDisInDeck_all_except_end(int unitid, double designZ, int segmentid)
        {
            List<RollerDis> cardistributes = new List<RollerDis>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where unitid=" + unitid + " and segmentid=" + segmentid + " and designZ=" + designZ + " and dtend is null and dtstart is null";
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    int carid = Convert.ToInt32(reader["carid"]);
                    DateTime dTEnd = DateTime.MinValue;
                    DateTime dTStart = DateTime.MinValue;
                    if (!reader["dtend"].Equals(DBNull.Value))
                    {
                        dTEnd = Convert.ToDateTime(reader["dtend"]);
                    }
                    if (!reader["dtstart"].Equals(DBNull.Value))
                    {
                        dTStart = Convert.ToDateTime(reader["dtstart"]);
                    }
                    CarDis_Status status = CheckStatus(dTStart, dTEnd);
                    RollerDis cardistribute = new RollerDis();
                    cardistribute.RollerID = carid;
                    cardistribute.UnitID = unitid;
                    cardistribute.DTEnd = dTEnd;
                    cardistribute.DTStart = dTStart;
                    cardistribute.DesignZ = designZ;
                    cardistribute.SegmentID = segmentid;
                    cardistribute.Status = status;
                    cardistributes.Add(cardistribute);
                }

                return cardistributes;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

        
        /// <summary>
        /// //得到与当前仓面完全没有关系的车辆
        /// </summary>
        public List<Roller> GetCars_not_in_this_Deck(int unitid, double designZ, int segmentid)
        {
            return GetOthers(GetCarDisInDeck_all_except_end(unitid,designZ,segmentid));
        }


        //从全部的车辆中去除这些车辆
        public List<Roller> GetOthers(List<RollerDis> cds)
        {
            List<Roller> others = new List<Roller>();
            List<Roller> carinfos = null;

            try
            {
                carinfos = _Control.VehicleControl.vehiclesInfo;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                throw e;
            }


            foreach (Roller carinfo in carinfos)
            {
                Boolean inused = false;
                foreach (RollerDis cd in cds)
                {
                    if (cd.RollerID == (carinfo.ID))
                    {
                        inused = true;
                        break;
                    }
                }
                if (!inused)
                {
                    others.Add(carinfo);
                }
            }
            return others;
        }

    
        /// <summary>
        ///     //从全部的车辆中去除这些车辆
        /// </summary>
        public List<Roller> GetOthers(List<Int32> ids)
        {
            List<Roller> others = new List<Roller>();
            List<Roller> carinfos = null;

            try
            {
                carinfos = _Control.VehicleControl.vehiclesInfo;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                throw e;
            }


            foreach (Roller carinfo in carinfos)
            {
                Boolean inused = false;
                foreach (Int32 id in ids)
                {
                    if (id == (carinfo.ID))
                    {
                        inused = true;
                        break;
                    }
                }
                if (!inused)
                {
                    others.Add(carinfo);
                }
            }
            return others;
        }
    }
}
