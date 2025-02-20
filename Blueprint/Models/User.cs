using System.Diagnostics.CodeAnalysis;

namespace Blueprint.Models
{
    [ExcludeFromCodeCoverage]
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool isLogin { get; set; } = false;
    }
}
