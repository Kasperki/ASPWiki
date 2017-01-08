using System.ComponentModel.DataAnnotations;

namespace ASPWiki.Model.Types
{
    public enum Label
    {
        Empty = 0,
        Web = 1,
        Games = 2,
        Other = 3,
        Info = 4,
        [Display(Name = "Important")]
        Exclamation = 5
    }
}
