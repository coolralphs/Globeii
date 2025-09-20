using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Services;

public class IItineraryReservationService : IIItineraryReservationService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public IItineraryReservationService(AppDbContext db, IMapper mapper, ICurrentUserService currentUser)
    {
        _db = db;
        _db.CurrentUserId = currentUser.UserId;
        _mapper = mapper;
        //_currentUser = currentUser;
    }

    public async Task<ItineraryReservationViewModel> AddItineraryReservationAsync(ItineraryReservationViewModel model)
    {
        try
        {
            var entity = _mapper.Map<ItineraryReservation>(model);

            _db.ItineraryReservations.Add(entity); 
            await _db.SaveChangesAsync();
            return _mapper.Map<ItineraryReservationViewModel>(entity);
        }
        catch (Exception ex)
        {

        }
        return new ItineraryReservationViewModel();
    }

    public async Task<ItineraryReservationViewModel> AddOrModifyItineraryReservationAsync(ItineraryReservationViewModel model)
    {
        try
        {
            //_db.CurrentUserId = 

            ItineraryReservation entity;

            if (model.Id > 0) // assume Id > 0 means it exists
            {
                entity = await _db.ItineraryReservations
                                  .Include(r => r.Place)
                                  .Include(r => r.DeparturePlace)
                                  .Include(r => r.DestinationPlace)
                                  .FirstOrDefaultAsync(r => r.Id == model.Id);

                if (entity == null)
                {
                    // fallback if not found
                    entity = _mapper.Map<ItineraryReservation>(model);
                    entity.CreatedBy = model.CreatedBy;
                    _db.ItineraryReservations.Add(entity);
                }
                else
                {
                    _mapper.Map(model, entity); // update existing entity
                    entity.CreatedBy = model.CreatedBy;
                    _db.ItineraryReservations.Update(entity);
                }
            }
            else
            {
                // new reservation
                entity = _mapper.Map<ItineraryReservation>(model);
                entity.CreatedBy = model.CreatedBy;
                _db.ItineraryReservations.Add(entity);
            }
            //_db.CurrentUserId = _currentUser.UserId;
            await _db.SaveChangesAsync();
            return _mapper.Map<ItineraryReservationViewModel>(entity);
        }
        catch (Exception ex)
        {
            // optionally log ex
            return new ItineraryReservationViewModel();
        }
    }

    // Update
    public async Task<ItineraryReservationViewModel?> UpdateItineraryReservationAsync(ItineraryReservationViewModel model)
    {
        var entity = await _db.ItineraryReservations
            .FirstOrDefaultAsync(r => r.ItineraryId == model.ItineraryId && r.Id == model.Id);

        if (entity == null)
            return null;

        // Map updated fields from ViewModel to entity
        _mapper.Map(model, entity);

        await _db.SaveChangesAsync();

        return _mapper.Map<ItineraryReservationViewModel>(entity);
    }

    // Delete
    public async Task<bool> DeleteByIdAsync(int itineraryId, int id)
    {
        var entity = await _db.ItineraryReservations
            .FirstOrDefaultAsync(i => i.ItineraryId == itineraryId && i.Id == id);

        if (entity == null)
            return false;

        _db.ItineraryReservations.Remove(entity);
        await _db.SaveChangesAsync();

        return true;
    }

}