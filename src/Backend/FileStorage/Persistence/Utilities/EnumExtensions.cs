using Persistence.Enums;

namespace Persistence.Utilities;

public static class EnumExtensions
{
    public static string GetString(this Permission permission)
    {
        return permission switch
        {
            Permission.Read => nameof(Permission.Read),
            Permission.Write => nameof(Permission.Write),
            Permission.Delete => nameof(Permission.Delete),
            _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, null)
        };
    }

    public static Permission GetPermissionEnum(this string permission, bool ignoreCase = false)
    {
        var caseComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (permission.Equals(nameof(Permission.Read), caseComparison))
        {
            return Permission.Read;
        }

        if (permission.Equals(nameof(Permission.Write), caseComparison))
        {
            return Permission.Write;
        }

        if (permission.Equals(nameof(Permission.Delete), caseComparison))
        {
            return Permission.Delete;
        }

        throw new ArgumentOutOfRangeException(nameof(permission), permission, null);
    }
}