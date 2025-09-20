using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailwindMauiBlazorApp.Core.Models.Entities;

public class UserLogin
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Foreign key to AppUser
    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;

    public string Provider { get; set; } = null!; // e.g., "Google", "Microsoft"
    public string ProviderUserId { get; set; } = null!; // unique ID from the provider
    public string? ProviderEmail { get; set; }
    public string? ProviderDisplayName { get; set; }
    public string? ProviderAvatarUrl { get; set; }
    public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
}