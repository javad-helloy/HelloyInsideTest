using System;

namespace Task.ImportCustomEvents
{
    public interface ICustomEventsExtractor
    {
        void ImportEvents(int clientId, DateTime fromDate, DateTime toDate);
    }
}
