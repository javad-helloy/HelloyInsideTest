using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Models;
using InsideReporting.Models.Client;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Controllers.Enity
{ 
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class ClientController : Controller
    {
        private readonly IRepository<Client> clientRepository;
        private readonly IRepository<Label> labelRepository;
        private readonly IRepository<InsideUser> _adminRepository;
        private IIdentityMembershipProvider userManager;


        public ClientController(
            IRepository<Client> clientRepository,
            IRepository<Label> labelRepository,
            IRepository<InsideUser> adminRepository,
            IIdentityMembershipProvider userManager)
        {
            this.clientRepository = clientRepository;
            this.labelRepository = labelRepository;
            _adminRepository = adminRepository;
            this.userManager = userManager;
        }


        //
        // GET: /Client/

        [Authorize(Roles = "consultant, sales")]
        public ViewResult Index(bool? showAllData, bool? showInactive)
        {
            var clientViewModels = new ClientsViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            var providerUserKey = User.Identity.GetUserId();
            
            if (showAllData == null)
            {
                showAllData = false;
            }
            IEnumerable<Client> clients = null;

            if (showInactive.HasValue && showInactive.Value)
            {
                clients = clientRepository.Where(c => !c.IsActive).Include(c => c.InsideUserSets);
            }
            else if (showAllData.HasValue && showAllData.Value)
            {
                clients = clientRepository.Where(c=>c.IsActive).Include(c => c.InsideUserSets);
            }
            else
            {
                var adminId = _adminRepository.First(a => a.Id == providerUserKey).Id;
                
                clients = clientRepository
                   .Where(c => c.AccountManagerId == adminId || c.ConsultantId == adminId).Where(c=>c.IsActive)
                   .Include(c => c.InsideUserSets)
                   .ToList();
            }
                
            foreach (var client in clients.ToList())
            {
                var clientViewModel = new ClientViewModel(client);

                DateTime? lastLogin = null;
                try
                {
                    var insideUserSets = client.InsideUserSets;
                    if (insideUserSets.Count > 0)
                    {
                        lastLogin = insideUserSets.Select(u => u.LastLoginDate).Max();
                    }
                }
                catch (Exception e)
                {
                    lastLogin = null;
                }
                

                clientViewModel.LastLogin = lastLogin;
                var now = DateTime.Now;
                if (!lastLogin.HasValue)
                {
                    clientViewModel.ActivityLevel = 0;
                }
                else if ((now - lastLogin.Value).Days < 7)
                {
                    clientViewModel.ActivityLevel = 2;
                }
                else if ((now - lastLogin.Value).Days < 30)
                {
                    clientViewModel.ActivityLevel = 1;
                }
                else
                {
                    clientViewModel.ActivityLevel = 0;
                }
                
                clientViewModels.Clients.Add(clientViewModel);
            }

            clientViewModels.Labels = labelRepository.All().ToList().Select(l => new LabelViewModel(l)).ToList();

            return View(clientViewModels);
        }

        //
        // GET: /Client/Create
        [Authorize(Roles = "consultant, sales")]
        public ActionResult Create()
        {
            var clientViewModel = new ClientViewModel();
            clientViewModel.IsActive = true;

            var clientPageLoggedInviewModel = new ClientPageLoggedViewModel(userManager.GetRoles(User.Identity.GetUserId()), clientViewModel);
            
            var consultantListToDropDown = _adminRepository.Where(cr=>!cr.IsLockedOut && cr.Role.Any(r=>r.Name=="consultant")).Select(c => new { Id = c.Id, Name = c.Name }).ToList();
            consultantListToDropDown.Insert(0, new { Id = "", Name = "" });
            clientPageLoggedInviewModel.Consultant = new SelectList(
                consultantListToDropDown, "Id", "Name");

            var accountManagerListToDropDown = _adminRepository.Where(cr => !cr.IsLockedOut && cr.Role.Any(r => r.Name == "sales")).Select(c => new { Id = c.Id, Name = c.Name }).ToList();
            accountManagerListToDropDown.Insert(0, new { Id = "", Name = "" });
            clientPageLoggedInviewModel.AccountManager = new SelectList(
                accountManagerListToDropDown, "Id", "Name");

            return View(clientPageLoggedInviewModel);
        } 

        //
        // POST: /Client/Create
        [Authorize(Roles = "consultant, sales")]
        [HttpPost]
        public ActionResult Create(ClientPageLoggedViewModel clientPageLoggedinViewModel)
        {
            var clientViewModel = clientPageLoggedinViewModel.ClientViewModel;
            if (ModelState.IsValid)
            {
                //client.Consultant = _adminRepository.Where(cr=>cr.Id==client.ConsultantId).Single();
                var client = new Client()
                {
                    AccountManagerId = clientViewModel.AccountManagerId,
                    Address = clientViewModel.Address,
                    CallTrackingMetricId = clientViewModel.CallTrackingMetricId,
                    ConsultantId = clientViewModel.ConsultantId,
                    AnalyticsTableId = clientViewModel.AnalyticsTableId,
                    Domain = clientViewModel.Domain,
                    EmailAddress = clientViewModel.EmailAddress,
                    IsActive = true,
                    Latitude = clientViewModel.Latitude,
                    Longitude = clientViewModel.Longitude,
                    Name = clientViewModel.Name,
                    FeeFixedMonthly = clientViewModel.FeeFixedMonthly,
                    FeePercent = ((decimal)(clientViewModel.FeePercent))/100m
                    
                };
                clientRepository.Add(client);
                clientRepository.SaveChanges();
                return RedirectToAction("Index");  
            }

            var consultantListToDropDown = _adminRepository.Where(cr => !cr.IsLockedOut && cr.Role.Any(r => r.Name == "consultant")).Select(c => new { Id = c.Id, Name = c.Name }).ToList();
            consultantListToDropDown.Insert(0, new { Id ="", Name = "" });
            clientPageLoggedinViewModel.Consultant = new SelectList(
                consultantListToDropDown, "Id", "Name");

            var accountManagerListToDropDown = _adminRepository.Where(cr => !cr.IsLockedOut && cr.Role.Any(r => r.Name == "sales")).Select(c => new { Id = c.Id, Name = c.Name }).ToList();
            accountManagerListToDropDown.Insert(0, new { Id = "", Name = "" });
            clientPageLoggedinViewModel.AccountManager = new SelectList(
                accountManagerListToDropDown, "Id", "Name");

            return View(clientPageLoggedinViewModel);
        }
        
        //
        // GET: /Client/Edit/5
        [Authorize(Roles = "consultant, sales")]
        public ActionResult Edit(int id)
        {
            
            Client client = clientRepository.Where(c => c.Id == id).Single();
            var clientViewModel = new ClientViewModel(client);
            var clientPageViewModel = new ClientPageViewModel(userManager.GetRoles(User.Identity.GetUserId()), clientViewModel);

            var consultantListToDropDown = _adminRepository.Where(cr => !cr.IsLockedOut && cr.Role.Any(r => r.Name == "consultant")).Select(c => new { Id = c.Id, Name = c.Name }).ToList();
            consultantListToDropDown.Insert(0,new { Id = "", Name = "" });
            clientPageViewModel.Consultant = new SelectList(
                consultantListToDropDown, "Id", "Name");

            var accountManagerListToDropDown = _adminRepository.Where(cr => !cr.IsLockedOut && cr.Role.Any(r => r.Name == "sales")).Select(c => new { Id = c.Id, Name = c.Name }).ToList();
            accountManagerListToDropDown.Insert(0, new { Id = "", Name = "" });
            clientPageViewModel.AccountManager = new SelectList(
                accountManagerListToDropDown, "Id", "Name");

            clientPageViewModel.HasAdminMenu = this.User.IsInRole("sales") || this.User.IsInRole("consultant");
            clientPageViewModel.ClientId = id;
            clientPageViewModel.ClientName = client.Name;
            clientPageViewModel.HasWebTab = client.Labels.Any(l => l.Name == "Webbflik");

            return View(clientPageViewModel);
        }

        //
        // POST: /Client/Edit/5
        [HttpPost]
        [Authorize(Roles = "consultant, sales")]
        public ActionResult Edit(ClientPageViewModel clientLoggedInViewModel)
        {
            var clientViewModel = clientLoggedInViewModel.ClientViewModel;
            if (ModelState.IsValid)
            {
                var client = new Client()
                {
                    AccountManagerId = clientViewModel.AccountManagerId,
                    Address = clientViewModel.Address,
                    CallTrackingMetricId = clientViewModel.CallTrackingMetricId,
                    ConsultantId = clientViewModel.ConsultantId,
                    AnalyticsTableId = clientViewModel.AnalyticsTableId,
                    Domain = clientViewModel.Domain,
                    EmailAddress = clientViewModel.EmailAddress,
                    Id = clientViewModel.Id,
                    IsActive = clientViewModel.IsActive,
                    Latitude = clientViewModel.Latitude,
                    Longitude = clientViewModel.Longitude,
                    Name = clientViewModel.Name,
                    FeeFixedMonthly = clientViewModel.FeeFixedMonthly,
                    FeePercent = ((decimal)(clientViewModel.FeePercent)) / 100m
                };
                //client.ConsultantId = 1;
                clientRepository.Attach(client);
                clientRepository.SetState(client, EntityState.Modified);
                clientRepository.SaveChanges();
                return RedirectToAction("Index");
            }
            var consultantListToDropDown = _adminRepository.Where(cr => !cr.IsLockedOut && cr.Role.Any(r => r.Name == "consultant")).Select(c => new { Id = c.Id, Name = c.Name }).ToList();
            consultantListToDropDown.Insert(0, new { Id = "", Name = "" });
            clientLoggedInViewModel.Consultant = new SelectList(
                consultantListToDropDown, "Id", "Name");

            var accountManagerListToDropDown = _adminRepository.Where(cr => !cr.IsLockedOut && cr.Role.Any(r => r.Name == "sales")).Select(c => new { Id = c.Id, Name = c.Name }).ToList();
            accountManagerListToDropDown.Insert(0, new { Id = "", Name = "" });
            clientLoggedInViewModel.AccountManager = new SelectList(
                accountManagerListToDropDown, "Id", "Name");

            return View(clientLoggedInViewModel);
        }



    }
}