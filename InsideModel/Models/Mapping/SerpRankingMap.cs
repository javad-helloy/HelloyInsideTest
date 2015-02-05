using System.Data.Entity.ModelConfiguration;

namespace InsideModel.Models.Mapping
{
    class SerpRankingMap : EntityTypeConfiguration<SerpRanking>
    {
        public SerpRankingMap()
        {
            ToTable("SerpRanking");
            this.HasKey(u => new {u.Date, u.Url, u.Keyword});
        }
    }
}
