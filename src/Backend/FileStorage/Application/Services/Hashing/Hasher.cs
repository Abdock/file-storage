﻿using System.Security.Cryptography;

namespace Application.Services.Hashing;

public sealed class Hasher : IHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 64;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
    
    public string ComputeHash(string value)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(value, salt, Iterations, Algorithm, HashSize);
        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        var parts = passwordHash.Split('-');
        var hash = Convert.FromHexString(parts[0]);
        var salt = Convert.FromHexString(parts[^1]);
        var newHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }
}