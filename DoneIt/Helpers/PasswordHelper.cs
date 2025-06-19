using System.Security.Cryptography;
using System.Text;

namespace DoneIt.Helpers
{
    public static class PasswordHelper
    {
        // Acá se crea el hash + el salt de las contraseñas.
        public static string HashPassword(string password)
        {
            // Genera un salt de 16 bytes aleatorios
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            // Genera el hash usando PBKDF2 (seguro)
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32); // 256 bits

            // Combina salt + hash en un solo string (base64)
            byte[] hashBytes = new byte[48]; // 16 salt + 32 hash
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }

        // Verifica si una contraseña coincide con el hash guardado
        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Separa el salt del hash
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Calcula el hash con el mismo salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            // Compara el hash generado con el guardado
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }
    }
}

