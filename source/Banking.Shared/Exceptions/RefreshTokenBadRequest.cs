
namespace Banking.Shared.Exceptions
{
    public sealed class RefreshTokenBadRequest : BadRequestException
    {
        public RefreshTokenBadRequest() : base("Invalid client request. The Token has some invalid values")
        {

        }
    }
}