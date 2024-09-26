using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Models
{
    public class CreateOrEditCustomerRequest
    {
        [Required(ErrorMessage = "Please, provide the First Name.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please, provide the Last Name.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please, provide the Email address.")]
        [EmailAddress(ErrorMessage = "Provided Email has an incorrect form.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Provide at least 1 Preference.")]
        public List<Guid> PreferenceIds { get; set; }
    }
}