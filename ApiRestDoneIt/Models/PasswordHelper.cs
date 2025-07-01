using System.Security.Cryptography;
using System.Text;

public static class PasswordHelper
{
    public static string GenerateSalt(int size = 32)
    {
        var randomBytes = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public static string HashPassword(string password, string salt)
    {
        var combined = Encoding.UTF8.GetBytes(password + salt);
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(combined);
        return Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(string password, string salt, string storedHash)
    {
        var hashToCheck = HashPassword(password, salt);
        return hashToCheck == storedHash;
    }
}