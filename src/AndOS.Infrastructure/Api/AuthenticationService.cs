using AndOS.Application.Extensions;
using AndOS.Domain.Consts;
using AndOS.Infrastructure.Authentication;
using AndOS.Shared.Requests.Auth;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;

namespace AndOS.Infrastructure.Api;

public class AuthenticationService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    private readonly CustomAuthenticationStateProvider _authenticationStateProvider;

    public AuthenticationService(
        ILocalStorageService localStorage,
        ILogger<AuthenticationService> logger,
        HttpClient httpClient,
        NavigationManager navigationManager,
        CustomAuthenticationStateProvider authenticationStateProvider)
    {
        _localStorage = localStorage;
        _logger = logger;
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _authenticationStateProvider = authenticationStateProvider;
    }

    const string _endpoint = "auth";

    public async Task Login(string email, string password, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_endpoint}/login", new LoginRequest() { Email = email, Password = password });
        response.EnsureSuccessStatusCode();

        // Extrai o token JWT da resposta
        var token = await response.Content.ReadAsStringAsync(cancellationToken);

        // Armazena o token no local storage
        await _localStorage.SetItemAsync("authToken", token);

        // Notifica o AuthenticationStateProvider sobre a autenticação do usuário
        _authenticationStateProvider.NotifyUserAuthentication(token);

        // Configura o HttpClient para incluir o token JWT em todas as requisições subsequentes
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        _logger.LogInformation("Redirecionando para o Home");
        _navigationManager.NavigateTo(Routes.Welcome, true, true);
    }

    public async Task Register(string email, string userName, string password, string confirmPassword, CancellationToken cancellationToken = default)
    {
        // Verifica se a senha e a confirmação da senha são iguais
        if (password != confirmPassword)
        {
            string message = "As senhas não coincidem.";
            throw new Exception(message);
        }

        var request = new RegisterRequest() { Email = email, Password = password, UserName = userName };
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_endpoint}/register", request, cancellationToken: cancellationToken);
        await response.HandleResponse(cancellationToken);
        _navigationManager.NavigateTo(Routes.Login);
    }

    public async Task Logout()
    {
        // Remove o token do armazenamento local
        await _localStorage.RemoveItemAsync("authToken");

        // Notifica o AuthenticationStateProvider sobre o logout do usuário
        _authenticationStateProvider.NotifyUserLogout();

        _navigationManager.NavigateTo(Routes.Login);

        // Remove o cabeçalho de autorização do HttpClient
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
