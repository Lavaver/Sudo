using com.Lavaver.WorldBackup.Core;
using com.Lavaver.WorldBackup.Global;
using com.Lavaver.WorldBackup.Global.ReadConfig;
using com.Lavaver.WorldBackup.Database.MySQL;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace com.Lavaver.WorldBackup
{
    internal class CheckConfig
    {
        public static void Run()
        {
            try
            {
                if (File.Exists(GlobalString.SoftwareConfigLocation())) // 检查配置文件是否存在
                {
                    UserConfig.ReadConfigFile(GlobalString.SoftwareConfigLocation());
                    if (SQLConfig.IsEnabled())
                    {
                        Auth.Test();
                    }
                    return;
                }
                else
                {
                    CreateConfigFile(GlobalString.SoftwareConfigLocation());
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log($"WorldBackup CheckConfig Error", $"发生错误: {ex.Message}", ConsoleColor.Red);
                Environment.Exit(1);
            }
        }



        public static bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            try
            {
                // 判断是否为绝对路径
                if (Path.IsPathFullyQualified(path))
                {
                    return Directory.Exists(path) || Path.GetFullPath(path) != null;
                }

                // 如果是相对路径，转换为绝对路径再进行检查
                string fullPath = Path.GetFullPath(path);
                return Directory.Exists(fullPath) || Path.GetFullPath(fullPath) != null;
            }
            catch
            {
                return false;
            }
        }

        public static void CreateConfigFile(string filePath)
        {
            int PID = Environment.ProcessId;

            try
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(declaration);

                XmlElement configNode = doc.CreateElement("Config");
                doc.AppendChild(configNode);

                XmlElement sourceNode = doc.CreateElement("source");
                sourceNode.InnerText = GlobalString.DefaultSourceFolder;
                configNode.AppendChild(sourceNode);

                XmlElement backupToNode = doc.CreateElement("backupto");
                backupToNode.InnerText = GlobalString.DefaultBackupToFolder;
                configNode.AppendChild(backupToNode);

                XmlElement NTPServerNode = doc.CreateElement("NTP-Server");
                NTPServerNode.InnerText = GlobalString.DefaultNTPServerAddress;
                configNode.AppendChild(NTPServerNode);

                doc.Save(filePath);

                LogConsole.Log($"WorldBackup CheckConfig Progress ({PID})", $"配置已生成。请使用 -config 修改你的配置。程序将退出以便配置生效", ConsoleColor.Blue);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                LogConsole.Log($"WorldBackup CheckConfig Error", $"创建配置文件时发生错误: {ex.Message}", ConsoleColor.Red);
                throw;
            }
        }
    }
}