using System;
using System.Collections.Generic;
using System.Linq;
using InsideModel.Models;

namespace InsideModel.repositories
{
    public class SerpRepository : Repository<SerpRanking>, ISerpRankingRepository
    {
        public SerpRepository()
            : base(context => context.SerpRanking)
        {

        }

        public virtual IList<SerpRanking> GetRankings(string domain, DateTime endDate)
        {
            var cutOfDate = endDate.AddMonths(-1);

            var latestSerpRankings = new List<SerpRanking>();

            var serpRankings = base.Where(sr => sr.Url == domain && sr.Date <= endDate && cutOfDate <= sr.Date).OrderByDescending(sr => sr.Date);
            var keywords = serpRankings.Select(sr => sr.Keyword).Distinct();

            foreach (var keyword in keywords)
            {
                latestSerpRankings.Add(serpRankings.Where(sr => sr.Keyword == keyword).First());
            }

            return latestSerpRankings;
        }

        public virtual IList<SerpRankingWithHistory> GetRankingsWithHistory(string domain, DateTime startDate, DateTime endDate)
        {
            var keywordsForDomain =
                base.Where(sr => startDate <= sr.Date && sr.Date <= endDate && sr.Url == domain).Select(sr => sr.Keyword).Distinct();

            IList<SerpRankingWithHistory> serpHistories = new List<SerpRankingWithHistory>();
            foreach (var keyword in keywordsForDomain)
            {
                var keywordHistory = new SerpRankingWithHistory();

                var latestKeyWord =
                    base.Where(sr => sr.Keyword == keyword && startDate <= sr.Date && sr.Date <= endDate && sr.Url == domain).
                        OrderByDescending(sr => sr.Date).First();

                keywordHistory.Keyword = keyword;
                keywordHistory.Position = latestKeyWord.Rank;
                keywordHistory.RankingUrl = latestKeyWord.RankingUrl;
                keywordHistory.Start = latestKeyWord.Start;
                keywordHistory.ChangeWeek = latestKeyWord.ChangeWeek;

                keywordHistory.History = base.Where(
                    sr => sr.Keyword == keyword && startDate <= sr.Date && sr.Date <= endDate && sr.Url == domain && sr.Rank != null).
                    OrderBy(sr => sr.Date).Select(
                        sr => new SerpDataPoint { Date = sr.Date, Position = sr.Rank }).ToArray();

                serpHistories.Add(keywordHistory);
            }

            return serpHistories;
        }
    }

    public interface ISerpRankingRepository : IRepository<SerpRanking>
    {
        IList<SerpRanking> GetRankings(string domain, DateTime endDate);
        IList<SerpRankingWithHistory> GetRankingsWithHistory(string domain, DateTime startDate, DateTime endDate);
    }

    public class SerpRankingWithHistory
    {
        public string Keyword { get; set; }
        public IEnumerable<SerpDataPoint> History { get; set; }
        public int? Position { get; set; }
        public int? ChangeWeek { get; set; }
        public int? Start { get; set; }
        public string RankingUrl { get; set; }
    }

    public class SerpDataPoint
    {
        public DateTime Date { get; set; }
        public int? Position { get; set; }
    }
}
