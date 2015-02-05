using InsideModel.Models;

namespace Task.ImportSeoData
{
    public interface ISeoDataImportMapper
    {
        void Map(SearchEngineRank searchPosition, SerpRanking serpRanking);
    }
}
