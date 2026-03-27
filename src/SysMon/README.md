# SysMon - 系统监控工具

SysMon 是一个基于 Spectre.Console 开发的命令行系统监控工具,提供系统信息查看、进程列表展示和进度演示等功能。

## 功能特性

1. **系统信息**: 显示 CPU 核心数、内存使用情况和操作系统名称
2. **进程列表**: 以表格形式展示运行中的进程信息
3. **进度演示**: 展示并发任务的进度条动画
4. **帮助信息**: 详细的操作说明
5. **退出确认**: 安全退出提示

## 技术栈

- .NET 8.0
- Spectre.Console 0.49.1

## 项目结构

```
SysMon/
├── Program.cs          # 主程序入口
├── Views/              # 视图层
│   ├── MainMenu.cs     # 主菜单
│   ├── SystemInfoView.cs  # 系统信息视图
│   ├── ProcessListView.cs # 进程列表视图
│   ├── ProgressDemoView.cs # 进度演示视图
│   └── HelpView.cs     # 帮助视图
├── Services/           # 服务层
│   ├── SystemInfoService.cs # 系统信息服务
│   └── ProcessService.cs    # 进程服务
├── Utils/              # 工具类
│   └── ConsoleUtils.cs # 控制台工具类
└── SysMon.csproj       # 项目配置文件
```

## 构建与运行

### 构建项目

```bash
dotnet build
```

### 运行项目

```bash
dotnet run
```

### 发布项目

```bash
dotnet publish -c Release
```

## 使用说明

1. **主菜单**: 使用上下箭头键选择操作,按 Enter 键确认
2. **系统信息**: 查看系统硬件和操作系统信息
3. **进程列表**: 查看模拟的进程运行情况
4. **进度演示**: 观看并发任务的进度条动画
5. **帮助**: 查看详细的操作说明
6. **退出**: 确认后退出程序

## 界面设计

- **统一风格**: 所有界面使用蓝色(Cyan)主题色
- **组件使用**: Table、Panel、Rule、Progress、SelectionPrompt、Markup
- **交互友好**: 清晰的提示信息和操作指引

## 注意事项

- 本项目使用模拟数据,实际应用中需要替换为真实的系统信息获取代码
- 确保已安装 .NET 8.0 SDK
