using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestTaskScheduler
{
    class Program
    {
        static Thread _thMonitor = new Thread(ShowStats);
        static int _planned = 0;
        static int _stated = 0;
        static int _ended = 0;

        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(200, 200);
            _thMonitor.Start();
            _thMonitor.IsBackground = true;

            while (true)
            {
                for (int i = 0; i < 2000; i++)
                {
                    Task t = new Task(() =>
                    {
                        Interlocked.Increment(ref _stated);
                        Thread.Sleep(5000);
                        Interlocked.Increment(ref _ended);
                    });

                    t.Start();
                    Interlocked.Increment(ref _planned);
                    Thread.Sleep(10);
                }

                var key = Console.ReadKey();
                if (key.KeyChar == 'q')
                    break;
            }
        }

        private static void ShowStats()
        {
            int min = 0, ioMin;
            var process = Process.GetCurrentProcess();
            while (true)
            {
                process.Refresh();
                ThreadPool.GetAvailableThreads(out min, out ioMin);
                Console.CursorLeft = 0;
                Console.CursorTop = 0;

                Console.WriteLine($"Availbe Thread={min}, IO={ioMin}");
                Console.WriteLine($"Planned={_planned} Started={_stated}, Ended={_ended}");
                Console.WriteLine($"HandleCount={process.HandleCount} ThreadsCount={process.Threads.Count}");
                Console.WriteLine("Press 'q' to exit. Another key start work.");

                Thread.Sleep(100);
            }
        }
    }
}
