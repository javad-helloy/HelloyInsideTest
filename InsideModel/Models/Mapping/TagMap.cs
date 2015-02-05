using System.Data.Entity.ModelConfiguration;

namespace InsideModel.Models.Mapping
{
    public class TagMap: EntityTypeConfiguration<Tag>
    {
        public TagMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("Tag");

            this.HasRequired(t => t.Client)
                .WithMany(t => t.Tags)
                .HasForeignKey(d => d.ClientId);
        }
    }
}
