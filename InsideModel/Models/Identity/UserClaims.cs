using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InsideModel.Models.Identity
{
    public class UserClaims 
    {
        public UserClaims(string type, string value) 
        {
            this.ClaimType = type;
            this.ClaimValue = value;
        }

        public UserClaims()
        {
            this.ClaimType = "";
            this.ClaimValue = "";
        }

        public int Id{ get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public string User_Id { get; set; }


        public virtual InsideUser User { get; set; }
    }
}
