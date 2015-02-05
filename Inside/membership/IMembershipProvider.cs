using System;
using System.Web.Security;

namespace Inside.membership
{
    public interface IMembershipProvider
    {
        bool IsUserInRoles(string role);
        void AddUserToRole(string username, string role);
        MembershipUser GetUser();
        MembershipUser GetUser(Guid id);
        MembershipUser GetUser(string username);
        MembershipUser CreateUser(string name, string password, string email);
        void DeleteUser(string userName, bool deleteAllRelatedData);
        bool ValidateUser(string username, string password);
    }
}