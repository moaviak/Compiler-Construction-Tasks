using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string regexPattern = @"^(?=.*SP)(?=.*[A-Z])(?=.*[!@#$%^&*()_+{}|:""<>?~\-=\[\]\\;',./]{2})(?=.*[moavia]{4}).{0,12}$";
        Regex regex = new Regex(regexPattern);

        Console.WriteLine("Enter a string to validate:");
        string userInput = Console.ReadLine();

        bool isValid = regex.IsMatch(userInput);

        if (isValid)
        {
            Console.WriteLine($"\"{userInput}\" is valid.");
        }
        else
        {
            Console.WriteLine($"\"{userInput}\" is invalid.");
        }
    }
}
