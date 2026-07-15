using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoutineFlow.Models.Entities;

namespace RoutineFlow.Data.Configurations;

public class TaskCompletionConfiguration : IEntityTypeConfiguration<TaskCompletion>
{
    public void Configure(EntityTypeBuilder<TaskCompletion> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => new { c.TaskId, c.Date }).IsUnique();
    }
}
