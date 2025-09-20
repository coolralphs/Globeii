using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Shared.Services;

namespace TailwindMauiBlazorApp.Tests;

public class UserServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        // Fake config with a test CurrentUserId
        var inMemorySettings = new Dictionary<string, string>
    {
        { "CurrentUserId", Guid.NewGuid().ToString() }
    };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        return new AppDbContext(options, configuration);
    }

    [Fact]
    public async Task AddOrUpdateGoogleUserAsync_AddsNewUserAndLogin()
    {
        var dbContext = GetInMemoryDbContext();
        var userService = new UserService(dbContext);

        string providerUserId = "google-test-sub-1";
        string email = "rafael.salazar@gmail.com";
        string displayName = "Rafael Salazar";
        string avatarUrl = "https://www.gravatar.com/avatar/?d=identicon";

        var user = await userService.AddOrUpdateGoogleUserAsync(
            providerUserId, email, displayName, avatarUrl);

        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Contains(user.Logins, login =>
            login.Provider == "Google" &&
            login.ProviderUserId == providerUserId &&
            login.ProviderEmail == email &&
            login.ProviderDisplayName == displayName &&
            login.ProviderAvatarUrl == avatarUrl
        );
    }
}