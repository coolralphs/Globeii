using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Services;

public interface IIItineraryPlaceService
{
    Task<ItineraryPlaceViewModel> AddItineraryPlaceAsync(ItineraryPlaceViewModel model);
    Task<List<ItineraryPlaceViewModel>> AddOrUpdateItineraryPlacesAsync(int itineraryId,List<ItineraryPlaceViewModel> models, Guid userId);
    Task<ItineraryPlaceViewModel> UpdateItineraryPlaceAsync(ItineraryPlaceViewModel model);
    Task<bool> DeleteByIdAsync(int itineraryId, int placeId);
}