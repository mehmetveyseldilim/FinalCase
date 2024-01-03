
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly ILogger<BaseController> _logger;
        public BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }

        protected int GetUserIdFromToken()
        {
             // Retrieve user id from the JWT token
            string? userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(!int.TryParse(userIdFromToken, out int userId))
            {
                _logger.LogError("Cannot get user id from token. Returning unauthorized");
                throw new UnauthorizedAccessException("Unauthorized");

            }

            return userId;
        }
    }
}