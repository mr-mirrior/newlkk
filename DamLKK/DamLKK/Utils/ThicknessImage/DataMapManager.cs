using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using DamLKK.Utils;
namespace DamLKK.DB.datamap
{
    class DataMapManager
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

                byte[] datamap = DAO.getInstance().getDatamap(blockid, lastDesignz, segments[ii].SegmentID);
                if (datamap == null)
                {
                    DebugUtil.fileLog("没有数据图" + blockid + " " + lastDesignz + " " + segments[ii].SegmentID);
                    return null;
                }
                segments[ii].Datamap = datamap;
            }
            
            byte[] bytes = DAO.getInstance().getDatamap(blockid, designz, segmentid);//本仓面的数据图
            if (bytes == null)
            {
                DebugUtil.fileLog("没有数据图" + blockid + " " + designz + " " + segmentid);
                return null;
            }
            DataMap dm = new DataMap(bytes);

            //通过vertex得到大坝坐标系的边界点
            List<Point> dam_points = getSegmentVertex_DAM(vertex);
            //大坝坐标系下的外切矩阵的原点,把屏幕坐标转换成大坝坐标会用到
            Point dam_origin = getOrigin(dam_points.ToArray());
            //转换成相对这组坐标的原点的“新点”也相当于屏幕坐标
            List<Point> screen_points = getRelatively(dam_points);
            //取得大坝坐标系边界点的外切矩阵
            int left_index = getLeftIndex(screen_points);
            int right_index = getRightIndex(screen_points);
            int top_index = getTopIndex(screen_points);
            int bottom_index = getBottomIndex(screen_points);
            //外切矩形四个顶点
            Point left_top = new Point(screen_points[left_index].X, screen_points[top_index].Y);
            Point left_bottom = new Point(screen_points[left_index].X, screen_points[bottom_index].Y);
            Point right_top = new Point(screen_points[right_index].X, screen_points[top_index].Y);
            Point right_bottom = new Point(screen_points[right_index].X, screen_points[bottom_index].Y);
            //大坝坐标下本仓面外切矩形的宽度
            int segment_dam_width = screen_points[right_index].X - screen_points[left_index].X;
            //大坝坐标下本仓面外切矩形的高度
            int segment_dam_height = screen_points[bottom_index].Y - screen_points[top_index].Y;
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
            int x = dm.getWidth();
            int y = dm.getHeight();
            Coordinate c = getOriginOfCoordinate(vertex);
            double c_x = c.getX();//当前x
            double c_y = c.getY();//当前y
            float max_thickness = float.MinValue;//厚度最大值
            float min_thickness = float.MaxValue;//厚度最小值
            float max_designz = float.MinValue;//高程最大值
            float min_designz = float.MaxValue;//高程最小值
            double sum_designz = 0;//高程总和
            float this_designz;//高程变量
            Pixel lastp;
            float difference = 0f;//厚度变量
            double sum = 0;//厚度综合
            Pixel p = null;
            int i = 0;//行号
            int m = 0;//列号
            List<float> difference_s = new List<float>();//存储厚度们
            List<float> designz_s = new List<float>();//存储高程们

            double designdepth = segment.DesignDepth;

            for (i = 0; i < y; i++)
            {
                //  string linestr = "";
                for (m = 0; m < x; m++)
                {

                    p = dm.getPixel(m, i);
                    this_designz = p.getRollthickness();


                    /* if (inThisSegment(vertex, "" + c_x, "" + c_y))
                     {

                         if (p.getRollcount() != 255)
                         {
                             if (p.getRollcount() >= 10)
                             {
                                 linestr += p.getRollcount();
                             }
                             else
                             {
                                 linestr += "0" + p.getRollcount();
                             }
                         }
                         else
                         {
                             linestr += "**";
                         }

                     }
                     else
                     {
                         if (p.getRollcount() != 255)
                         {
                             if (p.getRollcount() >= 10)
                             {
                                 linestr += p.getRollcount();
                             }
                             else
                             {
                                 linestr += "0" + p.getRollcount();
                             }
                         }
                         else
                         {
                             linestr += "&&";
                         }
                     }*/



                    if (p.getRollcount() != 255 && this_designz != 0)//是否为仓面上的点
                    {


                        //找本网格中心点对应的下一层 + (c_x + SCREEN_ONEMETER * WIDTH / 2)
                        lastp = DataMapManager4.getPixel("" + (c_x + WIDTH / 2), "" + (c_y - WIDTH / 2), segments);//得到上一点的数据
                        if (lastp != null && lastp.getRollthickness() != 0 && lastp.getRollcount() != 255)
                        {//
                            difference = this_designz - lastp.getRollthickness();

                            if (difference > 0 && difference < 1.25 * designdepth)
                            {
                                if (max_thickness < difference) { max_thickness = difference; };
                                if (min_thickness > difference) { min_thickness = difference; };
                                sum += difference;
                                difference_s.Add(difference);
                            }
                        }
                        //为了统计高程的均值和标准差
                        //实际高程的范围在设计高程减去1倍的设计厚度和设计高程加上2倍的设计厚度之间
                        if (this_designz > segment.DesignZ - 3 * designdepth && this_designz < segment.DesignZ + 3 * designdepth)
                        {
                            if (max_designz < this_designz) { max_designz = this_designz; };
                            if (min_designz > this_designz) { min_designz = this_designz; };

                            sum_designz += this_designz;
                            designz_s.Add(this_designz);
                            //double this_average = sum_designz / designz_s.Count;
                            //DebugUtil.fileLog("" + designz_s.Count + "\t" + this_designz + "\t" + sum_designz + "\t" + this_average);
                        }
                        //                         if (p.getRollcount() < 10)
                        //                         {
                        //                             linestr += "0" + p.getRollcount();
                        //                         }
                        //                         else
                        //                         {
                        //                             linestr += p.getRollcount();
                        //                         }
                        //  linestr += p.getRollthickness();
                    }
                    else
                    {
                        //  linestr += "##";
                    }

                    c_x += WIDTH;//x值加
                }
                c_y -= WIDTH;//y
                c_x = c.getX();
                //  DebugUtil.fileLog(linestr);
            }
            //计算方差和均值
            double average_difference = sum / difference_s.Count;//平均厚度
            double deviation = 0f;//方差
            double temp = 0;
            for (int temp_index = 0; temp_index < difference_s.Count; temp_index++)
            {
                temp += Math.Pow((difference_s[temp_index] - average_difference), 2);
            }
            deviation = temp / difference_s.Count;
            double standard_deviation = Math.Sqrt(deviation);

            double average_designz = sum_designz / designz_s.Count;//平均厚度


            deviation = 0f;//方差
            temp = 0;
            for (int temp_index = 0; temp_index < designz_s.Count; temp_index++)
            {

                temp += Math.Pow((designz_s[temp_index] - average_designz), 2);
            }


            deviation = temp / designz_s.Count;
            double standard_deviation_designz = Math.Sqrt(deviation);//标准差

            //初始化
            int hengxian = (left_bottom.Y - left_top.Y) / (grid * SCREEN_ONEMETER) + 1;
            int shuxian = (right_bottom.X - left_bottom.X) / (grid * SCREEN_ONEMETER) + 1;
            double[,] thickness_sum_grid = new double[hengxian, shuxian];//存储大网格的厚度和
            int[,] thickness_count_grid = new int[hengxian, shuxian];//存储每个大网格包含的可用小网格数
            float[,] elevation_sum_grid = new float[hengxian, shuxian];//存储每个高程大网格的和
            int[,] elevation_count_grid = new int[hengxian, shuxian];//存储每个高程大网格的小网格数

            Color[,] gridcolor = new Color[hengxian, shuxian];//存储每个大网格的颜色
            //最关键的部分.填形状
            Color thickness_color = Color.Black;
            Color elevation_color = thickness_color;


            for (int thisy = left_bottom.Y - (SCREEN_ONEMETER / 2); thisy >= left_top.Y - (SCREEN_ONEMETER / 2); thisy -= (SCREEN_ONEMETER / 2))//从屏幕坐标的最大y开始画
            {
                //string linestr = "";
                for (int thisx = left_top.X; thisx <= right_top.X; thisx += (SCREEN_ONEMETER / 2))
                {

                    //第一种实现方式,先判断该屏幕坐标点在不在屏幕坐标系下的仓面内.如果在,转换成大地坐标来找.

                    Point screen_point = new Point(thisx, thisy);
                    if (inThisSegment(screen_point, screen_points.ToArray()))
                    {
                        //屏幕转大坝
                        Point dam_point = screenToDam(dam_origin, screen_point);
                        //大坝转大地
                        Coordinate earth_point = damToEarth(dam_point);
                        //取得大地坐标在数据图中的行列
                        Point row_column = getRowColumn_Earth(vertex, earth_point);
                        //取得该行列上的数据
                        if (row_column.X < 0 || row_column.Y < 0)
                        {
                            continue;
                        }
                        Pixel pixel = dm.getPixel(row_column.X, row_column.Y);
                        if (pixel != null && pixel.getRollcount() != 255)//是否为仓面上的点
                        {
                            /*if (pixel.getRollcount() >= 10)
                            {
                                linestr += pixel.getRollcount();
                            }
                            else
                            {
                                linestr += "0" + pixel.getRollcount();
                            }*/

                            int zong = (screen_point.X - left_bottom.X) / (grid * SCREEN_ONEMETER);//所在的列
                            int heng = (right_bottom.Y - SCREEN_ONEMETER / 2 - screen_point.Y) / (grid * SCREEN_ONEMETER);

                            if (pixel.getRollthickness() != 0)
                            {
                                //找本网格中心点对应的下一层
                                Pixel lastpixel = getPixel("" + (earth_point.getX() + WIDTH / 2), "" + (earth_point.getY() + WIDTH / 2), segments);
                                if (lastpixel != null && lastpixel.getRollthickness() != 0 && lastpixel.getRollcount() != 255)
                                {//
                                    difference = pixel.getRollthickness() - lastpixel.getRollthickness();
                                }
                                else
                                {
                                    difference = -1;
                                }
                            }
                            else
                            {
                                difference = -1;
                            }
                            /*else//按照正态分布随机产生厚度
                            {
                                difference = (float)getRandomByNormalDistribution(average_difference, standard_deviation);
                            }*/

                            if (pixel.getRollthickness() > segment.DesignZ - designdepth && pixel.getRollthickness() < segment.DesignZ + 2 * designdepth)
                            {
                                elevation_sum_grid[heng, zong] += pixel.getRollthickness();
                                elevation_count_grid[heng, zong] += 1;
                            }

                            if (difference > 0 && difference < 1.25 * designdepth)
                            {
                                thickness_sum_grid[heng, zong] += difference;
                                thickness_count_grid[heng, zong] += 1;
                            }

                        }
                        /*else
                        {
                            linestr += "**";

                        }*/
                    }
                    /*else
                    {
                        linestr += "&&";
                    }*/
                }
                //DebugUtil.fileLog(linestr);
            }

            //分析出大网格的均值,然后计算出颜色

            System.Globalization.NumberFormatInfo centimeter = new System.Globalization.NumberFormatInfo();
            centimeter.NumberDecimalDigits = 2;

            System.Globalization.NumberFormatInfo meter = new System.Globalization.NumberFormatInfo();
            centimeter.NumberDecimalDigits = 2;



            double thickness_max_value = double.MinValue;//大网格最大值
            double thickness_min_value = double.MaxValue;//大网格最小值
            double thickness_sum_value = 0f;//大网格值总和
            int thickness_grid_count = 0;
            double thickness_average_value = 0f;//大网格平均值

            double elevation_max_value = double.MinValue;//大网格最大值
            double elevation_min_value = double.MaxValue;//大网格最小值
            double elevation_sum_value = 0f;//大网格值总和
            int elevation_grid_count = 0;
            double elevation_average_value = 0f;//大网格平均值

            for (m = 0; m < hengxian; m++)
            {
                for (int n = 0; n < shuxian; n++)
                {
                    //int x_n = left_bottom.X + n * (SCREEN_ONEMETER * grid);
                    //统计大网格厚度
                    int count = thickness_count_grid[m, n];
                    if (count != 0)
                    {
                        double average = thickness_sum_grid[m, n] / thickness_count_grid[m, n] * 100;
                        if (thickness_max_value < average)
                        {
                            thickness_max_value = average;
                        }
                        if (thickness_min_value > average)
                        {
                            thickness_min_value = average;
                        }
                        thickness_sum_value += average;
                        thickness_grid_count++;
                    }
                    //统计大网格高程
                    count = elevation_count_grid[m, n];
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
                        elevation_sum_value += average;
                        elevation_grid_count++;
                    }

                }
            }

            if (thickness_grid_count == 0 || elevation_grid_count == 0)
            {
                return null;
            }

            thickness_average_value = thickness_sum_value / thickness_grid_count;
            elevation_average_value = elevation_sum_value / elevation_grid_count;

            //大网格上的数字们

            for (m = 0; m < hengxian; m++)
            {
                int y_m = left_bottom.Y - (m + 1) * (SCREEN_ONEMETER * grid);
                for (int n = 0; n < shuxian; n++)
                {
                    int x_n = left_bottom.X + n * (SCREEN_ONEMETER * grid);

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
                        double cl = (this_biggrid_thickness_average - thickness_min_value) / (thickness_max_value - thickness_min_value) * 255;
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
                        int alpha = 150;
                        double cl = (this_biggrid_elevation_average - elevation_min_value) / (elevation_max_value - elevation_min_value) * 255;
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
                    int this_y_m = y_m;
                    int this_x_n = x_n;
                    if (y_m < left_top.Y)
                    {
                        r_height = r_height - left_top.Y + y_m;
                        this_y_m = left_top.Y;
                    }
                    if (x_n + SCREEN_ONEMETER * grid > right_top.X)
                    {
                        r_width = r_width - (x_n + SCREEN_ONEMETER * grid - right_top.X);
                    }

                    if (thickness_color != Color.Yellow)
                    {
                        thickness_g.FillRectangle(new SolidBrush(thickness_color), new Rectangle(new Point(this_x_n, this_y_m), new Size(r_width, r_height)));
                        //if (y_m < left_top.Y || x_n + SCREEN_ONEMETER * grid > right_top.X)
                        //continue;
                        thickness_g.DrawString((int)this_biggrid_thickness_average + "cm", f, new SolidBrush(Color.Black), new Point(this_x_n + 10, this_y_m + 10));
                    }

                    if (elevation_color != Color.Yellow)
                    {
                        elevation_g.FillRectangle(new SolidBrush(elevation_color), new Rectangle(new Point(this_x_n, this_y_m), new Size(r_width, r_height)));
                        //if (y_m < left_top.Y || x_n + SCREEN_ONEMETER * grid > right_top.X)
                        //continue;
                        elevation_g.DrawString(this_biggrid_elevation_average.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new Point(this_x_n + 10, this_y_m + 10));
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
            thickness_g.DrawLine(pen, left_bottom, new Point(left_top.X, left_top.Y - jiachang));
            elevation_g.DrawLine(pen, left_bottom, new Point(left_top.X, left_top.Y - jiachang));

            //横轴
            thickness_g.DrawLine(pen, left_bottom, new Point(right_bottom.X + jiachang, right_bottom.Y));
            elevation_g.DrawLine(pen, left_bottom, new Point(right_bottom.X + jiachang, right_bottom.Y));

            HatchBrush hb = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);
            pen = new Pen(Color.Black, 1);
            pen.DashStyle = DashStyle.Dash;
            //原点
            Point left_bottom_dam = screenToDam(dam_origin, left_bottom);//右下角点的大把坐标
            Point yuandian = new Point(left_bottom.X - 20, left_bottom.Y + 2);
            thickness_g.DrawString("(" + left_bottom_dam.X + "," + (left_bottom_dam.Y * -1) + ")", f, hb, yuandian);
            elevation_g.DrawString("(" + left_bottom_dam.X + "," + (left_bottom_dam.Y * -1) + ")", f, hb, yuandian);
            //横线们       
            for (i = 1; i <= hengxian; i++)
            {
                Point start = new Point(left_bottom.X, left_bottom.Y - i * grid * SCREEN_ONEMETER);
                Point end = new Point(right_bottom.X + putongjiachang, right_bottom.Y - i * grid * SCREEN_ONEMETER);
                //把横线限制在仓面之内
                if (start.Y < left_top.Y)
                    break;

                thickness_g.DrawLine(pen, start, end);
                elevation_g.DrawLine(pen, start, end);

                Point stringPoint = new Point(start.X - 50, start.Y);
                if (i % 2 == 1)
                {
                    thickness_g.DrawString("" + ((left_bottom_dam.Y - grid * i) * (-1)), f, hb, stringPoint);
                    elevation_g.DrawString("" + ((left_bottom_dam.Y - grid * i) * (-1)), f, hb, stringPoint);
                }
            }
            //纵线们       
            for (i = 1; i <= shuxian; i++)
            {
                Point start = new Point(left_bottom.X + i * grid * SCREEN_ONEMETER, left_bottom.Y);
                Point end = new Point(left_top.X + i * grid * SCREEN_ONEMETER, left_top.Y - putongjiachang);
                //把纵线限制在仓面之内
                if (end.Y > right_top.Y)
                    break;
                thickness_g.DrawLine(pen, start, end);
                elevation_g.DrawLine(pen, start, end);
                Point stringPoint = new Point(start.X, yuandian.Y);
                if (i % 2 == 1)
                {
                    thickness_g.DrawString("" + (left_bottom_dam.X + grid * i), f, hb, stringPoint);
                    elevation_g.DrawString("" + (left_bottom_dam.X + grid * i), f, hb, stringPoint);
                }
            }

            //标题
            Font titlefont = new Font("微软雅黑", 24f);
            Point titlep = new Point(10, 10);
            String thickness_title = "压实厚度图形报告";
            String elevation_title = "碾压高程图形报告";

            String sub_title = "单元   " + segment.SegmentName + "     仓面名称   " + segment.SegmentName + "      高程   " + segment.DesignZ + "m";
            StringFormat fmt = new StringFormat();
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Near;
            thickness_g.DrawString(thickness_title, titlefont, hb, new RectangleF(0, 10, thickness_map.Width, thickness_map.Height - 10), fmt);
            thickness_g.DrawLine(Pens.Black, new Point(left_top.X, titlep.Y + titlefont.Height + 3), new Point(right_top.X, titlep.Y + titlefont.Height + 3));
            thickness_g.DrawString(sub_title, f, hb, new RectangleF(0, 62, thickness_map.Width, thickness_map.Height - 62), fmt);

            elevation_g.DrawString(elevation_title, titlefont, hb, new RectangleF(0, 10, thickness_map.Width, thickness_map.Height - 10), fmt);
            elevation_g.DrawLine(Pens.Black, new Point(left_top.X, titlep.Y + titlefont.Height + 3), new Point(right_top.X, titlep.Y + titlefont.Height + 3));
            elevation_g.DrawString(sub_title, f, hb, new RectangleF(0, 62, thickness_map.Width, thickness_map.Height - 62), fmt);

            String startd = segment.StartDate.ToString();
            String endd = segment.EndDate.ToString();
            thickness_g.DrawString("开始时间:" + startd, f, hb, new Point(right_top.X - 150, titlep.Y + titlefont.Height + 5 + f.Height));
            thickness_g.DrawString("结束时间:" + endd, f, hb, new Point(right_top.X - 150, titlep.Y + titlefont.Height + 5 + f.Height * 2));

            elevation_g.DrawString("开始时间:" + startd, f, hb, new Point(right_top.X - 150, titlep.Y + titlefont.Height + 5 + f.Height));
            elevation_g.DrawString("结束时间:" + endd, f, hb, new Point(right_top.X - 150, titlep.Y + titlefont.Height + 5 + f.Height * 2));

            //坝
            Point right_bottom_dam = screenToDam(dam_origin, right_bottom);
            Point youxia = new Point(yuandian.X + jiachang + 20 + right_bottom.X - left_bottom.X, right_bottom.Y - 10);
            thickness_g.DrawString("坝(m)", f, hb, youxia);
            elevation_g.DrawString("坝(m)", f, hb, youxia);
            //g.DrawString(right_bottom_dam.X.ToString(), f, hb, new Point(yuandian.X+jiachang + 10 + right_bottom.X - left_bottom.X, yuandian.Y));
            //轴
            Point left_top_dam = screenToDam(dam_origin, left_top);
            Point zuoshang = new Point(left_top.X - 5, left_top.Y - jiachang - 20);
            thickness_g.DrawString("轴(m)", f, hb, zuoshang);
            elevation_g.DrawString("轴(m)", f, hb, zuoshang);

            //g.DrawString("("+left_top_dam.X+","+left_top_dam.Y+")", f, hb, new Point(yuandian.X-40 , left_top.Y-jiachang-10));
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
            elevation_g.DrawString("最小值:" + min_designz.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X - 25, juxingjianbian.Y + 25));
            //最大值
            thickness_g.DrawString("最大值:" + max_thickness.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width - 25, juxingjianbian.Y + 25));
            elevation_g.DrawString("最大值:" + max_designz.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width - 25, juxingjianbian.Y + 25));
            //中间值
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Center;
            thickness_g.DrawString("厚度均值:" + average_difference.ToString("N", centimeter) + "m    厚度标准差:" + standard_deviation.ToString("N", centimeter) + "m     高程均值:" + average_designz.ToString("N", centimeter) + "m    高程标准差:" + standard_deviation_designz.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new RectangleF(juxingjianbian.X, juxingjianbian.Y - 20, juxingjianbian_width, juxingjianbian_height), fmt);/*new PointF(juxingjianbian.X, juxingjianbian.Y + 25 + 25)*/
            elevation_g.DrawString("厚度均值:" + average_difference.ToString("N", centimeter) + "m    厚度标准差:" + standard_deviation.ToString("N", centimeter) + "m     高程均值:" + average_designz.ToString("N", centimeter) + "m    高程标准差:" + standard_deviation_designz.ToString("N", centimeter) + "m", f, new SolidBrush(Color.Black), new RectangleF(juxingjianbian.X, juxingjianbian.Y - 20, juxingjianbian_width, juxingjianbian_height), fmt);/*new PointF(juxingjianbian.X, juxingjianbian.Y + 25 + 25)*/
            //更新数据库字段
            string elevations = average_difference.ToString("N", centimeter) + "," + standard_deviation.ToString("N", centimeter) + "," + average_designz.ToString("N", centimeter) + "," + standard_deviation_designz.ToString("N", centimeter);
            DAO.updateElevations(blockid, designz, segmentid, elevations);
            //g.DrawString((min+(max-min)/2).ToString() + "m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width/2, juxingjianbian.Y + 25));
            //间隔
            //float jiange_y = juxingjianbian.Y + 25;
            //g.DrawString("<----             " + ((max - min)).ToString() + "m" + "             ---->", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width / 2, jiange_y));
            //出图时间
            DateTime now = DateTime.Now;
            thickness_g.DrawString("出图时间:" + now, f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width - 80, juxingjianbian.Y + 25 + 25));
            elevation_g.DrawString("出图时间:" + now, f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width - 80, juxingjianbian.Y + 25 + 25));
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
            Point[] point = new Point[points.Length - 1];//因为最后有个分号
            double getX = 100 * double.Parse(x);
            double getY = 100 * Double.Parse(y);
            for (int i = 0; i < points.Length - 1; i++)
            {
                String[] xy = points[i].Split(',');
                float thisx = float.Parse(xy[0]);
                float thisy = float.Parse(xy[1]);
                int xint = (int)(thisx * 100);
                int yint = (int)(thisy * (-100));
                point[i] = new Point(xint, yint);
            }

            System.Drawing.Drawing2D.GraphicsPath myGraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
            myGraphicsPath.Reset();
            myGraphicsPath.AddPolygon(point);
            Point p = new Point((int)((float.Parse(x)) * 100), (int)((float.Parse(y)) * (100)));
            return myGraphicsPath.IsVisible(p);
        }

        //判断x,y是不是在一个多边形里
        private static bool inThisSegment(Point p, Point[] vertex)
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
        static Point getRowColumn_Earth(String vertex, Coordinate earth_point)
        {
            Coordinate o = getOriginOfCoordinate(vertex);
            double x = (earth_point.getX() - o.getX()) / WIDTH;
            double y = (o.getY() - earth_point.getY()) / WIDTH;
            return new Point((int)x, (int)y);
        }

        //得到这些点外切矩形的原点
        public static Point getOrigin(Point[] points)
        {
            float x = float.MaxValue;
            float y = float.MaxValue;
            for (int i = 0; i < points.Length; i++)
            {
                Point p = points[i];
                int thisx = p.X;
                int thisy = p.Y;
                if (x > thisx)
                {
                    x = thisx;
                }
                if (y > thisy)
                {
                    y = thisy;
                }
            }
            return new Point((int)x, (int)y);
        }
        //大坝坐标转成屏幕坐标
        public static List<Point> getRelatively(List<Point> points)
        {
            List<Point> newPoints = new List<Point>();
            Point origin = getOrigin(points.ToArray());
            for (int i = 0; i < points.Count; i++)
            {
                newPoints.Add(damToScreen(origin, points[i]));
            }
            return newPoints;
        }
       
        
        //多层比较
        public static List<Point> getRelatively(Point origin,List<Point> points)
        {
            List<Point> newPoints = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                newPoints.Add(DDDdamToScreen(origin, points[i]));
            }
            return newPoints;
        }


        //大坝坐标转屏幕坐标   origin为大坝坐标外切矩形原点
        static Point DDDdamToScreen(Point origin, Point dam_point)
        {
            int thisx = (dam_point.X - origin.X);
            int thisy = (dam_point.Y - origin.Y);
            return new Point(thisx, thisy);
        }



        //大坝坐标转屏幕坐标   origin为大坝坐标外切矩形原点
        static Point damToScreen(Point origin, Point dam_point)
        {
            int thisx = (dam_point.X - origin.X) * SCREEN_ONEMETER + map_left;
            int thisy = (dam_point.Y - origin.Y) * SCREEN_ONEMETER + map_top;
            return new Point(thisx, thisy);
        }

        static Point screenToDam(Point origin, Point screen_point)
        {
            int thisx = (screen_point.X - map_left) / SCREEN_ONEMETER + origin.X;
            int thisy = (screen_point.Y - map_top) / SCREEN_ONEMETER + origin.Y;
            return new Point(thisx, thisy);
        }

        //通过大地坐标系的vertex字符串得到大坝坐标下的边界点值
       public static List<Point> getSegmentVertex_DAM(String vertex)
        {
            List<Point> cs = new List<Point>();
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

                cs.Add(new Point((int)c.getX(), -(int)c.getY()));
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
        static Coordinate damToEarth(Point p)
        {
            double x = -cos * p.X + sin * (-p.Y) + 50212.59;
            double y = -sin * p.X - cos * (-p.Y) + 8447;
            return new Coordinate(x, y);
        }
        //得到这一堆点最左的点在这个数组中的下标,最小的X
        public static int getLeftIndex(List<Point> cs)
        {
            int i = -1;
            double minx = double.MaxValue;
            foreach (Point c in cs)
            {
                if (minx > c.X)
                {
                    minx = c.X;
                }
            }
            i = 0;
            foreach (Point c in cs)
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
        public static int getRightIndex(List<Point> cs)
        {
            int i = -1;
            double maxx = double.MinValue;
            foreach (Point c in cs)
            {
                if (maxx < c.X)
                {
                    maxx = c.X;
                }
            }
            i = 0;
            foreach (Point c in cs)
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
        public static int getBottomIndex(List<Point> cs)
        {
            int i = -1;
            double maxy = double.MinValue;
            foreach (Point c in cs)
            {
                if (maxy < c.Y)
                {
                    maxy = c.Y;
                }
            }
            i = 0;
            foreach (Point c in cs)
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
        public static int getTopIndex(List<Point> cs)
        {
            int i = -1;
            double miny = double.MaxValue;
            foreach (Point c in cs)
            {
                if (miny > c.Y)
                {
                    miny = c.Y;
                }
            }
            i = 0;
            foreach (Point c in cs)
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

    }
}
