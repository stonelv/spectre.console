namespace SysMon.Services;

/// <summary>
/// 进程服务
/// </summary>
public class ProcessService
{
    /// <summary>
    /// 进程信息类
    /// </summary>
    public class ProcessInfo
    {
        public string Name { get; set; } = string.Empty;
        public int Pid { get; set; }
        public long MemoryUsage { get; set; } // MB
    }

    /// <summary>
    /// 获取进程列表
    /// </summary>
    public List<ProcessInfo> GetProcesses()
    {
        var processes = new List<ProcessInfo>();
        var random = new Random();

        // 模拟10个进程数据
        var processNames = new[] {
            "Chrome", "Safari", "VSCode", "Terminal", "Finder",
            "Spotify", "Docker", "Slack", "Teams", "IntelliJ"
        };

        for (int i = 0; i < 10; i++)
        {
            processes.Add(new ProcessInfo
            {
                Name = processNames[i],
                Pid = random.Next(1000, 9999),
                MemoryUsage = random.Next(50, 500)
            });
        }

        return processes.OrderByDescending(p => p.MemoryUsage).ToList();
    }
}