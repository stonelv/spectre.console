namespace SysMon.Services
{
    public class SystemInfo
    {
        public int CpuCores { get; set; }
        public double TotalMemory { get; set; }
        public double UsedMemory { get; set; }
        public double FreeMemory { get; set; }
        public string OSName { get; set; } = string.Empty;
    }

    public static class SystemInfoService
    {
        public static SystemInfo GetSystemInfo()
        {
            // 模拟系统信息数据
            return new SystemInfo
            {
                CpuCores = 8,
                TotalMemory = 16.0,
                UsedMemory = 8.5,
                FreeMemory = 7.5,
                OSName = "macOS Ventura 13.5"
            };
        }
    }
}
