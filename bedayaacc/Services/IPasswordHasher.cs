using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace bedayaacc.Services
{
    /// <summary>
    /// Service for hashing and verifying passwords
    /// </summary>
    public interface IPasswordHasher
    {
        (string hash, string salt) HashPassword(string password);
        bool VerifyPassword(string password, string hash, string salt);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 32; // 256 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 10000;

        /// <summary>
        /// Hash password with a random salt
        /// </summary>
        public (string hash, string salt) HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            // Generate random salt
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // Hash password with salt
            byte[] hashBytes = HashPasswordWithSalt(password, saltBytes);

            // Convert to base64 strings
            string hash = Convert.ToBase64String(hashBytes);
            string salt = Convert.ToBase64String(saltBytes);

            return (hash, salt);
        }

        /// <summary>
        /// Verify if password matches the hash
        /// </summary>
        public bool VerifyPassword(string password, string hash, string salt)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(hash) ||
                string.IsNullOrWhiteSpace(salt))
                return false;

            try
            {
                // Convert salt from base64
                byte[] saltBytes = Convert.FromBase64String(salt);

                // Hash the input password with the stored salt
                byte[] hashBytes = HashPasswordWithSalt(password, saltBytes);

                // Convert stored hash from base64
                byte[] storedHashBytes = Convert.FromBase64String(hash);

                // Compare hashes
                return CryptographicOperations.FixedTimeEquals(hashBytes, storedHashBytes);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Internal method to hash password with given salt
        /// </summary>
        private byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: HashSize
            );
        }
    }

    /// <summary>
    /// Alternative simpler password hasher using SHA256
    /// (Less secure but simpler implementation)
    /// </summary>
    public class SimplePasswordHasher : IPasswordHasher
    {
        public (string hash, string salt) HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            // Generate salt
            string salt = GenerateSalt();

            // Hash password with salt
            string hash = ComputeHash(password + salt);

            return (hash, salt);
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(hash) ||
                string.IsNullOrWhiteSpace(salt))
                return false;

            try
            {
                string computedHash = ComputeHash(password + salt);
                return hash == computedHash;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string ComputeHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}