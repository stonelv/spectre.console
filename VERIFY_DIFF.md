# Diff 组件验证命令

由于当前环境存在文件权限问题，请在您的本地环境中执行以下命令来验证 Diff 组件：

## 1. 编译项目

```bash
# 清理并重新编译
dotnet clean src/Spectre.Console/Spectre.Console.csproj
dotnet build src/Spectre.Console/Spectre.Console.csproj
```

## 2. 运行单元测试

```bash
# 运行所有 Diff 相关的测试
dotnet test src/Spectre.Console.Tests/Spectre.Console.Tests.csproj --filter "FullyQualifiedName~DiffTests"

# 运行所有测试
dotnet test src/Spectre.Console.Tests/Spectre.Console.Tests.csproj
```

## 3. 手动验证测试文件

```bash
# 查看原始文本
cat old.txt

# 查看修改后的文本
cat new.txt

# 运行 Python 演示脚本（显示预期输出）
python3 diff_demo.py
```

## 4. 创建并运行测试程序

创建一个测试程序 `TestDiff.cs`：

```csharp
using Spectre.Console;
using System;

class Program
{
    static void Main()
    {
        var oldText = File.ReadAllText("old.txt");
        var newText = File.ReadAllText("new.txt");

        // 测试内联模式
        AnsiConsole.MarkupLine("[bold yellow]=== Inline Mode ===[/]");
        var inlineDiff = new Diff(oldText, newText)
            .Mode(DiffMode.Inline);
        AnsiConsole.Write(inlineDiff);

        AnsiConsole.WriteLine();

        // 测试并排模式
        AnsiConsole.MarkupLine("[bold yellow]=== Side-by-Side Mode ===[/]");
        var sideBySideDiff = new Diff(oldText, newText)
            .Mode(DiffMode.SideBySide);
        AnsiConsole.Write(sideBySideDiff);

        AnsiConsole.WriteLine();

        // 测试自定义颜色
        AnsiConsole.MarkupLine("[bold yellow]=== Custom Colors ===[/]");
        var customDiff = new Diff(oldText, newText)
            .Mode(DiffMode.Inline)
            .DeletedColor(Color.Yellow)
            .InsertedColor(Color.Cyan)
            .UnchangedColor(Color.Grey);
        AnsiConsole.Write(customDiff);
    }
}
```

然后创建项目文件 `TestDiff.csproj`：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="src/Spectre.Console/Spectre.Console.csproj" />
  </ItemGroup>
</Project>
```

运行测试程序：

```bash
dotnet run --project TestDiff.csproj
```

## 5. 验证功能点

### 5.1 验证差异识别
- 检查是否正确识别了插入的行（绿色）
- 检查是否正确识别了删除的行（红色）
- 检查是否正确识别了未更改的行（蓝色）

### 5.2 验证渲染模式
- **内联模式**：删除和插入的行应该按顺序显示
- **并排模式**：左右两列分别显示旧文本和新文本

### 5.3 验证颜色编码
- 删除的行应该是红色（或自定义颜色）
- 插入的行应该是绿色（或自定义颜色）
- 未更改的行应该是蓝色（或自定义颜色）

### 5.4 验证行号
- 并排模式下应该显示正确的行号
- 行号应该与源文件对应

## 6. 预期输出

### 内联模式预期输出：
```
  Hello World
- This is the original text
+ This is the modified text
  Line 3
+ A new line inserted here
- Line 4
  Line 5
- Some important content
+ Some updated content
+ Another new line
  End of file
```

### 并排模式预期输出：
```
1 Hello World              1 Hello World
2 This is the original text 2 This is the modified text
3 Line 3                    3 Line 3
4 Line 4                    4 A new line inserted here
5 Line 5                    5 Line 5
6 Some important content    6 Some updated content
                           7 Another new line
7 End of file               8 End of file
```

## 7. 故障排除

如果遇到编译错误：

```bash
# 检查 Diff 组件文件是否存在
ls -la src/Spectre.Console/Widgets/Diff/

# 检查文件内容
cat src/Spectre.Console/Widgets/Diff/Diff.cs
cat src/Spectre.Console/Widgets/Diff/DiffAlgorithm.cs
cat src/Spectre.Console/Widgets/Diff/DiffTypes.cs
cat src/Spectre.Console/Widgets/Diff/DiffExtensions.cs
```

## 8. 测试覆盖率

单元测试覆盖以下场景：
- ✅ 构造函数参数验证
- ✅ 内联模式渲染
- ✅ 并排模式渲染
- ✅ 扩展方法（颜色、样式、标记设置）
- ✅ 差异算法（插入、删除、未更改检测）
