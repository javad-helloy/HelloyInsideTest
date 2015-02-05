using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Models.User;

namespace InsideReporting.Controllers.Enity
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class UserController : Controller
    {
        private readonly IRepository<Client> clientRepository;
        private readonly IRepository<InsideUser> userRepository;
        private IIdentityMembershipProvider userManager;

        public UserController(
            IRepository<Client> clientRepository,
            IRepository<InsideUser> userRepository,
            IIdentityMembershipProvider userManager)
        {
            this.clientRepository = clientRepository;
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        //
        // GET: /User/List
        [Authorize(Roles = "consultant,sales")]
        public ViewResult List(int clientId)
        {
            var client = clientRepository.Where(c => c.Id == clientId).Single();
            var viewModel = new UserListViewModel();

            foreach (var user in client.InsideUserSets)
            {
                viewModel.Users.Add(new UserViewModel(user));
            }

            viewModel.ClientId = clientId;
            viewModel.ClientName = client.Name;
            viewModel.HasAdminMenu = this.User.IsInRole("sales") || this.User.IsInRole("consultant");
            viewModel.HasWebTab = client.Labels.Any(l => l.Name == "Webbflik");

            return View(viewModel);
        }

        //
        // GET: /User/Create
        [Authorize(Roles = "consultant, sales")]
        public ActionResult Create(int clientId)
        {
            var viewModel = new UserCreateViewModel();
            var client = clientRepository.Single(c => c.Id == clientId);

            viewModel.ClientId = clientId;
            viewModel.ClientName = client.Name;
            
            viewModel.HasAdminMenu = User.IsInRole("sales") || User.IsInRole("consultant");
            viewModel.HasWebTab = client.Labels.Any(l => l.Name == "Webbflik");

            return View(viewModel);
        } 

        //
        // POST: /User/Create
        [Authorize(Roles = "consultant, sales")]
        [HttpPost]
        public ActionResult Create(UserViewModel user)
        {
            if (user.Password == null)
            {
                ModelState.AddModelError("password", "Saknade lösenord.");
                return Create(user.ClientId);
            }

            var newUser = new InsideUser
            {
                Email = user.Name,
                ClientId = user.ClientId,
                ReceiveEmail = user.ReceiveEmail,
                ReceiveSms =  user.ReceiveSms,
                UserName = user.Name,
                Phone = user.Phone,
                IsLockedOut = user.IsLockedOut
            };

            var newUserMembershipSuccess = userManager.Create(newUser, user.Password);
            if (newUserMembershipSuccess)
            {
                userManager.AddToRole(newUser.Id, "client");
            }
            else
            {
                ModelState.AddModelError("name", "Misslyckades skapa användare.");
                return Create(user.ClientId);
            }

            return RedirectToAction("List", new {clientId = user.ClientId});  
        }

        //
        // GET: /User/Edit/5
        [Authorize(Roles = "consultant,sales")]
        public ActionResult Edit(string id)
        {
            var user = userRepository.Single(ur => ur.Id ==  id);
            var client = user.Client;

            var userModel = new UserViewModel(user);
            var model = new UserEditViewModel(userModel);
            model.ClientId = client.Id;
            model.ClientName = client.Name;
            model.HasAdminMenu = this.User.IsInRole("sales") || this.User.IsInRole("consultant");
            model.HasWebTab = client.Labels.Any(l => l.Name == "Webbflik");

            return View(model);
        }


        //
        // POST: /User/Edit/5
        [Authorize(Roles = "consultant,sales")]
        [HttpPost]
        public ActionResult Edit(UserViewModel user)
        {
            var changedUser = userManager.FindById(user.Id);

            if(!string.IsNullOrEmpty(user.Password))
            {
                var changePasswordOperationSuccess = userManager.UpdatePassword(changedUser.Id, user.Password);
                if (!changePasswordOperationSuccess)
                {
                    ModelState.AddModelError("password", "Misslyckades med att uppdatera lösenordet för" + user.Name);
                    return Edit(user.Id);        
                }
            }

            changedUser.ReceiveEmail = user.ReceiveEmail;
            changedUser.ReceiveSms = user.ReceiveSms;
            changedUser.Phone = user.Phone;
            changedUser.IsLockedOut = user.IsLockedOut;
            userManager.Update(changedUser);

            return RedirectToAction("List", new { clientId = changedUser.ClientId });
        }

        //
        // POST: /User/Delete/5
        [Authorize(Roles = "consultant,sales")]
        public ActionResult DeleteClientUser(string userId, int clientId)
        {
            var user = userManager.FindById(userId);
            
            var isAdmin = userManager.IsInRole(userId, "consultant");
            var isSales = userManager.IsInRole(userId, "sales");

            if (isAdmin || isSales)
            {
                throw new UnauthorizedAccessException();
            }

            userManager.Delete(user);

            return RedirectToAction("List", new { clientId = clientId });
        }
    }
}