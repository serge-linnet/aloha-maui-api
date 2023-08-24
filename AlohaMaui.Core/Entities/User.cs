namespace AlohaMaui.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public IEnumerable<Place> Places { get; set; }
        public string Token { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
