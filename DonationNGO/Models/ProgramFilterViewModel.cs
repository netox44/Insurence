using Insurence.Models;
using System.Collections.Generic;

namespace Insurence.Models.ViewModel

{
    public class ProgramFilterViewModel
    {
        // Filter/Search inputs
        public string Search { get; set; }
        public string Category { get; set; }

        // List of programs to display
        public List<prmodel> Programs { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; }
    }
}
