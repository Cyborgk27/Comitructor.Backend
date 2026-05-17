using Comitructor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comitructor.Infrastructure.Persistence.Contexts.Configurations
{
    public class RequestConfiguration : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.ToTable("Requests");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(r => r.Description)
                .IsRequired();

            builder.Property(r => r.Area)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(r => r.Priority)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasOne(r => r.AssignedUser)
                .WithMany()
                .HasForeignKey(r => r.AssignedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.Histories)
                .WithOne(h => h.Request)
                .HasForeignKey(h => h.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.IsDeleted)
                .HasDefaultValue(false);

            builder.HasQueryFilter(r => !r.IsDeleted);
        }
    }
}