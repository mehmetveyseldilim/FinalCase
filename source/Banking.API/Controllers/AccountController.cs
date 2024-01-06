
using Banking.API.ActionFilters;
using Banking.Domain.Contracts;
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger) : base(logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("create-account")]
        [Authorize(Roles = "User")]
        [ServiceFilter(typeof(FluentValidationFilter))]
        public async Task<ActionResult<ReadAccountDTO>> CreateAccount(CreateAccountDTO createAccountDTO)
        {
            _logger.LogDebug("{@controllerName} -- HTTP POST -- {@methodName}", nameof(AccountController), nameof(CreateAccount));
            _logger.LogDebug("Create account DTO: {@dto}", createAccountDTO);

           int userId = GetUserIdFromToken();

            ReadAccountDTO dto = await _accountService.CreateAccountAsync(userId, createAccountDTO);

            return Ok(dto);

        }

        [HttpPost("deposit")]
        [Authorize(Roles = "User")]
        // [ServiceFilter(typeof(FluentValidationFilter))]
        public async Task<ActionResult<ReadAccountDTO>> Deposit(CreateDepositDTO createDepositDTO)
        {
            _logger.LogDebug("{@controllerName} -- HTTP POST -- {@methodName}", nameof(AccountController), nameof(Deposit));
            _logger.LogDebug("Create deposit DTO: {@dto}", createDepositDTO);

            int userId = GetUserIdFromToken();

            ReadAccountDTO dto = await _accountService.DepositAsync(userId, createDepositDTO);

            return Ok(dto);

        }

        [HttpPost("withdraw")]
        [Authorize(Roles = "User")]
        // [ServiceFilter(typeof(FluentValidationFilter))]
        public async Task<ActionResult<ReadAccountDTO>> Withdraw(CreateWithdrawDTO createWithdrawDTO)
        {
            _logger.LogDebug("{@controllerName} -- HTTP POST -- {@methodName}", nameof(AccountController), nameof(Withdraw));
            _logger.LogDebug("Create withdraw DTO: {@dto}", createWithdrawDTO);

            int userId = GetUserIdFromToken();

            ReadAccountDTO dto = await _accountService.WithdrawAsync(userId, createWithdrawDTO);

            return Ok(dto);

        }

        [HttpPost("transfer")]
        [Authorize(Roles = "User")]
        // [ServiceFilter(typeof(FluentValidationFilter))]
        public async Task<ActionResult<(ReadAccountDTO SourceAccount, ReadAccountDTO TargetAccount)>> Transfer(CreateTransferMoneyDTO createTransferMoneyDTO)
        {
            _logger.LogDebug("{@controllerName} -- HTTP POST -- {@methodName}", nameof(AccountController), nameof(Transfer));
            _logger.LogDebug("Create transfer money DTO: {@dto}", createTransferMoneyDTO);

            int userId = GetUserIdFromToken();

            Tuple<ReadAccountDTO, ReadAccountDTO> result = await _accountService.TransferMoneyAsync(userId, createTransferMoneyDTO);

            return Ok(result);

        }

        [HttpGet("transaction-history")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<ReadRecordDTO>>> GetTransactionHistoryForUser()
        {
            _logger.LogDebug("{@controllerName} -- HTTP GET -- {@methodName}", nameof(AccountController), nameof(Transfer));

            int userId = GetUserIdFromToken();

            _logger.LogDebug("User id extracted from access token is {@userId}", userId);

            IEnumerable<ReadRecordDTO> result = await _accountService.GetUserTransactionHistoryAsync(userId);

            return Ok(result);

        }


    }
}