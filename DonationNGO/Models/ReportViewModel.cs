using System;
using System.Collections.Generic;

namespace Insurence.Models.ViewModel
{
    // Main ViewModel for the Reports page
    public class ReportViewModel
    {
        // Key metrics
        public decimal TotalDonations { get; set; }
        public decimal AverageDonation { get; set; }
        public int NewDonors { get; set; }
        public double SuccessRate { get; set; } // in percentage (0-100)

        // List of programs for the table
        public List<ProgramPerformance> ProgramPerformances { get; set; } = new List<ProgramPerformance>();

        // Donation trends for chart
        public List<DonationTrend> DonationTrends { get; set; } = new List<DonationTrend>();
    }

    // Represents each program's performance
    public class ProgramPerformance
    {
        public string ProgramName { get; set; }
        public decimal FundsRaised { get; set; }
        public decimal Goal { get; set; }
        public int Donors { get; set; }
        public string Status { get; set; } // e.g., "Active", "Needs Boost"
    }

    // Represents daily/monthly donation trend
    public class DonationTrend
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
