namespace Application.Services.Hashing;

public interface IHasher
{
    string ComputeHash(string value);
}