using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InsideModel.Models.Identity
{
    public class UserLogins
    {
        [Key]
        public string UserId{ get; set; }
        [Key]
        public string LoginProvider { get; set; }
        [Key]
        public string ProviderKey{ get; set; }

        public virtual InsideUser User{ get; set; }

    }
}
