using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Models;
using InsideReporting.Models.AccountManager;
using Microsoft.AspNet.Identity;


namespace InsideReporting.Controllers.Enity
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class AccountManagerController : Controller
    {
        private readonly IRepository<InsideUser> adminRepository;
        private IIdentityMembershipProvider userManager;

        public AccountManagerController(
            IRepository<InsideUser> adminRepository,
            IIdentityMembershipProvider userManager)
        {
            this.adminRepository = adminRepository;
            this.userManager = userManager;
        }
        // GET: /AccountManager/
        [Authorize(Roles = "consultant")]
        public ActionResult Index()
        {
            var accountManagers = new AccountManagerListViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            foreach (var admin in userManager.GetUsers().Where(u => u.Role.Any(r => r.Name == "sales")))
            {
                var accountManagerViewModel = new AccountManagerViewModel
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    ImageUrl = admin.ImageUrl,
                    Name = admin.Name,
                    Phone = admin.Phone,
                    IsLockedOut = admin.IsLockedOut
                };

                accountManagers.Collection.Add(accountManagerViewModel);
            }
            return View(accountManagers);
        }

       
        // GET: /AccountManager/Create
        [Authorize(Roles = "consultant")]
        public ActionResult Create()
        {
            var accountManagers = new AccountManagerViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            
            return View(accountManagers);
        }

        // POST: /AccountManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "consultant")]
        [HttpPost]
        public ActionResult Create(AccountManagerViewModel admin)
        {
            if (String.IsNullOrEmpty(admin.Password))
            {
                ModelState.AddModelError("password", "Ange lösenord");
            }
            if (!ModelState.IsValid)
            {
                return View(admin);
            }
            try
            {
                var newAccountManager = new InsideUser
                {
                    Email = admin.Email,
                    UserName = admin.Email,
                    /*LoweredUserName = admin.Name.ToLower(),*/
                    Name = admin.Name,
                    ImageUrl = admin.ImageUrl,
                    Phone = admin.Phone
                };

                var newUserCreateSuccess = userManager.Create(newAccountManager, admin.Password);
                if (newUserCreateSuccess)
                {
                    userManager.AddToRole(newAccountManager.Id, "sales");
                }
                else
                {
                    ModelState.AddModelError("Email", "Kontoansvarig finns redan.");
                    return View(admin); 
                }

               /* adminRepository.Add(newAccountManager);
                adminRepository.SaveChanges();*/

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Email", "Kontoansvarig finns redan.");
                return View(admin);
            }
            
        }

        // GET: /AccountManager/Edit/5
        [Authorize(Roles = "consultant")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var admin = userManager.FindById(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            var accountManagerViewModel = new AccountManagerViewModel(userManager.GetRoles(User.Identity.GetUserId()))
            {
                Name = admin.Name,
                Phone = admin.Phone,
                ImageUrl = admin.ImageUrl,
                Email = admin.Email,
                Password = ""
            };

            return View(accountManagerViewModel);
    
        }

        // POST: /AccountManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "consultant")]
        [HttpPost]
        public ActionResult Edit(AccountManagerViewModel admin)
        {
            if (!ModelState.IsValid)
            {
                return View(admin);
            }
            var changedAccountManager = userManager.FindById(admin.Id);
            changedAccountManager.Phone = admin.Phone;
            changedAccountManager.Email = admin.Email;
            changedAccountManager.ImageUrl = admin.ImageUrl;
            changedAccountManager.Name = admin.Name;

            if (!string.IsNullOrEmpty(admin.Password))
            {
                if (!string.IsNullOrEmpty(changedAccountManager.Id))
                {
                    try
                    {

                        var changePasswordIdentityResult = userManager.UpdatePassword(changedAccountManager.Id, admin.Password);
                        if (!changePasswordIdentityResult)
                        {
                            ModelState.AddModelError("password",
                            "Misslyckades med att uppdatera lösenordet för" + admin.Name);
                            return View(admin);
                        }
                        /*var addPasswordIdentityResult= userManager.AddPassword(changedAccountManager.Id, admin.Password);
                        if (!addPasswordIdentityResult.Succeeded)
                        {
                            ModelState.AddModelError("password",
                            "Misslyckades med att uppdatera lösenordet för" + admin.Name);
                            return View(admin);
                        }*/
                    }
                    catch (MembershipPasswordException e)
                    {
                        ModelState.AddModelError("password",
                            "Misslyckades med att uppdatera lösenordet för" + admin.Name);
                        return View(admin);
                    }
                }
                else
                {
                    try
                    {
                        changedAccountManager.UserName = admin.Email;
                        var newUserCreateSuccess = userManager.Create(changedAccountManager, admin.Password);
                        if (newUserCreateSuccess)
                        {
                            userManager.AddToRole(changedAccountManager.Id, "sales");
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("password",
                            "Misslyckades med att uppdatera lösenordet för" + admin.Name);
                            return View(admin);
                        }
                    }
                    catch (MembershipPasswordException e)
                    {
                        ModelState.AddModelError("password",
                            "Misslyckades med att uppdatera lösenordet för" + admin.Name);
                        return View(admin);
                    }
                }
            }


            changedAccountManager.Name = admin.Name;
            userManager.Update(changedAccountManager);

            /*adminRepository.SetState(changedAccountManager, EntityState.Modified);
            adminRepository.SaveChanges();*/
            return RedirectToAction("Index");
        }

       // GET: /AccountManager/Delete/5
        [Authorize(Roles = "consultant")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userMembershipToDelete = userManager.FindById(id);

            if (userMembershipToDelete == null)
            {
                return HttpNotFound();
            }
            var accountManagerViewModel = new AccountManagerViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            accountManagerViewModel.Name = userMembershipToDelete.Name;
            return View(accountManagerViewModel);
        }

        // POST: /AccountManager/Delete/5
        [Authorize(Roles = "consultant")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            var userMembershipToDelete = userManager.FindById(id);

            if (userMembershipToDelete != null)
            {
                userManager.Delete(userMembershipToDelete);
            }

            /*adminRepository.Delete(admin);
            adminRepository.SaveChanges();*/
            return RedirectToAction("Index");
        }

        
    }
}
