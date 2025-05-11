using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MauiLaConcordia.Shared.Services;
using MauiLaConcordia.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the MauiLaConcordia.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
