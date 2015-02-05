using System;

namespace Task.ImportCustomEvents
{
    public class ImportEventsTaskMessage
    {
        public int ClientId { get; set; }
        public DateTime StartDate{ get; set; }
        public DateTime EndDate { get; set; }
    }
}
