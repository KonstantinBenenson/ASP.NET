using System;

namespace Pcf.ReceivingFromPartner.Core.Domain.Events;

public class GivePromocodeToCustomerEvent
{
    public required Guid PartnerId { get; set; }
    public required Guid PromoCodeId { get; set; }
    public required string BeginDate { get; set; }
    public required string EndDate { get; set; }
    public required Guid PreferenceId { get; set; }
    public required string PromoCode { get; set; }
    public required string ServiceInfo { get; set; }
    public required Guid? PartnerManagerId { get; set; }
}
