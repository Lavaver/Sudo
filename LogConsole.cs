using System;
using System.IO;

namespace WorldBackup
{
    internal class LogConsole
    {
        private static readonly object _lock = new object();
        private static readonly string logFileName = $"log_{Guid.NewGuid().ToString()}.log";
        private static StreamWriter logFile;

        static void Logger()
        {
            try
            {
                logFile = File.AppendText(logFileName);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} / LogConsole ERROR] Failed to initialize log file: {ex.Message}");
            }
        }

        /// <summary>
        /// 紧急关闭日志文件
        /// </summary>
        private static void ERRCloseLogFile()
        {
            if (logFile != null)
            {
                try
                {
                    logFile.Close();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} / LogConsole ERROR] Failed to close log file: {ex.Message}");
                }
                finally
                {
                    logFile = null;
                }
            }
        }

        private static string GetCurrentTime(bool withMilliseconds = false)
        {
            return withMilliseconds ? $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}" : $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        /// <summary>
        /// 统一的可缩放日志模块
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        /// <param name="log">日志内容</param>
        /// <param name="color">前景色</param>
        /// <param name="isError">是否为错误日志</param>
        /// <param name="logToFile">是否写入到日志文件</param>
        public static void Log(string logLevel, string log, ConsoleColor color, bool isError = false, bool logToFile = true)
        {
            lock (_lock)
            {
                string currentTime = GetCurrentTime(isError);
                ConsoleColor originalColor = Console.ForegroundColor;
                try
                {
                    if (isError)
                    {
                        Console.Error.Write($"[{currentTime} / ");
                        Console.ForegroundColor = color;
                        Console.Error.Write($"{logLevel}");
                        Console.ForegroundColor = originalColor;
                        Console.Error.WriteLine($"] {log}");
                    }
                    else
                    {
                        Console.Write($"[{currentTime} / ");
                        Console.ForegroundColor = color;
                        Console.Write($"{logLevel}");
                        Console.ForegroundColor = originalColor;
                        Console.WriteLine($"] {log}");
                    }

                    if (logToFile && logFile != null)
                    {
                        try
                        {
                            logFile.WriteLine($"[{currentTime} / {logLevel}] {log}");
                            logFile.Flush();
                        }
                        catch (Exception ex)
                        {
                            Console.Error.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} / ");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Error.Write($"LogConsole ERROR");
                            Console.ForegroundColor = originalColor;
                            Console.Error.WriteLine($"] Failed to write log to file: {ex.Message}");
                            ERRCloseLogFile();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} / ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.Write($"LogConsole ERROR");
                    Console.ForegroundColor = originalColor;
                    Console.Error.WriteLine($"] General logging error: {ex.Message}");
                }
                finally
                {

                }
            }
        }

        /// <summary>
        /// 将 INFO 输出至标准输出流
        /// </summary>
        /// <param name="log">日志</param>
        public static void Info(string log)
        {
            Log("Info", log, ConsoleColor.Green);
        }

        /// <summary>
        /// 将 WARN 输出至标准输出流
        /// </summary>
        /// <param name="log">日志</param>
        public static void Warn(string log)
        {
            Log("WARN", log, ConsoleColor.Yellow);
        }

        /// <summary>
        /// 将 ERROR 输出至标准输出流
        /// </summary>
        /// <param name="log">日志</param>
        public static void Err(string log)
        {
            Log("ERROR", log, ConsoleColor.Red, true);
        }

        /// <summary>
        /// 将 Init 输出至标准输出流
        /// </summary>
        /// <param name="log">日志</param>
        public static void Init(string log)
        {
            Log("Init", log, ConsoleColor.Blue);
        }

        /// <summary>
        /// 关闭日志文件
        /// </summary>
        public static void CloseLogFile()
        {
            logFile.Close();
        }
    }
}