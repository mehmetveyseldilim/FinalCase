using Microsoft.AspNetCore.Identity;

namespace Banking.Persistance.Entities
{
    public class CustomUserRole: IdentityUserRole<int>
    {
        public virtual required CustomUser User {get; set;}

        public virtual required CustomRole Role {get; set;}
    
        
    }
}