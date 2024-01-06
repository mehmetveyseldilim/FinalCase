using System.Data;
using AutoMapper;
using Banking.Domain.Contracts;
using Banking.Domain.Services;
using Banking.Persistance.Contracts;
using Banking.Persistance.Entities;
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;
using Banking.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Record = Banking.Persistance.Entities.Record;

namespace Banking.Test.Domain.AccountOperationsUnitTests
{
    public class WithdrawUnitTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ILogger<AccountService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRecordRepository> _recordRepositoryMock;
        private readonly Mock<IErrorRecordHandler> _errorRecordHandlerMock;
        private readonly Mock<IBillRepository> _billRepositoryMock;
        private AccountService _accountService;

        public WithdrawUnitTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _loggerMock = new Mock<ILogger<AccountService>>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _recordRepositoryMock = new Mock<IRecordRepository>();
            _errorRecordHandlerMock = new Mock<IErrorRecordHandler>();
            _billRepositoryMock = new Mock<IBillRepository>();

            _accountService = new AccountService(
                _accountRepositoryMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _recordRepositoryMock.Object,
                _errorRecordHandlerMock.Object,
                _billRepositoryMock.Object);
        }

        [Fact]
        public async Task WithdrawAsync_ThrowsInsufficientFundsException_WhenBalanceBelowWithdrawalAmount()
        {
            // Arrange
            const int userId = 1;
            const int amount = 15000;
            var createWithdrawDTO = new CreateWithdrawDTO { Amount = amount };
            var account = new Account { Id = 1, Balance = 1500 , DailyLimit = 500, OperationLimit = 250};

            var mockDbTransaction = new Mock<IDbTransaction>();

            _unitOfWorkMock.Setup(uow => uow.BeginTransaction())
                .Returns(mockDbTransaction.Object);

            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(userId)).ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<InsufficientFundsException>(() => _accountService.WithdrawAsync(userId, createWithdrawDTO));
        }

        [Fact]
        public async Task WithdrawAsync_DailyLimitExceededException_WhenAmountExceedsDailyLimit()
        {
            // Arrange
            const int userId = 1;
            const int amount = 550;
            var createWithdrawDTO = new CreateWithdrawDTO { Amount = amount };
            var account = new Account { Id = 1, Balance = 1500 , DailyLimit = 500, OperationLimit = 250};

            var mockDbTransaction = new Mock<IDbTransaction>();

            _unitOfWorkMock.Setup(uow => uow.BeginTransaction())
                .Returns(mockDbTransaction.Object);

            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(userId)).ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<DailyLimitExceededException>(() => _accountService.WithdrawAsync(userId, createWithdrawDTO));
        }

        [Fact]
        public async Task WithdrawAsync_OperationLimitExceededException_WhenAmountExceedsOperationLimit()
        {
            // Arrange
            const int userId = 1;
            const int amount = 260;
            var createWithdrawDTO = new CreateWithdrawDTO { Amount = amount };
            var account = new Account { Id = 1, Balance = 1500 , DailyLimit = 500, OperationLimit = 250};

            var mockDbTransaction = new Mock<IDbTransaction>();

            _unitOfWorkMock.Setup(uow => uow.BeginTransaction())
                .Returns(mockDbTransaction.Object);

            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(userId)).ReturnsAsync(account);

            // Act & Assert
            await Assert.ThrowsAsync<OperationLimitExceededException>(() => _accountService.WithdrawAsync(userId, createWithdrawDTO));


        }

        [Fact]
        public async Task Withdraw_ShouldReturnReadAccountDTO_WhenCalledWithValidParameters()
        {
            // Arrange
            int userId = 1;
            const int amount = 125;
            var account = new Account { Id = 1, UserId = userId, Balance = 250, DailyLimit = 500, OperationLimit = 250, DailySpend = 0};
            var createWithdrawDTO = new CreateWithdrawDTO { Amount = amount };
            var readAccountDTO = new ReadAccountDTO { Id = account.Id, UserId = account.UserId, Balance = 125, DailySpend = 125};
            var mockDbTransaction = new Mock<IDbTransaction>();

            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(userId)).ReturnsAsync(account);
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>())).Returns(readAccountDTO);
            _unitOfWorkMock.Setup(uow => uow.BeginTransaction()).Returns(mockDbTransaction.Object);


            // Act
            var result = await _accountService.WithdrawAsync(userId, createWithdrawDTO);

            

            // Assert
            Assert.Equal(readAccountDTO, result);
            Assert.True(result.DailySpend == 125);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
            _mapperMock.Verify(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>()), Times.Once);
            mockDbTransaction.Verify(m => m.Commit(), Times.Once());
        }

        [Fact]
        public async Task Withdraw_ShouldThrowDbUpdateConcurrencyException_WhenRowVersionIsDifferentThanInDb()
        {
            // Arrange
            const int userId = 1;
            var mockDbTransaction = new Mock<IDbTransaction>();
            var account = new Account { Id = 1, UserId = 1, Balance = 250, DailyLimit = 500, OperationLimit = 250, DailySpend = 0 };
            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(It.IsAny<int>())).ReturnsAsync(account);
            _unitOfWorkMock.Setup(uow => uow.BeginTransaction()).Returns(mockDbTransaction.Object);
            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(userId)).ReturnsAsync(account);
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .ThrowsAsync(new DbUpdateConcurrencyException("Simulated concurrency exception"));


            var task1 = _accountService.WithdrawAsync(userId, new CreateWithdrawDTO { Amount = 50 });


            // Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => Task.WhenAll(task1));


            mockDbTransaction.Verify(m => m.Commit(), Times.Never());
            mockDbTransaction.Verify(m => m.Rollback(), Times.Once());
            mockDbTransaction.Verify(m => m.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.Dispose(), Times.Exactly(1));
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
            _mapperMock.Verify(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>()), Times.Never());


        }
    }    
}