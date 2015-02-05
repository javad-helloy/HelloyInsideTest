using System;

namespace InsideModel.Models
{
    public class Token
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        
        public string AccessToken { get; set; }
        
        public DateTime ExpirationDate { get; set; }

        public virtual InsideUser InsideUser { get; set; }
    }
}
