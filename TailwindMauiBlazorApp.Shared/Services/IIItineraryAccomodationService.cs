using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Services;

public interface IIItineraryAccomodationService
{
    Task<ItineraryAccomodationViewModel> AddItineraryAccomodationAsync(ItineraryAccomodationViewModel model);
    Task<List<ItineraryAccomodationViewModel>> AddOrUpdateItineraryAccomodationsAsync(int itineraryId,List<ItineraryAccomodationViewModel> models);
    Task<bool> DeleteByIdAsync(int itineraryId, int placeId);
}