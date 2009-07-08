using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DamLKK.Utils;
using DamLKK._Model;

namespace DamLKK.DB.datamap
{
    class DAO
    {
        private DAO() { }

        private static DAO myInstance = null;

        public static DAO getInstance()
        {
            if (myInstance == null)
            {
                myInstance = new DAO();
            }
            return myInstance;
        }

        public static bool updateElevations(int blockid, double designz, int segmentid, string elevations)
        {

            string sqlTxt = "update segment set elevations = '" + elevations +
                "' where blockid=" + blockid + " and segmentid=" + segmentid +
                " and designz=" + designz;
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

        //得到当前高程的上一个高程
        public double getLastDesignZ(int blockid, double designz, int segmentid, String vertex)
        {
            List<Segment> segments = new List<Segment>();
            SqlConnection connection = null;
            SqlDataReader reader = null;
            List<double> last_designzs = new List<double>();
            List<String> last_vertexs = new List<string>();
            //dtend = "2009/4/23 17:00:00";
            String getdesignDepth = "(select designdepth from segment where blockid=" + blockid + " and segmentid=" + segmentid + " and designz=" + designz + ")";
            String sqlTxt = "select * from segment where blockid=" + blockid + " and (designz < " + (designz.ToString()) + "-" + getdesignDepth + "*0.5) and designz>(" + designz + "-" + getdesignDepth + "*1.25) and workstate=2 order by dtend desc";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                while (reader.Read())
                {
                    double d = (Double.Parse(reader["designz"].ToString()));
                    string v = reader["vertex"].ToString();
                    last_designzs.Add(d);
                    last_vertexs.Add(v);
                }
            }
            catch
            {
                return -1;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(connection);
            }

            if (last_designzs == null || last_designzs.Count == 0)
            {
                return -1;
            }
            else if (last_designzs.Count == 1)
            {
                return last_designzs[0];
            }
            else
            {
                int count = last_designzs.Count;
                List<PointF>[] points = new List<PointF>[count];
                Color[] colors = new Color[count];
                int[] pixel_count = new int[count];
                int color_range = 255 / (count + 2);
                List<PointF> all = new List<PointF>();
                List<PointF> up_Points = DataMapManager4.getSegmentVertex_DAM(vertex);//上一层所有点
                all.AddRange(up_Points);
                for (int i = 0; i < count; i++)
                {
                    int rgb = (i + 1) * color_range;
                    colors[i] = Color.FromArgb(rgb, rgb, rgb);//设定颜色
                    points[i] = DataMapManager4.getSegmentVertex_DAM(last_vertexs[i]);
                    all.AddRange(points[i]);
                }
                PointF origin = DataMapManager4.getOrigin(all.ToArray());
                up_Points = DataMapManager4.getRelatively(origin, up_Points);
                for (int i = 0; i < count; i++)
                {
                    points[i] = DataMapManager4.getRelatively(origin, points[i]);
                }
                int left_index = DataMapManager4.getLeftIndex(all);
                int right_index = DataMapManager4.getRightIndex(all);
                int top_index = DataMapManager4.getTopIndex(all);
                int bottom_index = DataMapManager4.getBottomIndex(all);
                int map_width =(int)Math.Round(all[right_index].X - all[left_index].X);
                int map_height = (int)Math.Round(all[bottom_index].Y - all[top_index].Y);

                Bitmap[] bitmaps = new Bitmap[count];
                Graphics[] graphicses = new Graphics[count];

                for (int i = 0; i < count; i++)
                {
                    bitmaps[i] = new Bitmap(map_width, map_height);
                    graphicses[i] = Graphics.FromImage(bitmaps[i]);
                    graphicses[i].Clear(Color.White);
                }

                System.Drawing.Drawing2D.GraphicsPath up_GraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                up_GraphicsPath.Reset();
                up_GraphicsPath.AddPolygon(up_Points.ToArray());

                for (int i = 0; i < count; i++)
                {
                    System.Drawing.Drawing2D.GraphicsPath this_GraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                    this_GraphicsPath.Reset();
                    this_GraphicsPath.AddPolygon(points[i].ToArray());
                    System.Drawing.Region this_region = new Region(this_GraphicsPath);
                    graphicses[i].FillRegion(Brushes.Red, this_region);
                    //本region与主region的交集
                    this_region.Intersect(up_GraphicsPath);
                    graphicses[i].FillRegion(new SolidBrush(colors[i]), this_region);
                    graphicses[i].Flush();
                    //bitmaps[i].Save("c:/" + blockid + " " + designz + " " + segmentid +" -"+i+ ".png", ImageFormat.Png);
                }


                for (int i = 0; i < count; i++)
                {

                    for (int x = 0; x < map_width; x++)
                    {
                        for (int y = 0; y < map_height; y++)
                        {
                            Color c = bitmaps[i].GetPixel(x, y);

                            if (c.A == colors[i].A && c.R == colors[i].R && c.G == colors[i].G)
                            {
                                pixel_count[i]++;
                                break;
                            }
                        }
                    }
                }
                int max = 0;
                for (int i = 0; i < count; i++)
                {
                    if (max < pixel_count[i])
                    {
                        max = pixel_count[i];
                    }
                }
                if (max != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (max == pixel_count[i])
                        {
                            return last_designzs[i];
                        }
                    }
                }
                return -1;
            }
        }

