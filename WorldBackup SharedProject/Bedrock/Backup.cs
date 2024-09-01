using System;
using System.IO.Compression;
using com.Lavaver.WorldBackup.Core;

namespace com.Lavaver.WorldBackup.Bedrock
{
    public class Backup
    {
        static readonly Guid GUId = Guid.NewGuid();

        /// <summary>
        /// 《Minecraft For Windows》存档文件夹。此文件夹是固定的，无需更改
        /// </summary>
        static readonly string MinecraftPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\AppData\\Local\\Packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\LocalState\\games\\com.mojang\\minecraftWorlds";
        
        /// <summary>
        /// 文件预存区
        /// </summary>
        static Dictionary<string, byte[]> BTE = [];

        /// <summary>
        /// 压缩文件名称
        /// </summary>
        static readonly string pr = $"Bedrock_Backup_{GUId:D}.zip";

        public static void Run()
        {
            ScanWorld(MinecraftPath, MinecraftPath, BTE);
            CreateZipArchive(pr, BTE);
            LogConsole.Log("Bedrock Backup", "完成", ConsoleColor.Blue);
        }

        /// <summary>
        /// 扫描《Minecraft For Windows》下的全部存档
        /// </summary>
        static void ScanWorld(string currentDir, string startingDir, Dictionary<string, byte[]> fileBytesStorage)
        {
            LogConsole.Log("Bedrock Backup - ScanWorld", $"扫描目录 {currentDir}", ConsoleColor.Blue);

            try
            {
                string[] files = Directory.GetFiles(currentDir);

                foreach (string file in files)
                {
                    byte[] fileBytes = File.ReadAllBytes(file);
                    string relativePath = GetRelativePath(startingDir, file);
                    fileBytesStorage[relativePath] = fileBytes;
                }

                string[] subdirectories = Directory.GetDirectories(currentDir);
                foreach (string subdir in subdirectories)
                {
                    ScanWorld(subdir, startingDir, fileBytesStorage);
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log("Bedrock Backup - ScanWorld", $"扫描目录 {currentDir} 时发生 {ex.Message}", ConsoleColor.Red, true);
            }
        }

        static string GetRelativePath(string rootPath, string fullPath)
        {
            return fullPath[(rootPath.Length + 1)..];
        }

        static void CreateZipArchive(string zipFileName, Dictionary<string, byte[]> fileBytesStorage)
        {
            try
            {
                using var zipArchive = ZipFile.Open(zipFileName, ZipArchiveMode.Create);
                foreach (var kvp in fileBytesStorage)
                {
                    string filePath = kvp.Key;
                    byte[] fileBytes = kvp.Value;

                    // 创建一个新的 ZipArchiveEntry 并将文件内容写入
                    var entry = zipArchive.CreateEntry(filePath, CompressionLevel.Optimal);
                    using var entryStream = entry.Open();
                    LogConsole.Log("Bedrock Backup - CreateZipArchive", $"将 {fileBytes.Length} 字节写入到 {zipFileName}", ConsoleColor.Blue);
                    entryStream.Write(fileBytes, 0, fileBytes.Length);
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log("Bedrock Backup - ScanWorld", $"写入时发生 {ex.Message}", ConsoleColor.Red, true);
            }
        }
    }
}
