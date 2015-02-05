using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Analytics;

namespace Analytics.ReportLayout
{
    public interface IReportLayout
    {
        IList<DataQuery> AdwordsQuery
        {
            get;
        }   

        IList<IGoalGraph> GoalGraphs
        {
            get;
        }

        IList<IFacebookProfile> FacebookProfiles
        {
            get;
        }

        IList<DataQuery> KeywordQuery
        {
            get;
        }

        IEnumerable<string> KeywordPositions { get; }

        string LogoImage { get; set; }
    }
}
