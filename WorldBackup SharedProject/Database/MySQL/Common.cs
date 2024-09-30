using MySql.Data.MySqlClient;

namespace com.Lavaver.WorldBackup.Database.MySQL
{
    /// <summary>
    /// This class contains methods for creating and dropping the `Backups` table in the MySQL database.
    /// </summary>
    internal class Tables
    {
        /// <summary>
        /// Creates the `Backups` table in the MySQL database if it doesn't exist already.
        /// </summary>
        public static void Create()
        {
            MySqlConnection conn = Auth.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS `Backups` ( `Identifier` VARCHAR(36) NOT NULL, `Time` DATETIME NOT NULL, `Path` VARCHAR(255) NOT NULL, PRIMARY KEY (`Identifier`)) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        /// <summary>
        /// Drops the `Backups` table in the MySQL database if it exists.
        /// </summary>
        public static void Drop()
        {
            MySqlConnection conn = Auth.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DROP TABLE IF EXISTS `Backups`;";
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static string GetBackupTable()
        {
            MySqlConnection conn = Auth.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SHOW TABLES LIKE 'Backups'";
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return "Backups";
            }
            else
            {
                return null;
            }
        }
    }
}
