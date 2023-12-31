
using Microsoft.AspNetCore.Identity;

namespace Banking.Persistance.Entities
{
    public class CustomRole : IdentityRole<int>
    {
        public virtual required ICollection<CustomUserRole> UserRoles {get; set;}
    }
}