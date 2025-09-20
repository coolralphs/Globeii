using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TailwindMauiBlazorApp.Shared.Models.Enums;

public enum MenuOption
{
    //ItineraryPlaces.razor
    [TitlePair("Add A Place")]
    Places,
    [TitlePair("Add An Accomodations")]
    Accomodations,
    [TitlePair("Edit Accomodation")]
    AccomodationEditor,
    [TitlePair("Reservations")]
    Reservations,
    [TitlePair("Edit Schedule")]
    ScheduleEditor,
    [TitlePair("View Schedule")]
    ScheduleViewer,
    [TitlePair("View Schedule")]
    ScheduleCardsViewer,
    [TitlePair("Add Flight Details")]
    Flights,
    [TitlePair("Add Transportation Details")]
    Transportation,
    [TitlePair("Add Expense Details")]
    Expenses,
    [TitlePair("Add Checklists")]
    Checklists,
    [TitlePair("Add Notes")]
    Notes,
    [TitlePair("Delete Itinerary")]
    Delete,

    //ItineraryEditor.razor
    [TitlePair("Adjust Date/Time")]
    Adjust,
    [TitlePair("Move")]
    Move,
    [TitlePair("Swap Date/Time")]
    Swap,
    [TitlePair("Shift Time Starting Here")]
    ShiftStarting,
    [TitlePair("Shift Time Ending Here")]
    ShiftEnding,
    [TitlePair("Edit Details")]
    EditDetails,
    [TitlePair("Remove From Itinerary")]
    Remove,
    [TitlePair("Save For Later")]
    SaveForLater,
    [TitlePair("Add Saved Items")]
    SavedItemEditMode,
    [TitlePair("Saved Items")]
    SavedItemViewMode
}

[AttributeUsage(AttributeTargets.Field)]
public class TitlePairAttribute : Attribute
{
    public string ScreenTitle { get; }

    public TitlePairAttribute(string title)
    {
        ScreenTitle = title;
    }
}

public static class EnumExtensions
{
    public static TitlePairAttribute Details(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null) return null;

        return (TitlePairAttribute?)Attribute.GetCustomAttribute(field, typeof(TitlePairAttribute));
    }
    public static string ScreenTitle(this Enum value)
    {
        return value.Details()?.ScreenTitle ?? value.ToString();
    }
}