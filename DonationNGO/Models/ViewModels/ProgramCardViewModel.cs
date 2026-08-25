namespace Insurence.Models.ViewModels
{
    public class ProgramCardViewModel
    {
        public int Id { get; set; }

        public string ProgramName { get; set; }
        public string Category { get; set; }

        public string Description { get; set; }

        public decimal FundingGoal { get; set; }
        public decimal AmountRaised { get; set; }

        public int DurationMonths { get; set; }

        public string ImageUrl { get; set; }
    }
}
