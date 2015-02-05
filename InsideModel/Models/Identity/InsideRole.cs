using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace InsideModel.Models.Identity
{
    public partial class InsideRole : IRole
    {

        public InsideRole()
        {
            this.UsersInRole = new List<InsideUser>();
            
        }

        [Key]
        public string Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<InsideUser> UsersInRole { get; set; }
    }
}
