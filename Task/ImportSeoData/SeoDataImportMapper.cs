using System;
using InsideModel.Models;

namespace Task.ImportSeoData
{
    public class SeoDataImportMapper : ISeoDataImportMapper
    {
        public void Map(SearchEngineRank searchPosition, SerpRanking serpRanking)
        {
            serpRanking.Keyword = searchPosition.KW;
            serpRanking.Url = searchPosition.URL;

            try
            {
                serpRanking.PageRank = int.Parse(searchPosition.PR);
            }
            catch (Exception)
            {
                serpRanking.PageRank = null;
            }

            serpRanking.Region = searchPosition.Region;
            serpRanking.Language = searchPosition.Language;

            try
            {
                serpRanking.Start = int.Parse(searchPosition.Start);
            }
            catch (Exception)
            {
                serpRanking.Start = null;
            }

            if (serpRanking.Start == 0)
            {
                serpRanking.Rank = null;
            }

            try
            {
                serpRanking.Rank = int.Parse(searchPosition.grank);
            }
            catch (Exception)
            {
                serpRanking.Rank = null;
            }

            if (serpRanking.Rank == 0)
            {
                serpRanking.Rank = null;
            }

            if (!string.IsNullOrEmpty(searchPosition.rankingurl) && searchPosition.rankingurl.ToLower() != "not found")
            {
                serpRanking.RankingUrl = searchPosition.rankingurl;
            }
            else
            {
                serpRanking.RankingUrl = null;
            }

            try
            {
                serpRanking.ChangeDay = int.Parse(searchPosition.day);
            }
            catch (Exception)
            {
                serpRanking.ChangeDay = null;
            }

            try
            {
                serpRanking.ChangeWeek = int.Parse(searchPosition.week);
            }
            catch (Exception)
            {
                serpRanking.ChangeWeek = null;
            }

            try
            {
                serpRanking.ChangeMonth = int.Parse(searchPosition.month);
            }
            catch (Exception)
            {
                serpRanking.ChangeMonth = null;
            }

            try
            {
                serpRanking.SearchVolume = int.Parse(searchPosition.searchvolume);
            }
            catch (Exception)
            {
                serpRanking.SearchVolume = null;
            }



            try
            {
                serpRanking.BackLinks = int.Parse(searchPosition.backlinks);
            }
            catch (Exception)
            {
                serpRanking.BackLinks = null;
            }
        }
    }
}