using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace CryptoStashStats.Utilities
{
    public interface IPasswordHelper
    {
        string HashPassword(string password);
    }

    public class PasswordHelper : IPasswordHelper
    {
        private readonly byte[] salt;

        public PasswordHelper(IConfiguration Configuration)
        {
            salt = Encoding.ASCII.GetBytes(ReadSalt(Configuration));
        }

        public string HashPassword(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
        }

        public void GenerateSalt()
        {
            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");
        }

        private static string ReadSalt(IConfiguration Configuration)
        {
            if (Environment.GetEnvironmentVariable("PASSWORD_SALT") != null)
            {
                return Environment.GetEnvironmentVariable("PASSWORD_SALT");
            }
            else if(Configuration["PasswordSalt"] != null)
            {
                return Configuration["PasswordSalt"];
            }

            throw new Exception("No password salt was found.");
        }
    }
}
