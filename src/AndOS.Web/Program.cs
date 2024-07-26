using AndOS.Infrastructure;
using AndOS.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

// Adiciona o arquivo de configuração appsettings.json
Console.WriteLine(builder.HostEnvironment.Environment);
builder.Configuration.AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json");

// Configurações adicionais
WebAssemblyHostConfiguration configuration = builder.Configuration;

// Adiciona serviços de infraestrutura
builder.Services.AddInfrastructureServices(configuration);

// Adiciona componentes raiz
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Executa o aplicativo
await builder.Build().RunAsync();
