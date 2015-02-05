using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace InsideModel.Models.Mapping
{
    [ExcludeFromCodeCoverage]
    public class TaskMap : EntityTypeConfiguration<Task>
    {
        public TaskMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("Task");
        }
    }
}
