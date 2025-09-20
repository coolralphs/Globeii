using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailwindMauiBlazorApp.Core.Models.Enums;

namespace TailwindMauiBlazorApp.Shared.Models.ViewModels;

public class ItineraryReservationViewModel
{
    public int Id { get; set; }
    public int ItineraryId { get; set; }

    public string? BookingReference { get; set; }
    public string? ConfirmationNumber { get; set; }

    public int? PlaceId { get; set; }
    public PlaceViewModel? Place { get; set; }

    public string? ProviderName { get; set; }

    public int? DeparturePlaceId { get; set; }
    public PlaceViewModel? DeparturePlace { get; set; }

    public int? DestinationPlaceId { get; set; }
    public PlaceViewModel? DestinationPlace { get; set; }

    public string? PickupLocation { get; set; }
    public string? DropOffLocation { get; set; }

    public string? Url { get; set; }
    public bool IsAvailableInApp { get; set; }
    public bool HasMeal { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime EndDate { get; set; } = DateTime.Today;
    public TimeSpan StartTime { get; set; } = new TimeSpan(12, 0, 0);
    public TimeSpan EndTime { get; set; } = new TimeSpan(0, 0, 0);

    public DateTime BookingDate { get; set; } = DateTime.Now;
    public ReservationType ReservationType { get; set; }
    public ReservationSubType? ReservationSubType { get; set; }
    public int PartySize { get; set; } = 1;

    public string? Notes { get; set; }
    public ItineraryReservationStatus Status { get; set; } = ItineraryReservationStatus.Confirmed;

    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ItineraryReservationViewModel Clone()
    {
        return new ItineraryReservationViewModel
        {
            Id = this.Id,
            ItineraryId = this.ItineraryId,
            BookingReference = this.BookingReference,
            ConfirmationNumber = this.ConfirmationNumber,
            PlaceId = this.PlaceId,            
            Place = this.Place,
            ProviderName = this.ProviderName,
            DeparturePlaceId = this.DeparturePlaceId,
            DeparturePlace = this.DeparturePlace,
            DestinationPlaceId = this.DestinationPlaceId,
            DestinationPlace = this.DestinationPlace,
            PickupLocation = this.PickupLocation,
            DropOffLocation = this.DropOffLocation,
            Url = this.Url,
            IsAvailableInApp = this.IsAvailableInApp,
            HasMeal = this.HasMeal,
            StartDate = this.StartDate,
            StartTime = this.StartTime,
            EndDate = this.EndDate,
            EndTime = this.EndTime,
            BookingDate = this.BookingDate,
            ReservationType = this.ReservationType,
            ReservationSubType = this.ReservationSubType,
            PartySize = this.PartySize,
            Notes = this.Notes,
            Status = this.Status
        };
    }

    //public class DateGreaterThanAttribute : ValidationAttribute
    //{
    //    private readonly string _comparisonProperty;

    //    public DateGreaterThanAttribute(string comparisonProperty)
    //    {
    //        _comparisonProperty = comparisonProperty;
    //    }

    //    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    //    {
    //        var currentValue = (DateTime?)value;
    //        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

    //        if (property == null)
    //            throw new ArgumentException("Property with this name not found");

    //        var comparisonValue = (DateTime?)property.GetValue(validationContext.ObjectInstance);

    //        if (currentValue.HasValue && comparisonValue.HasValue && currentValue.Value < comparisonValue.Value)
    //        {
    //            return new ValidationResult(ErrorMessage);
    //        }

    //        return ValidationResult.Success;
    //    }
    //}
}