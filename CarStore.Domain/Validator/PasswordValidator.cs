using System.Text.RegularExpressions;

namespace CarStore.Domain.Helpers;

public class PasswordValidator
{
    public static bool IsValid(string password)
    {
        return !string.IsNullOrEmpty(password) &&
               Regex.IsMatch(password, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{8,}$");
    }
}