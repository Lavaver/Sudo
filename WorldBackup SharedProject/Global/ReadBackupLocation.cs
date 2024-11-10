using System.Xml.Linq;

namespace com.Lavaver.WorldBackup.Global;

public class ReadBackupLocation
{
    public static string Get()
    {
        XDocument doc = XDocument.Load(GlobalString.SoftwareConfigLocation);
        XElement element = doc.Root.Element("backupto");

        if (!string.IsNullOrEmpty(element.Value))
        {
            return element.Value;
        } 
        return null;
    }
}