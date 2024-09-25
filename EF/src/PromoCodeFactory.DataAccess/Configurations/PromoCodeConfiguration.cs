using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Configurations
{
    internal class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.Property(x => x.Code).HasMaxLength(8).IsRequired();
            builder.Property(x => x.PartnerManagerId).IsRequired();
            builder.Property(x => x.PartnerName).HasMaxLength(25).IsRequired();
            builder.Property(x => x.PreferenceId).IsRequired();
            builder.Property(x => x.ServiceInfo).HasMaxLength(250).IsRequired();

            builder.HasOne(p => p.Customer).WithMany(c => c.PromoCodes).HasForeignKey(p => p.CustomerId);
        }
    }
}
