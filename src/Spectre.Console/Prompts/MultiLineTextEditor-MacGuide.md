# 在Mac系统上使用多行代码编辑器

## 问题描述

在Mac系统中，标准的快捷键可能有所不同。特别是对于撤销操作，Mac系统通常使用Command+Z，而不是Ctrl+Z。

## 解决方案

我已经修改了多行代码编辑器，使其在Mac系统上支持以下快捷键：

### 撤销操作
- **Command+Z**: 在Mac系统上执行撤销操作
- **Control+Z**: 在Mac系统上也可以执行撤销操作（为了兼容性）

### 其他快捷键
- **Command+C**: 复制当前字符
- **Command+V**: 粘贴复制的字符
- **Command+Y**: 重做操作

## 使用方法

1. 运行测试程序：
   ```bash
   cd src/Spectre.Console.Samples
   dotnet run
   ```

2. 在编辑器中，您可以使用以下方法执行撤销操作：
   - 按`Command+Z`（Mac标准方式）
   - 按`Control+Z`（兼容方式）

## 注意事项

- 在Mac系统上，Command键和Control键都可以用于快捷键操作
- 如果Command+Z不工作，请尝试Control+Z
- 撤销操作最多支持10步
- 按ESC键退出编辑器

## 技术实现

修改的核心是添加了一个辅助方法`IsControlOrCommand`，它检测当前操作系统并相应地处理Control键和Command键：

```csharp
private static bool IsControlOrCommand(ConsoleKeyInfo keyInfo)
{
    if (OperatingSystem.IsMacOS())
    {
        // On Mac, we consider both Control and Command as valid for our shortcuts
        return keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control);
    }
    
    // On Windows/Linux, we only want Control key
    return keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control);
}
```

这样，无论用户使用Command键还是Control键，都能正确触发相应的操作。