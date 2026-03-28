# NoteCLI - 命令行笔记管理器

基于 Spectre.Console 开发的功能强大的命令行笔记管理器。

## 功能特性

- 📝 **笔记列表**: 用 Table 展示笔记，支持按分类和标签过滤
- ✏️ **新增笔记**: 输入标题、内容、分类和多个标签
- ✏️ **编辑/删除**: 修改现有笔记或删除不需要的笔记
- 🔍 **搜索功能**: 按关键词和标签组合搜索，高亮匹配内容
- 📤 **导出功能**: 导出为 Markdown 或 JSON 格式，显示导出进度
- ❓ **帮助中心**: 详细的使用说明和示例
- 🚪 **安全退出**: 退出前确认，防止误操作

## 技术栈

- .NET 6
- Spectre.Console
- System.Text.Json

## 运行方法

### 1. 克隆项目

```bash
git clone <repository-url>
cd spectre.console/NoteCLI
```

### 2. 安装依赖

```bash
dotnet restore
```

### 3. 运行程序

```bash
dotnet run
```

## 使用说明

### 主菜单

使用上下箭头导航菜单，回车选择操作：

- 📝 笔记列表 - 查看所有笔记
- ✏️ 新增笔记 - 创建新笔记
- ✏️ 编辑/删除 - 修改或删除笔记
- 🔍 搜索 - 搜索笔记
- 📤 导出 - 导出笔记
- ❓ 帮助 - 查看帮助
- 🚪 退出 - 退出程序

### 笔记列表

- 支持按分类和标签过滤
- 分页显示，每页 10 条笔记
- 显示笔记的基本信息

### 新增/编辑笔记

- **标题**: 必填项，不能为空
- **内容**: 笔记的详细内容
- **分类**: 从预设分类中选择
- **标签**: 可多选，使用空格选择/取消

### 搜索功能

- 输入关键词，支持模糊匹配标题和内容
- 选择标签进行精确匹配
- 匹配结果用黄色高亮显示

### 导出功能

- 选择导出格式：Markdown 或 JSON
- 自定义文件名
- 显示导出进度条
- 导出文件保存在当前目录

### 帮助中心

查看详细的使用说明和快捷键提示。

## 项目结构

```
NoteCLI/
├── Models/          # 数据模型
│   └── Note.cs
├── Services/        # 服务层
│   ├── NoteStorageService.cs
│   └── NoteDataGenerator.cs
├── Views/           # 视图层
│   ├── MainMenu.cs
│   ├── NoteListView.cs
│   ├── NoteEditView.cs
│   ├── SearchView.cs
│   ├── ExportView.cs
│   └── HelpView.cs
├── Utils/           # 工具类
│   └── ConsoleUtils.cs
├── Program.cs       # 主程序入口
├── NoteCLI.csproj   # 项目配置
└── README.md        # 项目文档
```

## 数据存储

笔记数据存储在当前目录的 `notes.json` 文件中，使用 JSON 格式保存。

## 许可证

MIT