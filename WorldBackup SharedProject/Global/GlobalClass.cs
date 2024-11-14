using System.Diagnostics;

namespace com.Lavaver.WorldBackup.Global
{
    /// <summary>
    /// 全局类，用于存放一些常用常量。
    /// </summary>
    internal class GlobalString
    {
        static Process currentProcess = Process.GetCurrentProcess();

        /// <summary>
        /// 数据库位置
        /// </summary>
public static string DatabaseLocation()
{
    var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    var os = Environment.OSVersion;
    return os.Platform switch
    {
        PlatformID.Win32NT => "Backup.xmldb",
                
        PlatformID.Unix or PlatformID.MacOSX or PlatformID.Other => $"{homeDirectory}/.sudari-cli/Backup.xmldb",
        _ => "Backup.xmldb"
    };
}


        /// <summary>
        /// 软件配置位置
        /// </summary>
        public static string SoftwareConfigLocation()
        {
            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var os = Environment.OSVersion;
            return os.Platform switch
            {
                PlatformID.Win32NT => "Config.xml",
                
                PlatformID.Unix or PlatformID.MacOSX or PlatformID.Other => $"{homeDirectory}/.sudari-cli/Config.xml",
                _ => "Config.xml"
            };
        }

        /// <summary>
        /// 软件许可证位置
        /// </summary>
        public static readonly string LICENSE = "LICENSE";

        /// <summary>
        /// 日志目录名称
        /// </summary>
        public static readonly string LogLocation = "Log";

        /// <summary>
        /// “默认备份来源文件夹”配置。该预置值在软件初次启动（或是重构配置文件）时被写入到 <see cref="SoftwareConfigLocation"/> 文件中。
        /// </summary>
        public const string DefaultSourceFolder = "Default_Source_Folder";

        /// <summary>
        /// “备份到文件夹”配置。该预置值在软件初次启动（或是重构配置文件）时被写入到 <see cref="SoftwareConfigLocation"/> 文件中。
        /// </summary>
        public const string DefaultBackupToFolder = "Default_BackupTo_Folder";

        /// <summary>
        /// “NTP 时间来源”配置。该预置值在软件初次启动（或是重构配置文件）时被写入到 <see cref="SoftwareConfigLocation"/> 文件中。
        /// </summary>
        public const string DefaultNTPServerAddress = "time.windows.com";

        /// <summary>
        /// “全局 MySQL 数据库认证文件名称”常量。该常量在当用户编辑 config.xml > MySQL:true 节点，或是直接以 -config 模式启用 MySQL 模式并重启主程序时创建。
        /// </summary>
        public const string GlobalDataBaseAuthFileName = "SQLAuth.authfile";

        /// <summary>
        /// “进程名称”变量。该变量在软件每次启动时被读取，然后交由 <see cref="Start.CheckProgress.OnCheck()"/> 处理多进程冲突。
        /// </summary>
        public static string processName = currentProcess.ProcessName;
    }


}