using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "tokenCreated")]
        public DateTime TokenCreated { get; set; }

        [JsonProperty(PropertyName = "tokenExpires")]
        public DateTime TokenExpires { get; set; }

        public User()
        {
            
        }

        public User(string email, Role role)
        {
            Email = email.ToLowerInvariant();
            Role = Enum.GetName(role)!;
        }
    }

    public enum Role
    {
        Admin,
        User
    }
}
