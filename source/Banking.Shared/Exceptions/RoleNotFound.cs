

namespace Banking.Shared.Exceptions
{
    public sealed class RoleNotFound : BadRequestException
    {
        public RoleNotFound(string role) : base($"Role with name: {role} doesn't exist in the database")
        {
            
        }
    }
}