 using TailwindMauiBlazorApp.Core.Models.Enums;

namespace TailwindMauiBlazorApp.Core.Models.Entities;

public class ItineraryReservation : IAuditableEntity
{
    public int Id { get; set; }
    public int ItineraryId { get; set; }
    public Itinerary? Itinerary { get; set; }
    public string? BookingReference { get; set; }
    public string? ConfirmationNumber { get; set; }
    public int? PlaceId { get; set; }
    public Place? Place { get; set; }
    public string? ProviderName { get; set; }
    public int? DeparturePlaceId { get; set; }
    public Place? DeparturePlace { get; set; }
    public int? DestinationPlaceId { get; set; }
    public Place? DestinationPlace { get; set; }
    public string? PickupLocation { get; set; }
    public string? DropOffLocation { get; set; }
    public string? Url { get; set; }
    public bool IsAvailableInApp { get; set; }
    public bool HasMeal { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public DateTime BookingDate { get; set; }
    public ReservationType ReservationType { get; set; }
    public ReservationSubType? ReservationSubType { get; set; }
    public int PartySize { get; set; } = 1;
    public string? Notes { get; set; }
    public ItineraryReservationStatus Status { get; set; } = ItineraryReservationStatus.Confirmed;
    public Guid CreatedBy { get; set; } // foreign key to AppUser.Id
    public AppUser? Creator { get; set; } // navigation property
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public AppUser? Updator { get; set; } // navigation property
    public Guid? UpdatedBy { get; set; } // foreign key to AppUser.Id
    public DateTime? UpdatedAt { get; set; } 
}

