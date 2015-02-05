using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using InsideModel.Models;

namespace Inside.membership
{
    public interface IIdentityMembershipProvider
    {
        bool IsInRole(string id, string role);
        void AddToRole(string id, string role);
        InsideUser Create(string userName, string password);

        bool Create(InsideUser user, string password);
        bool Delete(InsideUser user);
        bool Update(InsideUser user);
        bool UpdatePassword(string id, string newPassword);
        void Lock(string id, bool enable);
        
        InsideUser ValidateAndReturnUser(string username, string password);
        InsideUser FindById(string id);
        
        ClaimsIdentity CreateIdentity(InsideUser user);
        IQueryable<InsideUser> GetUsers();
        IList<string> GetRoles(string userId);
        
        
    }
}