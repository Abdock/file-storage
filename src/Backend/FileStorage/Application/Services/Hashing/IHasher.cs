namespace Application.Services.Hashing;

public interface IHasher
{
    string ComputeHash(string value);
    bool Verify(string password, string passwordHash);
}