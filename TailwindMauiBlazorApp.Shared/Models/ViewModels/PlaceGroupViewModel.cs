namespace TailwindMauiBlazorApp.Shared.Models.ViewModels;

public class PlaceGroupViewModel
{
    public string Date { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    public List<ItineraryPlaceViewModel> ItineraryPlaces { get; set; } = new();
}
