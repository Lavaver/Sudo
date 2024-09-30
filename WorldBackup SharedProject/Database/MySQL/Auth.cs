using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using com.Lavaver.WorldBackup.Core;
using com.Lavaver.WorldBackup.Global;
using MySql.Data.MySqlClient;

namespace com.Lavaver.WorldBackup.Database.MySQL
{
    /// <summary>
    /// Auth 类用于管理 MySQL 数据库的认证信息
    /// </summary>
    internal class Auth
    {
        /// <summary>
        /// 检查 Auth 文件是否存在，如果不存在则询问用户输入 MySQL 数据库信息并写入配置文件，如果存在则进行 ping 测试
        /// </summary>
        public static void CheckAuthFile()
        {
            if (!File.Exists(GlobalString.GlobalDataBaseAuthFileName))
            {
                // 使用 using 语句自动管理 FileStream 的生命周期
                using (File.Create(GlobalString.GlobalDataBaseAuthFileName)) { }

                LogConsole.Log("MySQL Auth", "为了对远程 MySQL 数据库进行初始化设置，我们需要询问你几个问题：", ConsoleColor.Blue);
                LogConsole.Log("MySQL Auth", "MySQL 数据库用户名：", ConsoleColor.Blue);
                string username = Console.ReadLine();
                LogConsole.Log("MySQL Auth", "MySQL 数据库密码：", ConsoleColor.Blue);
                string password = Console.ReadLine();
                LogConsole.Log("MySQL Auth", "MySQL 数据库地址（如果是 IP 地址，请直接输入，不要在前面加上“http://”等，否则会导致连接失败）：", ConsoleColor.Blue);
                string address = Console.ReadLine();
                LogConsole.Log("MySQL Auth", "MySQL 数据库端口：", ConsoleColor.Blue);
                string port = Console.ReadLine();
                LogConsole.Log("MySQL Auth", "MySQL 数据库名称：", ConsoleColor.Blue);
                string database = Console.ReadLine();
                LogConsole.Log("MySQL Auth", "正在写入配置文件...", ConsoleColor.Blue);

                // 直接写入文件
                File.WriteAllText(GlobalString.GlobalDataBaseAuthFileName, $"Username={username}\nPassword={password}\nAddress={address}\nPort={port}\nDatabase={database}");

                LogConsole.Log("MySQL Auth", $"配置文件已写入到 {GlobalString.GlobalDataBaseAuthFileName} 文件中！你之后可以直接使用 WorldBackup CLI+ 使用你的 MySQL 数据库作为备份记录。", ConsoleColor.Green);
            }
        }


        /// <summary>
        /// 获取 MySQL 数据库连接信息
        /// </summary>
        /// <returns>连接信息（可用于任何需要 MySQL 数据库连接的地方）</returns>
        public static MySqlConnection GetConnection()
        {
            string[] authInfo = File.ReadAllLines(GlobalString.GlobalDataBaseAuthFileName);
            string username = authInfo[0].Split('=')[1];
            string password = authInfo[1].Split('=')[1];
            string address = authInfo[2].Split('=')[1];
            string port = authInfo[3].Split('=')[1];
            string database = authInfo[4].Split('=')[1];
            string connectionString = $"Server={address};Port={port};Database={database};User Id={username};Password={password};";
            MySqlConnection connection = new MySqlConnection(connectionString);
            return connection;
        }

        /// <summary>
        /// 测试 MySQL 数据库连接
        /// </summary>
        public static void Test()
        {
            CheckAuthFile();
            LogConsole.Log("MySQL Auth", "MySQL 正在测试认证...", ConsoleColor.Green);
            MySqlConnection connection = GetConnection();
            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand("SELECT VERSION()", connection);
                string version = command.ExecuteScalar().ToString();
                LogConsole.Log("MySQL Auth", $"MySQL 数据库版本：{version}", ConsoleColor.Green);
                LogConsole.Log("MySQL Auth", "MySQL 认证成功！", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                LogConsole.Log("MySQL Auth", $"MySQL 认证失败！请检查配置文件中的服务器信息是否正确！（最后一帧：{e.Message}）", ConsoleColor.Red, true);
                return;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
