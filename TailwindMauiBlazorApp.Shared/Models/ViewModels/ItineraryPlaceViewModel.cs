using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TailwindMauiBlazorApp.Core.Models.Entities;
using TailwindMauiBlazorApp.Core.Models.Enums;

namespace TailwindMauiBlazorApp.Shared.Models.ViewModels;

public class ItineraryPlaceViewModel
{
    public int Id { get; set; }    
    public int ItineraryId { get; set; }
    public int PlaceId { get; set; }
    public PlaceViewModel? Place { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool WasSkipped { get; set; }
    public bool IsBooked { get; set; }
    public bool IsPaid { get; set; }
    public bool BookingRequired { get; set; }
    public bool PrePaymentRequired { get; set; }
    public string? Url { get; set; }
    public Rating? UserRating { get; set; }
    public string? Notes { get; set; }
    public ItineraryPlaceStatus Status { get; set; } = ItineraryPlaceStatus.Planned;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ItineraryPlaceViewModel Clone()
    {
        return new ItineraryPlaceViewModel
        {
            Id = this.Id,
            ItineraryId = this.ItineraryId,
            StartDate = this.StartDate,
            StartTime = this.StartTime,
            EndDate = this.EndDate,
            EndTime = this.EndTime,
            Place = this.Place,
            WasSkipped = this.WasSkipped,
            IsBooked = this.IsBooked,
            IsPaid = this.IsPaid,
            BookingRequired = this.BookingRequired,
            PrePaymentRequired = this.PrePaymentRequired,
            Url = this.Url,
            UserRating = this.UserRating,
            Notes = this.Notes,
        };
    }

    //not part of the entity
    public int OldIndex { get; set; }
    public Guid MarkerId { get; set; }

    public bool HasTimeError { get; set; }
    public string? TimeErrorMessage { get; set; }
    public bool IsSelected { get; set; }
}

