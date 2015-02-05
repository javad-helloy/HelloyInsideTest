using System.Data.Entity.ModelConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace InsideModel.Models.Mapping
{
    [ExcludeFromCodeCoverage]
    public class ClientMap : EntityTypeConfiguration<Client>
    {
        public ClientMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Client");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.AnalyticsTableId).HasColumnName("AnalyticsTableId");
            this.Property(t => t.ConsultantId).HasColumnName("ConsultantId");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Longitude).HasColumnName("Longitude").HasPrecision(18,9);
            this.Property(t => t.Latitude).HasColumnName("Latitude").HasPrecision(18, 9);
            this.Property(t => t.AccountManagerId).HasColumnName("AccountManagerId");

            // Relationships
            this.HasRequired(t => t.Consultant)
                .WithMany(t => t.ConsultantsForClients)
                .HasForeignKey(d => d.ConsultantId);

            this.HasRequired(t => t.AccountManager)
                .WithMany(t => t.AccountManagerForClients)
                .HasForeignKey(d => d.AccountManagerId);

            this.HasMany(c => c.Labels).WithMany(l => l.Clients).Map(
                table =>
                {
                    table.MapLeftKey("ClientId");
                    table.MapRightKey("LabelId");
                    table.ToTable("ClientLabel");
                });
        }
    }
}
