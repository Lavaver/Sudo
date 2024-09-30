using com.Lavaver.WorldBackup.Global;
using System.Xml.Linq;

namespace com.Lavaver.WorldBackup.Database.MySQL
{
    internal class SQLConfig
    {
        public static bool IsEnabled()
        {
            var configXml = XDocument.Load(GlobalString.SoftwareConfigLocation);
            var mysqlNode = configXml.Root.Element("MySQL");

            if (mysqlNode != null && mysqlNode.Value == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
