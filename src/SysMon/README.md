# SysMon - 系统监控命令行工具

一个使用 Spectre.Console 开发的命令行系统监控工具。

## 功能特性

1. **📊 系统信息** - 显示CPU核心数、内存使用情况和操作系统信息
2. **🧵 进程列表** - 以表格形式展示进程名称、PID和内存占用
3. **⏳ 进度演示** - 展示5个并发任务的进度条动画
4. **❓ 帮助** - 显示操作指南
5. **🚪 退出** - 安全退出程序

## 技术栈

- .NET 8
- Spectre.Console

## 构建方法

### 前提条件

- 安装 .NET 8 SDK

### 构建命令

```bash
# 恢复依赖
cd /path/to/SysMon
dotnet restore

# 构建项目
dotnet build -c Release
```

## 运行方法

### 方法1：直接运行

```bash
cd /path/to/SysMon
dotnet run
```

### 方法2：运行编译后的程序

```bash
# Windows
cd src/SysMon/bin/Release/net8.0
SysMon.exe

# macOS/Linux
cd src/SysMon/bin/Release/net8.0
./SysMon
```

## 操作说明

- 使用方向键上下选择菜单选项
- 按回车键确认选择
- 查看完信息后按任意键返回主菜单
- 退出时会显示确认对话框

## 项目结构

```
SysMon/
├── Program.cs          # 入口文件
├── Views/              # 视图层
│   ├── MainMenu.cs     # 主菜单
│   ├── SystemInfoView.cs # 系统信息视图
│   ├── ProcessListView.cs # 进程列表视图
│   ├── ProgressDemoView.cs # 进度演示视图
│   └── HelpView.cs     # 帮助视图
├── Services/           # 服务层
│   ├── SystemInfoService.cs # 系统信息服务
│   └── ProcessService.cs # 进程服务
└── Utils/              # 工具类
    └── ConsoleUtils.cs # 控制台工具类
```

## 许可证

MIT