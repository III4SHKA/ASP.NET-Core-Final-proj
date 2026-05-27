using System.Security.Cryptography;
using System.Text;

namespace FinalProject.Services;

public class SimpleHashService : IHashService
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public string GenerateSalt()
    {
        var random = new Random();
        var length = random.Next(24, 41);
        var saltBuilder = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            saltBuilder.Append(Chars[random.Next(Chars.Length)]);
        }

        return saltBuilder.ToString();
    }

    public string BuildHash(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password + salt);
        var hashBytes = sha256.ComputeHash(bytes);
        var hashBuilder = new StringBuilder();

        foreach (var b in hashBytes)
        {
            hashBuilder.Append(b.ToString("x2"));
        }

        return hashBuilder.ToString();
    }

    public bool Verify(string password, string salt, string savedHash)
    {
        var generatedHash = BuildHash(password, salt);
        return generatedHash == savedHash;
    }
}
