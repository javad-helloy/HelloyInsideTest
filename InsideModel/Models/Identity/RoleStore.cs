using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsideModel.repositories;
using Microsoft.AspNet.Identity;

namespace InsideModel.Models.Identity
{
    public class RoleStore<TRole> : IRoleStore<TRole>, IQueryableRoleStore<TRole> where TRole:InsideRole 
    {
        private IRepository<TRole> roleTable;

        public RoleStore(IRepository<TRole> roleTable)
        {
            this.roleTable = roleTable;
        } 
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task CreateAsync(TRole role)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateAsync(TRole role)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task DeleteAsync(TRole role)
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TRole> Roles
        {
            get
            {
                return roleTable.All();
            }
        }
    }
}
