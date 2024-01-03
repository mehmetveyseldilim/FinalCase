using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Banking.Test.Domain
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _mockAccountRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<AccountService>> _mockLogger;

        private AccountService _accountService;

        public AccountServiceTests()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<AccountService>>();
            _accountService = new AccountService(_mockAccountRepository.Object, _mockLogger.Object, _mockMapper.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Deposit_ShouldNotD()
        {
            // Given
        
            // When
        
            // Then
        }

        



    }
}