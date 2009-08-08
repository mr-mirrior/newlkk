using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
namespace DamLKK.DB.datamap
{
    class DataMapManager4
    {
        //传入当地坐标

        public static int grid = 5;//"大网格"的边长, 单位m

        public static float WIDTH = 0.5f;//每个正方形网格的宽度/单位米
        public static int SCREEN_ONEMETER = 20;//每米的像素数,每个网格的大小刚好是SCREEN_ONEMETER的一半
        public static double sin = 0.5509670120356448784912018921605;
        public static double cos = 0.83452702271916488948079272306091;
        static int map_left = 80;//主图形左侧
        static int map_bottom = 200;//主图形下侧
        static int map_right = 80;//右侧
        static int map_top = 160;//上侧

        public static Bitmap[] draw(int blockid, double designz, int segmentid)
        {
//              string valuesstrs = "604.52,609.89";
//              Bitmap eimage = new Bitmap("d:/elevation/ED607.20OrignElevation.png");//segment.ElevationImage;
//              DAO.getInstance().updateElevationBitMap(blockid, 607.2, 0, DAO.getInstance().ToByte(eimage), valuesstrs);
// 

            //从数据库中读出本仓面,和本仓面上一层的所有仓面信息.
            Segment segment = DAO.getInstance().getSegment(blockid, designz, segmentid);
            if (segment == null)
                return null;
            //得到上一层的所有仓面信息
            DateTime dtend = segment.EndDate;
            String vertex = segment.Vertext;//大地坐标的vertex
            double lastDesignz = DAO.getInstance().getLastDesignZ(blockid, designz, segmentid, vertex);
            List<Segment> segments = DAO.getInstance().getSegments(blockid, lastDesignz);
            //将上一层所有仓面的数据图读出来
            
                        if (segments == null || segments.Count == 0)
                        {
                            return null;
                        }

                        for (int ii = 0; ii < segments.Count; ii++)
                        {

                            //byte[] datamap = DAO.getInstance().getDatamap(blockid, lastDesignz, segments[ii].SegmentID);
                            Bitmap this_e_map = DAO.getInstance().getElevationBitMap(blockid, lastDesignz, segments[ii].SegmentID);
                            if (this_e_map == null)
                            {
                                DamLKK.Utils.DebugUtil.fileLog("没有elevationImage图" + blockid + " " + lastDesignz + " " + segments[ii].SegmentID);
                                 return null;
                            }
                            else
                            {
                                segments[ii].ElevationImage = this_e_map;
                            }
                        }
            
            
            //byte[] bytes = DAO.getInstance().getDatamap(blockid, designz, segmentid);//本仓面的数据图

            Bitmap this_elevation_map = DAO.getInstance().getElevationBitMap(blockid, designz, segmentid);
            if (this_elevation_map== null)
            {

                DamLKK.Utils.DebugUtil.fileLog("没有elevationImage图" + blockid + " " + designz + " " + segmentid);
                return null;
            }
            segment.ElevationImage = this_elevation_map;

            //DataMap dm = new DataMap(bytes);

            //通过vertex得到大坝坐标系的边界点
            List<PointF> dam_points = getSegmentVertex_DAM(vertex);
            //大坝坐标系下的外切矩阵的原点,把屏幕坐标转换成大坝坐标会用到
            PointF dam_origin = getOrigin(dam_points.ToArray());
            //转换成相对这组坐标的原点的“新点”也相当于屏幕坐标
            List<PointF> screen_points = getRelatively(dam_points);
            //取得大坝坐标系边界点的外切矩阵
            int left_index = getLeftIndex(screen_points);
            int right_index = getRightIndex(screen_points);
            int top_index = getTopIndex(screen_points);
            int bottom_index = getBottomIndex(screen_points);
            //外切矩形四个顶点
            PointF left_top = new PointF(screen_points[left_index].X, screen_points[top_index].Y);
            PointF left_bottom = new PointF(screen_points[left_index].X, screen_points[bottom_index].Y);
            PointF right_top = new PointF(screen_points[right_index].X, screen_points[top_index].Y);
            PointF right_bottom = new PointF(screen_points[right_index].X, screen_points[bottom_index].Y);
            //大坝坐标下本仓面外切矩形的宽度
            int segment_dam_width =(int) Math.Round( screen_points[right_index].X - screen_points[left_index].X);
            //大坝坐标下本仓面外切矩形的高度
            int segment_dam_height = (int) Math.Round( screen_points[bottom_index].Y - screen_points[top_index].Y);
            //定义出图像大小
            Bitmap thickness_map = new Bitmap(segment_dam_width + map_left + map_right, segment_dam_height + map_top + map_bottom);
            Bitmap elevation_map = new Bitmap(segment_dam_width + map_left + map_right, segment_dam_height + map_top + map_bottom);

            Graphics thickness_g = Graphics.FromImage(thickness_map);
            thickness_g.SmoothingMode = SmoothingMode.HighQuality;
            thickness_g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            thickness_g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            thickness_g.Clear(Color.White);

            Graphics elevation_g = Graphics.FromImage(elevation_map);
            elevation_g.SmoothingMode = SmoothingMode.HighQuality;
            elevation_g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            elevation_g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            elevation_g.Clear(Color.White);


            Font f = new Font("微软雅黑", 12f);

            //取得最大的厚度.最小的厚度.这部分比较耗费时间
            //int x = dm.getWidth();
            //int y = dm.getHeight();
            Coordinate c = getOriginOfCoordinate(vertex);
            double c_x = c.getX();//当前x
            double c_y = c.getY();//当前y
            double max_thickness = double.MinValue;//厚度最大值
            double min_thickness = double.MaxValue;//厚度最小值
            

            int i = 0;//行号
            int m = 0;//列号

            double designdepth = segment.DesignDepth;

            //初始化
            double hengxiand = (left_bottom.Y - left_top.Y) / (grid * SCREEN_ONEMETER);
            int hengxian = (int)Math.Round(hengxiand);
            double shuxiand = (right_bottom.X - left_bottom.X) / (grid * SCREEN_ONEMETER);
            int shuxian = (int)Math.Round(shuxiand);

            double[,] thickness_sum_grid = new double[hengxian, shuxian];//存储大网格的厚度和
            int[,] thickness_count_grid = new int[hengxian, shuxian];//存储每个大网格包含的可用小网格数
            double[,] elevation_sum_grid = new double[hengxian, shuxian];//存储每个高程大网格的和
            int[,] elevation_count_grid = new int[hengxian, shuxian];//存储每个高程大网格的小网格数

            Color[,] gridcolor = new Color[hengxian, shuxian];//存储每个大网格的颜色
            //最关键的部分.填形状
            Color thickness_color = Color.Black;
            Color elevation_color = thickness_color;

            double max_value = segment.ElevationValues_maxDesignz;//255
            double min_value = segment.ElevationValues_minDesignz;//0
            int width = this_elevation_map.Width;
            int height = this_elevation_map.Height;


            double dam_width = dam_points[right_index].X - dam_points[left_index].X;
            double dam_height = dam_points[bottom_index].Y - dam_points[top_index].Y;

            double onepixel_width = dam_width / width;//1像素是多少米宽
            double onepixel_height = dam_height / height;//1像素是多少米高
            double biggrid_pixel_width = grid / onepixel_width;//大网格宽度映射到Image上的像素数
            double biggrid_pixel_height = grid / onepixel_height;//大网格高度映射到Image上的像素数


            double biggrid_pixel_width_old = width / shuxian;//大网格宽度映射到Image上的像素数
            double biggrid_pixel_height_old = height / hengxian;//大网格高度映射到Image上的像素数
            double onepixel_width_old = grid / biggrid_pixel_width;//每个像素数相当于的米数
            double onepixel_height_old = grid / biggrid_pixel_height;//每个像素相当于的高度*/


            double ss = biggrid_pixel_height_old + biggrid_pixel_width_old + onepixel_width_old + onepixel_height_old;
            /*double biggrid_pixel_width = width / shuxian;//大网格宽度映射到Image上的像素数
            double biggrid_pixel_height = height / hengxian;//大网格高度映射到Image上的像素数


            double onepixel_width = grid / biggrid_pixel_width;//每个像素数相当于的米数
            double onepixel_height = grid / biggrid_pixel_height;//每个像素相当于的高度*/

            double this_designz;//高程变量
            double sum_designz = 0;//高程总和
            int count_designz = 0;
            double this_difference;//厚度变量
            double sum_difference = 0;//厚度综合
            int count_difference = 0;

            byte[] rgbValues = segment.ElevationImageBytes;
           
            List<double> designz_s = new List<double>();
            List<double> difference_s = new List<double>();//存储厚度们

            //StringBuilder sb = new StringBuilder();
            for (int counter = 0; counter < rgbValues.Length; counter += 4)
            {
                int tr = rgbValues[counter];

                if (tr != 255)
                {
                    this_designz = min_value + (max_value - min_value) / 255 * tr;
                    designz_s.Add(this_designz);
                    sum_designz += this_designz;
                    count_designz++;

                    int thisx = counter / 4 % width;
                    int thisy = counter / 4 / width;

                    float this_dam_x = (float)(dam_origin.X + thisx * onepixel_width);
                    float this_dam_y = (float)(dam_height + dam_origin.Y - thisy * onepixel_height);
                    //大坝坐标
                    PointF thispoint = new PointF(this_dam_x, this_dam_y);

                    //sb.Append(""+thisx+" "+thisy+" "+this_dam_x+" "+this_dam_y+" \r\n");
                    //得到上一层对应的网格的高程
                    double getLastDesignz = getLastDesignzByImage(thispoint, segments);

                    if (getLastDesignz == -1)
                    {
                        difference_s.Add(-1);
                    }
                    else
                    {
                        this_difference = this_designz - getLastDesignz;

                        if (this_difference <= 0 || this_difference > 1.25 * designdepth)
                        {
                            difference_s.Add(-1);
                            continue;
                        }

                        if (max_thickness < this_difference)
                        {
                            max_thickness = this_difference;
                        }
                        if (min_thickness > this_difference)
                        {
                            min_thickness = this_difference;
                        }
                        difference_s.Add(this_difference);
                        sum_difference += (this_difference);
                        count_difference++;
                    }
                }
                else
                {
                    designz_s.Add(-1);
                    difference_s.Add(-1);
                }

                
            }
           // DebugUtil.fileLog(sb.ToString());
            //计算方差和均值
            double average_difference = sum_difference / count_difference;//平均厚度
            double deviation = 0f;//方差
            double temp = 0;
            for (int temp_index = 0; temp_index < difference_s.Count; temp_index++)
            {
                if(difference_s[temp_index]!=-1){
                    temp += Math.Pow((difference_s[temp_index] - average_difference), 2);
                }
            }
            deviation = temp / difference_s.Count;
            double standard_deviation = Math.Sqrt(deviation);//标准差


            double average_designz = sum_designz /count_designz;//平均高程
            deviation = 0f;//方差
            temp = 0;
            for (int temp_index = 0; temp_index < designz_s.Count; temp_index++)
            {
                if(designz_s[temp_index]!=-1){
                    temp += Math.Pow((designz_s[temp_index] - average_designz), 2);
                }
            }

            deviation = temp / designz_s.Count;
            double standard_deviation_designz = Math.Sqrt(deviation);//标准差

            //遍历Image
            for (int thisy = height - 1; thisy >= 0; thisy--)//从屏幕坐标的最大y开始画
            {
                //string linestr = "";
                for (int thisx = 0; thisx < width; thisx++)
                {
                    int index_ = thisy * width + thisx;
                    int zong = (int)Math.Round((thisx / biggrid_pixel_width));//所在的列
                    int heng = (int)Math.Round((thisy / biggrid_pixel_height));
                    if (heng >= hengxian)
                    {
                        continue;
                    }

                    if (zong >= shuxian)
                    {
                        continue;
                    }
                    
                    
                    this_designz = designz_s[index_];
                    if (this_designz != -1)
                    {
                        elevation_sum_grid[heng, zong] += this_designz;
                        elevation_count_grid[heng, zong] += 1;
                    }
                    this_difference = difference_s[index_];
                    if (this_difference!=-1)
                    {
                        thickness_sum_grid[heng, zong] += this_difference;
                        thickness_count_grid[heng, zong] += 1;
                    }
                }

            }

            //分析出大网格的均值,然后计算出颜色

//             double thickness_max_value = double.MinValue;//大网格最大值
//             double thickness_min_value = double.MaxValue;//大网格最小值
//            double thickness_sum_value = 0f;//大网格值总和
//            int thickness_grid_count = 0;
            //double thickness_average_value = 0f;//大网格平均值

//            double elevation_max_value = double.MinValue;//大网格最大值
//            double elevation_min_value = double.MaxValue;//大网格最小值
//            double elevation_sum_value = 0f;//大网格值总和
//            int elevation_grid_count = 0;
            //double elevation_average_value = 0f;//大网格平均值
// 
//             for (m = 0; m < hengxian; m++)
//             {
//                 for (int n = 0; n < shuxian; n++)
//                 {
//                     //int x_n = left_bottom.X + n * (SCREEN_ONEMETER * grid);
//                     //统计大网格厚度
//                     int count = thickness_count_grid[m, n];
//                     if (count != 0)
//                     {
//                         double average = thickness_sum_grid[m, n] / thickness_count_grid[m, n] * 100;
//                         if (thickness_max_value < average)
//                         {
//                             thickness_max_value = average;
//                         }
//                         if (thickness_min_value > average)
//                         {
//                             thickness_min_value = average;
//                         }
// //                        thickness_sum_value += average;
// //                        thickness_grid_count++;
//                     }
                    //统计大网格高程
                    /*                    count = elevation_count_grid[m, n];
                                        if (count != 0)
                                        {
                                            double average = elevation_sum_grid[m, n] / elevation_count_grid[m, n];
                                            if (elevation_max_value < average)
                                            {
                                                elevation_max_value = average;
                                            }
                                            if (elevation_min_value > average)
                                            {
                                                elevation_min_value = average;
                                            }

                    //                        elevation_sum_value += average;
                    //                        elevation_grid_count++;
                                        }
                     */
//                 }
//             }

//            if (thickness_grid_count == 0 || elevation_grid_count == 0)
//            {
              //  return null;
//            }

            //thickness_average_value = thickness_sum_value / thickness_grid_count;
            //elevation_average_value = elevation_sum_value / elevation_grid_count;

            System.Globalization.NumberFormatInfo centimeter = new System.Globalization.NumberFormatInfo();
            centimeter.NumberDecimalDigits = 3;

            System.Globalization.NumberFormatInfo meter = new System.Globalization.NumberFormatInfo();
            meter.NumberDecimalDigits = 1;

            //大网格上的数字们

            //for (m = 0; m < hengxian; m++)
            for (m = hengxian - 1; m >= 0; m--)
            {
                //int y_m = left_bottom.Y - (m + 1) * (SCREEN_ONEMETER * grid);
                double y_m = (left_bottom.Y - (hengxian - m) * SCREEN_ONEMETER * grid);
                for (int n = 0; n < shuxian; n++)
                {
                    double x_n =   left_bottom.X + n * (SCREEN_ONEMETER * grid);

                    int this_biggrid_thickness_count = thickness_count_grid[m, n];
                    double this_biggrid_thickness_average = 0;
                    if (this_biggrid_thickness_count != 0)
                    {
                        this_biggrid_thickness_average = thickness_sum_grid[m, n] / thickness_count_grid[m, n] * 100;

                        //限制在仓面里
                        // gridcolor[m, n] = color;
                        //if (y_m < left_top.Y || x_n + SCREEN_ONEMETER * grid > right_top.X)
                        //continue;
                    }

                    int this_biggrid_elevation_count = elevation_count_grid[m, n];
                    double this_biggrid_elevation_average = 0;
                    if (this_biggrid_elevation_count != 0)
                    {
                        this_biggrid_elevation_average = elevation_sum_grid[m, n] / elevation_count_grid[m, n];

                        //限制在仓面里
                        // gridcolor[m, n] = color;
                        //if (y_m < left_top.Y || x_n + SCREEN_ONEMETER * grid > right_top.X)
                        //continue;
                    }

                    if (this_biggrid_thickness_average != 0)
                    {
                        int alpha = 150;
                        double cl = (this_biggrid_thickness_average - min_thickness*100) / (max_thickness*100- min_thickness*100) * 255;
                        if (cl == double.NaN)
                        {
                            return null;
                        }
                        thickness_color = Color.FromArgb(alpha, (int)cl, (int)cl, (int)cl);
                    }
                    else
                    {
                        thickness_color = Color.Yellow;
                    }

                    if (this_biggrid_elevation_average != 0)
                    {
                        int alpha = 255;
                        double cl = (this_biggrid_elevation_average - min_value) / (max_value - min_value) * 255;
                        if (cl == double.NaN)
                        {
                            return null;
                        }
                        elevation_color = Color.FromArgb(alpha, (int)cl, (int)cl, (int)cl);
                    }
                    else
                    {
                        elevation_color = Color.Yellow;
                    }

                    int r_height = SCREEN_ONEMETER * grid;
                    int r_width = SCREEN_ONEMETER * grid;
                    double this_y_m = y_m;
                    double this_x_n = x_n;
                   
//                     if (y_m < left_top.Y)
//                     {
//                         r_height = r_height - left_top.Y + y_m;
//                         this_y_m = left_top.Y;
//                     }

                    /*if (x_n + SCREEN_ONEMETER * grid > right_top.X)
                    {
                        r_width = r_width - (x_n + SCREEN_ONEMETER * grid - right_top.X);
                    }*/

                    if (thickness_color != Color.Yellow)
                    {
                        thickness_g.FillRectangle(new SolidBrush(thickness_color), new Rectangle(new Point((int) Math.Round( this_x_n), (int) Math.Round( this_y_m)), new Size(r_width, r_height)));
                        //if (y_m < left_top.Y || x_n + SCREEN_ONEMETER * grid > right_top.X)
                        //continue;
                        thickness_g.DrawString(this_biggrid_thickness_average.ToString("N", meter) + "cm", f, new SolidBrush(Color.Black), new Point((int)Math.Round(this_x_n + 10), (int)Math.Round(this_y_m + 10)));
                    }

                    if (elevation_color != Color.Yellow && this_biggrid_elevation_average != 255)
                    {
                        elevation_g.FillRectangle(new SolidBrush(elevation_color), new Rectangle(new Point((int) Math.Round( this_x_n), (int) Math.Round( this_y_m)), new Size(r_width, r_height)));
                        //if (y_m < left_top.Y || x_n + SCREEN_ONEMETER * grid > right_top.X)
                        //continue;
                        elevation_g.DrawString(this_biggrid_elevation_average.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new Point((int) Math.Round(this_x_n + 10),(int) Math.Round( this_y_m + 10)));
                    }
                }
            }
            //纵轴

            //一个带箭头的pen
            Pen pen = new Pen(Color.Black, 2);
            //        pen.DashStyle = DashStyle.Dash;
            pen.StartCap = LineCap.Flat;
            pen.EndCap = LineCap.ArrowAnchor;

            int jiachang = 20;
            int putongjiachang = 10;
            thickness_g.DrawLine(pen, left_bottom, new PointF(left_top.X, left_top.Y - jiachang));
            elevation_g.DrawLine(pen, left_bottom, new PointF(left_top.X, left_top.Y - jiachang));

            //横轴
            thickness_g.DrawLine(pen, left_bottom, new PointF(right_bottom.X + jiachang, right_bottom.Y));
            elevation_g.DrawLine(pen, left_bottom, new PointF(right_bottom.X + jiachang, right_bottom.Y));

            HatchBrush hb = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);
            pen = new Pen(Color.Black, 1);
            pen.DashStyle = DashStyle.Dash;
            //原点
            PointF left_bottom_dam = screenToDam(dam_origin, left_bottom);//右下角点的大把坐标
            PointF yuandian = new PointF(left_bottom.X - 20, left_bottom.Y + 2);
            thickness_g.DrawString("(" + (int)Math.Round(left_bottom_dam.X) + "," + (int)Math.Round(left_bottom_dam.Y * -1) + ")", f, hb, yuandian);
            elevation_g.DrawString("(" + (int)Math.Round(left_bottom_dam.X) + "," + (int)Math.Round(left_bottom_dam.Y * -1) + ")", f, hb, yuandian);
            //横线们       
            for (i = 1; i <= hengxian; i++)
            {
                PointF start = new PointF(left_bottom.X, left_bottom.Y - i * grid * SCREEN_ONEMETER);
                PointF end = new PointF(right_bottom.X + putongjiachang, right_bottom.Y - i * grid * SCREEN_ONEMETER);
                //把横线限制在仓面之内
                if (start.Y < left_top.Y)
                    break;

                thickness_g.DrawLine(pen, start, end);
                elevation_g.DrawLine(pen, start, end);

                PointF stringPointF = new PointF(start.X - 50, start.Y);
                if (i % 2 == 1)
                {
                    thickness_g.DrawString("" + (int)Math.Round((left_bottom_dam.Y - grid * i) * (-1)), f, hb, stringPointF);
                    elevation_g.DrawString("" + (int)Math.Round((left_bottom_dam.Y - grid * i) * (-1)), f, hb, stringPointF);
                }
            }
            //纵线们       
            for (i = 1; i <= shuxian; i++)
            {
                PointF start = new PointF(left_bottom.X + i * grid * SCREEN_ONEMETER, left_bottom.Y);
                PointF end = new PointF(left_top.X + i * grid * SCREEN_ONEMETER, left_top.Y - putongjiachang);
                //把纵线限制在仓面之内
                if (end.Y > right_top.Y)
                    break;
                thickness_g.DrawLine(pen, start, end);
                elevation_g.DrawLine(pen, start, end);
                PointF stringPointF = new PointF(start.X, yuandian.Y);
                if (i % 2 == 1)
                {
                    thickness_g.DrawString("" + (int)Math.Round(left_bottom_dam.X + grid * i), f, hb, stringPointF);
                    elevation_g.DrawString("" + (int)Math.Round(left_bottom_dam.X + grid * i), f, hb, stringPointF);
                }
            }

