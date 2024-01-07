
using Banking.Domain.Contracts;
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/admins")]
    public class AdminController : BaseController
    {
        private readonly ILogger<AdminController> _logger;

        private readonly IAuthenticationService _authenticationService;

        public AdminController(IAuthenticationService authenticationService, ILogger<AdminController> logger) : base(logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet("getuser/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ReadUserDTO>> GetUserById(int id) 
        {
            _logger.LogDebug("In {@methodName} method", nameof(GetUserById));
            _logger.LogDebug("Parameter id is: {@id}", id);
            var user = await _authenticationService.GetUserByIdAsync(id);

            _logger.LogDebug("Returning user {@user}", user);

            return Ok(user);
        }

        [HttpPost("updaterole/{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ReadUserDTO>> UpdateUserRoles(int id, UpdateUserRoles userRoles)
        {
            _logger.LogDebug("In {@methodName} method", nameof(UpdateUserRoles));
            _logger.LogDebug("Parameter create user DTO is: {@userRoles}", userRoles);

            var result = await _authenticationService.UpdateUserRoles(id, userRoles);

            _logger.LogDebug("Returning user with updated roles: {@result}", result);
            return result;
        }





    }
}