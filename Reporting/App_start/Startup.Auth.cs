using System;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace InsideReporting
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Logon"),
                ExpireTimeSpan = new TimeSpan(7,0,0),
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = ctx =>
                    {
                        if (!(IsAjaxRequest(ctx.Request) || IsApiRequest(ctx.Request) || IsBasicAuthenticationRequest(ctx.Request)))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
        }
        
        private static bool IsAjaxRequest(IOwinRequest request)
        {
            IReadableStringCollection query = request.Query;
            if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }

            IHeaderDictionary headers = request.Headers;
            return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
        }

        private static bool IsApiRequest(IOwinRequest request)
        {
            if (request.Path.Value.Contains("/api/"))
            {
                return true;
            }
            else
            {
                return false;    
            }
        }

        private static bool IsBasicAuthenticationRequest(IOwinRequest request)
        {
            var header = request.Context.Request.Headers["Authorization"];
            if (header!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}