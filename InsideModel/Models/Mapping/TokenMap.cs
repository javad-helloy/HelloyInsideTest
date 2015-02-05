using System.Data.Entity.ModelConfiguration;

namespace InsideModel.Models.Mapping
{
    public class TokenMap : EntityTypeConfiguration<Token>
    {
        public TokenMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Token");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.AccessToken).HasColumnName("Accept_token");
            this.Property(t => t.ExpirationDate).HasColumnName("Expiration_date");

            this.HasRequired(t => t.InsideUser)
                .WithMany(t => t.Token)
                .HasForeignKey(d => d.UserId);
        }
    }
}
