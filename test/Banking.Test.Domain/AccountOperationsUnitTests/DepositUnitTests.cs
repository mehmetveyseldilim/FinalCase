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
    public class DepositUnitTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ILogger<AccountService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRecordRepository> _recordRepositoryMock;
        private readonly Mock<IErrorRecordHandler> _errorRecordHandlerMock;
        private readonly Mock<IBillRepository> _billRepositoryMock;
        private AccountService _accountService;

        public DepositUnitTests()
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
        public async Task DepositAsync_SuccessfulCreation_WhenParametersAreValid()
        {
            // Arrange
            var userId = 1;
            int amount = 150;
            int balance = 250;

            var createDepositDTO = new CreateDepositDTO 
            { 
                Amount = amount,
            };

            var account = new Account 
            { 
                Id = 1, 
                UserId = userId, 
                Balance = balance
            };

            var readAccountDTO = new ReadAccountDTO 
            { 
                Id = account.Id, 
                UserId = account.UserId, 
                Balance = balance + amount, 
            };
            var mockDbTransaction = new Mock<IDbTransaction>();

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>())).Returns(readAccountDTO);
            _unitOfWorkMock.Setup(uow => uow.BeginTransaction()).Returns(mockDbTransaction.Object);
            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(userId)).ReturnsAsync(account);


            // Act
            var result = await _accountService.DepositAsync(userId, createDepositDTO);

            // Assert
            Assert.Equal(readAccountDTO, result);
            Assert.True(result.UserId == 1);
            Assert.True(result.DailySpend == 0);
            Assert.True(result.Balance == amount + balance);


            _recordRepositoryMock.Verify(repo => repo.CreateRecord(It.IsAny<Record>()), Times.Once);
            mockDbTransaction.Verify(m => m.Commit(), Times.Once());
            mockDbTransaction.Verify(m => m.Rollback(), Times.Never());
            mockDbTransaction.Verify(m => m.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.Dispose(), Times.Never());
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
            _mapperMock.Verify(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>()), Times.Once());

        }

        [Fact]
        public async Task CreateAccountAsync_ExceptionThrown()
        {
            // Arrange
            var userId = 1;
            int amount = 150;
            int balance = 250;

            var createDepositDTO = new CreateDepositDTO 
            { 
                Amount = amount,
            };

            var account = new Account 
            { 
                Id = 1, 
                UserId = userId, 
                Balance = balance
            };

            var readAccountDTO = new ReadAccountDTO 
            { 
                Id = account.Id, 
                UserId = account.UserId, 
                Balance = balance + amount, 
            };
            var mockDbTransaction = new Mock<IDbTransaction>();

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>())).Returns(readAccountDTO);
            _unitOfWorkMock.Setup(uow => uow.BeginTransaction()).Returns(mockDbTransaction.Object);
            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(userId)).ReturnsAsync(account);
            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
               .ThrowsAsync(new Exception("Simulated concurrency exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _accountService.DepositAsync(userId, createDepositDTO));
            
            
            _recordRepositoryMock.Verify(repo => repo.CreateRecord(It.IsAny<Record>()), Times.Once());
            mockDbTransaction.Verify(m => m.Commit(), Times.Never());
            mockDbTransaction.Verify(m => m.Rollback(), Times.Exactly(1));
            mockDbTransaction.Verify(m => m.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
            _mapperMock.Verify(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>()), Times.Never());

        }




    }
}