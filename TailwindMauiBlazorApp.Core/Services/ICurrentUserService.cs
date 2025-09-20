using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TailwindMauiBlazorApp.Shared.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;

            var claim = user.FindFirst("AppUserId")?.Value;
            if (Guid.TryParse(claim, out var guid))
                return guid;

            return null;
        }
    }

    public string? Email
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}