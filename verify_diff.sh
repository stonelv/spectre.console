#!/bin/bash

echo "=========================================="
echo "Diff 组件验证脚本"
echo "=========================================="
echo ""

# 检查文件是否存在
echo "1. 检查 Diff 组件文件..."
files=(
    "src/Spectre.Console/Widgets/Diff/Diff.cs"
    "src/Spectre.Console/Widgets/Diff/DiffAlgorithm.cs"
    "src/Spectre.Console/Widgets/Diff/DiffTypes.cs"
    "src/Spectre.Console/Widgets/Diff/DiffExtensions.cs"
    "src/Spectre.Console.Tests/Unit/Widgets/DiffTests.cs"
)

all_exist=true
for file in "${files[@]}"; do
    if [ -f "$file" ]; then
        echo "  ✓ $file"
    else
        echo "  ✗ $file (不存在)"
        all_exist=false
    fi
done

if [ "$all_exist" = true ]; then
    echo ""
    echo "  所有 Diff 组件文件都已创建！"
else
    echo ""
    echo "  警告：部分文件缺失"
fi

echo ""
echo "2. 检查测试文件..."
if [ -f "old.txt" ]; then
    echo "  ✓ old.txt"
else
    echo "  ✗ old.txt (不存在)"
fi

if [ -f "new.txt" ]; then
    echo "  ✓ new.txt"
else
    echo "  ✗ new.txt (不存在)"
fi

echo ""
echo "3. 显示测试文件内容..."
echo ""
echo "--- old.txt ---"
cat old.txt
echo ""
echo "--- new.txt ---"
cat new.txt

echo ""
echo "=========================================="
echo "验证命令（请在本地环境执行）："
echo "=========================================="
echo ""
echo "# 编译项目"
echo "dotnet build src/Spectre.Console/Spectre.Console.csproj"
echo ""
echo "# 运行单元测试"
echo "dotnet test src/Spectre.Console.Tests/Spectre.Console.Tests.csproj --filter \"FullyQualifiedName~DiffTests\""
echo ""
echo "# 查看详细验证说明"
echo "cat VERIFY_DIFF.md"
echo ""
