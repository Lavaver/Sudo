using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Xml.Serialization;
using WorldBackup;

public class FirstBackup
{
    private const string BackupDatabaseFile = "备份数据库.xml";
    private const string ConfigFile = "WorldBackupConfig.xml";

    public static void Pullup()
    {
        CheckAndCreateBackupDatabase();
        var (source, backupto) = ReadConfig();
        PerformBackup(source, backupto);
    }

    private static void CheckAndCreateBackupDatabase()
    {
        if (!File.Exists(BackupDatabaseFile))
        {
            // 创建一个新的 XML 文档结构
            var newDoc = new XDocument(new XElement("Backups"));
            newDoc.Save(BackupDatabaseFile);
        }
    }

    private static (string source, string backupto) ReadConfig()
    {
        if (!File.Exists(ConfigFile))
        {
            LogConsole.Log($"WorldBackup Backup", "配置不存在", ConsoleColor.Red);
            Environment.Exit(1);
        }

        var configXml = XDocument.Load(ConfigFile);
        var source = configXml.Root.Element("source")?.Value;
        var backupto = configXml.Root.Element("backupto")?.Value;

        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(backupto))
        {
            LogConsole.Log($"WorldBackup Backup", "无效的路径配置，检查配置文件", ConsoleColor.Red);
            Environment.Exit(1);
        }

        return (source, backupto);
    }

    private static void PerformBackup(string source, string backupto)
    {
        if (!Directory.Exists(backupto))
        {
            Directory.CreateDirectory(backupto);
        }

        var backupIdentifier = Guid.NewGuid().ToString();
        var backupTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var backupPath = Path.Combine(backupto, $"{backupIdentifier}_{backupTime}");

        if (Directory.Exists(source))
        {
            CopyDirectory(source, backupPath);
            LogConsole.Log($"WorldBackup Backup", $"{source} （备份到）=> {backupPath}", ConsoleColor.Blue);
        }
        else if (File.Exists(source))
        {
            File.Copy(source, backupPath);
            LogConsole.Log($"WorldBackup Backup", $"{source} （备份到）=> {backupPath}", ConsoleColor.Blue);
        }
        else
        {
            LogConsole.Log($"WorldBackup Backup", $"无效的路径配置，检查配置文件的 source 值", ConsoleColor.Red);
            Environment.Exit(1);
        }

        SaveBackupRecord(backupPath, backupIdentifier);
    }

    private static void CopyDirectory(string sourceDir, string destinationDir)
    {
        Directory.CreateDirectory(destinationDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
            File.Copy(file, destFile);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var destDir = Path.Combine(destinationDir, Path.GetFileName(dir));
            CopyDirectory(dir, destDir);
        }
    }

    private static void SaveBackupRecord(string backupPath, string backupIdentifier)
    {
        var backupTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        var doc = XDocument.Load(BackupDatabaseFile);
        var root = doc.Element("Backups");

        var newBackupElement = new XElement($"Backup",
            new XElement("Identifier", backupIdentifier),
            new XElement("Time", backupTime),
            new XElement("Path", backupPath)
        );

        root.Add(newBackupElement);
        doc.Save(BackupDatabaseFile);
    }
}