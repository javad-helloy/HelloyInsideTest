using System.Data.Entity.ModelConfiguration;
using InsideModel.Models.Identity;

namespace InsideModel.Models.Mapping
{
    class InsideRoleMap: EntityTypeConfiguration<InsideRole>
    {
        public InsideRoleMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Role");
        }
    }
}
