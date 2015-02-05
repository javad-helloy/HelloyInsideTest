using System.Data.Entity.ModelConfiguration;

namespace InsideModel.Models.Mapping
{
    class ContactInteractionMap : EntityTypeConfiguration<ContactInteraction>
    {
        public ContactInteractionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.ContactId)
                .IsRequired();

            this.Property(t => t.Type)
                .IsRequired();


            // Table & Column Mappings
            this.ToTable("ContactInteraction");
            this.Property(t => t.ContactId).HasColumnName("ContactId");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Value).HasColumnName("Value");

            //relationships
            this.HasRequired(t => t.Contact)
                .WithMany(t => t.Interaction)
                .HasForeignKey(d => d.ContactId)
                .WillCascadeOnDelete(true);
        }
    }
}
