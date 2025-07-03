using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistence.Enums;
using Persistence.Utilities;

namespace Persistence.Converters;

public sealed class PermissionConversion : ValueConverter<Permission, string>
{
    public PermissionConversion() : base(permission => permission.GetString(), value => value.GetPermissionEnum(true))
    {
    }
}