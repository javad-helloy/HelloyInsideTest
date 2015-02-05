using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using InsideModel.Models;
using InsideModel.Models.Identity;
using Microsoft.AspNet.Identity;

namespace Inside.membership
{
    public class AspIdentityMembership:IIdentityMembershipProvider
    {
        private InsideUserManager userManager;

        public AspIdentityMembership(InsideUserManager userManager
        )
        {
            this.userManager = userManager;
        }

        public bool IsInRole(string id, string role)
        {
            //return userRepository.Any(u => u.Id == id && u.Role.Any(r => r.Name == role));
            return userManager.IsInRole(id, role);
        }

        public void AddToRole(string id, string role)
        {
            userManager.AddToRole(id, role);
        }

        public InsideUser Create(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public IList<string> GetRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }

        public InsideUser FindById(string id)
        {
            return userManager.FindById(id);
        }

        public bool Create(InsideUser user, string password)
        {
            IdentityResult identityResult = userManager.Create(user, password);
            return identityResult.Succeeded;
        }

        public bool Delete(InsideUser user)
        {
            return userManager.Delete(user).Succeeded;
        }

        public bool Update(InsideUser user)
        {
            return userManager.Update(user).Succeeded;
        }

        public bool UpdatePassword(string id, string newPassword)
        {
            var removeSuccess = userManager.RemovePassword(id);
            if (removeSuccess.Succeeded)
            {
                return userManager.AddPassword(id, newPassword).Succeeded;
            }

            return removeSuccess.Succeeded;
        }

        public void Lock(string id, bool enable)
        {
            var user = userManager.FindById(id);
            if (user == null) return;

            user.IsLockedOut = enable;
            if (enable==false)
            {
                user.LoginFailureCounter = 0;
            }
            userManager.Update(user);
        }

        public ClaimsIdentity CreateIdentity(InsideUser user)
        {
            return userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public InsideUser ValidateAndReturnUser(string username, string password)
        {
            //return userRepository.Where(u => u.UserName == username).Single();
            var userLookupbyUserName = userManager.FindByName(username);
            if (userLookupbyUserName != null)
            {
                var userPasswordCorrect = userManager.CheckPassword(userLookupbyUserName, password);
                if (!userPasswordCorrect)
                {
                    userLookupbyUserName.LoginFailureCounter++;
                    userLookupbyUserName.LastLoginFailed = DateTime.UtcNow;
                    if (userLookupbyUserName.LoginFailureCounter >= userManager.MaxFailedAccessAttemptsBeforeLockout)
                    {
                        userLookupbyUserName.IsLockedOut = true;
                    }
                    userManager.Update(userLookupbyUserName);
                    return null;
                }

                userLookupbyUserName.LastLoginDate = DateTime.UtcNow;
                userLookupbyUserName.LastActivityDate = DateTime.UtcNow;
                userManager.Update(userLookupbyUserName);
            }
            
            return userLookupbyUserName;
        }

        public IQueryable<InsideUser> GetUsers()
        {
            return userManager.Users;
        }
    }
}
