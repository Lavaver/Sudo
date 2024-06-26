using System;
using System.IO;
using System.Xml;

namespace WorldBackup
{
    internal class CheckConfig
    {
        private const string DefaultSourceFolder = "默认备份来源文件夹";
        private const string DefaultBackupToFolder = "默认备份到文件夹";
        private const string DefaultNTPServerAddress = "time.windows.com";

        public static void Run()
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WorldBackupConfig.xml");

            try
            {
                if (File.Exists(configFilePath)) // 检查配置文件是否存在
                {
                    ReadConfigFile(configFilePath);
                }
                else
                {
                    CreateConfigFile(configFilePath);
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log($"WorldBackup CheckConfig Error", $"发生错误: {ex.Message}", ConsoleColor.Red);
                Environment.Exit(1);
            }
        }

        private static void ReadConfigFile(string filePath)
        {
            int PID = Environment.ProcessId;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode sourceNode = doc.SelectSingleNode("/Config/source");
                string sourceFolder = sourceNode?.InnerText ?? DefaultSourceFolder;

                XmlNode backupToNode = doc.SelectSingleNode("/Config/backupto");
                string backupToFolder = backupToNode?.InnerText ?? DefaultBackupToFolder;

                // 验证路径是否有效
                if (!IsValidPath(sourceFolder) || !IsValidPath(backupToFolder))
                {
                    LogConsole.Log($"WorldBackup CheckConfig Error", $"无效的路径配置: {sourceFolder} 或 {backupToFolder}", ConsoleColor.Red);
                    Environment.Exit(1);
                }

                LogConsole.Log($"WorldBackup CheckConfig Progress ({PID})", $"读取配置成功（{sourceFolder} => {backupToFolder}）", ConsoleColor.Blue);
            }
            catch (Exception ex)
            {
                LogConsole.Log($"WorldBackup CheckConfig Error", $"读取配置文件时发生错误: {ex.Message}", ConsoleColor.Red);
                throw;
            }
        }

        private static bool IsValidPath(string path)
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

        private static void CreateConfigFile(string filePath)
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
                sourceNode.InnerText = DefaultSourceFolder;
                configNode.AppendChild(sourceNode);

                XmlElement backupToNode = doc.CreateElement("backupto");
                backupToNode.InnerText = DefaultBackupToFolder;
                configNode.AppendChild(backupToNode);

                XmlElement NTPServerNode = doc.CreateElement("NTP-Server");
                NTPServerNode.InnerText = DefaultNTPServerAddress;
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