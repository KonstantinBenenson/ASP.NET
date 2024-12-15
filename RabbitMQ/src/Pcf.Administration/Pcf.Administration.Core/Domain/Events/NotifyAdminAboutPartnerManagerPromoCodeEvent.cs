using System;

namespace Pcf.Administration.Core.Domain.Events;

public record NotifyAdminAboutPartnerManagerPromoCodeEvent(Guid PartnerManagerId);