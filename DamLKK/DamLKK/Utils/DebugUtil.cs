using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DamLKK.Utils
{
    public class DebugUtil
    {

        public const int  CONSOLE  = 1;
        public const int FILE = 2;
        
#if DEBUG
        public static int logstyle = CONSOLE;
#else
        public static int logstyle = FILE;
#endif


        public static void log(Exception e){
            if(logstyle==CONSOLE){
                System.Diagnostics.Debug.Print(e.Message);
                /*Console.WriteLine(e.Message);*/
            }else if(logstyle==FILE){
                fileLog(e.Message);
            }
        }


        //debugutil在建立文件的时候有问题
        public static void fileLog(string logstr){

            DirectoryInfo logpath = new DirectoryInfo("logs");
            FileInfo logfile = new FileInfo(logpath.FullName+"/"+string.Format("{0:yyyyMMdd}",DateTime.Now)+".log");

            if (!logpath.Exists)
            {
                logpath.Create();
            }

            StreamWriter streamWriter = new StreamWriter(logfile.FullName, true);//如果logfile不存在，此方法自动建立文件

            streamWriter.WriteLine(DateTime.Now.ToString()+" "+logstr);
            streamWriter.Flush();
            streamWriter.Close();
           
        }
    }
}
