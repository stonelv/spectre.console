#!/bin/bash
echo "Running CodeEditor Functional Tests..."
echo ""

dotnet run --project tests/CodeEditorTest/CodeEditorTest.csproj < /dev/null
echo ""
echo "Test completed with exit code: $?