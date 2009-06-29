using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DamLKK._Model;
using System.Data.SqlClient;
using DamLKK.Utils;

namespace DamLKK.DB
{
    public class RollerDAO
    {
        private RollerDAO() { }
        static RollerDAO _MyInstance = null;

        public static RollerDAO GetInstance()
        {
            if (_MyInstance == null)
            {
                _MyInstance = new RollerDAO();
            }
            return _MyInstance;
        }


        /// <summary>
        ///  //返回所有车辆信息
        /// </summary>
        public List<Roller> GetAllCarInfo()
        {
            List<Roller> carinfos = new List<Roller>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
           
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, "select * from carinfo");
                while (reader.Read())
                {
                    Roller carinfo = new Roller();
                    carinfo.ID = (Convert.ToInt32(reader["carid"]));
                    carinfo.Name = (reader["carname"].ToString());
                    carinfo.GPSHeight = (Convert.ToDouble(reader["gpsheight"]));
                    carinfo.ScrollWidth = (Convert.ToDouble(reader["scrollwidth"]));
                    carinfos.Add(carinfo);
                }
                return carinfos;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

        /// <summary>
        /// 获得id的车辆信息
        /// </summary>
        public Roller GetCarInfo(List<Roller> carinfos, int carInfoID)
        {
            foreach (Roller car in carinfos)
            {
                if (car.ID== carInfoID)
                {
                    return car;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据车名获得id
        /// </summary>
        public int GetCarNameByCarID(List<Roller> carinfos, string carname)
        {
            foreach (Roller car in carinfos)
            {
                if (car.Name.Equals(carname))
                {
                    return car.ID;
                }
            }

            return -1;
        }
    }
}
