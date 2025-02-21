using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        // Define the regular expression for logical operators
        string pattern = @"(\&\&|\|\||\!|\=\=|\!\=|\<|\>|\<\=|\>\=)";

        // Take input from the user
        Console.WriteLine("Enter a string to search for logical operators:");
        string input = Console.ReadLine();

        // Create a Regex object
        Regex regex = new Regex(pattern);

        // Find matches
        MatchCollection matches = regex.Matches(input);

        // Output the matches
        if (matches.Count > 0)
        {
            Console.WriteLine("Logical operators found:");
            foreach (Match match in matches)
            {
                Console.WriteLine(match.Value);
            }
        }
        else
        {
            Console.WriteLine("No logical operators found.");
        }
    }
}
