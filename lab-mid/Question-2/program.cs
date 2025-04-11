
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace CC_LAB_MID
{
    internal class question2
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Mini-Language Variable Parser");
            Console.WriteLine("============================");
            Console.WriteLine("Enter code in the mini-language format (example: var a1 = 12@; float b2 = 3.14$$;):");

            string input = Console.ReadLine();

            // Process the input
            List<VariableInfo> variables = ExtractVariables(input);

            // Display results in table format
            DisplayVariableTable(variables);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static List<VariableInfo> ExtractVariables(string code)
        {
            List<VariableInfo> result = new List<VariableInfo>();

            // Regex pattern to match:
            // 1. Variables that start with a, b, or c
            // 2. End with digits
            // 3. Have non-alphanumeric characters in the value

            // Group 1: Type (var, float, etc.)
            // Group 2: Variable name starting with a, b, or c and ending with digits
            // Group 3: Value with special characters
            // Group 4: Special character(s) in the value
            string pattern = @"(var|float|int|string|double)\s+([abc]\w*\d+)\s*=\s*([^;]*?)([@#$%^&*!~`<>?]+)[^;]*?;";

            var matches = Regex.Matches(code, pattern);

            foreach (Match match in matches)
            {
                // Extract all special characters
                string value = match.Groups[3].Value + match.Groups[4].Value;
                string specialChars = new string(value.Where(c => !char.IsLetterOrDigit(c) && c != '.').ToArray());

                result.Add(new VariableInfo
                {
                    VarName = match.Groups[2].Value,
                    SpecialSymbol = specialChars,
                    TokenType = match.Groups[1].Value
                });
            }

            return result;
        }

        static void DisplayVariableTable(List<VariableInfo> variables)
        {
            if (variables.Count == 0)
            {
                Console.WriteLine("\nNo matching variables found.");
                return;
            }

            Console.WriteLine("\nMatching Variables Table:");
            Console.WriteLine("=======================");

            // Calculate column widths for nicely formatted table
            int nameWidth = Math.Max(variables.Max(v => v.VarName.Length), "VarName".Length) + 2;
            int symbolWidth = Math.Max(variables.Max(v => v.SpecialSymbol.Length), "SpecialSymbol".Length) + 2;
            int typeWidth = Math.Max(variables.Max(v => v.TokenType.Length), "Token Type".Length) + 2;

            // Print header
            string headerFormat = $"{{0,-{nameWidth}}}{{1,-{symbolWidth}}}{{2,-{typeWidth}}}";
            Console.WriteLine(string.Format(headerFormat, "VarName", "SpecialSymbol", "Token Type"));
            Console.WriteLine(new string('-', nameWidth + symbolWidth + typeWidth));

            // Print rows
            string rowFormat = $"{{0,-{nameWidth}}}{{1,-{symbolWidth}}}{{2,-{typeWidth}}}";
            foreach (var variable in variables)
            {
                Console.WriteLine(string.Format(rowFormat,
                    variable.VarName,
                    variable.SpecialSymbol,
                    variable.TokenType));
            }
        }
    }

    class VariableInfo
    {
        public string VarName { get; set; }
        public string SpecialSymbol { get; set; }
        public string TokenType { get; set; }
    }
}
