using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        // Define the regular expression for floating-point numbers (length ≤ 6)
        string pattern = @"^[+-]?\d{0,4}(\.\d{1,2})?$";

        // Take input from the user
        Console.WriteLine("Enter a floating-point number (length ≤ 6):");
        string input = Console.ReadLine();

        // Create a Regex object
        Regex regex = new Regex(pattern);

        // Check if the input matches the pattern
        if (regex.IsMatch(input))
        {
            Console.WriteLine("Valid floating-point number.");
        }
        else
        {
            Console.WriteLine("Invalid floating-point number.");
        }
    }
}
