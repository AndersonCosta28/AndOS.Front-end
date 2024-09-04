using AndOS.Application.Extensions;
using AndOS.Domain.Consts;
using AndOS.Infrastructure.Authentication;
using AndOS.Shared.Requests.Auth.Login;
using AndOS.Shared.Requests.Auth.Register;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Net.Http.Headers;

namespace AndOS.Infrastructure.Api;

public class AuthenticationService(
    IJSRuntime jsRuntime,
    ILogger<AuthenticationService> logger,
    HttpClient httpClient,
    NavigationManager navigationManager,
    CustomAuthenticationStateProvider authenticationStateProvider)
{
    const string _endpoint = "auth";

    public async Task Login(string email, string password, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"{_endpoint}/login", new LoginRequest() { Email = email, Password = password }, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        var responseLogin = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);

        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", responseLogin.Token);

        // Notifica o AuthenticationStateProvider sobre a autenticação do usuário
        authenticationStateProvider.NotifyUserAuthentication(responseLogin.Token);

        // Configura o HttpClient para incluir o token JWT em todas as requisições subsequentes
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseLogin.Token);

        logger.Log(LogLevel.Debug, "Redirecionando para a página de bem-vindo");
        navigationManager.NavigateTo(RoutesConsts.Welcome, true, true);
    }

    public async Task Register(string email, string userName, string password, string confirmPassword, CancellationToken cancellationToken = default)
    {
        // Verifica se a senha e a confirmação da senha são iguais
        if (password != confirmPassword)
        {
            var message = "As senhas não coincidem.";
            throw new Exception(message);
        }

        var request = new RegisterRequest() { Email = email, Password = password, UserName = userName };
        var response = await httpClient.PostAsJsonAsync($"{_endpoint}/register", request, cancellationToken: cancellationToken);
        await response.HandleResponse(cancellationToken);
        navigationManager.NavigateTo(RoutesConsts.Login);
    }

    public async Task Logout()
    {
        // Remove o token do armazenamento local
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");

        // Notifica o AuthenticationStateProvider sobre o logout do usuário
        authenticationStateProvider.NotifyUserLogout();

        navigationManager.NavigateTo(RoutesConsts.Login);

        // Remove o cabeçalho de autorização do HttpClient
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
