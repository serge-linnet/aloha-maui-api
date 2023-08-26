using AlohaMaui.Core.Entities;

namespace AlohaMaui.Api.Models
{
    public class UserInfo
    {
        public UserInfo(User user)
        {
            Email = user.Email;
            Role = user.Role;
            TokenExpires = user.TokenExpires;            
        }

        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime TokenExpires { get; set; }
    }
}
