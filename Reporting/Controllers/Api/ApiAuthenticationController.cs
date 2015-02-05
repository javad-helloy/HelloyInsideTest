using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.SessionState;
using Inside.membership;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Controllers.Api
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class ApiAuthenticationController : ApiController
    {

        private IIdentityMembershipProvider userManager;

        public ApiAuthenticationController(IIdentityMembershipProvider userManager)
        {
            this.userManager = userManager;

        }

        public bool CurrentUserHasAccessToClient(int clientId)
        {
            var userId = this.User.Identity.GetUserId();
            if (userId == null)
            {
                return false;
            }
            if (userManager.IsInRole(userId, "consultant"))
            {
                return true;
            }
            else if (userManager.IsInRole(userId,  "demo"))
            {
                return true;
            }
            else if (userManager.IsInRole(userId, "sales"))
            {
                return true;
            }
            else if (userManager.IsInRole(userId,  "client"))
            {
                var user = userManager.FindById(userId);
                if (user.Client.Id == clientId)
                {
                    return true;
                }
                else { return false;}
            }
            else
            {
                return false;

            }
        }


        protected object GetPageResult<T>(IQueryable<T> pageResult, int page, int pageSize )
        {
            int totalCount = pageResult.Count();
            var numbverOfpages = (int)Math.Ceiling((decimal)totalCount / (decimal)pageSize);
            var pageContacts = pageResult.Skip((page-1) * pageSize).Take(pageSize);
            
            string prevPageLink;
            if (page <= 1)
            {
                prevPageLink = null;
            }
            else
            {
                var queryValues = this.Request.GetQueryNameValuePairs();
                var newValues = new Dictionary<string, object>();
                newValues.Add("page", page - 1);
                foreach (var queryValue in queryValues)
                {
                    if (queryValue.Key != "page")
                    {
                        newValues.Add(queryValue.Key, queryValue.Value);
                    }
                }

                prevPageLink = Url.Route(routeName: "ListContacts", routeValues: newValues);
            }

            string nextPageLink;
            if (page >= numbverOfpages)
            {
                nextPageLink = null;
            }
            else
            {
                var queryValues = this.Request.GetQueryNameValuePairs();
                var newValues = new Dictionary<string, Object>();
                newValues.Add("page", page + 1);
                foreach (var queryValue in queryValues)
                {
                    if (queryValue.Key != "page")
                    {
                        newValues.Add(queryValue.Key, queryValue.Value);    
                    }
                }
                nextPageLink = Url.Route(routeName: "ListContacts", routeValues: newValues);
            }

            return new PageResult<T>
            {
                totalCount = totalCount,
                totalPages = numbverOfpages,
                prevPageLink = prevPageLink,
                nextPageLink = nextPageLink,
                results = pageContacts
            };
        }
    }

    public class PageResult<T>
    {
        public int totalCount { get; set; }
        public int totalPages { get; set; }
        public string prevPageLink { get; set; }
        public string nextPageLink { get; set; }
        public IQueryable<T> results { get; set; }
    }
}
