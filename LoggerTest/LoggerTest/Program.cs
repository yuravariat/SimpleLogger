using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleLogger;

namespace LoggerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    SimpleLogger.Logger.Instance.WriteLog("Test test 1 " + i, LogLevel.Trace);
                }
            });
            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    SimpleLogger.Logger.Instance.WriteLog("Test test 2 " + i, LogLevel.Error);
                }
            });
            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    SimpleLogger.Logger.Instance.WriteLog("Test test 3 " + i, LogLevel.Critical);
                }
            });

            Thread.Sleep(TimeSpan.FromMinutes(1));

            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    SimpleLogger.Logger.Instance.WriteLog("Test test 4 " + i, LogLevel.Critical);
                }
            });

            Console.ReadKey();

        }
    }
}
