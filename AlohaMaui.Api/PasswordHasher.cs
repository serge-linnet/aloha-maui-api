using System.Security.Cryptography;
using System.Text;

namespace AlohaMaui.Api
{
    public interface IPasswordHasher
    {
        string HashPassword(string email, string password);
    }

    public class PasswordHasher : IPasswordHasher
    {
        public PasswordHasher(string salt)
        {
            Salt = salt;
        }

        private string Salt { get; }

        public string HashPassword(string email, string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(email + password + Salt));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
