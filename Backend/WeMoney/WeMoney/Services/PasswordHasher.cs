using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using WeMoney.Models.Constants;

namespace WeMoney.Services;

public class PasswordHasher(IOptions<AppSettings> options)
{
    public string Hash(string password)
    {
        var saltKey = options.Value.SecretKey;
        var salt = Encoding.UTF8.GetBytes(saltKey);
        var passwordHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return passwordHashed;
    }

    public bool Compare(string password, string hashedPassword)
    {
        return string.Equals(Hash(password), hashedPassword);
    }
}