using Microsoft.EntityFrameworkCore;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;
using AutoMapper;

namespace TailwindMauiBlazorApp.Shared.Services;

public class ItineraryService : IIItineraryService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ItineraryService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ItineraryViewModel> AddAsync(ItineraryViewModel model)
    {
        var entity = _mapper.Map<Itinerary>(model);

        _db.Itineraries.Add(entity);
        await _db.SaveChangesAsync();

        return _mapper.Map<ItineraryViewModel>(entity);
    }

    public async Task<ItineraryViewModel> UpdateAsync(ItineraryViewModel model)
    {
        var entity = await _db.Itineraries
            .Include(i => i.ItineraryPlaces)
            .Include(i => i.ItineraryReservations)
            .FirstOrDefaultAsync(i => i.Id == model.Id);

        if (entity == null)
            throw new Exception("Itinerary not found");

        _mapper.Map(model, entity);

        await _db.SaveChangesAsync();
        return _mapper.Map<ItineraryViewModel>(entity);
    }

    public async Task<bool> DeleteByIdAsync(int itineraryId)
    {
        var entity = await _db.Itineraries.FindAsync(itineraryId);
        if (entity == null) return false;

        _db.Itineraries.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ItineraryViewModel?> GetByIdAsync(int itineraryId, Guid userId)
    {
        var entity = await _db.Itineraries
            .Include(i => i.ItineraryAccomodations)
                .ThenInclude(ia => ia.Place)
            .Include(i => i.ItineraryPlaces)
                .ThenInclude(ip => ip.Place)
            .Include(i => i.ItineraryReservations)
                .ThenInclude(ir => ir.Place)
            .Include(i => i.ItineraryReservations)
                .ThenInclude(ir => ir.DeparturePlace)
            .Include(i => i.ItineraryReservations)
                .ThenInclude(ir => ir.DestinationPlace)
            .FirstOrDefaultAsync(i => i.Id == itineraryId && i.CreatedBy == userId);

        if (entity == null)
            return null;

        var vm = _mapper.Map<ItineraryViewModel>(entity);

        for (int i = 0; i < entity.ItineraryPlaces.Count; i++)
        {
            var placeEntity = entity.ItineraryPlaces.ElementAt(i).Place;
            var placeVm = vm.ItineraryPlaces.ElementAt(i).Place;

            placeVm.RegularOpeningHours = placeEntity?.OpeningHours != null
                ? new OpeningHoursViewModel
                {
                    WeekdayDescriptions = placeEntity.OpeningHours.WeekdayDescriptions.ToList()
                }
                : new OpeningHoursViewModel { WeekdayDescriptions = new List<string>() };
        }

        return vm;
    }
}