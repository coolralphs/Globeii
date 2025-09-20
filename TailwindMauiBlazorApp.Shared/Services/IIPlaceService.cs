using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Services;

public interface IIPlaceService
{
    Task<Place> AddOrGetPlaceAsync(Place place);
    Task<PlaceViewModel?> GetPlaceByGooglePlaceIdAsync(string googlePlaceId);
}
