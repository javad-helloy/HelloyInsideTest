using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using InsideModel.Models.Identity;

namespace InsideModel.Models.Mapping
{
    internal class UserClaimsMap: EntityTypeConfiguration<UserClaims>

{
    public UserClaimsMap()
    {
        // Primary Key
        this.HasKey(t => t.Id);

        // Properties
        this.Property(t => t.ClaimType)
            .IsRequired();

        this.Property(t => t.ClaimValue)
            .IsRequired();

        // Table & Column Mappings
        this.ToTable("UserClaim");
        this.Property(t => t.Id).HasColumnName("Id");
        this.Property(t => t.ClaimType).HasColumnName("ClaimType");
        this.Property(t => t.ClaimValue).HasColumnName("ClaimValue");
        this.Property(t => t.User_Id).HasColumnName("User_Id");

        //relationships
        this.HasRequired(uc => uc.User)
            .WithMany(u => (ICollection<UserClaims>) u.UserClaims)
            .HasForeignKey(uc => uc.User_Id)
            .WillCascadeOnDelete(true);
    }
}
}
