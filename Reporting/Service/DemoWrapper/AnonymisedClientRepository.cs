using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.AspNet.Identity;

namespace InsideReporting.WebRepositories.DemoWrapper
{
    public class AnonymisedClientRepository : Repository<Client>
    {
        private IIdentityMembershipProvider userManager;
        public AnonymisedClientRepository(IIdentityMembershipProvider userManager)
            :base(context => context.Clients)
        {
            this.userManager = userManager;
        }

        public override IQueryable<Client> Where(Expression<Func<Client, bool>> expression)
        {
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            var isBasicAuthenticationForTaskCreating = HttpContext.Current.User.Identity.AuthenticationType == "BasicAuthenticationForTaskCreating";
            if (isAuthenticated && !isBasicAuthenticationForTaskCreating && userManager.IsInRole(HttpContext.Current.User.Identity.GetUserId(), "demo"))
            {
                var result = base.Where(expression);
                var resultList = result.ToList();

                var iterator = 1;
                foreach (var client in resultList)
                {
                    client.Name = "Exempelkund " + iterator++;
                }

                return resultList.AsQueryable();
            }
            else
            {
                return base.Where(expression);
            }
        }

        public  override void SaveChanges()
        {
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            var isBasicAuthenticationForTaskCreating = HttpContext.Current.User.Identity.AuthenticationType == "BasicAuthenticationForTaskCreating";
            if (isAuthenticated && isBasicAuthenticationForTaskCreating && userManager.IsInRole(HttpContext.Current.User.Identity.GetUserId(), "demo"))
            {
                throw new Exception("Not allowed for sales");
            }
            else
            {
                base.SaveChanges();
            }
        }
    }
}
