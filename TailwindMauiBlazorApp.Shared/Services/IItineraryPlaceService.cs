using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Services;

public class IItineraryPlaceService : IIItineraryPlaceService
{
    //private readonly AppDbContext _db;
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly IMapper _mapper;

    //public IItineraryPlaceService(AppDbContext db, IMapper mapper)
    //{
    //    _db = db;
    //    _mapper = mapper;
    //}
    public IItineraryPlaceService(IDbContextFactory<AppDbContext> dbFactory, IMapper mapper)
    {
        _dbFactory = dbFactory;
        _mapper = mapper;
    }

    public async Task<ItineraryPlaceViewModel> AddItineraryPlaceAsync(ItineraryPlaceViewModel model)
    {
        await using var db = _dbFactory.CreateDbContext();

        var newItineraryPlace = new ItineraryPlace
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
            Status = model.Status,
            CreatedBy = model.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        db.ItineraryPlaces.Add(newItineraryPlace);
        try
        {

        await db.SaveChangesAsync();
        }
        catch(Exception ex)
        {

        }

        var existingItineraryPlace = await db.ItineraryPlaces
                .Include(p => p.Place)
                .Where(p => p.Id == newItineraryPlace.Id)
                .FirstOrDefaultAsync();

        return _mapper.Map<ItineraryPlaceViewModel>(existingItineraryPlace);
        //return itineraryPlace;
    }

    public async Task<bool> DeleteByIdAsync(int itineraryId, int id)
    {
        await using var db = _dbFactory.CreateDbContext();

        var entity = await db.ItineraryPlaces
        .FirstOrDefaultAsync(i => i.ItineraryId == itineraryId && i.Id == id);

        if (entity == null)
            return false;

        db.ItineraryPlaces.Remove(entity);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<List<ItineraryPlaceViewModel>> AddOrUpdateItineraryPlacesAsync(int itineraryId, List<ItineraryPlaceViewModel> models, Guid userId)
    {
        try
        {
            await using var db = _dbFactory.CreateDbContext();

            var updatedEntities = new List<ItineraryPlace>();

            //var currentUserId = _db.AppUsers
            //    .Where(u => u.Email == "metalpunk007@gmail.com")
            //    .Select(u => u.Id)
            //    .FirstOrDefault();

            var ids = models.Where(m => m.Id != 0).Select(m => m.Id).ToList();
            var existing = await db.ItineraryPlaces
                .Where(ip => ids.Contains(ip.Id))
                .ToDictionaryAsync(ip => ip.Id);

            foreach (var model in models)
            {
                ItineraryPlace entity;

                // If model has Id, try to fetch existing record
                if (model.Id != 0 && existing.TryGetValue(model.Id, out var found))
                {
                    entity = found;
                }
                else
                {
                    entity = new ItineraryPlace();
                    db.ItineraryPlaces.Add(entity);
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
                entity.Status = model.Status;
                updatedEntities.Add(entity);
            }

            await db.SaveChangesAsync();

            var updated = await db.ItineraryPlaces
                .Include(p => p.Place)
                .Where(p => p.ItineraryId == itineraryId)
                .ToListAsync();

            return _mapper.Map<List<ItineraryPlaceViewModel>>(updated);
        }
        catch (Exception ex)
        {
            throw;
            //return models;
        }
    }

    public async Task<ItineraryPlaceViewModel> UpdateItineraryPlaceAsync(ItineraryPlaceViewModel model)
    {
        try
        {
            if (model.Id == 0)
                throw new ArgumentException("Cannot update an entity with no Id", nameof(model));

            await using var db = _dbFactory.CreateDbContext();

            // Fetch the existing entity
            var entity = await db.ItineraryPlaces
                .FirstOrDefaultAsync(ip => ip.Id == model.Id);

            if (entity == null)
                throw new InvalidOperationException($"ItineraryPlace with Id {model.Id} not found.");

            // Map properties (do not map navigation properties)
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
            entity.Status = model.Status; // if you added Status

            await db.SaveChangesAsync();

            // Reload including navigation properties
            var updatedEntity = await db.ItineraryPlaces
                .Include(p => p.Place)
                .FirstOrDefaultAsync(p => p.Id == entity.Id);

            return _mapper.Map<ItineraryPlaceViewModel>(updatedEntity);
        }
        catch (Exception)
        {
            throw;
        }
    }
}