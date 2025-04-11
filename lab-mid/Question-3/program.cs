using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SymbolTablePalindrome
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Symbol Table with Palindrome Detection");
            Console.WriteLine("=====================================");
            Console.WriteLine("Enter variable declarations one line at a time.");
            Console.WriteLine("Only variables with names containing palindrome substrings (≥ 3 chars) will be added.");
            Console.WriteLine("Enter 'exit' to quit or 'display' to show the current symbol table.\n");

            SymbolTable51 symbolTable = new SymbolTable51();
            int lineNumber = 1;

            while (true)
            {
                Console.Write($"[{lineNumber}] > ");
                string input = Console.ReadLine();

                if (input.ToLower() == "exit")
                    break;

                if (input.ToLower() == "display")
                {
                    symbolTable.DisplayTable();
                    continue;
                }

                symbolTable.ProcessLine(input, lineNumber);
                lineNumber++;
            }

            Console.WriteLine("\nFinal Symbol Table:");
            symbolTable.DisplayTable();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }

    class SymbolTable51
    {
        private List<SymbolEntry> entries;

        public SymbolTable51()
        {
            entries = new List<SymbolEntry>();
        }

        public void ProcessLine(string line, int lineNumber)
        {
            // Pattern to match: type variableName = value;
            string pattern = @"(\w+)\s+(\w+)\s*=\s*([^;]+);";
            Match match = Regex.Match(line, pattern);

            if (match.Success)
            {
                string type = match.Groups[1].Value;
                string varName = match.Groups[2].Value;
                string value = match.Groups[3].Value.Trim();

                if (ContainsPalindromeSubstring(varName, 3))
                {
                    entries.Add(new SymbolEntry
                    {
                        VariableName = varName,
                        Type = type,
                        Value = value,
                        LineNumber = lineNumber
                    });

                    Console.WriteLine($"Added: {varName} (contains palindrome)");
                }
                else
                {
                    Console.WriteLine($"Skipped: {varName} (no palindrome ≥ 3 chars found)");
                }
            }
            else
            {
                Console.WriteLine("Invalid syntax. Expected format: type variableName = value;");
            }
        }

        public bool ContainsPalindromeSubstring(string text, int minLength)
        {
            // Check every possible substring of length >= minLength
            for (int length = minLength; length <= text.Length; length++)
            {
                for (int start = 0; start <= text.Length - length; start++)
                {
                    string substring = text.Substring(start, length);
                    if (IsPalindrome(substring))
                    {
                        Console.WriteLine($"Found palindrome: '{substring}' in '{text}'");
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsPalindrome(string text)
        {
            int left = 0;
            int right = text.Length - 1;

            while (left < right)
            {
                if (text[left] != text[right])
                    return false;
                left++;
                right--;
            }
            return true;
        }

        public void DisplayTable()
        {
            if (entries.Count == 0)
            {
                Console.WriteLine("\nSymbol Table is empty.");
                return;
            }

            Console.WriteLine("\nSymbol Table:");
            Console.WriteLine("=============");

            // Calculate column widths
            int nameWidth = Math.Max(entries.Max(e => e.VariableName.Length), "Variable Name".Length) + 2;
            int typeWidth = Math.Max(entries.Max(e => e.Type.Length), "Type".Length) + 2;
            int valueWidth = Math.Max(entries.Max(e => e.Value.Length), "Value".Length) + 2;
            int lineWidth = "Line".Length + 2;

            // Print header
            string headerFormat = $"{{0,-{nameWidth}}}{{1,-{typeWidth}}}{{2,-{valueWidth}}}{{3,{lineWidth}}}";
            Console.WriteLine(string.Format(headerFormat, "Variable Name", "Type", "Value", "Line"));
            Console.WriteLine(new string('-', nameWidth + typeWidth + valueWidth + lineWidth));

            // Print rows
            string rowFormat = $"{{0,-{nameWidth}}}{{1,-{typeWidth}}}{{2,-{valueWidth}}}{{3,{lineWidth}}}";
            foreach (var entry in entries)
            {
                Console.WriteLine(string.Format(rowFormat,
                    entry.VariableName,
                    entry.Type,
                    entry.Value,
                    entry.LineNumber));
            }
        }
    }

    class SymbolEntry
    {
        public string VariableName { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int LineNumber { get; set; }
    }
}