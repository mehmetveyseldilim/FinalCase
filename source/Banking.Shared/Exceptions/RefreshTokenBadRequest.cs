
namespace Banking.Shared.Exceptions
{
    public sealed class RefreshTokenBadRequest : BadRequestException
    {
        public RefreshTokenBadRequest(string errorMessage) : base(errorMessage)
        {

        }
    }
}