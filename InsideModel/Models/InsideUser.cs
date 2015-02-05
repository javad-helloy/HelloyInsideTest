using System;
using System.Collections.Generic;
using System.ComponentModel;
using InsideModel.Models.Identity;
using Microsoft.AspNet.Identity;

namespace InsideModel.Models
{
    
    public partial class InsideUser : IUser<string>
    {
        public InsideUser()
        {
            this.Token=new List<Token>();
            this.Role = new List<InsideRole>();
            this.UserLogins = new List<UserLogins>();
            this.UserClaims = new List<UserClaims>();
            this.ConsultantsForClients = new List<Client>();
            this.AccountManagerForClients = new List<Client>();
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.UtcNow;
            this.IsApproved = true;
            this.IsLockedOut = false;
            this.LastLoginDate = DateTime.UtcNow;
            this.LastLoginFailed = DateTime.UtcNow.AddYears(-10);
            this.LoginFailureCounter = 0;
            this.LastActivityDate = DateTime.UtcNow;
        }
        public int ? UserId { get; set; }
        public int ? ClientId { get; set; }
        public bool ? ReceiveEmail { get; set; }
        public bool? ReceiveSms { get; set; }

        public virtual Client Client { get; set; }
        public virtual ICollection<Token> Token { get; set; }

        [DisplayName("Namn")]
        public string Name { get; set; }

        public string Phone { get; set; }
        public string ImageUrl { get; set; }
       
        public virtual ICollection<Client> ConsultantsForClients { get; set; }

        public virtual ICollection<Client> AccountManagerForClients { get; set; }

        public virtual ICollection<InsideRole> Role { get; set; }
        public virtual ICollection<UserLogins> UserLogins { get; set; }
        public virtual ICollection<UserClaims> UserClaims { get; set; }

     
        public string Id { get;  set; }
        public string UserName { get; set; }
        public string LoweredUserName { get; set; }
        public virtual bool IsApproved { get; set; }
        public virtual bool IsLockedOut { get; set; }
        public virtual DateTime LastLoginDate { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime LastActivityDate { get; set; }
        public virtual DateTime LastLoginFailed { get; set; }
        public virtual string Email { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
        public virtual int LoginFailureCounter { get; set; }


        
	
	


    
}
}
