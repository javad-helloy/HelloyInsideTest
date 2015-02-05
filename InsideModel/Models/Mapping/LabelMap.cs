using System.Data.Entity.ModelConfiguration;

namespace InsideModel.Models.Mapping
{
    public class LabelMap : EntityTypeConfiguration<Label>
    {
        public LabelMap()
        {
            this.HasKey(t => t.Id);
            this.ToTable("Label");    
        }
    }
}
