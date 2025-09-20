using System.Text.Json;
using TailwindMauiBlazorApp.Shared.DTOs.GoogleAPIs;

namespace TailwindMauiBlazorApp.Shared.Helpers;

public class GooglePlacesService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GooglePlacesService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<PlaceDetailsResult> GetPlaceDetailsAsync(string placeId)
    {
        var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&fields=place_id,name,formatted_address,geometry,types,photos,icon,icon_mask_base_uri,address_components,rating,user_ratings_total,business_status&key={_apiKey}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        using var stream = await response.Content.ReadAsStreamAsync();

        using var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement.GetProperty("result");

        var result = new PlaceDetailsResult
        {
            PlaceId = root.GetProperty("place_id").GetString(),
            Name = root.GetProperty("name").GetString(),
            FormattedAddress = root.GetProperty("formatted_address").GetString(),
            Lat = root.GetProperty("geometry").GetProperty("location").GetProperty("lat").GetDouble(),
            Lng = root.GetProperty("geometry").GetProperty("location").GetProperty("lng").GetDouble(),
            Rating = root.TryGetProperty("rating", out var ratingProp) ? ratingProp.GetDouble() : null,
            UserRatingsTotal = root.TryGetProperty("user_ratings_total", out var totalProp) ? totalProp.GetInt32() : null,
            BusinessStatus = root.TryGetProperty("business_status", out var bs) ? bs.GetString() : null,
            Icon = root.TryGetProperty("icon", out var iconProp) ? iconProp.GetString() : null,
            IconMaskBaseUri = root.TryGetProperty("icon_mask_base_uri", out var maskProp) ? maskProp.GetString() : null,
            Types = root.TryGetProperty("types", out var typesEl) ? typesEl.EnumerateArray().Select(x => x.GetString()).ToList() : new(),
            PhotoUrls = root.TryGetProperty("photos", out var photosEl)
                ? photosEl.EnumerateArray()
                    .Select(p => $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference={p.GetProperty("photo_reference").GetString()}&key={_apiKey}")
                    .ToList()
                : new()
        };

        // Extract address components
        if (root.TryGetProperty("address_components", out var components))
        {
            foreach (var comp in components.EnumerateArray())
            {
                var types = comp.GetProperty("types").EnumerateArray().Select(t => t.GetString()).ToList();
                var longName = comp.GetProperty("long_name").GetString();

                if (types.Contains("country")) result.Country = longName;
                else if (types.Contains("administrative_area_level_1")) result.AdminAreaLevel1 = longName;
                else if (types.Contains("administrative_area_level_2")) result.AdminAreaLevel2 = longName;
                else if (types.Contains("locality")) result.Locality = longName;
                else if (types.Contains("postal_code")) result.PostalCode = longName;
            }
        }

        return result;
    }

    public async Task<string?> GetFirstPhotoUrlAsync(string placeId, string countryCode, int maxWidth = 400)
    {
        try
        {
            // 1️⃣ Get place details including photos
            var detailsUrl = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&fields=photos&key={_apiKey}";
            var response = await _httpClient.GetAsync(detailsUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("result", out var result) ||
                !result.TryGetProperty("photos", out var photos) ||
                photos.GetArrayLength() == 0)
            {
                Console.WriteLine("No photos available for this place.");
                if (countryCode is not null)
                {
                    return $"https://flagcdn.com/{countryCode.ToLower()}.svg";
                }
                else
                {
                    return null;
                }
            }

            // 2️⃣ Get the first photo reference
            var firstPhotoReference = photos[0].GetProperty("photo_reference").GetString();

            // 3️⃣ Construct the Place Photo URL
            var photoUrl = $"https://maps.googleapis.com/maps/api/place/photo?maxwidth={maxWidth}&photoreference={firstPhotoReference}&key={_apiKey}";

            return photoUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching first photo URL: {ex.Message}");
            return null;
        }
    }
}