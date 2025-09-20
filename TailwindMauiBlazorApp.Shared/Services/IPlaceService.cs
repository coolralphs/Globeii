using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TailwindMauiBlazorApp.Core.Data;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;
using TailwindMauiBlazorApp.Shared.Pages.Itinerary;

namespace TailwindMauiBlazorApp.Shared.Services;

public class IPlaceService : IIPlaceService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public IPlaceService(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<Place> AddOrGetPlaceAsync(Place place)
    {
        try
        {

            var existing = await _dbContext.Places
                .FirstOrDefaultAsync(p => p.GooglePlaceId == place.GooglePlaceId);

            if (existing != null)
                return existing;

            _dbContext.Places.Add(place);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {

        }
        return place;
    }

    //public async Task<PlaceViewModel> AddPlaceAsync(PlaceViewModel place)
    //{
    //    try
    //    {

    //        var existing = await _dbContext.Places
    //            .FirstOrDefaultAsync(p => p.GooglePlaceId == place.GooglePlaceId);

    //        if (existing != null)
    //            //return existing;
    //            return _mapper.Map<PlaceViewModel>(existing);

    //        var place = new Place
    //        {
    //            GooglePlaceId = place.GooglePlaceId,
    //            DisplayName = place.DisplayName,
    //            FormattedAddress = placeModel.FormattedAddress,
    //            Lat = placeModel.Location.Lat,
    //            Lng = placeModel.Location.Lng,

    //            ThumbnailUrl = thumbnailUrl,

    //            Rating = placeModel.Rating,
    //            UserRatingCount = placeModel.UserRatingCount,

    //            PrimaryType = placeModel.PrimaryType,
    //            PrimaryTypeDisplayName = placeModel.PrimaryTypeDisplayName,
    //            Types = placeModel.Types?.ToArray(), // convert List<string> to string[]

    //            AdministrativeArea = GetAddressComponent(placeModel.AddressComponents, "administrative_area_level_1"),
    //            Locality = GetAddressComponent(
    //            placeModel.AddressComponents,
    //            "locality",                // most reliable city
    //            "postal_town",             // UK fallback
    //            "administrative_area_level_2" // fallback region/city
    //        ),
    //            PostalCode = GetAddressComponent(placeModel.AddressComponents, "postal_code"),
    //            RegionCode = countryCode,

    //            OpeningHoursJson = placeModel.RegularOpeningHours != null
    //    ? JsonSerializer.Serialize(placeModel.RegularOpeningHours)
    //    : null

    //        };

    //        _dbContext.Places.Add(newPlace);
    //        await _dbContext.SaveChangesAsync();
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //    return place;
    //}

    public async Task<PlaceViewModel?> GetPlaceByGooglePlaceIdAsync(string googlePlaceId)
    {
        var placeEntity = await _dbContext.Places
            .FirstOrDefaultAsync(p => p.GooglePlaceId == googlePlaceId);

        if (placeEntity == null)
            return null;

        var openingHours = placeEntity.OpeningHours;

        var viewModel = new PlaceViewModel
        {
            Id = placeEntity.Id,
            GooglePlaceId = placeEntity.GooglePlaceId ?? string.Empty,
            DisplayName = placeEntity.DisplayName,
            FormattedAddress = placeEntity.FormattedAddress,
            Lat = placeEntity.Lat,
            Lng = placeEntity.Lng,
            //Location = new GeoLocationViewModel
            //{
            //    Lat = placeEntity.Lat,
            //    Lng = placeEntity.Lng
            //},
            RegularOpeningHours = openingHours != null
        ? new OpeningHoursViewModel
        {
            WeekdayDescriptions = openingHours.WeekdayDescriptions?.ToList() ?? new List<string>()
        }
        : null,
            ThumbnailUrl = placeEntity.ThumbnailUrl,
            PostalAddress = new PostalAddressViewModel
            {
                AdministrativeArea = placeEntity.AdministrativeArea,
                Locality = placeEntity.Locality,
                PostalCode = placeEntity.PostalCode,
                RegionCode = placeEntity.RegionCode
            },
            Rating = placeEntity.Rating,
            UserRatingCount = placeEntity.UserRatingCount,
            PrimaryType = placeEntity.PrimaryType,
            PrimaryTypeDisplayName = placeEntity.PrimaryTypeDisplayName,
            Types = placeEntity.Types?.ToList() ?? new List<string>()
        };

        return viewModel;
    }
}
