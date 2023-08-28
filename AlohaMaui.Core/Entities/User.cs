using Newtonsoft.Json;

namespace AlohaMaui.Core.Entities
{
    public class User
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty]
        public string Role { get; set; }

        [JsonProperty]
        public string Token { get; set; }

        [JsonProperty]
        public DateTime TokenExpires { get; set; }

        public string PasswordHash { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

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
