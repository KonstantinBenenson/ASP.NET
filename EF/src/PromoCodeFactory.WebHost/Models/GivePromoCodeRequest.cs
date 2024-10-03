using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PromoCodeFactory.WebHost.Models
{
    public class GivePromoCodeRequest
    {
        public string ServiceInfo { get; set; }

        public string PartnerFirstName { get; set; }

        public string PromoCode { get; set; }

        public string Preference { get; set; }
    }
}