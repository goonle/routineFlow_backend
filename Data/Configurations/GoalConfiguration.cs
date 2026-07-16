using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoutineFlow.Models.Entities;
using RoutineFlow.Models.Enums;

namespace RoutineFlow.Data.Configurations;

public class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).IsRequired();
        builder.Property(g => g.Icon).IsRequired().HasDefaultValue(GoalIcon.General);
        builder.HasIndex(g => g.UserId);

        builder.HasMany(g => g.Tasks)
            .WithOne(t => t.Goal)
            .HasForeignKey(t => t.GoalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.Plan)
            .WithOne(p => p.Goal)
            .HasForeignKey<Plan>(p => p.GoalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
