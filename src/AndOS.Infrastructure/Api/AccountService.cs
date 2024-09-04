using AndOS.Application.Extensions;
using AndOS.Shared.Requests.Accounts.Create;
using AndOS.Shared.Requests.Accounts.Delete;
using AndOS.Shared.Requests.Accounts.Get.GetAll;
using AndOS.Shared.Requests.Accounts.Get.GetById;
using AndOS.Shared.Requests.Accounts.Get.GetConfigByAccountId;
using AndOS.Shared.Requests.Accounts.Update;

namespace AndOS.Infrastructure.Api;

public class AccountService(HttpClient httpClient) : IAccountService
{
    public event Func<Task> OnAccountCreated;
    public event Func<Task> OnAccountUpdated;
    public event Func<Task> OnAccountDeleted;
    const string _endpoint = "Accounts";
    private readonly HttpClient _httpClient = httpClient;

    public async Task CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var response = await this._httpClient.PostAsJsonAsync(_endpoint, request, cancellationToken);
        await response.HandleResponse(cancellationToken);
        if (OnAccountCreated != null)
            await OnAccountCreated?.Invoke();
    }

    public async Task DeleteAsync(DeleteAccountRequest request, CancellationToken cancellationToken = default)
    {
        var response = await this._httpClient.DeleteAsync($"{_endpoint}?{request.ToQueryString()}", cancellationToken);
        await response.HandleResponse(cancellationToken);
        if (OnAccountDeleted != null)
            await OnAccountDeleted?.Invoke();
    }

    public async Task<List<AccountDTO>> GetAllAsync(GetAllAccontsRequest _, CancellationToken cancellationToken = default)
    {
        var response = await this._httpClient.GetAsync($"{_endpoint}", cancellationToken);
        await response.HandleResponse(cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<List<AccountDTO>>();
        return result;
    }

    public async Task<GetAccountByIdResponse> GetByIdAsync(GetAccountByIdRequest request, CancellationToken cancellationToken = default)
    {
        var response = await this._httpClient.GetAsync($"{_endpoint}/GetById?{request.ToQueryString()}", cancellationToken);
        await response.HandleResponse(cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<GetAccountByIdResponse>(cancellationToken: cancellationToken);
        return result;
    }

    public async Task UpdateAsync(UpdateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var response = await this._httpClient.PutAsJsonAsync(_endpoint, request, cancellationToken);
        await response.HandleResponse(cancellationToken);
        if (OnAccountUpdated != null)
            await OnAccountUpdated?.Invoke();
    }

    public Task SaveAsync(AccountDTO file, string content, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetConfigAsync(GetConfigByAccountIdRequest request, CancellationToken cancellationToken = default)
    {
        var url = $"{_endpoint}/Config?{request.ToQueryString()}";
        var response = await this._httpClient.GetAsync(url, cancellationToken);
        await response.HandleResponse(cancellationToken);
        var result = await response.Content.ReadAsStringAsync(cancellationToken);
        return result;
    }
}
