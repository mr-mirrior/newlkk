using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using DamLKK.Utils;

namespace DamLKK.DB
{
    public class DBconfig
    {
        private static DBconfig _MyInstance = null;

        public static DBconfig GetInstance()
        {
            if (_MyInstance == null)
            {
                _MyInstance = new DBconfig();
            }
            return _MyInstance;
        }

        const string encryptKey = "%^&*!@#$";//加密字符串

        private string server;

        public string Server
        {
            get { return server; }
            set { server = value; }
        }
        private string dbname;

        public string Dbname
        {
            get { return dbname; }
            set { dbname = value; }
        }
        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }
        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        private string damserver;

        public string Damserver
        {
            get { return damserver; }
            set { damserver = value; }
        }

        private double second;

        public double Second
        {
            get { return second; }
            set { second = value; }
        }

        private double meter;

        public double Meter
        {
            get { return meter; }
            set { meter = value; }
        }

                 private static string serverEncryptStr = Security.EncryptDES("server", encryptKey);
                 private static string dbnameEncryptStr = Security.EncryptDES("dbname", encryptKey);
                 private static string passwordEncryptStr = Security.EncryptDES("password", encryptKey);
                 private static string usernameEncryptStr = Security.EncryptDES("username", encryptKey);

        public bool Init()
        {
            DebugUtil.fileLog(serverEncryptStr + " " + dbnameEncryptStr + " " + usernameEncryptStr + " " + passwordEncryptStr);
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "db.config";

            FileInfo file = new FileInfo(strFileName);
            if(!file.Exists){
                /*throw new Exception();*/
                return false;
            }

            doc.Load(strFileName);
            //找出名称为“add”的所有元素
     
            XmlNodeList nodes = doc.GetElementsByTagName("db");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性
                XmlAttribute key = nodes[i].Attributes["key"];
                XmlAttribute value = nodes[i].Attributes["value"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素
                if (key.Value.Equals(serverEncryptStr))
                {
                   this.Server = Utils.Security.DecryptDES(value.Value,encryptKey);
                }
                else if (key.Value.Equals(dbnameEncryptStr))
                {
                   this.Dbname = Utils.Security.DecryptDES(value.Value,encryptKey);
                }
                else if (key.Value.Equals(passwordEncryptStr))
                {
                   this.Password = Utils.Security.DecryptDES(value.Value,encryptKey);
                }
                else if (key.Value.Equals(usernameEncryptStr))
                {
                   this.Username = Utils.Security.DecryptDES(value.Value,encryptKey);
                }
                else if (key.Value.Equals("damserver"))
                {
                    Damserver = value.Value;
                }
            }

            return true;
        }

        public void update(string configPath ,string server,string dbname,string username,string password){
             string serverValueEncryptStr = Utils.Security.EncryptDES(server, encryptKey);
             string dbnameValueEncryptStr = Utils.Security.EncryptDES(dbname, encryptKey);
             string passwordValueEncryptStr = Utils.Security.EncryptDES(password, encryptKey);
             string usernameValueEncryptStr = Utils.Security.EncryptDES(username, encryptKey);

             XmlDocument doc = new XmlDocument();
             //获得配置文件的全路径
             if (configPath == null)
             {
                 configPath = AppDomain.CurrentDomain.BaseDirectory.ToString();
             }
             string strFileName =  configPath + "db.config";

             doc.Load(strFileName);
             //找出名称为“add”的所有元素
             XmlNodeList nodes = doc.GetElementsByTagName("db");
             for (int i = 0; i < nodes.Count; i++)
             {
                 //获得将当前元素的key属性
                 XmlAttribute key = nodes[i].Attributes["key"];
                 XmlAttribute value = nodes[i].Attributes["value"];
                 //根据元素的第一个属性来判断当前的元素是不是目标元素
                 //Console.WriteLine(key.Value + "\t" + value.Value);
                 if (key.Value.Equals(serverEncryptStr))
                 {
                     value.Value = serverValueEncryptStr;
                 }
                 else if (key.Value.Equals(dbnameEncryptStr))
                 {
                     value.Value = dbnameValueEncryptStr;
                 }
                 else if (key.Value.Equals(passwordEncryptStr))
                 {
                     value.Value = passwordValueEncryptStr;
                 }
                 else if (key.Value.Equals(usernameEncryptStr))
                 {
                     value.Value = usernameValueEncryptStr;
                 }
             }
             doc.Save(strFileName);
        }

    }
}
