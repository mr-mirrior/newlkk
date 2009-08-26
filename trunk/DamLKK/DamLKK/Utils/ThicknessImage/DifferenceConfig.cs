using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace DM.DB
{
    public class DifferenceConfig
    {
        private static DifferenceConfig myInstance = null;

        public static DifferenceConfig getInstance()
        {
            if (myInstance == null)
            {
                myInstance = new DifferenceConfig();
            }
            //myInstance.init();
            return myInstance;
        }

        public XmlNodeList getXmlDocument(){
            //DebugUtil.fileLog(serverEncryptStr + " " + dbnameEncryptStr + " " + usernameEncryptStr + " " + passwordEncryptStr);
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "difference.config";

            FileInfo file = new FileInfo(strFileName);
            if (!file.Exists)
            {
                /*throw new Exception();*/
                return null;
            }

            doc.Load(strFileName);
            //找出名称为“add”的所有元素
            return doc.GetElementsByTagName("difference");
        }

        public string getValueByKey(XmlNodeList node,string keystr){
            string min = "";
            XmlNodeList nodes = getXmlDocument();
            if (nodes == null)
                return string.Empty;
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性
                XmlAttribute key = nodes[i].Attributes["key"];
                XmlAttribute value = nodes[i].Attributes["value"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素
                //Console.WriteLine(key.Value+"\t"+value.Value);
                if (key.Value.Equals(keystr))
                {
                    min = value.Value;
                    break;
                }
               
            }
            return min;
        }

        public string getMaxValue()
        {
            XmlNodeList nodes = getXmlDocument();
            return getValueByKey(nodes, "max");
        }
        public string getMinValue(){
            XmlNodeList nodes = getXmlDocument();
            return getValueByKey(nodes, "min");
        }

    }
}
