using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Core.Models;
using TailwindMauiBlazorApp.Core.Models.Entities;

namespace TailwindMauiBlazorApp.Shared.Services;

public class UserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AppUser> AddOrUpdateGoogleUserAsync(string providerUserId, string email, string? displayName, string? avatarUrl)
    {
        // Look for existing login by provider and providerUserId
        var login = await _db.UserLogins
            .Include(l => l.AppUser)
            .FirstOrDefaultAsync(l => l.Provider == "Google" && l.ProviderUserId == providerUserId);

        if (login != null)
        {
            // Update provider info in case it changed
            login.ProviderEmail = email;
            login.ProviderDisplayName = displayName;
            login.ProviderAvatarUrl = avatarUrl;
            login.LinkedAt = DateTime.UtcNow;

            // Optionally update AppUser display name if missing or changed
            if (!string.IsNullOrWhiteSpace(displayName) && login.AppUser.DisplayName != displayName)
            {
                login.AppUser.DisplayName = displayName;
            }

            await _db.SaveChangesAsync();
            return login.AppUser;
        }

        // No login found, check if a user with this email exists
        var user = await _db.AppUsers
            .Include(u => u.Logins)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            user = new AppUser
            {
                Email = email,
                DisplayName = displayName
            };
            _db.AppUsers.Add(user);
        }
        else
        {
            // Update display name if missing or changed
            if (!string.IsNullOrWhiteSpace(displayName) && user.DisplayName != displayName)
            {
                user.DisplayName = displayName;
            }
        }

        // Create new UserLogin
        var newLogin = new UserLogin
        {
            Provider = "Google",
            ProviderUserId = providerUserId,
            ProviderEmail = email,
            ProviderDisplayName = displayName,
            ProviderAvatarUrl = avatarUrl,
            AppUser = user,
            LinkedAt = DateTime.UtcNow
        };
        _db.UserLogins.Add(newLogin);

        await _db.SaveChangesAsync();
        return user;
    }
}