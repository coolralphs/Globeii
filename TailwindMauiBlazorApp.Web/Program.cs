using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Helpers;
using TailwindMauiBlazorApp.Shared.Mappings;
using TailwindMauiBlazorApp.Shared.Services;
using TailwindMauiBlazorApp.Web.Components;
using TailwindMauiBlazorApp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TailwindMauiBlazorApp.Core")));

builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped<IIItineraryService, IItineraryService>();
builder.Services.AddScoped<IIItineraryAccomodationService, IItineraryAccomodationService>();
builder.Services.AddScoped<IIItineraryPlaceService, IItineraryPlaceService>();
builder.Services.AddScoped<IIItineraryReservationService, IItineraryReservationService>();
builder.Services.AddScoped<IIPlaceService, IPlaceService>();

builder.Services.AddScoped<JsInterop>();
builder.Services.AddBlazorBootstrap();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.HttpOnly = true;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;

    options.LoginPath = "/api/account/login";
    options.LogoutPath = "/api/account/logout";

    options.Events.OnSigningOut = context =>
    {
        context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";
        return Task.CompletedTask;
    };
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";

    googleOptions.Events.OnRemoteFailure = ctx =>
    {
        //ctx.Response.Redirect("/");
        ctx.HandleResponse();
        return Task.CompletedTask;
    };
    googleOptions.Events.OnCreatingTicket = async ctx =>
    {
        try
        {
            var db = ctx.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            var provider = "Google";
            var providerUserId = ctx.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var email = ctx.Principal.FindFirstValue(ClaimTypes.Email)!;
            var displayName = ctx.Principal.Identity?.Name;
            var avatarUrl = ctx.Principal.FindFirstValue("urn:google:picture") ?? ctx.Principal.FindFirstValue("picture");

            var login = await db.UserLogins
                .Include(l => l.AppUser)
                .FirstOrDefaultAsync(l => l.Provider == provider && l.ProviderUserId == providerUserId);

            AppUser appUser;

            if (login != null)
            {
                appUser = login.AppUser!;
                login.ProviderDisplayName = displayName;
                login.ProviderAvatarUrl = avatarUrl;
                login.ProviderEmail = email;

                db.UserLogins.Update(login);
                await db.SaveChangesAsync();
            }
            else
            {
                appUser = await db.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
                if (appUser == null)
                {
                    appUser = new AppUser
                    {
                        Email = email,
                        DisplayName = displayName
                    };
                    db.AppUsers.Add(appUser);
                    await db.SaveChangesAsync();
                }

                login = new UserLogin
                {
                    AppUserId = appUser.Id,
                    Provider = provider,
                    ProviderUserId = providerUserId,
                    ProviderEmail = email,
                    ProviderDisplayName = displayName,
                    ProviderAvatarUrl = avatarUrl
                };
                db.UserLogins.Add(login);
                await db.SaveChangesAsync();
            }

            var identity = (ClaimsIdentity)ctx.Principal.Identity!;
            identity.AddClaim(new Claim("AppUserId", appUser.Id.ToString()));
        }
        catch(Exception ex)
        {

        }
        
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services.AddControllers();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<GooglePlacesService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var apiKey = builder.Configuration["GoogleApiKey"] ?? throw new Exception("Missing Google API key");
    return new GooglePlacesService(httpClient, apiKey);
});

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.Cookie.SameSite = SameSiteMode.None;
//    options.Cookie.HttpOnly = true;
//});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.Migrate();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(TailwindMauiBlazorApp.Shared._Imports).Assembly);

//var port = Environment.GetEnvironmentVariable("PORT") ?? "7164";
//app.Urls.Clear();
//app.Urls.Add($"http://*:{port}");

app.Run();
