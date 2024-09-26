using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Models
{
    public class GivePromoCodeRequest
    {
        [Required(ErrorMessage ="Please, provide the Service Info.")]
        public string ServiceInfo { get; set; }

        [Description("The first name only should be provided in order. Don`t pass in the last name.")]
        [Required(ErrorMessage = "Please, provide the First Name of your Partner Manager.")]
        public string PartnerFirstName { get; set; }

        [Required(ErrorMessage = "Promocode needs to be defined.")]
        public string PromoCode { get; set; }

        [Required(ErrorMessage = "Preference need to be defined.")]
        public string Preference { get; set; }
    }
}