            //标题
            Font titlefont = new Font("微软雅黑", 24f);
            PointF titlep = new PointF(10, 10);
            String thickness_title = "压实厚度图形报告";
            String elevation_title = "碾压高程图形报告";

            String sub_title = "单元   " +segment.SegmentName + "     仓面名称   " + segment.SegmentName + "      高程   " + segment.DesignZ + "m";
            StringFormat fmt = new StringFormat();
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Near;
            thickness_g.DrawString(thickness_title, titlefont, hb, new RectangleF(0, 10, thickness_map.Width, thickness_map.Height - 10), fmt);
            thickness_g.DrawLine(Pens.Black, new PointF(left_top.X, titlep.Y + titlefont.Height + 3), new PointF(right_top.X, titlep.Y + titlefont.Height + 3));
            thickness_g.DrawString(sub_title, f, hb, new RectangleF(0, 62, thickness_map.Width, thickness_map.Height - 62), fmt);

            elevation_g.DrawString(elevation_title, titlefont, hb, new RectangleF(0, 10, thickness_map.Width, thickness_map.Height - 10), fmt);
            elevation_g.DrawLine(Pens.Black, new PointF(left_top.X, titlep.Y + titlefont.Height + 3), new PointF(right_top.X, titlep.Y + titlefont.Height + 3));
            elevation_g.DrawString(sub_title, f, hb, new RectangleF(0, 62, thickness_map.Width, thickness_map.Height - 62), fmt);

