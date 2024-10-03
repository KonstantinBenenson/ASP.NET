using System;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.Core.Domain.Administration
{
    public class Employee
        : BaseEntity
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public int AppliedPromocodesCount { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Relations
        public Guid RoleId { get; set; }
        public Role Role { get; set; }

    }
}