using System.Data.Entity.ModelConfiguration;
using InsideModel.Models.Identity;

namespace InsideModel.Models.Mapping
{
    public class UserLoginsMap: EntityTypeConfiguration<UserLogins>
    {


        public UserLoginsMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);
            this.HasKey(t => t.LoginProvider);
            this.HasKey(t => t.ProviderKey);
            // Properties
            // Table & Column Mappings
            this.ToTable("UserLogin");


             this.HasRequired(ul => ul.User)
                .WithMany(u => u.UserLogins)
                .HasForeignKey(ul => ul.UserId)
                .WillCascadeOnDelete(true); ;
        }
        

    }
}
