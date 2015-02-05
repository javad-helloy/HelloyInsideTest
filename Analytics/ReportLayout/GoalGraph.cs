using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Analytics;

namespace Analytics.ReportLayout
{
    public class GoalGraph : IGoalGraph
    {
        public GoalGraph()
        {
            AddOthers = true;
            SumTotal = false;
            AddFacebookVisitorsToLineGraph = false;
            AddFacebookVisitorsMonthlyToLineGraph = false;
            AddFacebookVisitorsToPieGraph = false;
            AddFacebookVisitorsMonthlyToPieGraph = false;
        }

        public DataQuery LineGraphQuery { get; set; }

        public DataQuery PieGraphQuery { get; set; }

        public string Title { get; set; }

        public bool AddOthers { get; set; }

        public bool SumTotal { get; set; }

        public bool AddFacebookVisitorsToLineGraph { get; set; }
        public bool AddFacebookVisitorsMonthlyToLineGraph { get; set; }

        public bool AddFacebookVisitorsToPieGraph { get; set; }
        public bool AddFacebookVisitorsMonthlyToPieGraph { get; set; }
    }
}
