using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Security;

namespace Inside.membership
{
    [ExcludeFromCodeCoverage]
    public class AspMembershipAdapter : IMembershipProvider
    {
        public bool IsUserInRoles(string role)
        {
            return Roles.IsUserInRole(role);
        }

        public void AddUserToRole(string username, string role)
        {
            Roles.AddUserToRole(username, role);
        }

        public MembershipUser GetUser()
        {
            return Membership.GetUser();
        }

        public MembershipUser GetUser(Guid id)
        {
            return Membership.GetUser(id);
        }

        public MembershipUser GetUser(string username)
        {
            return Membership.GetUser(username);
        }

        public MembershipUser CreateUser(string name, string password, string email)
        {
            return Membership.CreateUser(name, password, email);
        }

        public void DeleteUser(string userName, bool deleteAllRelatedData)
        {
            Membership.DeleteUser(userName, deleteAllRelatedData);
        }

        public bool ValidateUser(string username, string password)
        {
            return Membership.ValidateUser(username, password);
        }
    }
}