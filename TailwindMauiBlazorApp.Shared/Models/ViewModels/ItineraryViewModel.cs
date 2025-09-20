using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailwindMauiBlazorApp.Shared.Models.ViewModels;

public class ItineraryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TimeSpan ConfigStartTime { get; set; }
    public int ConfigTimeIncrement { get; set; } //in minutes
    public Guid CreatedBy { get; set; }

    // Optional: to show creator info in UI (you can create an AppUserViewModel if needed)
    // public AppUserViewModel? Creator { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<ItineraryAccomodationViewModel> ItineraryAccomodations { get; set; } = new();
    public List<ItineraryPlaceViewModel> ItineraryPlaces { get; set; } = new();
    public List<ItineraryReservationViewModel> ItineraryReservations { get; set; } = new();
}
