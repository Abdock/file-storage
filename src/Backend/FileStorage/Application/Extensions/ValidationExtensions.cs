using System.Text.RegularExpressions;

namespace Application.Extensions;

internal static partial class ValidationExtensions
{
    public static bool IsValidEmail(this string email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex().IsMatch(email);
    }

    public static bool IsStrongPassword(this string password)
    {
        return !string.IsNullOrWhiteSpace(password) && PasswordRegex().IsMatch(password);
    }

    [GeneratedRegex(
        """^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$""",
        RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex EmailRegex();

    [GeneratedRegex("""^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#@'\"\-=!$%^&*\(\)\[\]\{\}`\\|\/\?><;:,])[A-Za-z\d#@'\"\-=!$%^&*\(\)\[\]\{\}`\\|\/\?><;:,]{8,}$""")]
    private static partial Regex PasswordRegex();
}