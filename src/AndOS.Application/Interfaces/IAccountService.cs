using AndOS.Shared.Requests.Accounts.Create;
using AndOS.Shared.Requests.Accounts.Delete;
using AndOS.Shared.Requests.Accounts.Get.GetAll;
using AndOS.Shared.Requests.Accounts.Get.GetById;
using AndOS.Shared.Requests.Accounts.Get.GetConfigByAccountId;
using AndOS.Shared.Requests.Accounts.Update;

namespace AndOS.Application.Interfaces;

public interface IAccountService : IService
{
    public event Func<Task> OnAccountCreated;
    public event Func<Task> OnAccountUpdated;
    public event Func<Task> OnAccountDeleted;

    Task<List<AccountDTO>> GetAllAsync(GetAllAccontsRequest request, CancellationToken cancellationToken = default);
    Task<GetAccountByIdResponse> GetByIdAsync(GetAccountByIdRequest request, CancellationToken cancellationToken = default);
    Task<string> GetConfigAsync(GetConfigByAccountIdRequest request, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateAccountRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteAccountRequest request, CancellationToken cancellationToken = default);


    Task SaveAsync(AccountDTO file, string content, CancellationToken cancellationToken = default);
}
