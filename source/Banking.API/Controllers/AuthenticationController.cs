using Banking.API.ActionFilters;
using Banking.API.Helper;
using Banking.Domain.Contracts;
using Banking.Shared;
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;


        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ReadUserDTO>> GetUserById(int id) 
        {
            _logger.LogDebug("In {@methodName} method", nameof(GetUserById));
            _logger.LogDebug("Parameter id is: {@id}", id);
            var user = await _authenticationService.GetUserByIdAsync(id);

            _logger.LogDebug("Returning user {@user}", user);

            return Ok(user);
        }

        [HttpPost]
        [ServiceFilter(typeof(FluentValidationFilter))]
        public async Task<IActionResult> RegisterUser(CreateUserDTO createUserDTO)
        {
            _logger.LogDebug("In {@methodName} method", nameof(RegisterUser));
            _logger.LogDebug("Parameter create user DTO is: {@createUserDTO}", createUserDTO);

            var result = await _authenticationService.RegisterUser(createUserDTO);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                _logger.LogDebug("Returning bad request");
                return BadRequest(ModelState);
            }

            _logger.LogDebug("Returning create user");
            return StatusCode(201);
        }

        [HttpPost("login")]
        [ServiceFilter(typeof(FluentValidationFilter))]
        public async Task<ActionResult<ReadTokenDTO>> Authenticate(LoginUserDTO loginUserDTO)
        {
            _logger.LogDebug("Register User Method");
            _logger.LogDebug("User for authentication: {@loginUserDTO}", loginUserDTO);

            if (!await _authenticationService.ValidateUser(loginUserDTO)) 
            {
                _logger.LogError("User login validation has been failed. Returning Unauthorized");
                var errorDetailsObject = ErrorDetailsHelper.CreateErrorDetails(ExceptionErrorMessages.InvalidLoginErrorMessage);

                return new BadRequestObjectResult(errorDetailsObject);
            }

            _logger.LogDebug("User authentication successfull.");
            _logger.LogDebug("Creating Access and Refresh Token");
            var tokenDto = await _authenticationService.CreateToken(populateExp: true);

            _logger.LogDebug("User token is: {@tokenDto}", tokenDto);
            _logger.LogDebug("Returning Ok with token dto");

            return Ok(tokenDto);
        }
    }
}