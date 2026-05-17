using Comitructor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comitructor.Infrastructure.Persistence.Contexts.Configurations
{
    public class RequestHistoryConfiguration : IEntityTypeConfiguration<RequestHistory>
    {
        public void Configure(EntityTypeBuilder<RequestHistory> builder)
        {
            builder.ToTable("RequestHistories");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.PreviousStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(h => h.NewStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(h => h.ChangeReason)
                .HasMaxLength(500);

            builder.HasOne(h => h.Request)
                .WithMany(r => r.Histories)
                .HasForeignKey(h => h.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}