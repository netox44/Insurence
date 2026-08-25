using System.Collections.Generic;

namespace Insurence.Models.ViewModels
{
    public class ProgramFilterViewModel
    {
        // ===== Filters =====
        public string Search { get; set; }
        public string Category { get; set; } = "All";

        // ===== Pagination =====
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; }

        // ===== Programs =====
        public List<ProgramCardViewModel> Programs { get; set; }
            = new List<ProgramCardViewModel>();
    }
}
