using Microsoft.AspNetCore.Identity;

namespace AuthServer.Core.Models
{
    public class AppUser : IdentityUser 
    {
        public string? City { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
