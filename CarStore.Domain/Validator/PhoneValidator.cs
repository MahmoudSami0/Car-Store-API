using System.Text.RegularExpressions;

namespace CarStore.Domain.Helpers;

public class PhoneValidator
{
    public static bool IsValid(string phone)
    {
        return !string.IsNullOrEmpty(phone) &&
               Regex.IsMatch(phone, @"^01[0125]\d{8}$");
    }
}