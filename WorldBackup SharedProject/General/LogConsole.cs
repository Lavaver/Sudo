using System;
using System.Diagnostics;
using System.IO;

namespace com.Lavaver.WorldBackup.Core
{
    internal class LogConsole
    {
        /// <summary>
        /// 并发锁，防止多个日志并发进行导致竞争问题
        /// </summary>
        public static readonly object Concurrent_Lock = new object();

        /// <summary>
        /// 日志文件名
        /// </summary>
        public static readonly string logFileName = $"log_{Guid.NewGuid().ToString()}.log";

        /// <summary>
        /// 日志文件夹
        /// </summary>
        public static readonly string logPath = "Log";

        /// <summary>
        /// 进程监听
        /// </summary>
        public static TextWriterTraceListener? _traceListener;


        public static string GetCurrentTime(bool withMilliseconds = false)
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

            if(_traceListener != null)
            {
                Trace.Listeners.Add(_traceListener); // 添加监听器
            }
            else
            {
                Trace.Listeners.Clear(); // 首先尝试清空监听器，然后重新 _traceListener = new TextWriterTraceListener(Path.Combine(logPath, logFileName)); 。鉴于本程序只允许单个日志文件，必须添加该代码以确保健壮性
                _traceListener = new TextWriterTraceListener(Path.Combine(logPath, logFileName)); // 这里我很诧异为啥 GPT 评估出来的是逻辑性问题，首先，检查如果是 null 必须尝试清空、重建并添加监听器，这在只允许单个日志文件的软件项目中从逻辑层面不存在逻辑问题，我已经在确保安全清空情况下重建我干嘛要套娃去做额外检查？
                Trace.Listeners.Add(_traceListener); // 添加监听器
            }

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
            lock (Concurrent_Lock)
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
            lock (Concurrent_Lock)
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

        public static void HandleException(Exception ex, string message)
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