using System;

namespace InsideModel.Models
{
    public class ExternalToken
    {
        public int Id { get; set; }

        public string AccessToken { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string Type { get; set; }

    }
}
