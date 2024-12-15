using System;

namespace Pcf.ReceivingFromPartner.Core.Domain.Events;
public record NotifyAdminAboutPartnerManagerPromoCodeEvent(Guid PartnerManagerId);