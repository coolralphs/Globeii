//using TailwindMauiBlazorApp.Core.Models.Entities;

using TailwindMauiBlazorApp.Shared.Models.ViewModels;

namespace TailwindMauiBlazorApp.Shared.Helpers;

public static class DateHelper
{
    public static string FormatDateRange(DateTime? start, DateTime? end)
    {
        if (!start.HasValue || !end.HasValue)
            return string.Empty; // or "Dates not set"

        var s = start.Value;
        var e = end.Value;

        if (s.Date == e.Date)
            return s.ToString("MMM d, yyyy"); // Single-day trip

        if (s.Year == e.Year)
        {
            if (s.Month == e.Month)
                return $"{s:MMM d}–{e:d, yyyy}"; // Example: Apr 3–7, 2025
            else
                return $"{s:MMM d} – {e:MMM d, yyyy}"; // Apr 28 – May 3, 2025
        }

        return $"{s:MMM d, yyyy} – {e:MMM d, yyyy}";
    }

    public static string FormatTime12Hour(TimeSpan time)
    {
        DateTime dateTime = DateTime.Today.Add(time);
        return dateTime.ToString("h:mm tt");
    }

    public static string FormatTime(TimeSpan time)
    {
        // Create a DateTime from the TimeSpan to use ToString with format
        var dt = DateTime.Today.Add(time);
        return dt.ToString("h:mm tt"); // 12-hour format, no seconds, AM/PM
    }

    public static int TotalDays(DateTime? start, DateTime? end)
    {
        if(start is null && end is null)
        {
            return 0;
        }
        var total = (end.Value - start.Value).Days + 1;

        return total;
    }

    public static readonly string[] DaysOfWeek = new[]
    {
        "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    };

    public static readonly Dictionary<string, string> DayColors = new Dictionary<string, string>
    {
        ["Monday"] = "#FF4B4B",
        ["Tuesday"] = "#4CAF50",
        ["Wednesday"] = "#8E44AD",
        ["Thursday"] = "#FF8C42",
        ["Friday"] = "#4287F5",
        ["Saturday"] = "#FFD93D",
        ["Sunday"] = "#FF66B3"
    };

    public static List<string> GetDateGaps(DateTime? StartDate, DateTime? EndDate, List<ItineraryReservationViewModel> reservations)
    {
        var gaps = new List<string>();

        var accomodations = reservations.Where(w => w.ReservationType == Core.Models.Enums.ReservationType.Accomodation).ToList();

        if (!accomodations.Any())
        {
            // Entire itinerary is a gap
            gaps.Add($"{StartDate:MM/dd/yyyy}-{EndDate:MM/dd/yyyy}");
            return gaps;
        }

        // Sort accommodations by StartDate
        var sorted = accomodations.OrderBy(a => a.StartDate).ToList();

        // Gap before first accommodation
        var firstStart = sorted.First().StartDate.Date;
        if (StartDate < firstStart)
        {
            gaps.Add($"{StartDate:MM/dd/yyyy}-{firstStart:MM/dd/yyyy}");
        }

        // Gaps between accommodations
        for (int i = 0; i < sorted.Count - 1; i++)
        {
            var currentEnd = sorted[i].EndDate.Date;
            var nextStart = sorted[i + 1].StartDate.Date;

            if (currentEnd < nextStart) // gap exists
            {
                gaps.Add($"{currentEnd:MM/dd/yyyy}-{nextStart:MM/dd/yyyy}");
            }
        }

        // Gap after last accommodation
        var lastEnd = sorted.Last().EndDate.Date;
        if (lastEnd < EndDate)
        {
            gaps.Add($"{lastEnd:MM/dd/yyyy}-{EndDate:MM/dd/yyyy}");
        }

        return gaps;
    }
}
