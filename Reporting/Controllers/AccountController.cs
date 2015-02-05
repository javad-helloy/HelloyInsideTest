using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace InsideReporting.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class AccountController : Controller
    {
        private IRepository<Token> tokenRepository;
        private IRepository<InsideUser> userRepository;
        private IIdentityMembershipProvider userManager;

        public AccountController(IRepository<Token> tokenRepository, 
            IRepository<InsideUser> userRepository,
            IIdentityMembershipProvider userManager)
        {
            this.tokenRepository = tokenRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        //
        // GET: /Account/LogOn

        public ActionResult LogOn(string returnUrl)
        {
            var model = new LogOnModel {ReturnUrl = returnUrl};


            if (string.IsNullOrEmpty(returnUrl) || !returnUrl.Contains("?"))
            {
                return View(model);
            }

            var queryString = returnUrl.Split('?')[1];
            var nvc = HttpUtility.ParseQueryString(queryString);
            if (nvc["utm_source"] != null && nvc["utm_medium"] != null)
            {
                return RedirectToAction("LogOnTrack",
                    new { returnUrl = returnUrl, utm_source = nvc["utm_source"], utm_medium = nvc["utm_medium"] });
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult LogOnTrack(string returnUrl, string utm_source, string utm_medium)
        {
            var model = new LogOnModel();
            model.ReturnUrl = returnUrl;
            return View("LogOn",model);
        }

        //
        // POST: /Account/LogOn
        [AllowAnonymous]
        [HttpPost]
        public  ActionResult LogOn(LogOnModel model)
        {
            var returnUrl = model.ReturnUrl;
            if (ModelState.IsValid)
            {
                var user =  userManager.ValidateAndReturnUser(model.UserName, model.Password);

                if (user != null && !user.IsLockedOut)
                {
                    userManager.Lock(user.Id, false);
                    SignIn(user, true);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\") &&
                        !returnUrl.Contains("/Account/LogOn"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("password", "Felaktigt användarnamn eller lösenord.");
                    //turn LogOn(model.ReturnUrl);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private  void SignIn(InsideUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity =   userManager.CreateIdentity(user);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        // GET: /Account/AuthenticateToken
        public  ActionResult AuthenticateToken(string token, string returnUrl)
        {
            var logOnModel = new LogOnModel();
            var today = DateTime.Now;
            var tokenUser =  tokenRepository.Where(t => t.AccessToken == token && t.ExpirationDate>=today).SingleOrDefault();
            if (tokenUser!=null)
            {
                
                var userMembershipId = tokenUser.UserId;
                var user =   userManager.FindById(userMembershipId);
                
                logOnModel.UserName = user.UserName;
                SignIn(user, true);
                if (returnUrl == null) { return RedirectToAction("Index", "Home"); }

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\") &&
                    !returnUrl.Contains("/Account/LogOn"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    if (returnUrl.ToLower().Contains("inside.helloy.se"))
                    {
                        return Redirect(returnUrl);
                        
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Länken är tyvärr ej längre giltig.");
                return View("LogOn",logOnModel);
            }
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("LogOn");
        }
    }
}
