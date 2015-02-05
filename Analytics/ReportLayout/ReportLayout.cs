using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Analytics;

namespace Analytics.ReportLayout
{
    public class ReportLayout : IReportLayout
    {
        public ReportLayout()
        {
            LogoImage = "bravura.png";
            GoalGraphs = new List<IGoalGraph>();
            KeywordQuery = new List<DataQuery>();
            FacebookProfiles = new List<IFacebookProfile>();
            AdwordsQuery = new List<DataQuery>();
        }

        public IList<IGoalGraph> GoalGraphs { get; set; }

        public IList<DataQuery> AdwordsQuery { get; set; }

        public IList<IFacebookProfile> FacebookProfiles { get; set; }

        public IList<DataQuery> KeywordQuery { get; set; }

        public IEnumerable<string> KeywordPositions { get; set; }

        public string LogoImage { get; set; }
    }
}
