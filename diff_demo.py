#!/usr/bin/env python3

old_text = """Hello World
This is the original text
Line 3
Line 4
Line 5
Some important content
End of file"""

new_text = """Hello World
This is the modified text
Line 3
A new line inserted here
Line 5
Some updated content
Another new line
End of file"""

print("=== Original Text (old.txt) ===")
print(old_text)
print()

print("=== Modified Text (new.txt) ===")
print(new_text)
print()

print("=== Expected Diff Output (Inline Mode) ===")
print("  Hello World")
print("- This is the original text")
print("+ This is the modified text")
print("  Line 3")
print("+ A new line inserted here")
print("- Line 4")
print("  Line 5")
print("- Some important content")
print("+ Some updated content")
print("+ Another new line")
print("  End of file")
print()

print("=== Expected Diff Output (Side-by-Side Mode) ===")
print("1 Hello World              1 Hello World")
print("2 This is the original text 2 This is the modified text")
print("3 Line 3                    3 Line 3")
print("4 Line 4                    4 A new line inserted here")
print("5 Line 5                    5 Line 5")
print("6 Some important content    6 Some updated content")
print("                           7 Another new line")
print("7 End of file               8 End of file")
