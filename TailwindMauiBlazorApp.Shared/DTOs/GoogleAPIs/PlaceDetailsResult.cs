using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailwindMauiBlazorApp.Shared.DTOs.GoogleAPIs;

public class PlaceDetailsResult
{
    public string PlaceId { get; set; }
    public string Name { get; set; }
    public string FormattedAddress { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public string Country { get; set; }
    public string AdminAreaLevel1 { get; set; }
    public string AdminAreaLevel2 { get; set; }
    public string Locality { get; set; }
    public string PostalCode { get; set; }
    public double? Rating { get; set; }
    public int? UserRatingsTotal { get; set; }
    public string BusinessStatus { get; set; }
    public List<string> Types { get; set; }
    public List<string> PhotoUrls { get; set; }
    public string Icon { get; set; }
    public string IconMaskBaseUri { get; set; }
}