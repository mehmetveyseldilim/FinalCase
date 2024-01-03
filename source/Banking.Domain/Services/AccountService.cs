using AutoMapper;
using Banking.Domain.Contracts;
using Banking.Persistance.Contracts;
using Banking.Persistance.Entities;
using Banking.Shared;
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;
using Banking.Shared.Exceptions;
using Microsoft.Extensions.Logging;

namespace Banking.Domain.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        private readonly ILogger<AccountService> _logger;

        private readonly IMapper _mapper;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IRecordRepository _recordRepository;

        private readonly IErrorRecordHandler _errorRecordHandler;

        private readonly string _serviceName = nameof(AccountService);

        public AccountService(IAccountRepository accountRepository,
        ILogger<AccountService> logger,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IRecordRepository recordRepository,
        IErrorRecordHandler errorRecordHandler)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _recordRepository = recordRepository;
            _errorRecordHandler = errorRecordHandler;
        }

        public async Task<ReadAccountDTO> CreateAccountAsync(int userId, CreateAccountDTO createAccountDTO)
        {
            int openingBalance = createAccountDTO.Balance;
            OperationType operationType = OperationType.CreateAccount;
            string methodName = nameof(CreateAccountAsync);

            _logger.LogDebug("Inside {@nameOfService}_{@methodName} Method", _serviceName, methodName);
            _logger.LogDebug("User id parameter is {@userId} and account opening balance is: {@balance}", userId, openingBalance);
            _logger.LogDebug("Creating an account for user id {@userId}", userId);


            Account account = CreateNewAccountEntity(userId, openingBalance);
            
            using var transaction = _unitOfWork.BeginTransaction(); 
            {
                _logger.LogDebug("Transaction has been started.");

                try
                {
                    _accountRepository.CreateAccount(account);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Newly created account: {@account}", account);

                    Record record = CreateSuccessfullOperationRecordEntity(userId, openingBalance, operationType, account.Id);
                    _recordRepository.CreateRecord(record);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Newly created record for opening new account: {@record}", record);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error has been occured in {@nameOfService} {@methodName} method!. Rolling back transaction.", _serviceName, methodName);
                    _logger.LogError("Exception message: {@exceptionMessage}", ex.Message);

                    transaction.Rollback();
                    transaction.Dispose();
                    _unitOfWork.ClearChangeTracker();
                    _unitOfWork.Dispose();

                    Record errorRecord = CreateUnSuccessfullOperationRecordEntity(userId, openingBalance, operationType, false, ex.Message);

                    _logger.LogError("Record has been created for unsuccessfull {@operationName} operation. Record: {@record}", methodName, errorRecord);
                    _errorRecordHandler.AddErrorRecord(errorRecord);
                    throw;
                }
            }

            var mappedAccount = _mapper.Map<ReadAccountDTO>(account);

            _logger.LogDebug("Returning mapped account: {@mappdAccount}", mappedAccount);
            return mappedAccount;
        }

        public async Task<ReadAccountDTO> DepositAsync(int userId, CreateDepositDTO createDepositDTO)
        {
            string methodName = nameof(DepositAsync);
            OperationType operationType = OperationType.Deposit;
            int amount = createDepositDTO.Amount;

            _logger.LogDebug("Inside {@serviceName} {@methodName} Method", _serviceName, methodName);
            _logger.LogDebug("User id parameter is {@userId}", userId);
            _logger.LogDebug("Create deposit DTO: {@createDepositDTO}", createDepositDTO);


            _logger.LogDebug("Checking database to find account with id {@userId}", userId);
            Account account = await GetAccountByUserIdOrThrowExceptionAsync(userId: userId);

            using var transaction = _unitOfWork.BeginTransaction();
            _logger.LogDebug("Transaction has been started.");

            try
            {
                _logger.LogDebug("Account balance old value: {@accountBalance}", account.Balance);
                account.Balance += amount;
                _logger.LogDebug("Account balance new value: {@accountBalance}", account.Balance);
                _logger.LogDebug("Creating record");
                Record record = CreateSuccessfullOperationRecordEntity(userId, amount, operationType, account.Id);
                _recordRepository.CreateRecord(record);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Record has been created and saved. Record: {@record}", record);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has been occured in account service {@methodName} method!. Rolling back transaction", methodName);
                _logger.LogError("Exception message: {@exceptionMessage}", ex.Message);
                transaction.Rollback();
                transaction.Dispose();
                _unitOfWork.ClearChangeTracker();
                _unitOfWork.Dispose();
                
                Record errorRecord = CreateUnSuccessfullOperationRecordEntity(userId, amount, operationType, false, ex.Message, account.Id);
                _logger.LogError("Record has been created for unsuccessfull {@operationName} operation. Record: {@record}", methodName, errorRecord);
                _errorRecordHandler.AddErrorRecord(errorRecord);
                throw;

            }

            var mappedAccount = _mapper.Map<ReadAccountDTO>(account);

            _logger.LogDebug("Returning mapped account: {@mappdAccount}", mappedAccount);
            return mappedAccount;

        }

        public async Task<ReadAccountDTO> WithdrawAsync(int userId, CreateWithdrawDTO createWithdrawDTO)
        {
            string methodName = nameof(WithdrawAsync);
            OperationType operationType = OperationType.Withdrawal;
            int amount = createWithdrawDTO.Amount;

            _logger.LogDebug("Inside {@serviceName} {@methodName} Method", _serviceName, methodName);
            _logger.LogDebug("User id parameter is {@userId}", userId);
            _logger.LogDebug("Create withdraw DTO: {@createDepositDTO}", createWithdrawDTO);


            _logger.LogDebug("Checking database to find account with id {@userId}", userId);
            Account account = await GetAccountByUserIdOrThrowExceptionAsync(userId);


            using var transaction = _unitOfWork.BeginTransaction();
            _logger.LogDebug("Transaction has been started.");

            try
            {
                if(account.Balance - amount < 0) 
                {
                    _logger.LogError("Account balance is not enough for withdrawing {@amount} of money. Throwing {@exceptionName} exception."
                    ,amount, nameof(InsufficientFundsException));

                    throw new InsufficientFundsException(ExceptionErrorMessages.InsufficientFundsErrorMessage);
                }

                else if(account.DailyLimit < amount) 
                {
                    _logger.LogError("Account daily limit is not enough for withdrawing {@amount} of money. Throwing {@exceptionName} exception."
                    , amount, nameof(DailyLimitExceededException));

                    throw new DailyLimitExceededException(ExceptionErrorMessages.DailyLimitExceededErrorMessage(account.Id));
                }

                else if(account.OperationLimit < amount) 
                {
                    _logger.LogError("Account operation limit is not enough for withdrawing {@amount} of money. Throwing {@exceptionName} exception."
                    , amount, nameof(OperationLimitExceededException));

                    throw new OperationLimitExceededException(ExceptionErrorMessages.OperationLimitExceededErrorMessage(account.Id));
                }

                _logger.LogDebug("Account balance old value: {@accountBalance}", account.Balance);
                account.Balance -= amount;
                _logger.LogDebug("Account balance new value: {@accountBalance}", account.Balance);
                _logger.LogDebug("Creating record");
                Record record = CreateSuccessfullOperationRecordEntity(userId, amount, operationType, account.Id);
                _recordRepository.CreateRecord(record);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Record has been created and saved. Record: {@record}", record);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has been occured in account service {@methodName} method!. Rolling back transaction", methodName);
                _logger.LogError("Exception message: {@exceptionMessage}", ex.Message);
                transaction.Rollback();
                transaction.Dispose();
                _unitOfWork.ClearChangeTracker();
                _unitOfWork.Dispose();

                bool isPending = ex is OperationLimitExceededException;
                Record errorRecord = CreateUnSuccessfullOperationRecordEntity(userId, amount, operationType, isPending, ex.Message, account.Id);
                _logger.LogError("Record has been created for unsuccessfull {@operationName} operation. Record: {@record}", methodName, errorRecord);
                _errorRecordHandler.AddErrorRecord(errorRecord);
                throw;
            }

            var mappedAccount = _mapper.Map<ReadAccountDTO>(account);

            _logger.LogDebug("Returning mapped account: {@mappedAccount}", mappedAccount);
            return mappedAccount;

        }

        public async Task<Tuple<ReadAccountDTO, ReadAccountDTO>> TransferMoneyAsync(int senderUserId, CreateTransferMoneyDTO createTransferMoneyDTO)
        {
            string methodName = nameof(TransferMoneyAsync);
            OperationType operationType = OperationType.Transfer;
            int amount = createTransferMoneyDTO.Amount;
            int receiverAccountId = createTransferMoneyDTO.ReceiverAccountId;

            _logger.LogDebug("Inside {@serviceName} {@methodName} Method", _serviceName, methodName);

            _logger.LogDebug("Sender User id parameter is {@senderUserId}", senderUserId);
            _logger.LogDebug("Create transfer money DTO: {@createTransferMoneyDTO}", createTransferMoneyDTO);


            _logger.LogDebug("Checking database to find sender account with id {@senderUserId}", senderUserId);
            Account senderAccount = await GetAccountByUserIdOrThrowExceptionAsync(userId: senderUserId);
            _logger.LogInformation("The sender account has been found. Account: {@senderAccount}", senderAccount);

            _logger.LogDebug("Checking database to find receiver account with id {@receiverAccountId}", receiverAccountId);
            Account receiverAccount = await GetAccountByIdOrThrowExceptionAsync(accountId: receiverAccountId);
            _logger.LogInformation("The receiver account has been found. Account: {@receiverAccount}", receiverAccount);



            using var transaction = _unitOfWork.BeginTransaction();
            _logger.LogDebug("Transaction has been started.");

            try
            {
                if(senderAccount.Balance - amount < 0) 
                {
                    _logger.LogError("Sender account balance is not enough for sending {@amount} of money. Throwing {@exceptionName} exception."
                    ,amount, nameof(InsufficientFundsException));

                    throw new InsufficientFundsException(ExceptionErrorMessages.InsufficientFundsErrorMessage);
                }

                else if(senderAccount.DailyLimit < amount) 
                {
                    _logger.LogError("Sender account daily limit is not enough for sending {@amount} of money. Throwing {@exceptionName} exception."
                    , amount, nameof(OperationLimitExceededException));

                    throw new DailyLimitExceededException(ExceptionErrorMessages.DailyLimitExceededErrorMessage(senderAccount.Id));
                }

                else if(senderAccount.OperationLimit < amount) 
                {
                    _logger.LogError("Sender account operation limit is not enough for sending {@amount} of money. Throwing {@exceptionName} exception."
                    , amount, nameof(OperationLimitExceededException));

                    throw new OperationLimitExceededException(ExceptionErrorMessages.OperationLimitExceededErrorMessage(senderAccount.Id));
                }


                _logger.LogDebug("Sender account balance old value: {@senderAccountBalance}", senderAccount.Balance);
                senderAccount.Balance -= amount;
                _logger.LogDebug("Sender account balance new value: {@senderAccountBalance}", senderAccount.Balance);

                _logger.LogDebug("Receiver account balance old value: {@receiverAccountBalance}", receiverAccount.Balance);
                receiverAccount.Balance += amount;
                _logger.LogDebug("Sender account balance new value: {@receiverAccountBalance}", receiverAccount.Balance);

                _logger.LogDebug("Creating record");
                Record record = CreateSuccessfullOperationRecordEntity(senderUserId, amount, operationType, senderAccount.Id, receiverAccount.Id);
                _recordRepository.CreateRecord(record);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Record has been created and saved. Record: {@record}", record);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error has been occured in account service {@methodName} method!. Rolling back transaction", methodName);
                _logger.LogError("Exception message: {@exceptionMessage}", ex.Message);
                transaction.Rollback();
                transaction.Dispose();
                _unitOfWork.ClearChangeTracker();
                _unitOfWork.Dispose();

                bool isPending = ex is OperationLimitExceededException;

                Record errorRecord = CreateUnSuccessfullOperationRecordEntity(senderUserId, amount, operationType, isPending, ex.Message, 
                senderAccount.Id, receiverAccount.Id);

                _logger.LogError("Record has been created for unsuccessfull {@operationName} operation. Record: {@record}", methodName, errorRecord);
                _errorRecordHandler.AddErrorRecord(errorRecord);
                throw;
            }

            ReadAccountDTO mappedSenderAccount = _mapper.Map<ReadAccountDTO>(senderAccount);
            ReadAccountDTO mappedReceiverAccount = _mapper.Map<ReadAccountDTO>(receiverAccount);

            _logger.LogInformation("Returning mapped account: {@mappedSenderAccount}", mappedSenderAccount);
            _logger.LogInformation("Returning mapped account: {@mappedReceiverAccount}", mappedReceiverAccount);

            return new Tuple<ReadAccountDTO, ReadAccountDTO>(mappedSenderAccount, mappedReceiverAccount);

        }

        public Task<ReadAccountDTO> AddAutomaticBillPaymentAsync(int userId, string billNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<ReadAccountDTO> GetAccountByIdAsync(int accountId)
        {
            Account account = await GetAccountByIdOrThrowExceptionAsync(accountId);

            ReadAccountDTO mappedAccount = _mapper.Map<ReadAccountDTO>(account);

            _logger.LogDebug("Returning mapped account: {@mappedAccount}", mappedAccount);
            return mappedAccount;
        }


        private async Task<Account> GetAccountByIdOrThrowExceptionAsync(int accountId) 
        {
            _logger.LogDebug("Inside Account Service {@methodName} Method", nameof(GetAccountByIdOrThrowExceptionAsync));
            _logger.LogDebug("Searching for account with id {@id}", accountId);

            var account = await _accountRepository.GetAccountById(accountId);

            if(account == null) 
            {
                _logger.LogError("No account with id {@id} found. Throwing {@exceptionName}, exception", accountId, nameof(AccountNotFound));
                throw new AccountNotFound(ExceptionErrorMessages.AccountNotFoundForGivenIdErrorMessage(accountId));
            }

            _logger.LogInformation("Account with id {@id} has been found!", accountId);
            _logger.LogInformation("Account: {@account}", account);

            return account;

        }


        private async Task<Account> GetAccountByUserIdOrThrowExceptionAsync(int userId) 
        {
            _logger.LogDebug("Inside Account Service {@methodName} Method", nameof(GetAccountByUserIdOrThrowExceptionAsync));
            _logger.LogDebug("Searching for account with id {@userId}", userId);

            Account? account = await _accountRepository.GetAccountByUserId(userId);

            if(account == null) 
            {
                _logger.LogError("No account with user id {@userId} found. Throwing {@exceptionName}, exception", userId, nameof(AccountNotFound));
                throw new AccountNotFound(ExceptionErrorMessages.AccountNotFoundForGivenUserIdErrorMessage(userId));
            }

            _logger.LogInformation("Account with user id {@userId} has been found!", userId);
            _logger.LogInformation("Account: {@account}", account);

            return account;
        }

        private Account CreateNewAccountEntity(int userId, int openingBalance)
        {
            Account account = new Account()
            {
                Balance = openingBalance,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                DailyLimit = 500,
                OperationLimit = 250,
            };

            return account;
        }

        private Record CreateSuccessfullOperationRecordEntity(int userId, 
        int amount, OperationType operationType, 
        int? accountId = null, int? receiverAccountId = null)
        {
            Record record = new Record()
            {
                TimeStamp = DateTime.UtcNow,
                OperationType = operationType,
                Amount = amount,
                UserId = userId,
                AccountId = accountId,
                ReceiverAccountId = receiverAccountId,
                IsSuccessfull = true,
                IsPending = false
            };

            return record;
        }

        private Record CreateUnSuccessfullOperationRecordEntity(int userId, 
        int amount, OperationType operationType, bool isPending, string? errorMessage, 
        int? accountId = null, int? receiverAccountId = null)
        {
            Record record = new Record()
            {
                TimeStamp = DateTime.UtcNow,
                OperationType = operationType,
                Amount = amount,
                UserId = userId,
                AccountId = accountId,
                ReceiverAccountId = receiverAccountId,
                IsSuccessfull = false,
                IsPending = isPending,
                ErrorMessage = errorMessage
            };

            return record;
        }

    }
}