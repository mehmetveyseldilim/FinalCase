using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Banking.Domain.Contracts;
using Banking.Persistance.Entities;
using Banking.Shared;
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;
using Banking.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Banking.Domain.Services
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IOptionsMonitor<JwtConfiguration> _configuration;
        private readonly RoleManager<CustomRole> _roleManager;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly string _secretKey;

        private CustomUser? _user;

        public AuthenticationService(IMapper mapper,UserManager<CustomUser> userManager, 
        IOptionsMonitor<JwtConfiguration> configuration, 
        RoleManager<CustomRole> roleManager, ILogger<AuthenticationService> logger)
        {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _jwtConfiguration = _configuration.CurrentValue;
            _logger = logger;
            _secretKey = Environment.GetEnvironmentVariable("SecretKey") ?? throw new ArgumentNullException("Secret Key is null");
            _logger.LogDebug("In {@className} constructor", nameof(AuthenticationService));
            _logger.LogDebug("Secret key value: {@secretKey}", _secretKey);
            _logger.LogDebug("JWT Configuration: {@jwtConfiguration}", _jwtConfiguration);

        }

        public async Task<ReadUserDTO> GetUserByIdAsync(int id)
        {
            _logger.LogDebug($"In {nameof(GetUserByIdAsync)} method");
            _logger.LogDebug("Searching for user based on provided id with value: {@id}",id);
            var user = await _userManager.FindByIdAsync(id.ToString());

            if(user == null) 
            {
                _logger.LogError("The user with id {@id} does not exist in the db. Throwing {@UserNotFound} exception}",id, nameof(UserNotFound));
                throw new UserNotFound(id);
            }

            _logger.LogInformation("User has been found. User is {@_user}", _user);
            _logger.LogDebug("Getting roles of the user");
            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("User roles: {@roles}", roles);

            var mappedUser = _mapper.Map<ReadUserDTO>(user);
            // mappedUser.Roles = roles;
            _logger.LogDebug("Returning read user dto : {@mappedUser}", mappedUser);
            return mappedUser;
        }

        public async Task<IdentityResult> RegisterUser(CreateUserDTO createUserDTO)
        {
            _logger.LogDebug("In {@nameof(RegisterUser)} method", nameof(RegisterUser));
            _logger.LogDebug("Create User DTO: {@createUserDto}", createUserDTO);

            var user = _mapper.Map<CustomUser>(createUserDTO);
            _logger.LogDebug("Mapped user is: {@user}", user);
            await CheckIfRoleExists(createUserDTO.Roles);

            _logger.LogDebug("Roles do exist. Creating User");

            var result = await _userManager.CreateAsync(user, createUserDTO.Password);

            _logger.LogDebug("Create User Result is: {@result}", result);

            if (result.Succeeded)
                _logger.LogDebug("Creating user operation is successful.");
                await _userManager.AddToRolesAsync(user, createUserDTO.Roles);

            return result;
        }
    
        private async Task CheckIfRoleExists(ICollection<string> roles)
        {
            _logger.LogDebug("In {@nameof(CheckIfRoleExists)} method", nameof(CheckIfRoleExists));
            
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogError("The role {@role} does not exist. Throwing {@RoleNotFound} exception", role, nameof(RoleNotFound));
                    throw new RoleNotFound(role);
                }
            }

        }

        public async Task<bool> ValidateUser(LoginUserDTO loginUserDTO)
        {
            _logger.LogDebug("In {@MethodName} Method", nameof(ValidateUser));
            _logger.LogDebug("Login User DTO: {@userForAuth}", loginUserDTO);

            string? userName = loginUserDTO.UserName;
            _logger.LogDebug("String userName = {@userName}", userName);

            if(userName == null) 
            {
                _logger.LogDebug("Username is null. Returning false");
                return false;
            }

            string? password = loginUserDTO.Password;

            if(password == null) 
            {
                _logger.LogDebug("Password is null. Returning false");
                return false;
            }

            _user = await _userManager.FindByNameAsync(userName);

            _logger.LogDebug("The user is : {@_user}", _user);

            var result = _user != null && await _userManager.CheckPasswordAsync(_user, password);

            _logger.LogDebug("Password checking and _user is not null : {@result}", result);

            if (!result)
            {
                _logger.LogDebug($"{nameof(ValidateUser)}: Authentication failed. Wrong user name or password.");
            }

            return result;
        }

        public async Task<ReadTokenDTO> CreateToken(bool populateExp)
        {
            _logger.LogDebug("In {@MethodName} Method", nameof(CreateToken));
            _logger.LogDebug("PopulateExp : {@populateExp}", populateExp);

            _logger.LogDebug($"Requesting {nameof(GetSigningCredentials)}");
            var signingCredentials = GetSigningCredentials();
            _logger.LogDebug($"Requesting {nameof(GetClaims)}");
            var claims = await GetClaims();
            _logger.LogDebug($"Requesting {nameof(GenerateTokenOptions)}");
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken();

            _user!.RefreshToken = refreshToken;

            if (populateExp)
                _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new ReadTokenDTO(accessToken, refreshToken);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_secretKey);

            _logger.LogDebug("Secret key is {@secretKey}", key);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user!.UserName!),
                new Claim(ClaimTypes.NameIdentifier, _user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {

            _logger.LogDebug($"liveLoading value : {_jwtConfiguration.liveLoading}");

            var tokenOptions = new JwtSecurityToken
            (
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);

            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            Console.WriteLine(_secretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                ValidateLifetime = true,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task<ReadTokenDTO> RefreshToken(ReadTokenDTO tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

            var user = await _userManager.FindByNameAsync(principal.Identity!.Name!);

            if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new RefreshTokenBadRequest();

            _user = user;
            return await CreateToken(populateExp: false);
        }
    
        
    }
}