using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsideModel.Models
{
    public class SerpRanking
    {
        [Column(Order = 0, TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public string Url { get; set; }
        public string Keyword { get; set; }
            public int? PageRank { get; set; }
        public string Region { get; set; }
        public string Language { get; set; }
            public int? Start { get; set; }
            public int? Rank { get; set; }
        public int? ChangeDay { get; set; }
        public int? ChangeWeek { get; set; }
        public int? ChangeMonth { get; set; }
        public string RankingUrl { get; set; }
        public int? BackLinks { get; set; }
        public int? SearchVolume { get; set; }
    }
}
