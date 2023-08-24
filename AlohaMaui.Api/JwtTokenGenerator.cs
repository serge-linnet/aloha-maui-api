using AlohaMaui.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AlohaMaui.Api
{
    public class JwtToken
    {
        public string UserName { get; }
        public string Token { get; }

        public JwtToken(string userName, string token)
        {
            UserName = userName;
            Token = token;
        }
    }

    public interface IJwtTokenGenerator
    {
        JwtToken Generate(User user);
    }

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly string _secret;

        public JwtTokenGenerator(string secret)
        {
            _secret = secret;
        }

        public JwtToken Generate(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Email), new Claim(ClaimTypes.Role, user.Role) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encrypterToken = tokenHandler.WriteToken(token);

            return new JwtToken(user.Email, encrypterToken);
        }
    }
}
