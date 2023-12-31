using Banking.Domain.Contracts;
using Banking.Shared.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/token")]
    public class TokenController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public TokenController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("refresh")]
        // [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh(ReadTokenDTO tokenDto)
        {
            var tokenDtoToReturn = await _authenticationService.RefreshToken(tokenDto);
            return Ok(tokenDtoToReturn);
        }
    }
}