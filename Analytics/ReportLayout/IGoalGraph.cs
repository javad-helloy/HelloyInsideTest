using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Analytics;

namespace Analytics.ReportLayout
{
    public interface IGoalGraph
    {
        DataQuery LineGraphQuery { get; }

        DataQuery PieGraphQuery { get; }

        String Title { get; }

        bool AddOthers { get; }

        bool SumTotal { get; }

        bool AddFacebookVisitorsToLineGraph { get;  }
        bool AddFacebookVisitorsMonthlyToLineGraph { get;  }

        bool AddFacebookVisitorsToPieGraph { get;  }
        bool AddFacebookVisitorsMonthlyToPieGraph { get;  }
    }
}
