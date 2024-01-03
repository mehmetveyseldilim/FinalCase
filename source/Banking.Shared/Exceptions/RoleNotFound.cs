

namespace Banking.Shared.Exceptions
{
    public sealed class RoleNotFound : BadRequestException
    {
        public RoleNotFound(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}