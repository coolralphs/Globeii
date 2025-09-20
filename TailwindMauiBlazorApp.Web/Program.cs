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

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TailwindMauiBlazorApp.Core")));



// Add device-specific services used by the TailwindMauiBlazorApp.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped<IIItineraryService, IItineraryService>();
builder.Services.AddScoped<IIItineraryAccomodationService, IItineraryAccomodationService>();
builder.Services.AddScoped<IIItineraryPlaceService, IItineraryPlaceService>();
builder.Services.AddScoped<IIItineraryReservationService, IItineraryReservationService>();
builder.Services.AddScoped<IIPlaceService, IPlaceService>();

builder.Services.AddScoped<JsInterop>();
builder.Services.AddBlazorBootstrap();

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie(options =>
//{
//    options.Cookie.Name = ".AspNetCore.Cookies"; // ensure it matches the cookie you're deleting manually
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SameSite = SameSiteMode.Lax; // or None with Secure = true if cross-site
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS required
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//    options.SlidingExpiration = true;

//    options.LoginPath = "/Account/Login";
//    options.LogoutPath = "/Account/Logout";

//    options.Events.OnSigningOut = context =>
//    {
//        context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
//        context.Response.Headers["Pragma"] = "no-cache";
//        context.Response.Headers["Expires"] = "0";
//        return Task.CompletedTask;
//    };
//})
//.AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//    googleOptions.CallbackPath = "/signin-google";
//});

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie(options =>
//{
//    options.Cookie.Name = ".AspNetCore.Cookies";
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SameSite = SameSiteMode.None;   // must be None for OAuth cross-site redirect
//    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // only for localhost
//})
//.AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//    googleOptions.CallbackPath = "/signin-google"; // must match Google console
//});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Cookies"; // ensure it matches the cookie you're deleting manually
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None; // or None with Secure = true if cross-site
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS required
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
        // Handle correlation failures or other errors gracefully
        ctx.Response.Redirect("/"); // or "/" if you prefer
        ctx.HandleResponse(); // suppress exception
        return Task.CompletedTask;
    };
    googleOptions.Events.OnCreatingTicket = async ctx =>
    {
        var db = ctx.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

        var provider = "Google";
        var providerUserId = ctx.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var email = ctx.Principal.FindFirstValue(ClaimTypes.Email)!;
        var displayName = ctx.Principal.Identity?.Name;
        var avatarUrl = ctx.Principal.FindFirstValue("urn:google:picture") ?? ctx.Principal.FindFirstValue("picture");

        // Look up login by provider + providerUserId (not just email!)
        var login = await db.UserLogins
            .Include(l => l.AppUser)
            .FirstOrDefaultAsync(l => l.Provider == provider && l.ProviderUserId == providerUserId);

        AppUser appUser;

        if (login != null)
        {
            // Existing login — update fields if changed
            appUser = login.AppUser!;
            login.ProviderDisplayName = displayName;
            login.ProviderAvatarUrl = avatarUrl;
            login.ProviderEmail = email; // <-- add this

            db.UserLogins.Update(login);
            await db.SaveChangesAsync();
        }
        else
        {
            // Check if AppUser exists by email
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

            // Create new UserLogin
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

        // Add AppUserId claim
        var identity = (ClaimsIdentity)ctx.Principal.Identity!;
        identity.AddClaim(new Claim("AppUserId", appUser.Id.ToString()));
    };
});

builder.Services.AddAuthorization();
//builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>(); // optional, for easier injection
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie(options =>
//{
//    options.Cookie.Name = ".AspNetCore.Cookies"; // ensure it matches the cookie you're deleting manually
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SameSite = SameSiteMode.Lax; // or None with Secure = true if cross-site
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS required
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//    options.SlidingExpiration = true;

//    options.LoginPath = "/Account/Login";
//    options.LogoutPath = "/Account/Logout";

//    options.Events.OnSigningOut = context =>
//    {
//        context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
//        context.Response.Headers["Pragma"] = "no-cache";
//        context.Response.Headers["Expires"] = "0";
//        return Task.CompletedTask;
//    };
//})
//.AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//});

builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
//builder.Services.AddAuthenticationCore();

builder.Services.AddControllers();

builder.Services.AddHttpClient();

// Register the service with a factory to pass the API key
builder.Services.AddSingleton<GooglePlacesService>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var apiKey = builder.Configuration["GoogleApiKey"] ?? throw new Exception("Missing Google API key");
    return new GooglePlacesService(httpClient, apiKey);
});

// Add AutoMapper and provide the config inline
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

//builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();  // <-- must be before Authorization
app.UseAuthorization();
app.MapControllers();

//app.MapGet("/account/login", async context =>
//{
//    var props = new AuthenticationProperties
//    {
//        RedirectUri = "/"
//    };
//    await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, props);
//});

//app.MapGet("/account/logout", async context =>
//{
//    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//    context.Response.Redirect("/");
//});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(TailwindMauiBlazorApp.Shared._Imports).Assembly);

app.Run();
