using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Services;

public class IItineraryAccomodationService : IIItineraryAccomodationService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public IItineraryAccomodationService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ItineraryAccomodationViewModel> AddItineraryAccomodationAsync(ItineraryAccomodationViewModel model)
    {
        var itineraryAccomodation = new ItineraryAccomodation
        {
            ItineraryId = model.ItineraryId,
            PlaceId = model.PlaceId,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            WasSkipped = model.WasSkipped,
            IsBooked = model.IsBooked,
            IsPaid = model.IsPaid,
            BookingRequired = model.BookingRequired,
            PrePaymentRequired = model.PrePaymentRequired,
            Url = model.Url,
            UserRating = model.UserRating,
            Notes = model.Notes,
            CreatedBy = model.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        _db.ItineraryAccomodations.Add(itineraryAccomodation);
        await _db.SaveChangesAsync();

        return _mapper.Map<ItineraryAccomodationViewModel>(itineraryAccomodation);
        //return itineraryPlace;
    }

    public async Task<bool> DeleteByIdAsync(int itineraryId, int placeId)
    {
        var entity = await _db.ItineraryAccomodations
        .FirstOrDefaultAsync(i => i.ItineraryId == itineraryId && i.PlaceId == placeId);

        if (entity == null)
            return false;

        _db.ItineraryAccomodations.Remove(entity);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<List<ItineraryAccomodationViewModel>> AddOrUpdateItineraryAccomodationsAsync(int itineraryId, List<ItineraryAccomodationViewModel> models)
    {
        try
        {
            var updatedEntities = new List<ItineraryAccomodation>();

            var ids = models.Where(m => m.Id != 0).Select(m => m.Id).ToList();
            var existing = await _db.ItineraryAccomodations
                .Where(ip => ids.Contains(ip.Id))
                .ToDictionaryAsync(ip => ip.Id);

            foreach (var model in models)
            {
                ItineraryAccomodation entity;

                // If model has Id, try to fetch existing record
                if (model.Id != 0 && existing.TryGetValue(model.Id, out var found))
                {
                    entity = found;
                }
                else
                {
                    entity = new ItineraryAccomodation();
                    _db.ItineraryAccomodations.Add(entity);
                }

                // Map all properties except Place navigation to avoid tracking conflicts
                // Assuming your AutoMapper profile ignores Place or you map manually here:
                entity.ItineraryId = itineraryId;
                //entity.Place = null;
                entity.PlaceId = model.Place.Id.Value; // <-- assign FK directly, do NOT map Place nav property
                entity.StartDate = model.StartDate;
                entity.EndDate = model.EndDate;
                entity.StartTime = model.StartTime;
                entity.EndTime = model.EndTime;
                entity.WasSkipped = model.WasSkipped;
                entity.IsBooked = model.IsBooked;
                entity.IsPaid = model.IsPaid;
                entity.BookingRequired = model.BookingRequired;
                entity.PrePaymentRequired = model.PrePaymentRequired;
                entity.Url = model.Url;
                entity.UserRating = model.UserRating;
                entity.Notes = model.Notes;
                updatedEntities.Add(entity);
            }

            await _db.SaveChangesAsync();

            return _mapper.Map<List<ItineraryAccomodationViewModel>>(updatedEntities);
        }
        catch (Exception ex)
        {
            throw;
            //return models;
        }
    }
}