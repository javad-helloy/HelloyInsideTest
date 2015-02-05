using System;

namespace Task.UpdatePhonecalls.UpdatePhoneCalls
{
    public class UpdatePhonecallsForClientsTaskMessage
    {
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
