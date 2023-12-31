
using Microsoft.AspNetCore.Identity;

namespace Banking.Persistance.Entities
{
    public class CustomUser : IdentityUser<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        //. Navigation Properties

        public virtual required ICollection<CustomUserRole> UserRoles {get; set;}
    
    }
}