            String startd = segment.StartDate.ToString();
            String endd = segment.EndDate.ToString();
            thickness_g.DrawString("开始时间:" + startd, f, hb, new PointF(right_top.X - 150, titlep.Y + titlefont.Height + 5 + f.Height));
            thickness_g.DrawString("结束时间:" + endd, f, hb, new PointF(right_top.X - 150, titlep.Y + titlefont.Height + 5 + f.Height * 2));

            elevation_g.DrawString("开始时间:" + startd, f, hb, new PointF(right_top.X - 150, titlep.Y + titlefont.Height + 5 + f.Height));
            elevation_g.DrawString("结束时间:" + endd, f, hb, new PointF(right_top.X - 150, titlep.Y + titlefont.Height + 5 + f.Height * 2));

            //坝
            PointF right_bottom_dam = screenToDam(dam_origin, right_bottom);
            PointF youxia = new PointF(yuandian.X + jiachang + 20 + right_bottom.X - left_bottom.X, right_bottom.Y - 10);
            thickness_g.DrawString("坝(m)", f, hb, youxia);
            elevation_g.DrawString("坝(m)", f, hb, youxia);
            //g.DrawString(right_bottom_dam.X.ToString(), f, hb, new PointF(yuandian.X+jiachang + 10 + right_bottom.X - left_bottom.X, yuandian.Y));
            //轴
            PointF left_top_dam = screenToDam(dam_origin, left_top);
            PointF zuoshang = new PointF(left_top.X - 5, left_top.Y - jiachang - 20);
            thickness_g.DrawString("轴(m)", f, hb, zuoshang);
            elevation_g.DrawString("轴(m)", f, hb, zuoshang);

