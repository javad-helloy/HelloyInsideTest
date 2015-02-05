using System.Data.Entity.ModelConfiguration;

namespace InsideModel.Models.Mapping
{
    public class InsideUserMap : EntityTypeConfiguration<InsideUser>
    {
        public InsideUserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            this.Property(t => t.UserName).IsRequired();

            // Properties
            // Table & Column Mappings
            this.ToTable("User");

            this.HasMany(u => u.Role).WithMany(r => r.UsersInRole).Map(
                table =>
                {
                    table.MapLeftKey("UserId");
                    table.MapRightKey("RoleId");
                    table.ToTable("UserRole");
                });
        }
    }
}
