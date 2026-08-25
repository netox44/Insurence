using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Insurence.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }       // Could be stored in Identity user claims or extra table
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalDonations { get; set; }
        public string StatusBadge => IsActive ? "bg-success" : "bg-secondary";
        public string RoleBadge => Roles.Contains("Admin") ? "bg-danger" : "bg-secondary";
    }
}