            //g.DrawString("("+left_top_dam.X+","+left_top_dam.Y+")", f, hb, new PointF(yuandian.X-40 , left_top.Y-jiachang-10));
            //图例
            //三角指示
            int sanjiao = 25;
            int sanjiao_height = 15;
            PointF[] points = new PointF[3];
            points[0] = new PointF(left_bottom.X + sanjiao, left_bottom.Y + sanjiao + sanjiao_height);//左下
            points[1] = new PointF(right_bottom.X - sanjiao, right_bottom.Y + sanjiao);//右上
            points[2] = new PointF(right_bottom.X - sanjiao, right_bottom.Y + sanjiao + sanjiao_height);//右下
            //             g.FillPolygon(Brushes.Gray, points);
            //             g.DrawPolygon(Pens.Black, points);
            //矩形渐变            
            PointF juxingjianbian = new PointF(points[0].X, points[0].Y + 10);
            float juxingjianbian_width = points[2].X - points[0].X;
            float juxingjianbian_height = 15f;
            RectangleF r = new RectangleF(juxingjianbian, new SizeF(juxingjianbian_width, juxingjianbian_height));
            Color from = Color.FromArgb(180, 0, 0, 0);
            Color to = Color.FromArgb(180, 255, 255, 255);
            Brush b = new LinearGradientBrush(r, from, to, 0f);
            thickness_g.DrawRectangle(Pens.Black, Rectangle.Round(r));
            thickness_g.FillRectangle(b, r);
            elevation_g.DrawRectangle(Pens.Black, Rectangle.Round(r));
            elevation_g.FillRectangle(b, r);
            //矩形渐变
            /*juxingjianbian = new PointF(points[0].X, points[0].Y + 10 +juxingjianbian_height+4);          
            r = new RectangleF(juxingjianbian, new SizeF(juxingjianbian_width, juxingjianbian_height));
            from = Color.FromArgb(180, 255, 0, 0);
            to = Color.FromArgb(180, 255, 255, 255);
            b = new LinearGradientBrush(r, from, to, 0f);
            g.DrawRectangle(Pens.Black, Rectangle.Round(r));
            g.FillRectangle(b, r);*/
            //文字
            //最小值
            thickness_g.DrawString("最小值:" + min_thickness.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X - 25, juxingjianbian.Y + 25));
            elevation_g.DrawString("最小值:" + min_value.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X - 25, juxingjianbian.Y + 25));
            //最大值
            thickness_g.DrawString("最大值:" + max_thickness.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width - 25, juxingjianbian.Y + 25));
            elevation_g.DrawString("最大值:" + max_value.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width - 25, juxingjianbian.Y + 25));
            //中间值
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Center;
            thickness_g.DrawString("厚度均值:" + average_difference.ToString("N", centimeter) + "m    厚度标准差:" + standard_deviation.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new RectangleF(juxingjianbian.X, juxingjianbian.Y - 20, juxingjianbian_width, juxingjianbian_height), fmt);/*new PointF(juxingjianbian.X, juxingjianbian.Y + 25 + 25)*/
            elevation_g.DrawString("高程均值:" + average_designz.ToString("N", centimeter) + "m    高程标准差:" + standard_deviation_designz.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new RectangleF(juxingjianbian.X, juxingjianbian.Y - 20, juxingjianbian_width, juxingjianbian_height), fmt);/*new PointF(juxingjianbian.X, juxingjianbian.Y + 25 + 25)*/
            //更新数据库字段
            string elevations = average_difference.ToString("N", centimeter) + "," + standard_deviation.ToString("N", centimeter) + "," + average_designz.ToString("N", centimeter) + "," + standard_deviation_designz.ToString("N", centimeter);
            DAO.updateElevations(blockid, designz, segmentid, elevations);
            //g.DrawString((min+(max-min)/2).ToString() + "m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width/2, juxingjianbian.Y + 25));
            //间隔
            //float jiange_y = juxingjianbian.Y + 25;
            //g.DrawString("<----             " + ((max - min)).ToString() + "m" + "             ---->", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width / 2, jiange_y));
            //出图时间
            DateTime now = DateTime.Now;
            thickness_g.DrawString("出图时间:" + now, f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width - 100, juxingjianbian.Y + 25 + 25));
            elevation_g.DrawString("出图时间:" + now, f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width - 100, juxingjianbian.Y + 25 + 25));
            //垃圾回收尚未进行,需要看书了解.
            //大坝边界线
            thickness_g.DrawLines(Pens.Green, screen_points.ToArray());
            elevation_g.DrawLines(Pens.Green, screen_points.ToArray());

            Bitmap[] maps = { thickness_map, elevation_map };
            return maps;
            //存图
            /* try
             {
                // map.Save("c:/test.png", ImageFormat.Png);

                  Console.WriteLine("导出数据成功！");
             }
             catch (Exception e)
             {
                 Console.WriteLine("导出数据失败！");
                 //e.printStackTrace();
             }*/

        }

        public static Pixel getPixel(String x, String y, List<Segment> segments)
        {
            //确定所属仓面
            //System.out.println(blockid+" "+designz);		 
            for (int i = 0; i < segments.Count; i++)
            {
                Segment segment = segments[i];
                String vertex = segment.Vertext;
                //System.out.println(vertex+" "+x+" "+y);
                //byte[] bytes1 = getBytes(blockid,designz,segment.getSegmentID());
                //DataMap.test(bytes1);
                if (inThisSegment(vertex, x, y))
                {
                    //System.out.println("在此舱面内");
                    //根据原点坐标来计算出他对应数据图中的哪行哪列
                    float x_ = float.Parse(x);
                    float y_ = float.Parse(y);
                    Coordinate origin = getOriginOfCoordinate(vertex);
                    int m = (int)((x_ - origin.getX()) / WIDTH);
                    int n = (int)((origin.getY() - y_) / WIDTH);
                    if (m > 0 && n > 0)
                    {
                        byte[] bytes = segment.Datamap;
                        DataMap dmap = new DataMap(bytes);
                        if (m < dmap.getWidth() && n < dmap.getHeight())
                        {
                            Pixel p = dmap.getPixel(m, n);
                            p.setSegmentid(segment.SegmentID);
                            return p;
                        }

                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        //判断x,y是不是在一个多边形里
        private static bool inThisSegment(String vertex, String x, String y)
        {
            String[] points = vertex.Split(';');
            //int[] x_points = new int[points.Length]; 
            //int[] y_points = new int[points.Length];
            PointF[] point = new PointF[points.Length - 1];//因为最后有个分号
            double getX = 100 * double.Parse(x);
            double getY = 100 * Double.Parse(y);
            for (int i = 0; i < points.Length - 1; i++)
            {
                String[] xy = points[i].Split(',');
                float thisx = float.Parse(xy[0]);
                float thisy = float.Parse(xy[1]);
                int xint = (int)(thisx * 100);
                int yint = (int)(thisy * (-100));
                point[i] = new PointF(xint, yint);
            }
            PointF p = new PointF(((float.Parse(x)) * 100), ((float.Parse(y)) * (100)));
            return inThisSegment(p,point);
        }

        //判断x,y是不是在一个多边形里
        private static bool inThisSegment(PointF p, PointF[] vertex)
        {

            System.Drawing.Drawing2D.GraphicsPath myGraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
            myGraphicsPath.Reset();
            myGraphicsPath.AddPolygon(vertex);
            return myGraphicsPath.IsVisible(p);
        }


        //得到原点坐标,因为vertex中的x坐标为正数，y坐标为负数.所以取vertex中的最小x，和绝对值最小y
        static Coordinate getOriginOfCoordinate(String vertex)
        {
            String[] points = vertex.Split(';');
            float x = float.MaxValue;
            float y = float.MinValue;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].Equals(""))
                {
                    break;
                }
                String[] xy = points[i].Split(',');
                float thisx = float.Parse(xy[0]);
                float thisy = float.Parse(xy[1]) * (-1);
                if (x > thisx)
                {
                    x = thisx;
                }
                if (y < thisy)
                {
                    y = thisy;
                }
            }
            Coordinate c = new Coordinate(x, y);
            return c;
        }
        //传入大地坐标的vertex,和大地坐标的一个点.返回这个点在大地坐标中的行号列号点
        static PointF getRowColumn_Earth(String vertex, Coordinate earth_point)
        {
            Coordinate o = getOriginOfCoordinate(vertex);
            double x = (earth_point.getX() - o.getX()) / WIDTH;
            double y = (o.getY() - earth_point.getY()) / WIDTH;
            return new PointF((float)x, (float)y);
        }

        //得到这些点外切矩形的原点
        public static PointF getOrigin(PointF[] points)
        {
            float x = float.MaxValue;
            float y = float.MaxValue;
            for (int i = 0; i < points.Length; i++)
            {
                PointF p = points[i];
                float thisx = p.X;
                float thisy = p.Y;
                if (x > thisx)
                {
                    x = thisx;
                }
                if (y > thisy)
                {
                    y = thisy;
                }
            }
            return new PointF(x, y);
        }
        //大坝坐标转成屏幕坐标
        public static List<PointF> getRelatively(List<PointF> points)
        {
            List<PointF> newPointFs = new List<PointF>();
            PointF origin = getOrigin(points.ToArray());
            for (int i = 0; i < points.Count; i++)
            {
                newPointFs.Add(damToScreen(origin, points[i]));
            }
            return newPointFs;
        }
       
        
        //多层比较
        public static List<PointF> getRelatively(PointF origin,List<PointF> points)
        {
            List<PointF> newPointFs = new List<PointF>();
            for (int i = 0; i < points.Count; i++)
            {
                newPointFs.Add(DDDdamToScreen(origin, points[i]));
            }
            return newPointFs;
        }


        //大坝坐标转屏幕坐标   origin为大坝坐标外切矩形原点
        static PointF DDDdamToScreen(PointF origin, PointF dam_point)
        {
            float thisx = (dam_point.X - origin.X);
            float thisy = (dam_point.Y - origin.Y);
            return new PointF(thisx, thisy);
        }



        //大坝坐标转屏幕坐标   origin为大坝坐标外切矩形原点
        static PointF damToScreen(PointF origin, PointF dam_point)
        {
            float thisx = (dam_point.X - origin.X) * SCREEN_ONEMETER + map_left;
            float thisy = (dam_point.Y - origin.Y) * SCREEN_ONEMETER + map_top;
            return new PointF(thisx, thisy);
        }

        static PointF screenToDam(PointF origin, PointF screen_point)
        {
            float thisx = (screen_point.X - map_left) / SCREEN_ONEMETER + origin.X;
            float thisy = (screen_point.Y - map_top) / SCREEN_ONEMETER + origin.Y;
            return new PointF(thisx, thisy);
        }

        //通过大地坐标系的vertex字符串得到大坝坐标下的边界点值
       public static List<PointF> getSegmentVertex_DAM(String vertex)
        {
            List<PointF> cs = new List<PointF>();
            Coordinate o = getOriginOfCoordinate(vertex);//得到原点坐标
            String[] points = vertex.Split(';');
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].Equals(""))
                {
                    break;
                }
                String[] xy = points[i].Split(',');
                double thisx = float.Parse(xy[0]);
                double thisy = float.Parse(xy[1]) * (-1);
                Coordinate c = earthToDam(new Coordinate(thisx, thisy));

                cs.Add(new PointF((float)c.getX(), -(float)c.getY()));
            }
            return cs;
        }
        //将大地坐标转换成大坝坐标
        static Coordinate earthToDam(Coordinate p)
        {
            double x0 = -cos * p.getX() - sin * p.getY() + 46557.7811830799932563179112397188;
            double y0 = +sin * p.getX() - cos * p.getY() - 20616.2311146461071871455578251375;
            return new Coordinate(x0, y0);

        }
        //将大把坐标转换成大地坐标
        static Coordinate damToEarth(PointF p)
        {
            double x = -cos * p.X + sin * (-p.Y) + 50212.59;
            double y = -sin * p.X - cos * (-p.Y) + 8447;
            return new Coordinate(x, y);
        }
        //得到这一堆点最左的点在这个数组中的下标,最小的X
        public static int getLeftIndex(List<PointF> cs)
        {
            int i = -1;
            double minx = double.MaxValue;
            foreach (PointF c in cs)
            {
                if (minx > c.X)
                {
                    minx = c.X;
                }
            }
            i = 0;
            foreach (PointF c in cs)
            {
                if (minx == c.X)
                {
                    break;
                }
                i++;
            }
            return i;
        }
        //最大的X
        public static int getRightIndex(List<PointF> cs)
        {
            int i = -1;
            double maxx = double.MinValue;
            foreach (PointF c in cs)
            {
                if (maxx < c.X)
                {
                    maxx = c.X;
                }
            }
            i = 0;
            foreach (PointF c in cs)
            {
                if (maxx == c.X)
                {
                    break;
                }
                i++;
            }
            return i;
        }
        //最大的Y
        public static int getBottomIndex(List<PointF> cs)
        {
            int i = -1;
            double maxy = double.MinValue;
            foreach (PointF c in cs)
            {
                if (maxy < c.Y)
                {
                    maxy = c.Y;
                }
            }
            i = 0;
            foreach (PointF c in cs)
            {
                if (maxy == c.Y)
                {
                    break;
                }
                i++;
            }
            return i;
        }
        //最小的Y
        public static int getTopIndex(List<PointF> cs)
        {
            int i = -1;
            double miny = double.MaxValue;
            foreach (PointF c in cs)
            {
                if (miny > c.Y)
                {
                    miny = c.Y;
                }
            }
            i = 0;
            foreach (PointF c in cs)
            {
                if (miny == c.Y)
                {
                    break;
                }
                i++;
            }
            return i;
        }

        //得到正态分布随机数
        static double getRandomByNormalDistribution(double ep, double ds)
        {
            double z1 = (new Random()).NextDouble();
            double z2 = (new Random()).NextDouble();
            double z3 = Math.Sqrt(-2 * Math.Log(z1)) * Math.Cos(6.283 * z2);
            double z = ep + z3 * ds;
            return z;
        }



        public static double getLastDesignzByImage(PointF point, List<Segment> segments)
        {
            
            //遍历segment
            for (int i = 0; i < segments.Count; i++)
            {
                Segment segment = segments[i];
                String vertex = segment.Vertext;
                List<PointF> dam_points = getSegmentVertex_DAM(vertex);
                //判断大坝坐标的point在不在范围内
                if (inThisSegment(point,dam_points.ToArray()))
                {
                    

                    Bitmap eimage = segment.ElevationImage;
                    if (eimage==null)
                    {
                        continue;
                    }
                    double minvalue = segment.ElevationValues_minDesignz;
                    double maxvalue = segment.ElevationValues_maxDesignz;

                    PointF dam_origin = getOrigin(dam_points.ToArray());
                    int left_index = getLeftIndex(dam_points);
                    int right_index = getRightIndex(dam_points);
                    int top_index = getTopIndex(dam_points);
                    int bottom_index = getBottomIndex(dam_points);
                    //外切矩形四个顶点
                    PointF left_top = new PointF(dam_points[left_index].X, dam_points[top_index].Y);
                    PointF left_bottom = new PointF(dam_points[left_index].X, dam_points[bottom_index].Y);
                    PointF right_top = new PointF(dam_points[right_index].X, dam_points[top_index].Y);
                    PointF right_bottom = new PointF(dam_points[right_index].X, dam_points[bottom_index].Y);

                    float dam_width = right_bottom.X - left_bottom.X;
                    float dam_height = right_bottom.Y - right_top.Y;

                    int img_width = eimage.Width;
                    int img_height = eimage.Height;
                    
                    float one_pixel_meter_width = dam_width/img_width;//1像素是多少米宽
                    float one_pixel_meter_height = dam_height / img_height;//1像素是多少米高

                    float one_pixel_meter = (one_pixel_meter_height + one_pixel_meter_width) / 2;
                    

                    int pixel_x = (int)Math.Round(/*img_width -*/ (point.X - dam_origin.X) / one_pixel_meter);
                    int pixel_y = (int)Math.Round((dam_origin.Y + dam_height - point.Y) / one_pixel_meter);

                    if (pixel_x>=img_width||pixel_y>=img_height||pixel_x<0||pixel_y<0)
                    {
                        continue;
                    }

                    //pixel_x = img_width - pixel_x;
                    //pixel_y = img_height - pixel_y;

                    byte[] rgbValues = segment.ElevationImageBytes;
                    int r =rgbValues[(pixel_y * img_width + pixel_x)*4+1];

                    //DebugUtil.fileLog("" + pixel_x + " " + pixel_y + " " + (minvalue + (maxvalue - minvalue) / 255 * r));
                    return minvalue + (maxvalue - minvalue) / 255 * r;
                }
            }
            return -1;
        }
    }
}
