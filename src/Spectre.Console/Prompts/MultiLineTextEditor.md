# 多行代码编辑组件

## 概述

这是为 Spectre.Console 开发的一个多行代码编辑组件，支持以下功能：

- 标准的文本输入
- 退格删除和 Delete 删除
- 最多支持10步的撤销操作
- 剪切板的复制粘贴
- 快捷键操作（Ctrl+C, Ctrl+V）

## 使用方法

```csharp
using Spectre.Console;

var editor = new MultiLineTextEditor
{
    Title = "多行代码编辑器",
    Height = 12,
    Width = 80,
    Text = "初始文本"
};

var result = await editor.ShowAsync(AnsiConsole.Console);
Console.WriteLine($"编辑结果: {result}");
```

## 快捷键

### Windows/Linux
- **方向键**: 移动光标
- **Home/End**: 移动到行首/行尾
- **PageUp/PageDown**: 向上/向下滚动
- **Ctrl+Home/Ctrl+End**: 移动到文档开头/结尾
- **Backspace**: 删除光标左侧的字符
- **Delete**: 删除光标右侧的字符
- **Enter**: 插入新行
- **Ctrl+C**: 复制当前字符
- **Ctrl+V**: 粘贴复制的字符
- **Ctrl+Z**: 撤销操作
- **Ctrl+Y**: 重做操作
- **Tab**: 插入制表符
- **ESC**: 退出编辑器

### macOS
- **方向键**: 移动光标
- **Home/End**: 移动到行首/行尾
- **PageUp/PageDown**: 向上/向下滚动
- **Cmd+Home/Cmd+End**: 移动到文档开头/结尾
- **Backspace**: 删除光标左侧的字符
- **Delete**: 删除光标右侧的字符
- **Enter**: 插入新行
- **Cmd+C**: 复制当前字符
- **Cmd+V**: 粘贴复制的字符
- **Cmd+Z**: 撤销操作
- **Cmd+Y**: 重做操作
- **Tab**: 插入制表符
- **ESC**: 退出编辑器

**注意**: 在Mac系统上，Command键（Cmd）和Control键都可以用于快捷键操作。例如，您可以使用Cmd+Z或Ctrl+Z来执行撤销操作。

## 测试程序

要测试多行代码编辑组件，可以运行以下命令：

```bash
cd src/Spectre.Console.Samples
dotnet run
```

这将启动一个简单的测试程序，您可以在其中测试所有功能。