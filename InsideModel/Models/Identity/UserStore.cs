using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using InsideModel.repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Ninject;

namespace InsideModel.Models.Identity
{
    public class UserStore<TUser> : IUserStore<TUser>, 
                                    IUserRoleStore<TUser>, 
                                    IUserPasswordStore<TUser>, 
                                    IQueryableUserStore<TUser>, 
                                    IUserSecurityStampStore<TUser>
                                    where TUser : InsideUser

    {


        
        private IRepository<TUser> userTable;
        private IRepository<InsideRole> roleTable;
        
        public UserStore(IRepository<TUser> userTable,
                        IRepository<InsideRole> roleTable)
        {
            this.userTable = userTable;
            this.roleTable = roleTable;
        
        }

        public string flag { get; set; }
        
        #region user store
        public System.Threading.Tasks.Task CreateAsync(TUser user)
        {
            this.ThrowIfDisposed();
            user.LoweredUserName = user.UserName.ToLower();
            userTable.Add(user);
            return System.Threading.Tasks.Task.Factory.StartNew(() => userTable.SaveChanges());
        }

        private void ThrowIfDisposed()
        {
            if (this.Disposed)
            {
                throw new ObjectDisposedException(base.GetType().Name);
            }
        }



        public System.Threading.Tasks.Task UpdateAsync(TUser user)
        {
            userTable.SetState(user,EntityState.Modified);
            return System.Threading.Tasks.Task.Factory.StartNew(() => userTable.SaveChanges());

        }

        public System.Threading.Tasks.Task DeleteAsync(TUser user)
        {
            userTable.Delete(user);
            return System.Threading.Tasks.Task.Factory.StartNew(() => userTable.SaveChanges());
        }

        Task<TUser> IUserStore<TUser, string>.FindByIdAsync(string userId)
        {
            this.ThrowIfDisposed();
            var queryable = userTable.Where(u => u.Id == userId);
            return queryable.SingleOrDefaultAsync();
        }

        Task<TUser> IUserStore<TUser, string>.FindByNameAsync(string userName)
        {
            this.ThrowIfDisposed();
            return userTable.Where(u => u.UserName == userName).SingleOrDefaultAsync();
        }
        #endregion

        #region user role store
        public System.Threading.Tasks.Task AddToRoleAsync(TUser user, string roleName)
        {
            this.ThrowIfDisposed();
            user.Role.Add(roleTable.Single(r =>r.Name==roleName));
            return System.Threading.Tasks.Task.Factory.StartNew(() => userTable.SaveChanges());
        }

        public System.Threading.Tasks.Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            this.ThrowIfDisposed();
            var roleToRemove = user.Role.SingleOrDefault(r => r.Name == roleName);
            if (roleToRemove == null)
            {return System.Threading.Tasks.Task.FromResult<Task>(null); }

            user.Role.Remove(roleToRemove);
            return System.Threading.Tasks.Task.Factory.StartNew(() => userTable.SaveChanges());
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            this.ThrowIfDisposed();

            var list = user.Role.Select(r => r.Name).ToList();
            return Task<IList<string>>.Factory.StartNew(() => list);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            this.ThrowIfDisposed();
            var hasRoles = user.Role.Any(r => r.Name == roleName);
            return Task<bool>.Factory.StartNew(() => hasRoles);
        }

        #endregion
        #region password store
        public System.Threading.Tasks.Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            this.ThrowIfDisposed();
            user.PasswordHash = passwordHash;
            
            return System.Threading.Tasks.Task.FromResult<object>(null);
            //return System.Threading.Tasks.Task.Factory.StartNew(() => user.PasswordHash = passwordHash);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            this.ThrowIfDisposed();
            return Task<string>.Factory.StartNew(() => user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            this.ThrowIfDisposed();
            return Task<bool>.Factory.StartNew(() => user.PasswordHash != null);
        }
        #endregion
        #region security stamp store

        public System.Threading.Tasks.Task SetSecurityStampAsync(TUser user, string stamp)
        {
            this.ThrowIfDisposed();
            return Task<string>.Factory.StartNew(() => user.SecurityStamp=stamp);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            this.ThrowIfDisposed();
            return Task<string>.Factory.StartNew(() => user.SecurityStamp);
        }
        #endregion
        public IQueryable<TUser> Users
        {
            get { return userTable.All(); } 
            
        }
    
        private bool Disposed;

        public void Dispose()
        {
            /*this.Dispose(true);

            GC.SuppressFinalize(this);*/
        }

        protected void Dispose(
            bool disposing)
        {
            this.Disposed = true;
        }

    }
}
