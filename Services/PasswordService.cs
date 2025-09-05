using System.Security.Cryptography;

namespace ReservasSalas.Services
{
    public static class PasswordService
    {
        private const int SaltSize = 16; // 128 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 100_000;

        public static string HashPassword(string password)
        {
            using var rfc2898 = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256);
            var salt = rfc2898.Salt;
            var hash = rfc2898.GetBytes(HashSize);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string stored)
        {
            var parts = stored.Split(':');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            using var rfc2898 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var testHash = rfc2898.GetBytes(HashSize);

            return CryptographicOperations.FixedTimeEquals(hash, testHash);
        }
    }
}
