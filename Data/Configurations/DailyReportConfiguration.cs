using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoutineFlow.Models.Entities;

namespace RoutineFlow.Data.Configurations;

public class DailyReportConfiguration : IEntityTypeConfiguration<DailyReport>
{
    public void Configure(EntityTypeBuilder<DailyReport> builder)
    {
        builder.HasKey(d => d.Id);
        builder.HasIndex(d => new { d.UserId, d.Date }).IsUnique();
        builder.Property(d => d.Emotion).HasConversion<string>();
    }
}
