using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analytics.ReportLayout
{
    public class KeywordPosition
    {
        public KeywordPosition(string keyword, int position, int change, int lastPosition)
        {
            Keyword = keyword;
            Position = position;
            Change = change;
            LastPosition = lastPosition;
        }

        public String Keyword { get; set; }
        public int? Position { get; set; }
        public int? Change { get; set; }
        public int? LastPosition { get; set; }
    }

}
