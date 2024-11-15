﻿using System;
using System.IO.Compression;
using com.Lavaver.WorldBackup.Core;

namespace com.Lavaver.WorldBackup.Bedrock
{
    public class Backup
    {


        static readonly Guid UUID = Guid.NewGuid();

        /// <summary>
        /// 《Minecraft For Windows》存档文件夹。此文件夹是固定的，无需更改
        /// </summary>
        static readonly string MinecraftUWP_8wekyb3d8bbwe_LocalPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\AppData\\Local\\Packages\\Microsoft.MinecraftUWP_8wekyb3d8bbwe\\LocalState\\games\\com.mojang\\minecraftWorlds";
        
        /// <summary>
        /// 文件预存区
        /// </summary>
        static Dictionary<string, byte[]> FilePreSaveArea = [];

        /// <summary>
        /// 压缩文件名称
        /// </summary>
        static readonly string ZipFileName_String = $"Bedrock_Backup_{UUID:D}.zip";

        public static void Run()
        {
            var os = Environment.OSVersion;

            // 检查是否为 Windows NT 系统
            if (os.Platform == PlatformID.Win32NT)
            {
                // 检查内部版本是否大于等于 10
                if (os.Version.Major >= 10)
                {
                    ScanWorld(MinecraftUWP_8wekyb3d8bbwe_LocalPath, MinecraftUWP_8wekyb3d8bbwe_LocalPath, FilePreSaveArea);
                    CreateZipArchive(ZipFileName_String, FilePreSaveArea);
                    LogConsole.Log("Bedrock Backup", "完成", ConsoleColor.Blue);
                }
                else
                {
                    LogConsole.Log("Bedrock Backup", $"该功能仅支持系统内部版本 ≥10 （即 Windows 10/11 及之后的系统）才能使用该 UWP 应用特殊功能，但你的操作系统版本为 {os.Version}", ConsoleColor.Yellow);
                }
            }
            else
            {
                LogConsole.Log("Bedrock Backup", $"该功能仅支持 Windows NT 环境，但你的操作系统环境为 {os.Platform}", ConsoleColor.Yellow);
            }
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
                LogConsole.Log("Bedrock Backup - Archive", $"正在写入到 {zipFileName}", ConsoleColor.Green);
                foreach (var kvp in fileBytesStorage)
                {
                    string filePath = kvp.Key;
                    byte[] fileBytes = kvp.Value;

                    // 创建一个新的 ZipArchiveEntry 并将文件内容写入
                    var entry = zipArchive.CreateEntry(filePath, CompressionLevel.Optimal);
                    using var entryStream = entry.Open();
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
