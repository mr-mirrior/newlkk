using System.IO;

namespace DamLKK.Utils.Xml
{

    public static class XMLUtil<T> where T : class
    {
        public static bool SaveXml(string xml, T value)
        {
            try
            {
                if (value == null)
                    return false;
                string path = Path.GetFullPath(xml);
                if (path == null)
                    return false;
                DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(path));
                if (!di.Exists)
                {
                    di.Create();
                }
                ObjectXMLSerializer<T>.Save(value, xml);
                return true;
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.Print(e.ToString());
                return false;
            }
        }
        public static T LoadXml(string xml)
        {
            try
            {
                if (!File.Exists(xml))
                    return null;
                return ObjectXMLSerializer<T>.Load(xml);
            }
            catch
            {
                return default(T);
            }
        }
    }
}