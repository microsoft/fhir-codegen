using FhirCodeGenBlazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IPackageManagerWebService, PackageManagerWebService>();
builder.Services.AddSingleton<ISpecManagerWebService, SpecManagerWebService>();
builder.Services.AddSingleton<ISpecExporterWebService, SpecExporterWebService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// warm up services that take a long time to start
app.Services.GetService<IPackageManagerWebService>()?.Init();
app.Services.GetService<ISpecManagerWebService>()?.Init();
app.Services.GetService<ISpecExporterWebService>()?.Init();

app.Run();
