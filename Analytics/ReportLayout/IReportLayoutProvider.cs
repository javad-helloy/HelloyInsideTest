using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analytics.ReportLayout
{
    public interface IReportLayoutProvider
    {
        int CompanyId { get; set; }
        DateTime EndDate { get; set; }

        IReportLayout GetReportLayout();
    }
}
