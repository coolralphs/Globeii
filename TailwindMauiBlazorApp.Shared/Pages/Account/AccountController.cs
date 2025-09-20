using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Core.Models.Entities;

namespace TailwindMauiBlazorApp.Shared.Pages.Account;

//[Route("[controller]/[action]")]
[Route("api/account/[action]")]
public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string returnUrl = "/")
    {
        var redirectUrl = Url.Content(returnUrl);
        var props = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };

        // Force Google to show account selection every time
        props.Items["prompt"] = "select_account";

        return Challenge(props, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete(".AspNetCore.Cookies");

        // Add cache control headers to prevent caching
        Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "0";

        var redirectPageUrl = Url.Action("LogoutRedirect", "Account", null, Request.Scheme);
        return Redirect(redirectPageUrl);
    }

    [HttpGet]
    public IActionResult LogoutRedirect()
    {
        // This page will hit Google logout and then return the user to login page
        var loginUrl = Url.Action("Login", "Account", null, Request.Scheme);

        var html = $@"
            <html>
            <head>
                <title>Logging out...</title>
            </head>
            <body>
                <p>Logging out from Google...</p>
                <iframe src='https://accounts.google.com/Logout' style='display:none;'></iframe>
                <script>
                    // Wait a short moment for Google logout to complete
                    setTimeout(function() {{
                        window.location.href = '{loginUrl}';
                    }}, 1000);
                </script>
            </body>
            </html>";
        return Content(html, "text/html");
    }

    private readonly AppDbContext _db;

    public AccountController(AppDbContext db)
    {
        _db = db;
    }
    [HttpGet("signin-google")]
    public async Task<IActionResult> GoogleResponse()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
            return Redirect("/account/login"); // or handle error

        var claims = authenticateResult.Principal.Claims.ToList();

        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        // Check if user exists in AppUser table
        var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                DisplayName = name,
                CreatedAt = DateTime.UtcNow
            };

            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();
        }

        // Issue new identity with AppUserId claim
        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, user.DisplayName ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim("AppUserId", user.Id.ToString()) // <--- important
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Redirect("/");
    }
}