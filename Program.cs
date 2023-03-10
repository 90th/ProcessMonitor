using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ProcessMonitor {
    internal class Program {
        static void Main(string[] args) {
            Console.Title = "Process Monitor - by 90th";
            Console.WriteLine("Process Monitor started...");
            Console.WindowWidth = 51;
            List<string> runningProcesses = Process.GetProcesses().Select(p => p.ProcessName).ToList();

            // Create a thread for the first foreach loop
            Thread t1 = new Thread(() => {
                while (true) {
                    Process[] processes = Process.GetProcesses(Environment.MachineName);

                    foreach (Process process in processes) {
                        string processName = process.ProcessName;
                        int processId = process.Id;

                        if (!runningProcesses.Contains(processName)) {
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("+");
                            Console.ResetColor();
                            Console.WriteLine("] -> {0} [{1}] Started.", processName, processId);
                            runningProcesses.Add(processName);
                        }
                    }

                    // Delay for 1 second before next iteration
                    Thread.Sleep(1000);
                }
            });

            // Create a thread for the second foreach loop
            Thread t2 = new Thread(() => {
                while (true) {
                    foreach (string processName in runningProcesses.ToList()) {
                        Process[] matchingProcesses = Process.GetProcessesByName(processName);

                        if (matchingProcesses.Length == 0) {
                            Console.Write("[");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("-");
                            Console.ResetColor();
                            Console.WriteLine("] -> {0} Stopped.", processName);
                            runningProcesses.Remove(processName);
                        }
                    }

                    // Delay for 1 second before next iteration
                    Thread.Sleep(1000);
                }
            });

            // Start the threads
            t1.Start();
            t2.Start();

            // Update the console title with CPU and memory usage every second
            while (true) {
                double cpuUsage = Process.GetCurrentProcess().TotalProcessorTime.TotalSeconds;
                double memUsage = Process.GetCurrentProcess().PrivateMemorySize64 / 1024.0 / 1024.0;
                Console.Title = string.Format("Process Monitor - Time: {0:F2}s MEM: {1:F2}%", cpuUsage, memUsage);

                Thread.Sleep(1000);
            }
        }
    }
}
