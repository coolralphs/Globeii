using System.ComponentModel.DataAnnotations;
using System.Globalization;

public class DateTimeRangeViewModel /*: IValidatableObject*/
{
    [Required]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime EndDate { get; set; } = DateTime.Today;

    [Required]
    public TimeSpan StartTime { get; set; } = new(8, 0, 0);

    [Required]
    public TimeSpan EndTime { get; set; } = new(17, 0, 0);

    // 12-hour display properties
    public string StartTime12HrDisplay => DateTime.Today.Add(StartTime).ToString("hh:mm tt");
    public string EndTime12HrDisplay => DateTime.Today.Add(EndTime).ToString("hh:mm tt");

    //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    //{
    //    // Validate date range
    //    if (StartDate > EndDate)
    //    {
    //        yield return new ValidationResult(
    //            "Start date must be before or equal to end date.",
    //            new[] { nameof(StartDate) });
    //    }

    //    if (EndDate < StartDate)
    //    {
    //        yield return new ValidationResult(
    //            "End date must be after or equal to start date.",
    //            new[] { nameof(EndDate) });
    //    }

    //    // Validate time range
    //    if (StartTime > EndTime)
    //    {
    //        yield return new ValidationResult(
    //            "Start time must be before or equal to end time.",
    //            new[] { nameof(StartTime) });
    //    }

    //    if (EndTime < StartTime)
    //    {
    //        yield return new ValidationResult(
    //            "End time must be after or equal to start time.",
    //            new[] { nameof(EndTime) });
    //    }
    //}

    public ValidationResult? ValidateStartDate()
    {
        if (StartDate > EndDate)
            return new ValidationResult("Start date must be before or equal to end date.", new[] { nameof(StartDate) });
        return ValidationResult.Success;
    }

    public ValidationResult? ValidateEndDate()
    {
        if (EndDate < StartDate)
            return new ValidationResult("End date must be after or equal to start date.", new[] { nameof(EndDate) });
        return ValidationResult.Success;
    }

    public ValidationResult? ValidateStartTime()
    {
        if (StartTime > EndTime)
            return new ValidationResult("Start time must be before or equal to end time.", new[] { nameof(StartTime) });
        return ValidationResult.Success;
    }

    public ValidationResult? ValidateEndTime()
    {
        if (EndTime < StartTime)
            return new ValidationResult("End time must be after or equal to start time.", new[] { nameof(EndTime) });
        return ValidationResult.Success;
    }
}