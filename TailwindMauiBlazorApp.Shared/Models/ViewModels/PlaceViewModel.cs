using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailwindMauiBlazorApp.Shared.Models.ViewModels;

public class PlaceViewModel
{
    public int? Id { get; set; }
    public string GooglePlaceId { get; set; }
    public string? DisplayName { get; set; }
    public string? FormattedAddress { get; set; }
    public List<AddressComponentViewModel> AddressComponents { get; set; }
    public GeoLocationViewModel Location { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public string? Locality { get; set; }
    public string? RegionCode { get; set; }
    public OpeningHoursViewModel RegularOpeningHours { get; set; }
    public string? ThumbnailUrl { get; set; }
    public PostalAddressViewModel PostalAddress { get; set; }
    public double? Rating { get; set; }
    public int? UserRatingCount { get; set; }
    public string? PrimaryType { get; set; }
    public string? PrimaryTypeDisplayName { get; set; }
    public List<string>? Types { get; set; }
}

public class AddressComponentViewModel
{
    public string LongText { get; set; }
    public string ShortText { get; set; }
    public List<string> Types { get; set; }
}
