using System.Text.Json;
using Banking.Domain.Contracts;
using Banking.Shared.DTOs.Response;
using Banking.Shared.RequestParameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Banking.API.Controllers
{
    [ApiController]
    [Route("api/support")]

    public class SupportController : BaseController
    {
        private readonly ILogger<SupportController> _logger;

        private readonly IAuthenticationService _authenticationService;

        private readonly IAccountService _accountService;

        public SupportController(IAuthenticationService authenticationService,
        ILogger<SupportController> logger,
        IAccountService accountService) : base(logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
            _accountService = accountService;
        }

        [HttpGet("records")]
        [Authorize(Roles = "Support")]
        public async Task<ActionResult<ReadRecordDTO>> GetAllRecords([FromQuery] RecordParameters recordParameters) 
        {
            _logger.LogDebug("In {@methodName} method", nameof(GetAllRecords));

            var pagedRecords = await _accountService.GetAllRecordsAsync(recordParameters);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedRecords.MetaData));

            return Ok(pagedRecords);
        }

        [HttpGet("pending-withdrawal/{recordId}")]
        [Authorize(Roles = "Support")]
        public async Task<IActionResult> ExecutePendingRecord(int recordId) 
        {
            _logger.LogDebug("In {@methodName} method", nameof(ExecutePendingRecord));

            await _accountService.ExecutePendingRecord(recordId);

            return Ok();
        }

    }
}