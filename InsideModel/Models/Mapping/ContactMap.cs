using System.Data.Entity.ModelConfiguration;

namespace InsideModel.Models.Mapping
{
    class ContactMap : EntityTypeConfiguration<Contact>
    {
        public ContactMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.ClientId)
                .IsRequired();

            this.Property(t => t.LeadType)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Contact");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ClientId).HasColumnName("ClientId");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.LeadType).HasColumnName("ContactType");
            this.Property(t => t.AutoRatingScore).HasColumnName("AutoRatingScore");
            this.Property(t => t.RatingScore).HasColumnName("RatingScore");
            this.Property(t => t.Campaign).HasColumnName("Campaign");
            this.Property(t => t.Source).HasColumnName("Source");
            this.Property(t => t.Medium).HasColumnName("Medium");
            this.Property(t => t.Product).HasColumnName("Product");
            this.Property(t => t.SearchPhrase).HasColumnName("SearchPhrase");
            this.Property(t => t.ReadStatus).HasColumnName("ReadStatus");

            this.HasMany(c => c.Tags).WithMany(l => l.Contacts).Map(
                table =>
                {
                    table.MapLeftKey("ContactId");
                    table.MapRightKey("TagId");
                    table.ToTable("ContactTag");
                });
 
        }
    }
}
