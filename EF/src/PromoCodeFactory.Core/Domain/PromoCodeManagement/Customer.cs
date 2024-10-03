﻿using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Customer
        : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }

        // Relations
        public IList<CustomerPreference> CustomersPreferences { get; set; } = [];
        //public ICollection<Preference> Preferences { get; set; }
        public IList<PromoCode> PromoCodes { get; set; } = [];
    }
}