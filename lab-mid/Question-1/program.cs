using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CC_LAB_MID
{
    internal class question1
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a string in the format: x:value; y:value; z:value; result: operation;");
            string input = Console.ReadLine();

            // Process the input string
            ProcessString51(input);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void ProcessString51(string input)
        {
            try
            {
                // Extract variable declarations using regex
                var variablePattern = new Regex(@"(\w+):([^;]+);");
                var resultPattern = new Regex(@"result:\s*([^;]+);");

                var variableMatches = variablePattern.Matches(input);
                var resultMatch = resultPattern.Match(input);

                if (!resultMatch.Success)
                {
                    Console.WriteLine("Error: Result operation not found in the input string.");
                    return;
                }

                // Create a dictionary to store variables and their values
                var variables = new System.Collections.Generic.Dictionary<string, double>();

                // Process each variable match
                foreach (Match match in variableMatches)
                {
                    string varName = match.Groups[1].Value.Trim();

                    // Skip the "result" part as it's not a variable
                    if (varName.ToLower() == "result")
                        continue;

                    string varValueStr = match.Groups[2].Value.Trim();

                    // If the value is "userinput", prompt the user to enter a value
                    if (varValueStr.ToLower() == "userinput")
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
                    // Otherwise, try to parse the value directly
                    else if (double.TryParse(varValueStr, out double value))
                    {
                        variables[varName] = value;
                    }
                    else
                    {
                        Console.WriteLine($"Error: Invalid value for {varName}: {varValueStr}");
                        return;
                    }

                    // Display the variable and its value
                    Console.WriteLine($"{varName} = {variables[varName]}");
                }

                // Get the result expression
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

        static double EvaluateExpression(string expression, System.Collections.Generic.Dictionary<string, double> variables)
        {
            // Replace variable names with their values
            string modifiedExpression = expression;
            foreach (var variable in variables)
            {
                modifiedExpression = Regex.Replace(modifiedExpression,
                    $"\\b{variable.Key}\\b",
                    variable.Value.ToString());
            }

            // Parse the expression part by part
            try
            {
                // Handle multiplication first
                string[] addParts = modifiedExpression.Split('+');
                double total = 0;

                foreach (string addPart in addParts)
                {
                    if (addPart.Contains("*"))
                    {
                        string[] mulParts = addPart.Split('*');
                        double product = 1;
                        foreach (string part in mulParts)
                        {
                            product *= double.Parse(part.Trim());
                        }
                        total += product;
                    }
                    else
                    {
                        total += double.Parse(addPart.Trim());
                    }
                }

                return total;
            }
            catch (FormatException)
            {
                Console.WriteLine($"Failed to evaluate expression: {modifiedExpression}");
                Console.WriteLine("Original expression: " + expression);
                throw;
            }
        }
    }
}
