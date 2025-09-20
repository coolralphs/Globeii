using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Services;

public interface IIItineraryReservationService
{
    Task<ItineraryReservationViewModel> AddItineraryReservationAsync(ItineraryReservationViewModel model);
    Task<ItineraryReservationViewModel> AddOrModifyItineraryReservationAsync(ItineraryReservationViewModel model);
    Task<ItineraryReservationViewModel> UpdateItineraryReservationAsync(ItineraryReservationViewModel model);
    Task<bool> DeleteByIdAsync(int itineraryId, int id);
}
