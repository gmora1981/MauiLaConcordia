using MauiLaConcordia.Web.Components;
using MauiLaConcordia.Shared.Services;
using MauiLaConcordia.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using MauiLaConcordia.Shared.Auth;
using MauiLaConcordia.Shared.Helpers;
using MauiLaConcordia.Shared.Repository;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the MauiLaConcordia.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// IMPORTANTE: Añade el HttpClient aquí, antes de los servicios que lo usan
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:2102/") });

// Agrega los servicios de autorización
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

// Migra los servicios de autenticación
builder.Services.AddScoped<TokenRenewer>();
builder.Services.AddScoped<JWTAuthenticationStateProvider>();
builder.Services.AddScoped<ITokenStorage, BrowserTokenStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, JWTAuthenticationStateProvider>(
    provider => provider.GetRequiredService<JWTAuthenticationStateProvider>());
builder.Services.AddScoped<ILoginService, JWTAuthenticationStateProvider>(
    provider => provider.GetRequiredService<JWTAuthenticationStateProvider>());

// Registra los otros servicios que puedas necesitar
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IAccountsRepository, AccountsRepository>();
builder.Services.AddScoped<GenericoRepositorio>();
builder.Services.AddScoped<IGenericoRepositorio, GenericoRepositorio>();
builder.Services.AddScoped<IUsersRepository, UserRepository>();
//builder.Services.AddScoped<IDisplayMessage, DisplayMessage>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(MauiLaConcordia.Shared._Imports).Assembly,
        typeof(MauiLaConcordia.Web.Client._Imports).Assembly);

app.Run();