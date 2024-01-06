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
    public class TransferMoneyUnitTests
    {
        private readonly Mock<IAccountRepository> _accountRepositoryMock;
        private readonly Mock<ILogger<AccountService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRecordRepository> _recordRepositoryMock;
        private readonly Mock<IErrorRecordHandler> _errorRecordHandlerMock;
        private readonly Mock<IBillRepository> _billRepositoryMock;
        private AccountService _accountService;

        public TransferMoneyUnitTests()
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
        public async Task TransferMoneyAsync_ShouldThrowAccountNotFoundException_WhenSenderAccountDoesNotExist()
        {
            // Arrange
            int senderUserId = 1;
            int receiverUserId = 2;
            int transferAmount = 150;

            var createTransferMoneyDTO = new CreateTransferMoneyDTO 
            { 
                Amount = transferAmount,
                ReceiverAccountId = receiverUserId

            };
            // var account = new Account { Id = 1, UserId = userId, Balance = 100, DailyLimit = 500, OperationLimit = 250};
            // var readAccountDTO = new ReadAccountDTO { Id = account.Id, UserId = account.UserId, Balance = account.Balance, DailyLimit = account.DailyLimit, OperationLimit = account.OperationLimit };
            var mockDbTransaction = new Mock<IDbTransaction>();




            // Act
            await Assert.ThrowsAsync<AccountNotFound>(() => _accountService.TransferMoneyAsync(senderUserId, createTransferMoneyDTO));


            _accountRepositoryMock.Verify(repo => repo.CreateAccount(It.IsAny<Account>()), Times.Never);
            _recordRepositoryMock.Verify(repo => repo.CreateRecord(It.IsAny<Record>()), Times.Never);
            mockDbTransaction.Verify(m => m.Commit(), Times.Never());
            mockDbTransaction.Verify(m => m.Rollback(), Times.Never());
            mockDbTransaction.Verify(m => m.Dispose(), Times.Never());
            _unitOfWorkMock.Verify(uow => uow.Dispose(), Times.Never());
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            _mapperMock.Verify(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>()), Times.Never());

        }

        [Fact]
        public async Task TransferMoneyAsync_ShouldThrowAccountNotFoundException_WhenReceiverAccountDoesNotExist()
        {
            // Arrange
            int senderUserId = 1;
            int receiverUserId = 2;
            int transferAmount = 150;

            var createTransferMoneyDTO = new CreateTransferMoneyDTO 
            { 
                Amount = transferAmount,
                ReceiverAccountId = receiverUserId

            };
            var account = new Account { Id = 1, UserId = senderUserId, Balance = 100, DailyLimit = 500, OperationLimit = 250};
            var mockDbTransaction = new Mock<IDbTransaction>();


            _unitOfWorkMock.Setup(uow => uow.BeginTransaction()).Returns(mockDbTransaction.Object);
            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(senderUserId)).ReturnsAsync(account);


            // Act
            await Assert.ThrowsAsync<AccountNotFound>(() => _accountService.TransferMoneyAsync(senderUserId, createTransferMoneyDTO));


            _accountRepositoryMock.Verify(repo => repo.CreateAccount(It.IsAny<Account>()), Times.Never);
            _recordRepositoryMock.Verify(repo => repo.CreateRecord(It.IsAny<Record>()), Times.Never);
            mockDbTransaction.Verify(m => m.Commit(), Times.Never());
            mockDbTransaction.Verify(m => m.Rollback(), Times.Never());
            mockDbTransaction.Verify(m => m.Dispose(), Times.Never());
            _unitOfWorkMock.Verify(uow => uow.Dispose(), Times.Never());
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            _mapperMock.Verify(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>()), Times.Never());

        }

        [Fact]
        public async Task TransferMoneyAsync_ThrowsInsufficientFundsException_WhenBalanceBelowWithdrawalAmount()
        {
            // Arrange
            const int senderUserId = 1;
            const int receiverUserId = 2;
            const int transferAmount = 150;

            var createTransferMoneyDTO = new CreateTransferMoneyDTO 
            { 
                Amount = transferAmount,
                ReceiverAccountId = receiverUserId

            };
            var senderAccount = new Account 
            { 
                Id = 1, 
                UserId = senderUserId, 
                Balance = 100, 
                DailyLimit = 500, 
                OperationLimit = 250
            };

            var receiverAccount = new Account 
            { 
                Id = 2, 
                UserId = receiverUserId, 
                Balance = 100, 
                DailyLimit = 500, 
                OperationLimit = 250
            };

            var mockDbTransaction = new Mock<IDbTransaction>();

            _unitOfWorkMock.Setup(uow => uow.BeginTransaction()).Returns(mockDbTransaction.Object);
            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(senderUserId)).ReturnsAsync(senderAccount);
            _accountRepositoryMock.Setup(x => x.GetAccountById(receiverUserId)).ReturnsAsync(receiverAccount);


            // Act & Assert
            await Assert.ThrowsAsync<InsufficientFundsException>(() => _accountService.TransferMoneyAsync(senderUserId, createTransferMoneyDTO));
            _recordRepositoryMock.Verify(repo => repo.CreateRecord(It.IsAny<Record>()), Times.Never);
            mockDbTransaction.Verify(m => m.Commit(), Times.Never());
            mockDbTransaction.Verify(m => m.Rollback(), Times.Once());
            mockDbTransaction.Verify(m => m.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            _mapperMock.Verify(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>()), Times.Never());

        }

        [Fact]
        public async Task TransferMoneyAsync_DailyLimitExceededException_WhenAmountExceedsDailyLimit()
        {
            // Arrange
            const int senderUserId = 1;
            const int receiverUserId = 2;
            const int transferAmount = 550;

            var createTransferMoneyDTO = new CreateTransferMoneyDTO 
            { 
                Amount = transferAmount,
                ReceiverAccountId = receiverUserId

            };
            var senderAccount = new Account 
            { 
                Id = 1, 
                UserId = senderUserId, 
                Balance = 1000, 
                DailyLimit = 500, 
                OperationLimit = 250
            };

            var receiverAccount = new Account 
            { 
                Id = 2, 
                UserId = receiverUserId, 
                Balance = 100, 
                DailyLimit = 500, 
                OperationLimit = 250
            };

            var mockDbTransaction = new Mock<IDbTransaction>();

            _unitOfWorkMock.Setup(uow => uow.BeginTransaction()).Returns(mockDbTransaction.Object);
            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(senderUserId)).ReturnsAsync(senderAccount);
            _accountRepositoryMock.Setup(x => x.GetAccountById(receiverUserId)).ReturnsAsync(receiverAccount);


            // Act & Assert
            await Assert.ThrowsAsync<DailyLimitExceededException>(() => _accountService.TransferMoneyAsync(senderUserId, createTransferMoneyDTO));
            
            
            _recordRepositoryMock.Verify(repo => repo.CreateRecord(It.IsAny<Record>()), Times.Never);
            mockDbTransaction.Verify(m => m.Commit(), Times.Never());
            mockDbTransaction.Verify(m => m.Rollback(), Times.Once());
            mockDbTransaction.Verify(m => m.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            _mapperMock.Verify(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>()), Times.Never());


        }

        [Fact]
        public async Task TransferMoneyAsync_OperationLimitExceededException_WhenAmountExceedsOperationLimit()
        {
            // Arrange
            const int senderUserId = 1;
            const int receiverUserId = 2;
            const int transferAmount = 270;

            var createTransferMoneyDTO = new CreateTransferMoneyDTO 
            { 
                Amount = transferAmount,
                ReceiverAccountId = receiverUserId

            };
            var senderAccount = new Account 
            { 
                Id = 1, 
                UserId = senderUserId, 
                Balance = 450, 
                DailyLimit = 500, 
                OperationLimit = 250
            };

            var receiverAccount = new Account 
            { 
                Id = 2, 
                UserId = receiverUserId, 
                Balance = 100, 
                DailyLimit = 500, 
                OperationLimit = 250
            };

            var mockDbTransaction = new Mock<IDbTransaction>();

            _unitOfWorkMock.Setup(uow => uow.BeginTransaction()).Returns(mockDbTransaction.Object);
            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(senderUserId)).ReturnsAsync(senderAccount);
            _accountRepositoryMock.Setup(x => x.GetAccountById(receiverUserId)).ReturnsAsync(receiverAccount);


            // Act & Assert
            await Assert.ThrowsAsync<OperationLimitExceededException>(() => _accountService.TransferMoneyAsync(senderUserId, createTransferMoneyDTO));
            
            
            _recordRepositoryMock.Verify(repo => repo.CreateRecord(It.IsAny<Record>()), Times.Never);
            mockDbTransaction.Verify(m => m.Commit(), Times.Never());
            mockDbTransaction.Verify(m => m.Rollback(), Times.Once());
            mockDbTransaction.Verify(m => m.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
            _mapperMock.Verify(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>()), Times.Never());

        }

        [Fact]
        public async Task TransferMoneyAsync_ReturnsBothReadAccountDTO_WhenTransferOccurred()
        {
            // Arrange
            const int senderUserId = 1;
            const int receiverUserId = 2;
            const int transferAmount = 100;

            var createTransferMoneyDTO = new CreateTransferMoneyDTO 
            { 
                Amount = transferAmount,
                ReceiverAccountId = receiverUserId

            };
            var senderAccount = new Account 
            { 
                Id = 1, 
                UserId = senderUserId, 
                Balance = 450, 
                DailyLimit = 500, 
                OperationLimit = 250,
                DailySpend = 0
                
            };

            var receiverAccount = new Account 
            { 
                Id = 2, 
                UserId = receiverUserId, 
                Balance = 100, 
                DailyLimit = 500, 
                OperationLimit = 250
            };

            var mappedSenderAccount = new ReadAccountDTO
            {
                Id = 1, 
                UserId = senderUserId, 
                Balance = 450 - transferAmount, 
                DailyLimit = 500, 
                OperationLimit = 250,
                DailySpend = transferAmount
                

            };

            var mappedReceivedAccount = new ReadAccountDTO
            {
                Id = 2, 
                UserId = receiverUserId, 
                Balance = 100 + transferAmount, 
                DailyLimit = 500, 
                OperationLimit = 250


            };

            var mockDbTransaction = new Mock<IDbTransaction>();

            _unitOfWorkMock.Setup(uow => uow.BeginTransaction()).Returns(mockDbTransaction.Object);
            _accountRepositoryMock.Setup(x => x.GetAccountByUserId(senderUserId)).ReturnsAsync(senderAccount);
            _accountRepositoryMock.Setup(x => x.GetAccountById(receiverUserId)).ReturnsAsync(receiverAccount);
            _mapperMock.Setup(mapper => mapper.Map<ReadAccountDTO>(senderAccount)).Returns(mappedSenderAccount);
            _mapperMock.Setup(mapper => mapper.Map<ReadAccountDTO>(receiverAccount)).Returns(mappedReceivedAccount);

            // Act 
            Tuple<ReadAccountDTO, ReadAccountDTO> result = await _accountService.TransferMoneyAsync(senderUserId, createTransferMoneyDTO);            
            

            ReadAccountDTO senderReadAccountDTO = result.Item1;
            ReadAccountDTO receiverReadAccountDTO = result.Item2;

            // Assert
            Assert.Equal(mappedSenderAccount, senderReadAccountDTO);
            Assert.Equal(mappedSenderAccount.Id, senderReadAccountDTO.Id);
            Assert.Equal(mappedSenderAccount.Balance, senderReadAccountDTO.Balance);
            Assert.Equal(mappedSenderAccount.DailyLimit, senderReadAccountDTO.DailyLimit);
            Assert.Equal(mappedSenderAccount.OperationLimit, senderReadAccountDTO.OperationLimit);
            Assert.True(senderReadAccountDTO.Balance == 450 - transferAmount);
            Assert.True(senderReadAccountDTO.DailySpend == transferAmount);


            Assert.Equal(receiverReadAccountDTO, receiverReadAccountDTO);
            Assert.Equal(receiverReadAccountDTO.Id, receiverReadAccountDTO.Id);
            Assert.Equal(receiverReadAccountDTO.Balance, receiverReadAccountDTO.Balance);
            Assert.Equal(receiverReadAccountDTO.DailyLimit, receiverReadAccountDTO.DailyLimit);
            Assert.Equal(receiverReadAccountDTO.OperationLimit, receiverReadAccountDTO.OperationLimit);
            Assert.Equal(100 + transferAmount, receiverReadAccountDTO.Balance);


            _recordRepositoryMock.Verify(repo => repo.CreateRecord(It.IsAny<Record>()), Times.Once);
            mockDbTransaction.Verify(m => m.Commit(), Times.Once());
            mockDbTransaction.Verify(m => m.Rollback(), Times.Never());
            mockDbTransaction.Verify(m => m.Dispose(), Times.Once());
            _unitOfWorkMock.Verify(uow => uow.Dispose(), Times.Never());
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            _mapperMock.Verify(mapper => mapper.Map<ReadAccountDTO>(It.IsAny<Account>()), Times.Exactly(2));

        }

    }
}