namespace TailwindMauiBlazorApp.Core.Models.Enums;

public enum ReservationType
{
    [TitlePair("Eat & Drink")]
    Dining,
    [TitlePair("Travel & Transportation")]
    Travel,
    [TitlePair("Accomodation & Lodging")]
    Accomodation,
    [TitlePair("Activities & Experiences")]
    Activities,
    [TitlePair("Passes & Tickets")]
    Pass,
    [TitlePair("Other")]
    Other
}

public enum ReservationSubType
{
    // -------------------- Travel & Transportation --------------------
    [TitlePair("Cable Car / Funicular")]
    Travel_CableCar,

    [TitlePair("Car / Vehicle Rental")]
    Travel_CarRental,

    [TitlePair("Bike / Scooter")]
    Travel_BikeScooter,

    [TitlePair("Bus")]
    Travel_Bus,

    [TitlePair("Ferry / Water Transport")]
    Travel_Ferry,

    [TitlePair("Flight")]
    Travel_Flight,

    [TitlePair("Helicopter")]
    Travel_Helicopter,

    [TitlePair("Shuttle / Airport Transfer")]
    Travel_Shuttle,

    [TitlePair("Taxi / Ride-share")]
    Travel_Taxi,

    [TitlePair("Train / Rail Transport")]
    Travel_Train,

    [TitlePair("Tram / Streetcar / Trolley")]
    Travel_Tram,

    [TitlePair("Other")]
    Travel_Other,

    // -------------------- Accommodation & Lodging --------------------
    [TitlePair("Hotel / Resort")]
    Accomodation_Hotel,

    [TitlePair("Hostel / Dorm")]
    Accomodation_Hostel,

    [TitlePair("Vacation Rental / Airbnb / Cottage")]
    Accomodation_VacationRental,

    [TitlePair("Bed & Breakfast / Inn")]
    Accomodation_BnB,

    [TitlePair("Camping / Glamping")]
    Accomodation_Camping,

    [TitlePair("Other")]
    Accomodation_Other,

    // -------------------- Activities & Experiences --------------------
    [TitlePair("Adventure Activity")]
    Activities_Adventure,

    [TitlePair("Brewery / Distillery / Wine Tour")]
    Activities_BreweryTour,

    [TitlePair("Art Class / Workshop")]
    Activities_ArtClass,

    [TitlePair("Cooking Class / Food Tasting")]
    Activities_CookingClass,

    [TitlePair("Cruise Excursion")]
    Activities_Cruise,

    [TitlePair("Private Guide / Custom Experience")]
    Activities_PrivateGuide,

    [TitlePair("Spa / Wellness")]
    Activities_Spa,

    [TitlePair("Sports / Recreation")]
    Activities_Sports,

    [TitlePair("Wellness Retreats")]
    Activities_WellnessRetreat,

    [TitlePair("Other")]
    Activities_Other,

    // -------------------- Passes & Tickets --------------------
    [TitlePair("City Pass")]
    Pass_CityPass,

    [TitlePair("Museum Pass")]
    Pass_MuseumPass,

    [TitlePair("Concert / Show Ticket")]
    Pass_ConcertTicket,

    [TitlePair("Theme Park / Attraction Ticket")]
    Pass_ThemeParkTicket,

    [TitlePair("Public Transport / Metro Pass")]
    Pass_PublicTransport,

    [TitlePair("Tour / Experience Ticket")]
    Pass_TourTicket,

    [TitlePair("Sports Event Ticket")]
    Pass_SportsEvent,

    [TitlePair("Other")]
    Pass_Other
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

public static class ReservationTypeExtensions
{
    public static string GetTitle(this ReservationType type)
    {
        var typeInfo = type.GetType().GetField(type.ToString());
        var attr = typeInfo.GetCustomAttributes(typeof(TitlePairAttribute), false)
                           .FirstOrDefault() as TitlePairAttribute;
        return attr?.ScreenTitle ?? type.ToString();
    }
    public static string GetIconClass(this ReservationType type)
    {
        return type switch
        {
            ReservationType.Dining => "fa fa-cutlery me-2",
            ReservationType.Travel => "fa fa-plane me-1",
            ReservationType.Accomodation => "fa fa-bed me-1",
            ReservationType.Activities => "fa fa-binoculars me-2",
            ReservationType.Pass => "fa fa-qrcode me-2",
            ReservationType.Other => "fa fa-question-circle me-2",
            _ => "fa fa-question-circle me-2" // fallback
        };
    }
}

public static class ReservationSubTypeExtensions
{
    public static string GetTitle(this ReservationSubType subType)
    {
        var typeInfo = subType.GetType().GetField(subType.ToString());
        var attr = typeInfo.GetCustomAttributes(typeof(TitlePairAttribute), false)
                           .FirstOrDefault() as TitlePairAttribute;
        return attr?.ScreenTitle ?? subType.ToString();
    }

    public static string GetIconClass(this ReservationSubType subType)
    {
        return subType switch
        {
            // Travel & Transportation
            ReservationSubType.Travel_CableCar => "fa-solid fa-train-subway",
            ReservationSubType.Travel_CarRental => "fa-solid fa-car",
            ReservationSubType.Travel_BikeScooter => "fa-solid fa-bicycle",
            ReservationSubType.Travel_Bus => "fa-solid fa-bus",
            ReservationSubType.Travel_Ferry => "fa-solid fa-ship",
            ReservationSubType.Travel_Flight => "fa-solid fa-plane",
            ReservationSubType.Travel_Helicopter => "fa-solid fa-helicopter",
            ReservationSubType.Travel_Shuttle => "fa-solid fa-shuttle-space",
            ReservationSubType.Travel_Taxi => "fa-solid fa-taxi",
            ReservationSubType.Travel_Train => "fa-solid fa-train",
            ReservationSubType.Travel_Tram => "fa-solid fa-tram",
            ReservationSubType.Travel_Other => "fa-solid fa-question-circle",

            // Accommodation & Lodging
            ReservationSubType.Accomodation_Hotel => "fa-solid fa-hotel",
            ReservationSubType.Accomodation_Hostel => "fa-solid fa-bed",
            ReservationSubType.Accomodation_VacationRental => "fa-solid fa-house-chimney",
            ReservationSubType.Accomodation_BnB => "fa-solid fa-house-flag",
            ReservationSubType.Accomodation_Camping => "fa-solid fa-campground",
            ReservationSubType.Accomodation_Other => "fa-solid fa-question-circle",

            // Activities & Experiences
            ReservationSubType.Activities_Adventure => "fa-solid fa-mountain",
            ReservationSubType.Activities_BreweryTour => "fa-solid fa-beer-mug-empty",
            ReservationSubType.Activities_ArtClass => "fa-solid fa-paint-brush",
            ReservationSubType.Activities_CookingClass => "fa-solid fa-utensils",
            ReservationSubType.Activities_Cruise => "fa-solid fa-ship",
            ReservationSubType.Activities_PrivateGuide => "fa-solid fa-user-tie",
            ReservationSubType.Activities_Spa => "fa-solid fa-spa",
            ReservationSubType.Activities_Sports => "fa-solid fa-football-ball",
            ReservationSubType.Activities_WellnessRetreat => "fa-solid fa-leaf",
            ReservationSubType.Activities_Other => "fa-solid fa-question-circle",

            // Fallback
            _ => "fa-solid fa-question-circle"
        };
    }
}