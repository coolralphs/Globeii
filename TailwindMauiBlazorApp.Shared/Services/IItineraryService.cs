using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Core.Models.ValueObjects;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Services;

public class IItineraryService : IIItineraryService
{
    //private readonly AppDbContext _db;
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly IMapper _mapper;
    //public IItineraryService(AppDbContext db, IMapper mapper)
    //{
    //    _db = db;
    //    _mapper = mapper;
    //}
    public IItineraryService(IDbContextFactory<AppDbContext> dbFactory, IMapper mapper)
    {
        _dbFactory = dbFactory;
        _mapper = mapper;
    }

    public async Task<ItineraryViewModel> AddAsync(ItineraryViewModel model)
    {
        await using var db = _dbFactory.CreateDbContext();

        var entity = _mapper.Map<Itinerary>(model);

        db.Itineraries.Add(entity);
        await db.SaveChangesAsync();

        return _mapper.Map<ItineraryViewModel>(entity);
    }

    public async Task<ItineraryViewModel> UpdateAsync(ItineraryViewModel model)
    {
        await using var db = _dbFactory.CreateDbContext();

        var entity = await db.Itineraries
        .FirstOrDefaultAsync(i => i.Id == model.Id);

        if (entity == null)
            throw new Exception("Itinerary not found");

        _mapper.Map(model, entity); // Will NOT overwrite ItineraryPlaces

        await db.SaveChangesAsync();

        return _mapper.Map<ItineraryViewModel>(entity);
    }

    public async Task<bool> DeleteByIdAsync(int itineraryId)
    {
        await using var db = _dbFactory.CreateDbContext();
        var entity = await db.Itineraries
        .FirstOrDefaultAsync(i => i.Id == itineraryId);

        if (entity == null)
            return false;

        db.Itineraries.Remove(entity);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<ItineraryViewModel?> GetByIdAsync(int itineraryId, Guid userId)
    {
        await using var db = _dbFactory.CreateDbContext();

        Itinerary entity = new();
        try
        {

         entity = await db.Itineraries
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
        }
        catch(Exception ex)
        {

        }

        if (entity == null)
            return null;

        var vm = _mapper.Map<ItineraryViewModel>(entity);

        for (int i = 0; i < entity.ItineraryPlaces.Count; i++)
        {
            var placeEntity = entity.ItineraryPlaces.ElementAt(i).Place;
            var placeVm = vm.ItineraryPlaces.ElementAt(i).Place;

            if (placeEntity?.OpeningHours != null)
            {
                placeVm.RegularOpeningHours = new OpeningHoursViewModel
                {
                    WeekdayDescriptions = placeEntity.OpeningHours.WeekdayDescriptions.ToList()
                };
            }
            else
            {
                placeVm.RegularOpeningHours = new OpeningHoursViewModel
                {
                    WeekdayDescriptions = new List<string>()
                };
            }
        }

        return vm;
    }
}
