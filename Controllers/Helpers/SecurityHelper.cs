using System.Security.Cryptography;
using System.Text;

public static class SecurityHelper
{
    // Generar un salt aleatorio
    public static string GenerateSalt()
    {
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    // Hashea la contraseña usando SHA256 + salt
    public static string HashPassword(string password, string salt)
    {
        var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hash = sha256.ComputeHash(combined);
        return Convert.ToBase64String(hash);
    }

    // Verifica si la contraseña ingresada es correcta
    public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
    {
        string hashOfEntered = HashPassword(enteredPassword, storedSalt);
        return hashOfEntered == storedHash;
    }
}
