using System.Data.Entity.ModelConfiguration;

namespace InsideModel.Models.Mapping
{
    class BudgetMap : EntityTypeConfiguration<Budget>
    {
        public BudgetMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.ToTable("Budget");

            //relationships
            this.HasRequired(t => t.Client)
                .WithMany(t => t.Budgets)
                .HasForeignKey(d => d.ClientId);
        }
    }
}
