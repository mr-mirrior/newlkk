using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DamLKK.Utils;
using DamLKK._Model;

namespace DamLKK.DB
    {
        //结束仓面结果
        public enum EndSegmengResult
        {
            THIS_LAYER_END = 1,//当前层结束

            ONLY_SEGMENT_END,//只有仓面被结束

            END_ERROR//结束失败
        }
   
        //操作仓面,包含车辆车辆操作
        public enum DeckVehicleResult
        {
            SUCCESS = 1,//成功
            SEGMENT_FAIL,//仓面操作失败
            CARS_FAIL //车辆操作失败
        }
        public enum UpdateDeckResult
        {
            NO_SEGMENT = 1,//没有仓面信息
            SUCCESS,//成功
            FAIL_BUT_SEGMENT_DELETED,//失败,但是删除了仓面信息

            FAIL//失败
        }

        public class DeckDAO
        {
            private DeckDAO() { }

            private static DeckDAO _MyInstance = null;

            public static DeckDAO GetInstance()
            {
                if (_MyInstance == null)
                {
                    _MyInstance = new DeckDAO();
                }
                return _MyInstance;
            }


            
           
            /// <summary>
            /// 添加舱面
            /// </summary>
            /// <returns></returns>
            public bool AddDeck(DamLKK._Model.Deck p_Deck)
            {
                int segmentID = 0;
                DamLKK._Model.DeckWorkState workState = p_Deck.WorkState;
                int unitID = p_Deck.Unit.ID;
                double designZ = p_Deck.Elevation.Height;
                string vertext = p_Deck.Vertex;
                DateTime startDate = p_Deck.DTStart;
                DateTime endDate = p_Deck.DTEnd;
                double maxSpeed = p_Deck.MaxSpeed;
                string designRollCount = p_Deck.NOLibRollCount.ToString()+","+p_Deck.LibRollCount.ToString();
                double errorParam = p_Deck.ErrorParam;
                double spreadZ = p_Deck.SpreadZ;
                double designDepth = p_Deck.DesignDepth;
                double pop = p_Deck.POP;
                string segmentName = p_Deck.Name;
                Double startZ = p_Deck.StartZ;
                string startDateStr = "'" + startDate.ToString() + "'";
                string endDateStr = "'" + endDate.ToString() + "'";

                if (startDate.Equals(DateTime.MinValue))
                {
                    startDateStr = "NULL";
                }
                if (endDate.Equals(DateTime.MinValue))
                {
                    endDateStr = "NULL";
                }

                string sqlTxt = "select max(segmentid)+1 from segment where unitid=" + unitID + " and designz=" + designZ;
                SqlDataReader dr = DBConnection.executeQuery(DBConnection.getSqlConnection(), sqlTxt);

                if (dr.Read())
                {
                    if (dr[0] != DBNull.Value)
                        segmentID = (int)dr[0];
                }

                sqlTxt = string.Format("insert into segment  (SegmentID, WorkState, UnitID, DesignZ, Vertex, DTStart, DTEnd, MaxSpeed, DesignRollCount, ErrorParam, SpreadZ, DesignDepth, SegmentName,StartZ,pop) values(" +
                    "{0},{1},{2},'{3}','{4}',{5},{6},{7},'{8}',{9},{10},'{11}','{12}','{13}','{14}'"
                    + ")", segmentID, (int)workState, unitID, designZ, vertext, startDateStr, endDateStr, maxSpeed, designRollCount, errorParam, spreadZ, designDepth, segmentName, startZ, pop);
                
                 try
                 {
                    if (DBConnection.executeUpdate(sqlTxt) != 1)
                    {
                        return false;
                    }
                }
                catch (Exception exp)
                {
                    DebugUtil.log(exp);
                    return false;
                }
                finally
                {
                    DBConnection.closeDataReader(dr);
                }
                return true;
            }


            /// <summary>
            /// 删除仓面
            /// </summary>
            public bool DeleteDeck(int p_unitid, double p_designz, int p_deckid)
            {
                String sqlTxt = "delete from segment where unitid = " + p_unitid + " and designz='" + p_designz + "' and segmentid=" + p_deckid;
                try
                {
                    int updateCount = DBConnection.executeUpdate(sqlTxt);
                    if (updateCount != 1)
                    {
                        return false;
                    }
                }
                catch (Exception exp)
                {
                    DebugUtil.log(exp);
                    return false;
                }
          
                
                return true;
            }


            /// <summary>
            /// 修改仓面信息
            /// </summary>
            public bool ModifyDeck(DamLKK._Model.Deck p_Deck)
            {
                int segmentID = p_Deck.ID;
                DamLKK._Model.DeckWorkState workState = p_Deck.WorkState;
                int unitID = p_Deck.Unit.ID;
                double designZ = p_Deck.Elevation.Height;
                string vertext = p_Deck.Vertex;
                DateTime startDate = p_Deck.DTStart;
                DateTime endDate = p_Deck.DTEnd;
                double maxSpeed = p_Deck.MaxSpeed;
                string designRollCount = p_Deck.NOLibRollCount.ToString()+","+p_Deck.LibRollCount.ToString();
                double errorParam = p_Deck.ErrorParam;
                double spreadZ = p_Deck.SpreadZ;
                double designDepth = p_Deck.DesignDepth;
                double pop = p_Deck.POP;
                string segmentName = p_Deck.Name;
                double startZ = p_Deck.StartZ;

                string startDateStr = "'" + startDate.ToString() + "'";
                string endDateStr = "'" + endDate.ToString() + "'";
                if (startDate.Equals(DateTime.MinValue))
                {
                    startDateStr = "NULL";
                }
                if (endDate.Equals(DateTime.MinValue))
                {
                    endDateStr = "NULL";
                }
                string sqlTxt = string.Format("update segment set SegmentID={0}, WorkState={1}, unitID={2}, DesignZ='{3}', Vertex='{4}', DTStart={5}, DTEnd={6}, MaxSpeed='{7}', DesignRollCount='{8}', ErrorParam='{9}', SpreadZ='{10}', DesignDepth='{11}', SegmentName='{12}',StartZ='{13}',pop='{14}',SenseOrganState='{15}',NotRolling='{16}',CommentNR='{17}' where unitid={18} and designz={19} and segmentid={20}",
                   segmentID, (int)workState, unitID, designZ, vertext, startDateStr, endDateStr, maxSpeed, designRollCount, errorParam, spreadZ, designDepth, segmentName, startZ, pop, 0, 0, 0, unitID, designZ, segmentID);
                try
                {
                     if (DBConnection.executeUpdate(sqlTxt) != 1)
                     {
                       return false;
                     }
                }
                catch (Exception exp)
                {
                    DebugUtil.log(exp);
                    return false;
                }
                return true;

            }

            /// <summary>
            /// 更新仓面的边界点
            /// </summary>
            /// <param name="deck"></param>
            /// <returns></returns>
            public bool UpdateVertex(DamLKK._Model.Deck deck)
            {

                string sqlTxt = "update segment set Vertex='"+deck.Vertex+"' where unitid="+deck.Unit.ID+" and Designz='"+deck.Elevation.Height.ToString()+"'and Segmentid="+deck.ID.ToString();
                try
                {
                    if (DBConnection.executeUpdate(sqlTxt) != 1)
                    {
                        return false;
                    }
                }
                catch (Exception exp)
                {
                    DebugUtil.log(exp);
                    return false;
                }
                return true;
            }

            /// <summary>
            /// 获得某单元的某高程上的所有仓面
            /// </summary>
            public List<Deck> GetDecks(int unitid, double designZ)
            {
                List<Deck> segments = new List<Deck>();
                SqlConnection connection = null;
                SqlDataReader reader = null;
                string sqlTxt = "select * from segment where (unitid=" + unitid+ ") and (designZ='" + designZ + "')";
                try
                {
                    connection = DBConnection.getSqlConnection();
                    reader = DBConnection.executeQuery(connection, sqlTxt);
                    while (reader.Read())
                    {
                        Deck segment = ReadDeck(reader);

                        segments.Add(segment);
                    }
                    return segments;
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

            /// <summary>
            /// 读仓面
            /// </summary>
            private Deck ReadDeck(SqlDataReader reader)
            {
                Deck segment = new Deck();
                DeckWorkState workState = (DeckWorkState)Convert.ToInt32(reader["workState"]);;
                string vertex = reader["vertex"].ToString();
                DateTime enddate = DateTime.MinValue;
                DateTime startdate = DateTime.MinValue;
                if (!reader["dtend"].Equals(DBNull.Value))
                {
                    enddate = Convert.ToDateTime(reader["dtend"]);
                }
                if (!reader["dtstart"].Equals(DBNull.Value))
                {
                    startdate = Convert.ToDateTime(reader["dtstart"]);
                }
                string remark = reader["remark"].ToString();
                string segmentname = reader["segmentname"].ToString();
                double startZ = Convert.ToDouble(reader["startz"]);
                double maxSpeed = Convert.ToDouble(reader["maxspeed"]);
                string designRollCount = reader["designRollCount"].ToString();
                double errorParam = Convert.ToDouble(reader["errorParam"]);
                segment.MaxSpeed = maxSpeed;
                string[] RollCount= designRollCount.Split(',');
                if(RollCount.Length!=2)
                    return null;
                segment.NOLibRollCount = Convert.ToInt32(RollCount[0]);
                segment.LibRollCount = Convert.ToInt32(RollCount[1]);
                segment.ErrorParam = errorParam;
                segment.Unit.ID = Convert.ToInt32(reader["unitid"]);
                segment.ID = Convert.ToInt32(reader["segmentid"]);
                segment.WorkState = (DeckWorkState)workState;
                segment.Elevation.Height = Convert.ToDouble(reader["designz"]);
                segment.Vertex = vertex;
                segment.DTStart= startdate;
                segment.DTEnd = enddate;
                segment.Name= (segmentname);
              
                segment.StartZ = startZ;
                segment.POP = (double)reader["POP"];
                segment.DesignDepth = (double)reader["designdepth"];
                //if (reader["SenseOrganState"] != DBNull.Value)
                //    segment.LibratedCounts =reader["SenseOrganState"].ToString();
                return segment;
            }

            /// <summary>
            /// 获取指定仓面
            /// </summary>
            public Deck GetDeck(int unitID, double designZ, int segmentid)
            {
                Deck segment = null;
                SqlConnection connection = null;
                SqlDataReader reader = null;
                string sqlTxt = "select * from segment where (unitid=" + unitID + ") and (designZ='" + designZ +
                    "') and (segmentid=" + segmentid + ")";
                try
                {
                    connection = DBConnection.getSqlConnection();
                    reader = DBConnection.executeQuery(connection, sqlTxt);
                    while (reader.Read())
                    {
                        segment = ReadDeck(reader);
                    }
                    return segment;
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
            /// // 启动某分区下的某工作层下的全部舱面
            /// </summary>
            public Boolean StartAllDecks(int blockid, double designz)
            {
                string sqlTxt = "update segment set workstate=" + (int)DeckWorkState.WORK +
                    ",dtstart=getdate() where blockid=" + blockid + " and designz='" + designz +
                    "' and wrokstate =" + (int)DeckWorkState.WAIT;
                try
                {
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
            }


           
            /// <summary>
            ///  // 启动某分区下的某工作层下的舱面,设置仓面状态和启动时间
            /// </summary>
            public bool StartThisDeck(int unitid, double designZ, int segmentid, DeckWorkState state)
            {
                string sqlTxt = "update segment set workstate=" + (int)DeckWorkState.WORK +
                    ",dtstart=getdate() where unitid=" + unitid + " and segmentid=" + segmentid +
                    " and designz='" + designZ + "' and workstate<>" + (int)DeckWorkState.WORK;

                if (state == DeckWorkState.END)
                {
                    sqlTxt = "update segment set workstate=" + (int)DeckWorkState.WORK +
                    ",dtend=null where unitid=" + unitid + " and segmentid=" + segmentid +
                    " and designz='" + designZ + "' and workstate<>" + (int)DeckWorkState.WORK;
                }

                try
                {
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

            }


            /// <summary>
            ///  // 结束某分区下的某工作层下的正在工作的舱面.
            /// //如果该舱面是最后一个未被结束的舱面,则结束该层.
            /// </summary>
            public EndSegmengResult EndThisDeck(int unitid, double designZ, int segmentid)
            {
                SqlConnection connection = null;
                SqlDataReader reader = null;
                string sqlTxt = "update segment set workstate=" + (int)DeckWorkState.END +
                    ",dtend=getdate() where unitid=" +unitid + " and segmentid=" + segmentid +
                    " and designZ='" + designZ+"'" ;
                try
                {
                    int updateCount = DBConnection.executeUpdate(sqlTxt);

                    if (updateCount <= 0)
                    {
                        return EndSegmengResult.END_ERROR;
                    }
                    //结束仓面更新unit
                    sqlTxt = "update unit set endtime=getdate() where id=" + unitid ;

                    updateCount = DBConnection.executeUpdate(sqlTxt);

                    if (updateCount <= 0)
                    {
                        return EndSegmengResult.END_ERROR;
                    }

                    return EndSegmengResult.ONLY_SEGMENT_END;

                    // 查看当前处于非结束状态的segment的数量

                }
                catch (Exception exp)
                {
                    DebugUtil.log(exp);
                    return EndSegmengResult.END_ERROR;
                }
                finally
                {
                    DBConnection.closeDataReader(reader);
                    DBConnection.closeSqlConnection(connection);
                }
            }


            
            /// <summary>
            /// /// 包含车辆操作的启动仓面
            /// </summary>
            public DeckVehicleResult StartDeck(int unitid, double designZ, int segmentid, double maxSpeed, DeckWorkState state)
            {
                //更新舱面状态.
                if (StartThisDeck(unitid, designZ, segmentid, state))
                {
                    //分配车辆.
                    RollerDis cd = new RollerDis();
                    cd.UnitID =unitid;
                    cd.DesignZ = designZ;
                    cd.SegmentID = segmentid;
                    Deck deck = GetDeck(unitid, designZ, segmentid);
                    if (CarDistributeDAO.GetInstance().StartCars(cd, maxSpeed, deck.NOLibRollCount.ToString()+","+deck.LibRollCount.ToString(), deck.Elevation.Height))
                    {

                        return DeckVehicleResult.SUCCESS;
                    }


                    else
                    {
                        return DeckVehicleResult.CARS_FAIL;
                    }
                }
                return DeckVehicleResult.SEGMENT_FAIL;
            }
           
            /// <summary>
            /// //包含车辆操作的结束仓面
            /// </summary>
            public DeckVehicleResult EndDeck(int unitid, double designZ, int segmentid)
            {
                //结束本仓面全部车辆.		
                RollerDis cd = new RollerDis();
                cd.UnitID = unitid;
                cd.DesignZ = designZ;
                cd.SegmentID = segmentid;

                if (CarDistributeDAO.GetInstance().EndCars(cd) >= 0)
                {//成功结束了车辆

                    if (EndThisDeck(unitid, designZ, segmentid) != EndSegmengResult.END_ERROR)
                    {
                        return DeckVehicleResult.SUCCESS;
                    }
                    else
                    {
                        return DeckVehicleResult.SEGMENT_FAIL;
                    }
                }
                return DeckVehicleResult.CARS_FAIL;
            }


            /// <summary>
            /// //更新指定仓面的pop值
            /// </summary>
            public bool SetDeckPOP(int unitid, double designz, int segmentid, double pop)
            {
                string sqlTxt = "update segment set pop  = '" + pop + "'  where unitid = " + unitid + " and designz='" + designz + "' and segmentid=" + segmentid;
                try
                {
                    return (DBConnection.executeUpdate(sqlTxt) == 1);
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    DebugUtil.log(E);
                    return false;
                }
            }

            /// <summary>
            /// 更新数据库仓面面积和碾压遍数百分比字段
            /// </summary>
            public bool UpdateDeckAreaAndRollingPercentages(int unitid, double designz, int segmentid, double area, string perent)
            {
                string sqlTxt = "update segment set SegmentArea = '" + area + "',RollingPercentages='" + perent + "'  where unitid = " + unitid + " and designz='" + designz + "' and segmentid=" + segmentid;
                try
                {
                    return (DBConnection.executeUpdate(sqlTxt) == 1);
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    DebugUtil.log(E);
                    return false;
                }
            }

            /// <summary>
            /// 更新高程图字段
            /// </summary>
            public int UpdateElevationBitMap(int unitid, double designz, int segmentid, byte[] elevationImage, string values)
            {
                string sqlTxt = "update segment set elevationImage  = @elevationImage,elevationValues='" + values + "'  where unitid = " + unitid + " and designz='" + designz + "' and segmentid=" + segmentid;

                SqlConnection conn = null;
                SqlCommand cmd = null;
                try
                {
                    conn = DBConnection.getSqlConnection();
                    cmd = new SqlCommand(sqlTxt, conn);
                    SqlParameter sqlImage = cmd.Parameters.Add("@elevationImage", System.Data.SqlDbType.Image);
                    sqlImage.Value = elevationImage;
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    DebugUtil.log(E);
                    return -1;
                }
                finally
                {
                    DBConnection.closeSqlConnection(conn);
                }

            }

            /// <summary>
            /// 更新边数图字段
            /// </summary>
            public int UpdateRollBitMap(int unitid, double designz,int segmentid, byte[] rollImage)
            {
                string sqlTxt = "update segment set rollImage  = @rollImage where unitid = " + unitid + " and designz='" + designz + "' and segmentid=" + segmentid;

                SqlConnection conn = null;
                SqlCommand cmd = null;
                try
                {
                    conn = DBConnection.getSqlConnection();
                    cmd = new SqlCommand(sqlTxt, conn);
                    SqlParameter sqlImage = cmd.Parameters.Add("@rollImage", System.Data.SqlDbType.Image);
                    sqlImage.Value = rollImage;
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    DebugUtil.log(E);
                    return -1;
                }
                finally
                {
                    DBConnection.closeSqlConnection(conn);
                }

            }

            /// <summary>
            /// 图片转字节流
            /// </summary>
            public byte[] ToByte(System.Drawing.Image image)
            {
                System.IO.MemoryStream Ms = new System.IO.MemoryStream();
                image.Save(Ms, System.Drawing.Imaging.ImageFormat.Bmp);//把图像数据序列化到内存
                byte[] imgByte = new byte[Ms.Length];
                Ms.Position = 0;
                Ms.Read(imgByte, 0, Convert.ToInt32(Ms.Length));//反序列，存放在字节数组里
                Ms.Close();

                return imgByte;//这里我们就得到了图像的字节数组了

            }


            /// <summary>
            /// 读取指定舱面的备注信息
            /// </summary>
            public string ReadSegmentRemark(int unitid, double designz, int segmentid)
            {
                string sqlTxt = "select remark from Segment" +
                    "  where unitid = " + unitid +
                    " and designz='" + designz +
                    "' and segmentid=" + segmentid;
                try
                {
                    SqlDataReader dr = DBConnection.executeQuery(DBConnection.getSqlConnection(), sqlTxt);
                    while (dr.Read())
                    {
                        if (dr["Remark"] == DBNull.Value)
                        {
                            return string.Empty;
                        }
                        return dr["Remark"].ToString();
                    }
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    DebugUtil.log(E);
                    return string.Empty;
                }
                return string.Empty;
            }
        }
    }
