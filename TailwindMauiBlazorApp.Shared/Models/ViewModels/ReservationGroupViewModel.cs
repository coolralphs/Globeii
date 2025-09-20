using TailwindMauiBlazorApp.Core.Models.Enums;

namespace TailwindMauiBlazorApp.Shared.Models.ViewModels;

public class ReservationGroupViewModel
{
    public ReservationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<ItineraryReservationViewModel> Items { get; set; } = new List<ItineraryReservationViewModel>();
}