﻿using System;

namespace Pcf.GivingToCustomer.WebHost.Models
{
    public class CustomerShortResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}