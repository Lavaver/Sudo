using System;
using System.Diagnostics;
using System.IO;

namespace WorldBackup
{
    internal class LogConsole
    {
        private static readonly object _lock = new object();
        private static readonly string logFileName = $"log_{Guid.NewGuid().ToString()}.log";
        private static readonly string logPath = "Log";
        private static TextWriterTraceListener _traceListener;


        private static string GetCurrentTime(bool withMilliseconds = false)
        {
            return withMilliseconds ? $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}" : $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }
        
        /// <summary>
        /// 初始化日志模块
        /// </summary>
        public static void Initialize()
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            _traceListener = new TextWriterTraceListener(Path.Combine(logPath, logFileName));
            Trace.Listeners.Add(_traceListener);
        }


        /// <summary>
        /// 统一的可缩放日志模块（最近更改：史 诗 级 重 构 日 志 模 块）
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        /// <param name="log">日志内容</param>
        /// <param name="color">前景色</param>
        /// <param name="isError">（可选）是否为错误日志。默认为否（false）</param>
        /// <param name="logToFile">（可选）是否写入到日志文件。默认为是（true）</param>
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

                    if (logToFile && _traceListener != null)
                    {
                        Trace.WriteLine($"[{currentTime} / {logLevel}] {log}");
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex, "General logging error");
                }
                finally
                {
                    Trace.Flush();
                }
            }
        }

        /// <summary>
        /// 正在执行进程时可用的日志记录方式
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        /// <param name="log">日志</param>
        public static void TaskLog(string logLevel, string log)
        {
            lock (_lock)
            {
                try
                {
                        Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} / ");
                        Console.Write($"{logLevel}");
                        Console.Write($"] {log}");
                }
                catch (Exception ex)
                {
                    HandleException(ex, "General logging error");
                }
                finally
                {
                    Trace.Flush();
                }
            }
        }

        private static void HandleException(Exception ex, string message)
        {
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.Error.Write($"[{currentTime} / ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write($"LogConsole ERROR");
            Console.ForegroundColor = originalColor;
            Console.Error.WriteLine($"] {message}: {ex.Message}");

            Trace.WriteLine($"[{currentTime} / LogConsole ERROR] {message}: {ex.Message}");
            Trace.WriteLine(ex.StackTrace);
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
        /// 将 Debug 输出至标准输出流
        /// </summary>
        /// <param name="log">日志</param>
        public static void Debug(string log)
        {
            Log("Debug", log, ConsoleColor.Green);
        }

        /// <summary>
        /// 将 R5 输出至标准输出流
        /// </summary>
        /// <param name="log">日志</param>
        public static void R5(string log)
        {
            Log("Console R5", log, ConsoleColor.Blue);
        }
    }
}