using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DamLKK.Geo;
using DamLKK._Model;
using DamLKK.Utils;

namespace DamLKK.DB
{
    class TracePointDAO
    {

        private static TracePointDAO _MyInstance = null;

        public static TracePointDAO GetInstance()
        {
            if (_MyInstance == null)
            {
                _MyInstance = new TracePointDAO();
            }
            return _MyInstance;
        }

        private string statusLimit = " and status not in ("+_Model.Config.I.BASE_STATUS+") order by dttrace";
        //private const String statusLimit = " and status not in (0)";
        //private const String statusLimit = " and status not in (0,1)";
        /// <summary>
        /// 插入一条gps点
        /// </summary>
        /// <param name="p">按照数据库字段顺序</param>
        /// <returns></returns>
        public bool InsertOneTP(params string[] p)
        {
            try
            {
                string sqltxt = "insert ZTracePoint" + DateUtil.GetDate().Year.ToString() + DateUtil.GetDate().Month.ToString("00") + " values(" + p[0] + ",'" + p[1] + "','" + p[2] + "','" + p[3] + "'," + p[4] + ",'" + p[5] + "'," + p[6] + ")";
                int i = DBConnection.executeUpdate(sqltxt);
                if (i == 1)
                    return true;
                else
                    return false;
            }
            catch (System.Exception e)
            {
                DamLKK.Utils.DebugUtil.log(e);
                return false;
            }
        }

        /// <summary>
        /// 插入一条击震力信息
        /// </summary>
        /// <param name="p">按照数据库字段顺序</param>
        /// <returns></returns>
        public bool InsertOneOsense(params string[] p)
        {
            try
            {
                string sqltxt = "insert SenseOrgan values(" + p[0] + "," + p[1] + ",'" + p[2]  + "')";
                int i = DBConnection.executeUpdate(sqltxt);
                if (i == 1)
                    return true;
                else
                    return false;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                return false;
            }
           
        }


        
        /// <summary>
        /// //得到当前正在使用的表名称
        /// </summary>
        /// <returns></returns>
        public string GetThisTableName(){
            return GetTableNameByDateTime(DateTime.Now);
        }

        public string GetTableNameByDateTime(DateTime datetime){
            return "ztracepoint" + string.Format("{0:yyyyMM}", datetime);
        }

        
        /// <summary>
        /// //半路来看正在施工的仓面
        /// //取得某一个仓面的List<TracePoint>.可能有多辆车,可能跨月。

        /// // 按不同车 分列表
        /// </summary>
        /// <param name="blockid"></param>
        /// <param name="designz"></param>
        /// <param name="segmentid"></param>
        /// <returns></returns>
        public List<List<GPSCoord>> GetHistoryTracePoints(int unitid,double designz,int segmentid)
        {
            //得到所有在此舱面工作过的车辆

            List<List<GPSCoord>> tracepointLists = new List<List<GPSCoord>>();
            //当前仓面上的车辆
            List<RollerDis> carDistributes = null;
            
            try
            {
                carDistributes = CarDistributeDAO.GetInstance().GetCarDisInDeck(unitid, designz, segmentid);
            }
            catch (System.Exception e)
            {
                throw e;
                //return null;
            }
            
            foreach (RollerDis cd in carDistributes){
                tracepointLists.Add(GetGPSCoordList(cd.RollerID,cd.DTStart,cd.DTEnd));
            }

            return tracepointLists;
        }

        public List<GPSCoord> GetGPSCoordList(Int32 carid, DateTime dtstart, DateTime dtend)
        {
            List<GPSCoord> tracePoints = new List<GPSCoord>();            
            if(dtstart != DateTime.MinValue){

                if(dtend==DateTime.MinValue){
                    dtend = DateTime.Now;
                }                    
                    
                DateTime[] datetimes = DateUtil.GetDateTimes(Compare_Type.MONTH,dtstart,dtend);
                if(datetimes.Length==1){
                    string tablename = string.Format("{0:yyyyMM}", datetimes[0]);
                    String sqltxt = "select * from " + GetTableNameByDateTime(datetimes[0]) + " where carid=" + carid + " and dttrace between '" + dtstart.ToString() + "' and '" + dtend.ToString() + "'" + statusLimit;
                    tracePoints=GetGPSCoordList(tracePoints, sqltxt);
                }else {
                    for(int i=0;i<datetimes.Length;i++){
                        if(i==0){
                            string tablename = string.Format("{0:yyyyMM}", datetimes[0]);
                            String sqltxt = "select * from " + GetTableNameByDateTime(datetimes[0]) + " where carid=" + carid + " and dttrace >= '" + dtstart.ToString() + "'" + statusLimit;
                            GetGPSCoordList(tracePoints,sqltxt);
                        }else if(i==datetimes.Length-1){
                            string tablename = string.Format("{0:yyyyMM}", datetimes[datetimes.Length-1]);
                            String sqltxt = "select * from " + GetTableNameByDateTime(datetimes[datetimes.Length - 1]) + " where carid=" + carid + " and dttrace <= '" + dtend.ToString() + "'" + statusLimit;
                            GetGPSCoordList(tracePoints,sqltxt);
                        }else{
                            string tablename = string.Format("{0:yyyyMM}", datetimes[i]);
                            String sqltxt = "select * from " + GetTableNameByDateTime(datetimes[i]) + " where carid=" + carid + statusLimit;
                            GetGPSCoordList(tracePoints,sqltxt);
                        }
                    }
                }                
            }            

            return tracePoints;

        }


        public List<GPSCoord> GetGPSCoordList(List<GPSCoord> tracepoints, string sqlTxt)
        {            
            SqlConnection connection = null;
            SqlDataReader reader = null;           
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                GPSCoord tracePoint;
                while (reader.Read())
                {
                    
                    int RollerID = Convert.ToInt32(reader["carid"]);
                    double X = Convert.ToDouble((reader["x"]));
                    double Y = Convert.ToDouble((reader["y"]));
                    double Z = Convert.ToDouble((reader["z"]));
                    double V = Convert.ToInt32((reader["v"]));
                    DateTime When = Convert.ToDateTime(reader["dttrace"]);
                    int LibratedStatus = Convert.ToInt32(reader["libratedstatus"]);

                    tracePoint = new GPSCoord(X,Y,Z,V,0,When,LibratedStatus);
                    tracepoints.Add(tracePoint);                
                }
                return tracepoints;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(connection);
            }
        }
        
    }
}
