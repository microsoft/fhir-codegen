using FhirCodeGenWeb.Client;
using FhirCodeGenWeb.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IClientPackageService, ClientPackageService>();
//builder.Services.AddSingleton<IPackageIndexService, PackageIndexService>();

await builder.Build().RunAsync();
