namespace TailwindMauiBlazorApp.Core.Models.Entities;

public class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Email { get; set; } = null!;

    public string? DisplayName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();
}