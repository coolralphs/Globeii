using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using TailwindMauiBlazorApp.Core.Models.ValueObjects;

namespace TailwindMauiBlazorApp.Core.Models.Entities;

public class Place
{
    public int Id { get; set; }
    public string? GooglePlaceId { get; set; }
    public string? DisplayName { get; set; }
    public string? FormattedAddress { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    // Backing string field saved in DB
    public string? OpeningHoursJson { get; set; }
    [NotMapped]
    public OpeningHours OpeningHours
    {
        get => string.IsNullOrEmpty(OpeningHoursJson)
            ? null
            : JsonSerializer.Deserialize<OpeningHours>(OpeningHoursJson);

        set => OpeningHoursJson = JsonSerializer.Serialize(value);
    }
    public string? ThumbnailUrl { get; set; }
    // Start address info
    public string? AdministrativeArea { get; set; }
    public string? Locality { get; set; }
    public string? PostalCode { get; set; }
    public string? RegionCode { get; set; }
    // End address info
    public double? Rating { get; set; }
    public int? UserRatingCount { get; set; }
    public string? PrimaryType { get; set; }
    public string? PrimaryTypeDisplayName { get; set; }
    public string[]? Types { get; set; }
}

