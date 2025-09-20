 using TailwindMauiBlazorApp.Core.Models.Enums;

namespace TailwindMauiBlazorApp.Core.Models.Entities;

public class ItineraryAccomodation
{
    public int Id { get; set; }
    public int ItineraryId { get; set; }
    public Itinerary? Itinerary { get; set; }
    public int PlaceId { get; set; }
    public Place? Place { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool WasSkipped { get; set; }
    public bool IsBooked { get; set; }
    public bool IsPaid { get; set; }
    public bool BookingRequired { get; set; }
    public bool PrePaymentRequired { get; set; }
    public string? Url { get; set; }
    public Rating? UserRating { get; set; }
    public string? Notes { get; set; }
    public ItineraryPlaceStatus Status { get; set; } = ItineraryPlaceStatus.Planned;
    public Guid CreatedBy { get; set; } // foreign key to AppUser.Id
    public AppUser? Creator { get; set; } // navigation property
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public AppUser? Updator { get; set; } // navigation property
    public Guid? UpdatedBy { get; set; } // foreign key to AppUser.Id
    public DateTime? UpdatedAt { get; set; } 
}

