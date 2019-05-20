using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SimpleLogger
{
    /// <summary>
    /// Singleton Logger class
    /// </summary>
    public class Logger
    {
        private static volatile Logger _logger;
        
        // Locker for new instance creation
        private static object _newInstanceLocker = new object();
        // Locker for the _messageQueue
        private object _messageQueueLocker = new object();

        // All messages first, added to the _messageQueue
        private Queue<LogMessage> _messageQueue;
        
        private const int writeInterval = 3000; // every 3s
        // writing thread going to deep sleep if there are no logs
        private bool inDeepSleep;
        // Thread that periodically writes collected logs to the file
        private Thread logWriterThread;
        // Reset event for critical and error logs.
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        private const string _logFilePath = "Logs/log.txt";

        public static Logger Instance
        {
            get
            {
                if (_logger == null)
                {
                    lock (_newInstanceLocker)
                    {
                        if (_logger == null)
                        {
                            _logger = new Logger();
                        }
                    }
                }
                return _logger;
            }
        }
        private Logger()
        {
            if (!Directory.Exists(Path.GetDirectoryName(_logFilePath)))
            {
                var Dirinfo = Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));
            }

            _messageQueue = new Queue<LogMessage>();

            logWriterThread = new Thread(new ThreadStart(_FlushLogsToFile))
            {
                IsBackground = true
            };
            logWriterThread.Start();
        }
        private void _FlushLogsToFile()
        {
            while (true)
            {
                if (inDeepSleep)
                {
                    autoResetEvent.WaitOne();
                }
                else
                {
                    autoResetEvent.WaitOne(writeInterval, true);
                }

                if (_messageQueue.Count > 0)
                {
                    List<LogMessage> messages;
                    lock (_messageQueueLocker)
                    {
                        messages = _messageQueue.ToList();
                        _messageQueue.Clear();
                    }
                    _writeLogs(messages);
                }
                else
                {
                    inDeepSleep = true;
                }
            }
        }
        private void _writeLogs(List<LogMessage> messages)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(_logFilePath, true))
                {
                    foreach (LogMessage message in messages)
                    {
                        file.WriteLine($"[{message.Level.ToString()}]\t{message.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}\t{message.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    // try to write notification to other file
                    string newpath = AppDomain.CurrentDomain.BaseDirectory.PathCombine("logger-error" + Guid.NewGuid().ToString().Substring(8) + ".txt");
                    File.AppendAllText(newpath, $"Exception writing {messages.Count} enteries, ex={ex.ToString()}");
                }
                catch
                {
                    // or other destination
                    System.Diagnostics.Debug.WriteLine($"Exception writing {messages.Count} enteries, ex={ex.ToString()}");
                }
            }
        }
        private void AddLogToQueue(LogMessage message)
        {
            lock (_messageQueueLocker)
            {
                _messageQueue.Enqueue(message);
            }

            if (inDeepSleep || (message.Level & (LogLevel.Critical | LogLevel.Error)) != 0)
            {
                // if it is urgent log, release logWriterThread to flush all collected logs to the file now
                inDeepSleep = false;
                autoResetEvent.Set();
            }
        }
        public void WriteLog(string message, Exception ex, LogLevel level, params object[] parameters)
        {
            AddLogToQueue(new LogMessage()
            {
                Message = message,
                Level = level,
                TimeStamp = DateTime.Now,
                Exception = ex,
                Parameters = parameters
            });
        }
        public void WriteLog(string message, LogLevel level, params object[] parameters)
        {
            AddLogToQueue(new LogMessage()
            {
                Message = message,
                Level = level,
                TimeStamp = DateTime.Now,
                Exception = null,
                Parameters = parameters
            });
        }
    }
}
