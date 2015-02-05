using System.Data.Entity.ModelConfiguration;

namespace InsideModel.Models.Mapping
{
    public class ExternalTokenMap : EntityTypeConfiguration<ExternalToken>
    {
        public ExternalTokenMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("ExternalToken");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.AccessToken).HasColumnName("Access_token");
            this.Property(t => t.ExpirationDate).HasColumnName("Expiration_date");
            this.Property(t => t.Type).HasColumnName("Type");
        }
    }
}
