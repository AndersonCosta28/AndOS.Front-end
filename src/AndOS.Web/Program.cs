using AndOS.Infrastructure;
using AndOS.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Adiciona o arquivo de configura��o appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json");

// Configura��es adicionais
var configuration = builder.Configuration;

// Adiciona servi�os de infraestrutura
builder.Services.AddInfrastructureServices(configuration);

// Adiciona componentes raiz
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Executa o aplicativo
await builder.Build().RunAsync();
