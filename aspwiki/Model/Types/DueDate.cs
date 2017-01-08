using System.ComponentModel.DataAnnotations;

namespace ASPWiki.Model.Types
{
    public enum DueDate
    {
        [Display(Name = "Forever")]
        Forever = 0,
        [Display(Name = "10 Minutes")]
        Minutes_10 = 1,
        [Display(Name = "30 Minutes")]
        Minutes_30 = 2,
        [Display(Name = "1 Hour")]
        Hour = 3,
        [Display(Name = "6 Hours")]
        Hour_6 = 4,
        [Display(Name = "24 Hours")]
        Day = 5,
    }

    public enum DueDateAnonymous
    {
        [Display(Name = "10 Minutes")]
        Minutes_10 = DueDate.Minutes_10,
        [Display(Name = "30 Minutes")]
        Minutes_30 = DueDate.Minutes_30,
        [Display(Name = "1 Hour")]
        Hour = DueDate.Hour,
        [Display(Name = "6 Hours")]
        Hour_6 = DueDate.Hour_6,
        [Display(Name = "24 Hours")]
        Day = DueDate.Day,
    }
}
