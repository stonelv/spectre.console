namespace SysMon.Services;

/// <summary>
/// 系统信息服务
/// </summary>
public class SystemInfoService
{
    /// <summary>
    /// 获取CPU核心数
    /// </summary>
    public int GetCpuCores()
    {
        // 模拟数据，实际项目中可使用System.Environment.ProcessorCount
        return Environment.ProcessorCount;
    }

    /// <summary>
    /// 获取总内存（MB）
    /// </summary>
    public long GetTotalMemory()
    {
        // 模拟数据，实际项目中可使用系统API获取
        return 16384; // 16GB
    }

    /// <summary>
    /// 获取已用内存（MB）
    /// </summary>
    public long GetUsedMemory()
    {
        // 模拟数据，实际项目中可使用系统API获取
        var random = new Random();
        return random.Next(4096, 8192); // 4-8GB
    }

    /// <summary>
    /// 获取操作系统名称
    /// </summary>
    public string GetOsName()
    {
        // 模拟数据，实际项目中可使用System.Runtime.InteropServices.RuntimeInformation
        return "macOS Sonoma 14.5";
    }
}