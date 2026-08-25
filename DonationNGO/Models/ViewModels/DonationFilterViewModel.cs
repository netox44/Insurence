using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Insurence.Models.ViewModels
{
    public class DonationFilterViewModel
    {
        public string SearchTerm { get; set; }
        public string Status { get; set; }
        public string Program { get; set; }
        public string DateRange { get; set; }

        public List<SelectListItem> StatusList { get; set; }
        public List<SelectListItem> ProgramList { get; set; }
        public List<SelectListItem> DateRangeList { get; set; }

        public IEnumerable<Donation> Donations { get; set; }
    }
}
