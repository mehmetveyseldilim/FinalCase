
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;

namespace Banking.Domain.Contracts
{
    public interface IAccountService
    {
        Task<ReadAccountDTO> CreateAccountAsync(int userId, CreateAccountDTO createAccountDTO);

        Task<IEnumerable<ReadRecordDTO>> GetUserTransactionHistoryAsync(int userId);


        Task<ReadAccountDTO> GetAccountByIdAsync(int accountId);

        Task<ReadAccountDTO> DepositAsync(int userId, CreateDepositDTO createDepositDTO);

        Task<ReadAccountDTO> WithdrawAsync(int userId, CreateWithdrawDTO createWithdrawDTO);

        Task<Tuple<ReadAccountDTO,ReadAccountDTO>> TransferMoneyAsync(int senderUserId, CreateTransferMoneyDTO createTransferMoneyDTO);

        Task<ReadAccountDTO> AddAutomaticBillPaymentAsync(int userId, CreateBillDTO createBillDTO);


        // For data seeding at startup
        Task CreateAccountsAsync(IEnumerable<CreateAccountDTO> createAccountsDTOs);

        Task CreateBillsAsync(IEnumerable<CreateBillDTO> createBillDTOs);



    }
}