using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Inside.AutoRating;
using Inside.ContactService;
using Inside.membership;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Models;
using InsideReporting.Models.Chat;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using RestSharp;

namespace InsideReporting.Controllers.Data
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class ChatController : Controller
    {
        private readonly IRepository<Contact> chatRepository;
        private readonly IRepository<Client> clientRepository;
        private readonly IContactService contactService;
        private readonly IRestClient restClient;
        private IServerTime serverTime;
        private IIdentityMembershipProvider userManager;
        private readonly IContactAutoRating contactAutoRating;

        public ChatController(
            IRepository<Contact> chatRepository,
            IRepository<Client> clientRepository,
            IContactService contactService,
            IServerTime serverTime,
            IRestClient restClient,
             IIdentityMembershipProvider userManager,
             IContactAutoRating contactAutoRating)
        {
            this.chatRepository = chatRepository;
            this.clientRepository = clientRepository;
             this.contactService = contactService;
             this.serverTime = serverTime;
             this.restClient = restClient;
             this.userManager = userManager;
             this.contactAutoRating = contactAutoRating;
        }
        // GET: /Chat/
        [Authorize(Roles = "consultant")]
        public ActionResult Index()
        {
            var chats = new ChatListViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            var chatEntities =
                chatRepository.Where(l => l.LeadType == "Chat")
                    .Include(c => c.Interaction)
                    .Include(c => c.Property)
                    .AsNoTracking()
                    .OrderByDescending(c => c.Date);

            foreach (var chatItemInRepository in chatEntities)
            {
                var chatViewModel = new ChatViewModel();
                chatViewModel.Id = chatItemInRepository.Id;
                chatViewModel.ClientId = chatItemInRepository.ClientId;

                chatViewModel.Date = serverTime.ConvertServerTimeToStandardUserTime(chatItemInRepository.Date);
                if (chatItemInRepository.HasProperty("Description"))
                {
                    var description = chatItemInRepository.GetProperty("Description").Value;
                    if (description.Length > 60)
                    {
                        chatViewModel.Description = description.Substring(0, 57) + "...";    
                    }
                    else
                    {
                        chatViewModel.Description = description;    
                    }
                }
                if (chatItemInRepository.HasProperty("Email"))
                {
                    chatViewModel.Email = chatItemInRepository.GetProperty("Email").Value;
                }
                if (chatItemInRepository.HasProperty("Phone"))
                {
                    chatViewModel.Phone = chatItemInRepository.GetProperty("Phone").Value;
                }

                chatViewModel.ClientName = clientRepository.Where(cr => cr.Id == chatItemInRepository.ClientId).First().Name;
                chats.Collection.Add(chatViewModel);

            }
            return View(chats);
        }

        // GET: /Chat/Create
        [Authorize(Roles = "consultant")]
        public ActionResult Create()
        {
            ViewBag.ClientIds = new SelectList(
                   clientRepository.Where(cr => cr.IsActive).OrderBy(c => c.Name), "Id", "Name");
            var chatViewModel = new ChatViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            return View(chatViewModel);
        }
        //
        // POST: /Chat/Create
        [Authorize(Roles = "consultant")]
        [HttpPost]
        public ActionResult Create(ChatViewModel chatViewModel)
        {

            ViewBag.ClientIds = new SelectList(
                    clientRepository.Where(cr=>cr.IsActive).OrderBy(c=>c.Name), "Id", "Name");
            
            if (!ModelState.IsValid) 
                return View(chatViewModel);

            var contact = new Contact()
            {
                LeadType = "Chat",
                ClientId = chatViewModel.ClientId,
                Date = serverTime.ConvertUserStandardTimeToServerTime(chatViewModel.Date)
            };
            if (!chatViewModel.Description.IsNullOrWhiteSpace())
            {
                contact.SetPropertyValue("Description", chatViewModel.Description);
            }
            if (!chatViewModel.Email.IsNullOrWhiteSpace())
            {
                contact.SetPropertyValue("Email", chatViewModel.Email);
            }
            if (!chatViewModel.Phone.IsNullOrWhiteSpace())
            {
                contact.SetPropertyValue("Phone", chatViewModel.Phone);
            }
            if (!chatViewModel.LiveChatId.IsNullOrWhiteSpace())
            {
                contact.SetPropertyValue("LiveChatId", chatViewModel.LiveChatId);
            }

            contactAutoRating.SetAutoRating(contact);

            chatRepository.Add(contact);
            chatRepository.SaveChanges();
            contactService.NotifyClientsForNewContactWithEmail(contact.Id);
            contactService.NotifyClientsForNewContactWithPhoneNotification(contact.Id);
            contactService.NotifyClientsForNewContactWithSmsNotification(contact.Id);
            return RedirectToAction("Index");
        }


        //
        // GET: /Chat/Edit/5
        [Authorize(Roles = "consultant")]
        public ActionResult Edit(int id)
        {
            Contact contact = chatRepository.Where(ch => ch.Id == id).Single();

            ChatViewModel chatViewModel = new ChatViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            chatViewModel.Id = id;
            chatViewModel.ClientId = contact.ClientId;
            chatViewModel.Date = serverTime.ConvertServerTimeToStandardUserTime(contact.Date);
            if (contact.HasProperty("Description"))
            {
                chatViewModel.Description = contact.GetProperty("Description").Value;
            }
            if (contact.HasProperty("Email"))
            {
                chatViewModel.Email = contact.GetProperty("Email").Value;
            }
            if (contact.HasProperty("Phone"))
            {
                chatViewModel.Phone = contact.GetProperty("Phone").Value;
            }
            if (contact.HasProperty("LiveChatId"))
            {
                chatViewModel.LiveChatId = contact.GetProperty("LiveChatId").Value;
            }
            chatViewModel.ClientName = clientRepository.Where(cr => cr.Id == contact.ClientId).First().Name;

            ViewBag.ClientIds = new SelectList(
                clientRepository.Where(cr => cr.IsActive).OrderBy(c => c.Name), "Id", "Name");

            return View(chatViewModel);
        }

        //
        // POST: /Chat/Edit/5
        [Authorize(Roles = "consultant")]
        [HttpPost]
        public ActionResult Edit(ChatViewModel chatViewModel)
        {
            ViewBag.ClientIds = new SelectList(
                            clientRepository.Where(cr => cr.IsActive).OrderBy(c => c.Name), "Id", "Name");
            if (!ModelState.IsValid)
                return View(chatViewModel);
            
            var changedChatContact = chatRepository.Where(ch => ch.Id == chatViewModel.Id).Single();

            changedChatContact.ClientId = chatViewModel.ClientId;
            changedChatContact.Date = serverTime.ConvertUserStandardTimeToServerTime(chatViewModel.Date);
            if (!chatViewModel.Description.IsNullOrWhiteSpace())
            {
                changedChatContact.SetPropertyValue("Description", chatViewModel.Description);
            }
            if (!chatViewModel.Email.IsNullOrWhiteSpace())
            {
                changedChatContact.SetPropertyValue("Email", chatViewModel.Email);
            }
            if (!chatViewModel.Phone.IsNullOrWhiteSpace())
            {
                changedChatContact.SetPropertyValue("Phone", chatViewModel.Phone);
            }
            if (!chatViewModel.LiveChatId.IsNullOrWhiteSpace())
            {
                changedChatContact.SetPropertyValue("LiveChatId", chatViewModel.LiveChatId);
            }

            chatRepository.SaveChanges();

            return RedirectToAction("Index");
        }

        //
        // GET: /Chat/Delete/5
        [Authorize(Roles = "consultant")]
        public ActionResult Delete(int id)
        {
            Contact chat = chatRepository.Where(ch => ch.Id == id).Single();
            ChatViewModel chatViewModel = new ChatViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            chatViewModel.Id = chat.Id;
            chatViewModel.ClientId = chat.ClientId;
            chatViewModel.Date = serverTime.ConvertServerTimeToStandardUserTime(chat.Date);
            if (chat.HasProperty("Description"))
            {
                chatViewModel.Description = chat.GetProperty("Description").Value;
            }
            if (chat.HasProperty("Email"))
            {
                chatViewModel.Email = chat.GetProperty("Email").Value;
            }
            if (chat.HasProperty("Phone"))
            {
                chatViewModel.Phone = chat.GetProperty("Phone").Value;
            }
            chatViewModel.ClientName = clientRepository.Where(cr => cr.Id == chat.ClientId).First().Name;
            
            return View(chatViewModel);
        }

        //
        // POST: /Chat/Delete/5
        [Authorize(Roles = "consultant")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Contact chat = chatRepository.Where(ch => ch.Id == id).Include(c => c.Interaction).Include(c => c.Property).Single();
            chatRepository.Delete(chat);
            chatRepository.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ChatMessages(int? page, DateTime fromDate)
        {
            var request = new RestRequest("/chats", Method.GET);
            var username = "peter@helloy.se";
            var password = "9814cb931c5a90b14a7d9d5facb0f7b1";

            restClient.BaseUrl = new Uri("https://api.livechatinc.com");
            request.AddParameter("date_from", fromDate.ToString("yyyy-MM-dd"));
            //request.AddParameter("date_to", dateTo.ToString("yyyy-MM-dd"));

            if (page != null)
            {
                request.AddParameter("page", page);
            }

            request.AddHeader("X-API-VERSION", "2");
            restClient.Authenticator = new HttpBasicAuthenticator(username,password);
            
            var result = restClient.Execute(request);

            return Content(result.Content, "application/json");
        }
    }
}
