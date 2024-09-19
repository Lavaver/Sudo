namespace com.Lavaver.WorldBackup.Global
{
    /// <summary>
    /// 全局类，用于存放一些常用常量。
    /// </summary>
    internal class GlobalString
    {
        /// <summary>
        /// 数据库位置
        /// </summary>
        public static readonly string DatabaseLocation = "Backup_DataBase.xml";
        /// <summary>
        /// 软件配置位置
        /// </summary>
        public static readonly string SoftwareConfigLocation = "config.xml";
        /// <summary>
        /// 软件许可证位置
        /// </summary>
        public static readonly string LICENSE = "LICENSE";
        public static readonly string LogLocation = "Log";
        public static readonly string destinationPath = "/";

        public static readonly string ExampleDAVUsrname = "Example";
        public static readonly string ExampleDAVPassword = "Example";
        public static readonly string ExampleDAVServer = "https://example.com";

        public static readonly string DesktopIL = Environment.SpecialFolder.Desktop.ToString();
    }


}