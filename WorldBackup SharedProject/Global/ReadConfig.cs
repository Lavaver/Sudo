using com.Lavaver.WorldBackup.Global;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using com.Lavaver.WorldBackup.Core;
using System.Xml;

namespace com.Lavaver.WorldBackup.Global.ReadConfig
{

    internal class UserConfig
    {
        public static void ReadConfigFile(string filePath)
        {
            int PID = Environment.ProcessId;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode sourceNode = doc.SelectSingleNode("/Config/source");
                string sourceFolder = sourceNode?.InnerText ?? GlobalString.DefaultSourceFolder;

                XmlNode backupToNode = doc.SelectSingleNode("/Config/backupto");
                string backupToFolder = backupToNode?.InnerText ?? GlobalString.DefaultBackupToFolder;

                // 验证路径是否有效
                if (!CheckConfig.IsValidPath(sourceFolder) || !CheckConfig.IsValidPath(backupToFolder))
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
    }
}

