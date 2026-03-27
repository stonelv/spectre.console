using System.Collections.Generic;

namespace SysMon.Services
{
    public class ProcessInfo
    {
        public string Name { get; set; } = string.Empty;
        public int Pid { get; set; }
        public double MemoryUsage { get; set; }
    }

    public static class ProcessService
    {
        public static List<ProcessInfo> GetProcesses()
        {
            // 模拟进程数据
            return new List<ProcessInfo>
            {
                new ProcessInfo { Name = "Google Chrome", Pid = 1234, MemoryUsage = 1250.5 },
                new ProcessInfo { Name = "Visual Studio Code", Pid = 5678, MemoryUsage = 890.2 },
                new ProcessInfo { Name = "Terminal", Pid = 9012, MemoryUsage = 150.8 },
                new ProcessInfo { Name = "Spotify", Pid = 3456, MemoryUsage = 320.1 },
                new ProcessInfo { Name = "Slack", Pid = 7890, MemoryUsage = 450.3 },
                new ProcessInfo { Name = "Finder", Pid = 2345, MemoryUsage = 280.7 },
                new ProcessInfo { Name = "Docker", Pid = 6789, MemoryUsage = 620.4 },
                new ProcessInfo { Name = "SystemUIServer", Pid = 1011, MemoryUsage = 180.9 },
                new ProcessInfo { Name = "iTerm2", Pid = 1213, MemoryUsage = 210.6 },
                new ProcessInfo { Name = "Activity Monitor", Pid = 1415, MemoryUsage = 190.3 }
            };
        }
    }
}
