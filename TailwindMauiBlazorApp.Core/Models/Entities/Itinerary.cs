using System.ComponentModel.DataAnnotations;

namespace TailwindMauiBlazorApp.Core.Models.Entities;

public class Itinerary
{
    public int Id { get; set; }
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; } 
    public DateTime? EndDate { get; set; }
    public TimeSpan ConfigStartTime { get; set; } = TimeSpan.FromHours(9);
    public int ConfigTimeIncrement { get; set; } = 60;//in minutes
    public Guid CreatedBy { get; set; } // foreign key to AppUser.Id
    public AppUser? Creator { get; set; } // navigation property
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public AppUser? Updator { get; set; } // navigation property
    public Guid? UpdatedBy { get; set; } // foreign key to AppUser.Id
    public DateTime? UpdatedAt { get; set; } 

    public ICollection<ItineraryAccomodation> ItineraryAccomodations { get; set; } = new List<ItineraryAccomodation>();
    public ICollection<ItineraryPlace> ItineraryPlaces { get; set; } = new List<ItineraryPlace>();
    public ICollection<ItineraryReservation> ItineraryReservations { get; set; } = new List<ItineraryReservation>();
}