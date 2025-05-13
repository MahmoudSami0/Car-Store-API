using System.Text.RegularExpressions;

namespace CarStore.Domain.Helpers;

public class EmailValidator
{
    public static bool IsValidEmail(string email)
    {
        return !string.IsNullOrEmpty(email) &&
               Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}