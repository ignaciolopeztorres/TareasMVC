using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TareasMVC;

var builder = WebApplication.CreateBuilder(args);

//aplicacndo filtros de autenticacion
var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Add services to the container.
builder.Services.AddControllersWithViews(option =>
{
    option.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
});
builder.Services.AddDbContext<ApplicationDbContext>(option =>
    option.UseSqlServer("name=DefaultConnection"));

//configurando identity en la aplicacion
builder.Services.AddAuthentication().AddMicrosoftAccount(opciones =>
{
    opciones.ClientId = builder.Configuration["MicrosoftClientId"];
    opciones.ClientSecret = builder.Configuration["MicrosoftSecretId"];
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>(option =>
{
    option.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones =>
{
    opciones.LoginPath = "/Usuarios/login";
    opciones.AccessDeniedPath = "/Usuarios/login";
});

builder.Services.AddLocalization();

var app = builder.Build();

var culturasUISoportadas = new[] { "es-MX", "en-US" };

app.UseRequestLocalization( opciones => { 
    opciones.DefaultRequestCulture = new RequestCulture("es-MX");
    opciones.SupportedUICultures = culturasUISoportadas
        .Select(cultura => new CultureInfo(cultura))
        .ToList();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();