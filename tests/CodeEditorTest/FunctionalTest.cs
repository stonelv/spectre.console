using Spectre.Console;
using System;

namespace CodeEditorTest;

public static class FunctionalTest
{
    public static void Run()
    {
        System.Console.WriteLine("=== CodeEditor Functional Test ===\n");
        
        bool allPassed = true;
        
        // Test 1: Basic text input
        try
        {
            var editor = new CodeEditor();
            editor.SetText("");
            foreach (var c in "Hello World")
            {
                var key = new ConsoleKeyInfo(c, ConsoleKey.A, false, false, false);
                editor.HandleKey(key);
            }
            if (editor.GetText() == "Hello World")
            {
                System.Console.WriteLine("✓ Test 1: Basic text input PASSED");
            }
            else
            {
                System.Console.WriteLine("✗ Test 1: Basic text input FAILED");
                allPassed = false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("✗ Test 1: Basic text input FAILED with exception: " + ex.Message);
            allPassed = false;
        }
        
        // Test 2: Backspace
        try
        {
            var editor = new CodeEditor();
            editor.SetText("Hello");
            var endKey = new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false);
            editor.HandleKey(endKey);
            var backspaceKey = new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false);
            editor.HandleKey(backspaceKey);
            editor.HandleKey(backspaceKey);
            if (editor.GetText() == "Hel")
            {
                System.Console.WriteLine("✓ Test 2: Backspace PASSED");
            }
            else
            {
                System.Console.WriteLine("✗ Test 2: Backspace FAILED");
                allPassed = false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("✗ Test 2: Backspace FAILED with exception: " + ex.Message);
            allPassed = false;
        }
        
        // Test 3: Delete
        try
        {
            var editor = new CodeEditor();
            editor.SetText("Hello");
            var deleteKey = new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false);
            editor.HandleKey(deleteKey);
            editor.HandleKey(deleteKey);
            if (editor.GetText() == "llo")
            {
                System.Console.WriteLine("✓ Test 3: Delete PASSED");
            }
            else
            {
                System.Console.WriteLine("✗ Test 3: Delete FAILED");
                allPassed = false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("✗ Test 3: Delete FAILED with exception: " + ex.Message);
            allPassed = false;
        }
        
        // Test 4: Enter (new line)
        try
        {
            var editor = new CodeEditor();
            editor.SetText("Hello World");
            
            // Move to position 5
            for (int i = 0; i < 5; i++)
            {
                var rightKey = new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false);
                editor.HandleKey(rightKey);
            }
            
            var enterKey = new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false);
            editor.HandleKey(enterKey);
            
            if (editor.GetText() == "Hello\n World")
            {
                System.Console.WriteLine("✓ Test 4: Enter (new line) PASSED");
            }
            else
            {
                System.Console.WriteLine("✗ Test 4: Enter (new line) FAILED - Got: " + editor.GetText());
                allPassed = false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("✗ Test 4: Enter (new line) FAILED with exception: " + ex.Message);
            allPassed = false;
        }
        