        public List<Segment> getSegments(Int32 blockID, Double designZ)
        {
            List<Segment> segments = new List<Segment>();
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "select * from segment where (blockid=" + blockID + ") and (designZ=" + designZ + ") and workstate=2 ";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                while (reader.Read())
                {
                    segments.Add(readSegment(reader));
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

        public List<Segment> getSegments(int blockid, double begin_designz, double end_designz)
        {
            List<Segment> segments = new List<Segment>();
            Segment segment = null;
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "select * from segment where blockid=" + blockid + " and designz between " + begin_designz + " and " + end_designz + "and workstate=2 order by designz desc,segmentid desc";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                while (reader.Read())
                {
                    //                     SegmentWorkState workState = (SegmentWorkState)Convert.ToInt32(reader["workState"]);
                    //                     //Int32 segmentid = Convert.ToInt32(reader["segmentid"]);
                    //                     string vertex = reader["vertex"].ToString();
                    //                     DateTime enddate = DateTime.MinValue;
                    //                     DateTime startdate = DateTime.MinValue;
                    //                     if (!reader["dtend"].Equals(DBNull.Value))
                    //                     {
                    //                         enddate = Convert.ToDateTime(reader["dtend"]);
                    //                     }
                    //                     if (!reader["dtstart"].Equals(DBNull.Value))
                    //                     {
                    //                         startdate = Convert.ToDateTime(reader["dtstart"]);
                    //                     }
                    //                     string remark = reader["remark"].ToString();
                    //                     string segmentname = reader["segmentname"].ToString();
                    //                     Double startZ = Convert.ToDouble(reader["startz"]);
                    //                     Double maxSpeed = Convert.ToDouble(reader["maxspeed"]);
                    //                     Int32 designRollCount = Convert.ToInt32(reader["designRollCount"]);
                    //                     Double errorParam = Convert.ToDouble(reader["errorParam"]);
                    //                     segment = new Segment();
                    //                     segment.MaxSpeed = maxSpeed;
                    //                     segment.DesignRollCount = designRollCount;
                    //                     segment.ErrorParam = errorParam;
                    //                     segment.Remark = (remark);
                    //                     segment.BlockID = (blockID);
                    //                     segment.SegmentID = (segmentid);
                    //                     segment.WorkState = (DB.SegmentWorkState)(workState);
                    //                     segment.DesignZ = (designZ);
                    //                     segment.Vertext = vertex;
                    //                     segment.StartDate = (startdate);
                    //                     segment.EndDate = (enddate);
                    //                     segment.SegmentName = (segmentname);
                    //                     segment.StartZ = startZ;
                    //                     segment.POP = (double)reader["POP"];
                    //                     segment.DesignDepth = (double)reader["DESIGNDEPTH"];
                    segment = readSegment(reader);
                    segments.Add(segment);


                }
                return segments;

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

        public Segment getSegment(Int32 blockID, double designZ, Int32 segmentid)
        {

            Segment segment = null;
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "select * from segment where (blockid=" + blockID + ") and (designZ=" + designZ +
                ") and (segmentid=" + segmentid + ")";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                if (reader.Read())
                {
                    //                     SegmentWorkState workState = (SegmentWorkState)Convert.ToInt32(reader["workState"]);
                    //                     //Int32 segmentid = Convert.ToInt32(reader["segmentid"]);
                    //                     string vertex = reader["vertex"].ToString();
                    //                     DateTime enddate = DateTime.MinValue;
                    //                     DateTime startdate = DateTime.MinValue;
                    //                     if (!reader["dtend"].Equals(DBNull.Value))
                    //                     {
                    //                         enddate = Convert.ToDateTime(reader["dtend"]);
                    //                     }
                    //                     if (!reader["dtstart"].Equals(DBNull.Value))
                    //                     {
                    //                         startdate = Convert.ToDateTime(reader["dtstart"]);
                    //                     }
                    //                     string remark = reader["remark"].ToString();
                    //                     string segmentname = reader["segmentname"].ToString();
                    //                     Double startZ = Convert.ToDouble(reader["startz"]);  
                    //                     Double maxSpeed = Convert.ToDouble(reader["maxspeed"]);
                    //                     Int32 designRollCount = Convert.ToInt32(reader["designRollCount"]);
                    //                     Double errorParam = Convert.ToDouble(reader["errorParam"]);
                    //                     segment = new Segment();
                    //                     segment.MaxSpeed = maxSpeed;
                    //                     segment.DesignRollCount = designRollCount;
                    //                     segment.ErrorParam = errorParam;
                    //                     segment.Remark = (remark);
                    //                     segment.BlockID = (blockID);
                    //                     segment.SegmentID = (segmentid);
                    //                     segment.WorkState = (DB.SegmentWorkState)(workState);
                    //                     segment.DesignZ = (designZ);
                    //                     segment.Vertext = vertex;
                    //                     segment.StartDate = (startdate);
                    //                     segment.EndDate = (enddate);
                    //                     segment.SegmentName = (segmentname);
                    //                     segment.StartZ = startZ;
                    //                     segment.POP = (double)reader["POP"];
                    //                     segment.DesignDepth = (double)reader["DESIGNDEPTH"];
                    segment = readSegment(reader);

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

        private Segment readSegment(SqlDataReader reader)
        {
            Segment segment = new Segment();
            DeckWorkState workState = (DeckWorkState)Convert.ToInt32(reader["workState"]);
            //Int32 segmentid = Convert.ToInt32(reader["segmentid"]);
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
            Double startZ = Convert.ToDouble(reader["startz"]);
            Double maxSpeed = Convert.ToDouble(reader["maxspeed"]);
            Int32 designRollCount = Convert.ToInt32(reader["designRollCount"]);
            Double errorParam = Convert.ToDouble(reader["errorParam"]);
            string elevationValues = reader["elevationValues"].ToString();
            segment = new Segment();
            segment.MaxSpeed = maxSpeed;

            segment.ElevationValues = elevationValues;
            segment.DesignRollCount = designRollCount;
            segment.ErrorParam = errorParam;
            segment.Remark = (remark);
            segment.BlockID = (int)(reader["BLOCKID"]);
            segment.SegmentID = (int)(reader["SEGMENTID"]);
            segment.WorkState = (DeckWorkState)(workState);
            segment.DesignZ = (double)(reader["DESIGNZ"]);
            //segment.BlockName=
            segment.StartDate = (startdate);
            segment.EndDate = (enddate);
            segment.SegmentName = (segmentname);
            segment.StartZ = startZ;
            segment.POP = (double)reader["POP"];
            segment.DesignDepth = (double)reader["DESIGNDEPTH"];
            segment.Vertext = vertex;

            return segment;
        }

        public byte[] getDatamap(int blockid, double designz, int segmentid)
        {
            string sqltext = string.Format(
                "select DATAMAP from SEGMENT where (BLOCKID ={0}) and (DESIGNZ={1}) and (SEGMENTID={2})",
                blockid, designz, segmentid);
            SqlConnection connection = null;
            SqlDataReader reader = null;
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqltext);
                if (reader != null)
                {
                    if (reader.Read())
                        return (byte[])reader["DATAMAP"];
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public byte[] ToByte(Image image)
        {
            System.IO.MemoryStream Ms = new System.IO.MemoryStream();
            image.Save(Ms, System.Drawing.Imaging.ImageFormat.Bmp);//把图像数据序列化到内存
            byte[] imgByte = new byte[Ms.Length];
            Ms.Position = 0;
            Ms.Read(imgByte, 0, Convert.ToInt32(Ms.Length));//反序列，存放在字节数组里
            Ms.Close();

            return imgByte;//这里我们就得到了图像的字节数组了

        }



        public int updateElevationBitMap(Int32 blockid, Double designz, Int32 segmentid, byte[] elevationImage, string values)
        {
            string sqlTxt = "update segment set elevationImage  = @elevationImage,elevationValues='" + values + "'  where blockid = " + blockid + " and designz=" + designz + " and segmentid=" + segmentid;

            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                cmd = new SqlCommand(sqlTxt, conn);
                SqlParameter sqlImage = cmd.Parameters.Add("@elevationImage", SqlDbType.Image);
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


        public int updateRollBitMap(Int32 blockid, Double designz, Int32 segmentid, byte[] rollImage)
        {
            string sqlTxt = "update segment set rollImage  = @rollImage where blockid = " + blockid + " and designz=" + designz + " and segmentid=" + segmentid;

            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                cmd = new SqlCommand(sqlTxt, conn);
                SqlParameter sqlImage = cmd.Parameters.Add("@rollImage", SqlDbType.Image);
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

        public Bitmap getElevationBitMap(int blockid, double designz, int segmentid)
        {
            string sqltext = string.Format(
                "select elevationImage  from SEGMENT where (BLOCKID ={0}) and (DESIGNZ={1}) and (SEGMENTID={2})", blockid, designz, segmentid);
            SqlConnection connection = null;
            SqlDataReader reader = null;
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqltext);
                if (reader != null)
                {
                    if (reader.Read()){
                        byte[] b = (byte[])reader["elevationImage"];
                        MemoryStream stream = new MemoryStream(b, true);
                        stream.Write(b, 0, b.Length);
                        Bitmap bitmap=new Bitmap(stream);
                        stream.Close();
                        return bitmap;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

    }

}