using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;
using Microsoft.AspNetCore.Identity;

namespace Banking.Domain.Contracts
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(CreateUserDTO createUserDTO);
        Task<bool> ValidateUser(LoginUserDTO loginUserDTO);

        Task<ReadTokenDTO> CreateToken(bool populateExp);

        Task<ReadTokenDTO> RefreshToken(ReadTokenDTO tokenDto);

        Task<ReadUserDTO> GetUserByIdAsync(int id);
    }
}