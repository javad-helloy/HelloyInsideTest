using System.Data.Entity.ModelConfiguration;

namespace InsideModel.Models.Mapping
{
    class ContactPropertyMap : EntityTypeConfiguration<ContactProperty>
    {
        public ContactPropertyMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.ContactId)
                .IsRequired();

            this.Property(t => t.Type)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ContactProperty");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ContactId).HasColumnName("ContactId");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Value).HasColumnName("Value");

             //relationships
            this.HasRequired(lp => lp.Contact)
                .WithMany(l => l.Property)
                .HasForeignKey(lp => lp.ContactId)
                .WillCascadeOnDelete(true);
        }
    }
}
