namespace Persistence.Constants;

public static class MaxLengthConstraints
{
    public const int Username = 64;
    public const int PasswordHash = (64 + 32) * 2 + 1; // hash + salt + divider, hash and salt will be in hex format, it's a reason for *2
    public const int FileName = 256;
    public const int ApiKeyName = 64;
    public const int ApiKeyToken = 256;
}