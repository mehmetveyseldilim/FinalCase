
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;
using Banking.Shared.RequestParameters;

namespace Banking.Domain.Contracts
{
    public interface IAccountService
    {
        Task<IEnumerable<ReadRecordDTO>> GetUserTransactionHistoryAsync(int userId);

        Task<PagedList<ReadRecordDTO>> GetAllRecordsAsync(RecordParameters recordParameters);

        Task<ReadAccountDTO> CreateAccountAsync(int userId, CreateAccountDTO createAccountDTO);

        Task ExecutePendingRecord(int recordId);


        Task<ReadAccountDTO> GetAccountByUserIdIdAsync(int userId);


        Task<ReadAccountDTO> DepositAsync(int userId, CreateDepositDTO createDepositDTO);

        Task<ReadAccountDTO> WithdrawAsync(int userId, CreateWithdrawDTO createWithdrawDTO, int? recordId = null);

        Task<Tuple<ReadAccountDTO,ReadAccountDTO>> TransferMoneyAsync(int senderUserId, CreateTransferMoneyDTO createTransferMoneyDTO, int? recordId = null);

        Task<ReadAccountDTO> AddAutomaticBillPaymentAsync(int userId, CreateBillDTO createBillDTO);


        // For data seeding at startup
        Task CreateAccountsAsync(IEnumerable<CreateAccountDTO> createAccountsDTOs);

        Task CreateBillsAsync(IEnumerable<CreateBillDTO> createBillDTOs);



    }
}