using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CompleteStringProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Complete String Processor");
            Console.WriteLine("=========================");

            // The input string with specific format
            string inputString = "x:5; y:1; z:{userinput}; result: x * y + z;";
            Console.WriteLine($"Processing string: \"{inputString}\"");

            // Process the string
            ProcessString51(inputString);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static void ProcessString51(string input)
        {
            try
            {
                // Dictionary to store variables and their values
                var variables = new Dictionary<string, double>();

                // Extract variable declarations using regex
                string pattern = @"(\w+):([^;]+);";
                var matches = Regex.Matches(input, pattern);

                foreach (Match match in matches)
                {
                    string varName = match.Groups[1].Value.Trim();

                    // Skip "result" as it's not a variable
                    if (varName.ToLower() == "result")
                        continue;

                    string varValue = match.Groups[2].Value.Trim();

                    // Check if this variable needs user input
                    if (varValue == "{userinput}")
                    {
                        Console.Write($"Enter value for {varName}: ");
                        string userInput = Console.ReadLine();

                        if (double.TryParse(userInput, out double userValue))
                        {
                            variables[varName] = userValue;
                        }
                        else
                        {
                            Console.WriteLine($"Error: Invalid input for {varName}.");
                            return;
                        }
                    }
                    else if (double.TryParse(varValue, out double value))
                    {
                        variables[varName] = value;
                    }
                    else
                    {
                        Console.WriteLine($"Error: Invalid value for {varName}: {varValue}");
                        return;
                    }

                    // Display the variable and its value
                    Console.WriteLine($"{varName} = {variables[varName]}");
                }

                // Extract the result expression
                Match resultMatch = Regex.Match(input, @"result:\s*([^;]+);");
                if (!resultMatch.Success)
                {
                    Console.WriteLine("Error: Result expression not found in the input string.");
                    return;
                }

                string resultExpression = resultMatch.Groups[1].Value.Trim();

                // Evaluate the expression
                double result = EvaluateExpression(resultExpression, variables);

                // Display the result
                Console.WriteLine($"Result = {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing the string: {ex.Message}");
            }
        }

        static double EvaluateExpression(string expression, Dictionary<string, double> variables)
        {
            // Clone the expression for modification
            string modifiedExpression = expression;

            // Replace variable names with their values
            foreach (var variable in variables)
            {
                modifiedExpression = Regex.Replace(modifiedExpression,
                    $"\\b{variable.Key}\\b",
                    variable.Value.ToString());
            }

            // Parse the expression with operations in correct order
            try
            {
                // Handle multiplication first
                string[] addParts = modifiedExpression.Split('+');
                double total = 0;

                foreach (string addPart in addParts)
                {
                    string trimmedPart = addPart.Trim();

                    if (trimmedPart.Contains("*"))
                    {
                        string[] mulParts = trimmedPart.Split('*');
                        double product = 1;

                        foreach (string part in mulParts)
                        {
                            product *= double.Parse(part.Trim());
                        }

                        total += product;
                    }
                    else
                    {
                        total += double.Parse(trimmedPart);
                    }
                }

                return total;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Failed to evaluate expression: {modifiedExpression}");
                Console.WriteLine($"Original expression: {expression}");
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}