        // Test 5: Undo
        try
        {
            var editor = new CodeEditor();
            editor.SetText("");
            // Type one character at a time to create undo steps
            foreach (var c in "Hello")
            {
                var key = new ConsoleKeyInfo(c, ConsoleKey.A, false, false, false);
                editor.HandleKey(key);
            }
            // Undo once
            var undoKey = new ConsoleKeyInfo('\0', ConsoleKey.Z, false, false, true);
            editor.HandleKey(undoKey);
            // After undoing "Hello", we should have empty string (all typed in one step)
            if (editor.GetText() == "")
            {
                System.Console.WriteLine("✓ Test 5: Undo (Ctrl+Z) PASSED");
            }
            else
            {
                System.Console.WriteLine("✗ Test 5: Undo (Ctrl+Z) FAILED - Got: " + editor.GetText());
                allPassed = false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("✗ Test 5: Undo (Ctrl+Z) FAILED with exception: " + ex.Message);
            allPassed = false;
        }
        
        // Test 6: Redo
        try
        {
            var editor = new CodeEditor();
            editor.SetText("");
            foreach (var c in "Hello")
            {
                var key = new ConsoleKeyInfo(c, ConsoleKey.A, false, false, false);
                editor.HandleKey(key);
            }
            var undoKey = new ConsoleKeyInfo('\0', ConsoleKey.Z, false, false, true);
            editor.HandleKey(undoKey);
            var redoKey = new ConsoleKeyInfo('\0', ConsoleKey.Y, false, false, true);
            editor.HandleKey(redoKey);
            if (editor.GetText() == "Hello")
            {
                System.Console.WriteLine("✓ Test 6: Redo (Ctrl+Y) PASSED");
            }
            else
            {
                System.Console.WriteLine("✗ Test 6: Redo (Ctrl+Y) FAILED - Got: " + editor.GetText());
                allPassed = false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("✗ Test 6: Redo (Ctrl+Y) FAILED with exception: " + ex.Message);
            allPassed = false;
        }
        
        // Test 7: Undo limit (10 steps)
        try
        {
            var editor = new CodeEditor();
            editor.SetText("");
            for (int i = 0; i < 15; i++)
            {
                var key = new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false);
                editor.HandleKey(key);
            }
            var undoKey = new ConsoleKeyInfo('\0', ConsoleKey.Z, false, false, true);
            for (int i = 0; i < 15; i++)
            {
                editor.HandleKey(undoKey);
            }
            var result = editor.GetText();
            if (result.Length == 5)
            {
                System.Console.WriteLine("✓ Test 7: Undo limit (10 steps) PASSED");
            }
            else
            {
                System.Console.WriteLine("✗ Test 7: Undo limit (10 steps) FAILED - Expected 5 chars, got " + result.Length);
                allPassed = false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("✗ Test 7: Undo limit (10 steps) FAILED with exception: " + ex.Message);
            allPassed = false;
        }
        
        // Test 8: Arrow navigation
        try
        {
            var editor = new CodeEditor();
            editor.SetText("Hello");
            var rightKey = new ConsoleKeyInfo('\0', ConsoleKey.RightArrow, false, false, false);
            for (int i = 0; i < 5; i++)
            {
                editor.HandleKey(rightKey);
            }
            var leftKey = new ConsoleKeyInfo('\0', ConsoleKey.LeftArrow, false, false, false);
            for (int i = 0; i < 3; i++)
            {
                editor.HandleKey(leftKey);
            }
            var xKey = new ConsoleKeyInfo('X', ConsoleKey.X, false, false, false);
            editor.HandleKey(xKey);
            if (editor.GetText() == "HeXlo")
            {
                System.Console.WriteLine("✓ Test 8: Arrow navigation PASSED");
            }
            else
            {
                System.Console.WriteLine("✗ Test 8: Arrow navigation FAILED - Got: " + editor.GetText());
                allPassed = false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("✗ Test 8: Arrow navigation FAILED with exception: " + ex.Message);
            allPassed = false;
        }
        
        // Test 9: Home and End
        try
        {
            var editor = new CodeEditor();
            editor.SetText("Hello World");
            var endKey = new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false);
            editor.HandleKey(endKey);
            var exclamationKey = new ConsoleKeyInfo('!', ConsoleKey.Oem1, false, false, false);
            editor.HandleKey(exclamationKey);
            var homeKey = new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false);
            editor.HandleKey(homeKey);
            var gtKey = new ConsoleKeyInfo('>', ConsoleKey.OemPeriod, false, false, false);
            editor.HandleKey(gtKey);
            if (editor.GetText() == ">Hello World!")
            {
                System.Console.WriteLine("✓ Test 9: Home and End PASSED");
            }
            else
            {
                System.Console.WriteLine("✗ Test 9: Home and End FAILED - Got: " + editor.GetText());
                allPassed = false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("✗ Test 9: Home and End FAILED with exception: " + ex.Message);
            allPassed = false;
        }
        
        // Test 10: Multi-line paste simulation
        try
        {
            var editor = new CodeEditor();
            editor.SetText("First line");
            
            // Simulate paste by setting text directly (clipboard would do this)
            editor.SetText("Line 1\nLine 2\nLine 3");
            
            var lines = editor.GetText().Split('\n');
            if (lines.Length == 3 && lines[0] == "Line 1" && lines[1] == "Line 2" && lines[2] == "Line 3")
            {
                System.Console.WriteLine("✓ Test 10: Multi-line support PASSED");
            }
            else
            {
                System.Console.WriteLine("✗ Test 10: Multi-line support FAILED");
                allPassed = false;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("✗ Test 10: Multi-line support FAILED with exception: " + ex.Message);
            allPassed = false;
        }
        
        System.Console.WriteLine();
        if (allPassed)
        {
            System.Console.WriteLine("=== All tests PASSED! ===");
        }
        else
        {
            System.Console.WriteLine("=== Some tests FAILED ===");
        }
    }
}