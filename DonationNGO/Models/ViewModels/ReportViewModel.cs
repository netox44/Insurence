using System;
using System.Collections.Generic;

namespace Insurence.Models.ViewModels
{
    public class ReportViewModel
    {
        // ===== Key Metrics =====
        public decimal TotalDonations { get; set; }
        public decimal AverageDonation { get; set; }
        public int NewDonors { get; set; }
        public double SuccessRate { get; set; }

        // ===== Charts =====
        public List<DonationTrendViewModel> DonationTrends { get; set; }
            = new List<DonationTrendViewModel>();

        // ===== Table =====
        public List<ProgramPerformanceViewModel> ProgramPerformances { get; set; }
            = new List<ProgramPerformanceViewModel>();
    }

    // ===============================
    // CHILD VIEW MODELS
    // ===============================

    public class DonationTrendViewModel
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProgramPerformanceViewModel
    {
        public string ProgramName { get; set; }
        public decimal FundsRaised { get; set; }
        public decimal Goal { get; set; }
        public int Donors { get; set; }
        public string Status { get; set; }
    }
}
