using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Services;

public interface IIItineraryService
{
    Task<ItineraryViewModel> AddAsync(ItineraryViewModel model);
    Task<ItineraryViewModel> UpdateAsync(ItineraryViewModel model);
    Task<bool> DeleteByIdAsync(int itineraryId);
    Task<ItineraryViewModel?> GetByIdAsync(int itineraryId, Guid userId);
}
