using AutoMapper;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity → ViewModel
        CreateMap<Itinerary, ItineraryViewModel>()
            .ForMember(dest => dest.ItineraryPlaces, opt => opt.MapFrom(src => src.ItineraryPlaces))
            .ForMember(dest => dest.ItineraryAccomodations, opt => opt.MapFrom(src => src.ItineraryAccomodations))
            .ForMember(dest => dest.ItineraryReservations, opt => opt.MapFrom(src => src.ItineraryReservations));

        CreateMap<ItineraryAccomodation, ItineraryAccomodationViewModel>()
           // No need to map StartDate/EndDate explicitly if names & types match
           .ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place));

        CreateMap<ItineraryPlace, ItineraryPlaceViewModel>()
            // No need to map StartDate/EndDate explicitly if names & types match
            .ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place));

        CreateMap<ItineraryReservation, ItineraryReservationViewModel>()
                    .ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place))
                    .ForMember(dest => dest.DeparturePlace, opt => opt.MapFrom(src => src.DeparturePlace))
                    .ForMember(dest => dest.DestinationPlace, opt => opt.MapFrom(src => src.DestinationPlace));

        CreateMap<Place, PlaceViewModel>()
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new GeoLocationViewModel
            {
                Lat = src.Lat,
                Lng = src.Lng
            }))
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src =>
                src.ThumbnailUrl != null && src.ThumbnailUrl.Length > 0
                    ? $"/api/photos/{src.Id}"     // or your photo API
                    : "https://placehold.co/400x300?text=No+Image"))
            .ForMember(dest => dest.RegularOpeningHours, opt => opt.MapFrom(src =>
            src.OpeningHours != null
                ? new OpeningHoursViewModel
                {
                    WeekdayDescriptions = src.OpeningHours.WeekdayDescriptions.ToList()
                }
                : new OpeningHoursViewModel { WeekdayDescriptions = new List<string>() }
            ));

        // ViewModel → Entity
        CreateMap<ItineraryViewModel, Itinerary>()
            .ForMember(dest => dest.ItineraryPlaces, opt => opt.Ignore())
            .ForMember(dest => dest.ItineraryAccomodations, opt => opt.Ignore())
            .ForMember(dest => dest.ItineraryReservations, opt => opt.Ignore());

        CreateMap<ItineraryAccomodationViewModel, ItineraryAccomodation>()
            .ForMember(dest => dest.Place, opt => opt.Ignore());

        CreateMap<ItineraryPlaceViewModel, ItineraryPlace>()
            .ForMember(dest => dest.Place, opt => opt.Ignore());

        CreateMap<ItineraryReservationViewModel, ItineraryReservation>()
            .ForMember(dest => dest.Place, opt => opt.Ignore())
            .ForMember(dest => dest.DeparturePlace, opt => opt.Ignore())
            .ForMember(dest => dest.DestinationPlace, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

        CreateMap<PlaceViewModel, Place>().ReverseMap();
    }
}