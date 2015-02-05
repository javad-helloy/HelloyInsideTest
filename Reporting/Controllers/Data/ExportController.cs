using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Inside.membership;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Helpers;

namespace InsideReporting.Controllers.Data
{
    public class ExportController : AuthenticationController
    {

         private IRepository<Contact> contactRepository;
         
         public ExportController(IRepository<Contact> contactRepository, IIdentityMembershipProvider userManager) : base(userManager)
        {
            this.contactRepository = contactRepository;
        }

        [AuthorizeClientAccess]
        public ActionResult ExportData(int clientId)
        {
            var contacts =
                contactRepository.Where(l => l.ClientId == clientId)
                    .OrderByDescending(l => l.Date)
                    .AsNoTracking()
                    .Include(l => l.Interaction)
                    .Include(l => l.Property);

            var contactsToExport = new List<ExportModel>();

            foreach (var contact in contacts)
            {
                var exportContact = new ExportModel
                {
                    Id = contact.Id,
                    Date = contact.Date
                };

                if (contact.LeadType == "Phone")
                {
                    exportContact.ContactType = "Telefon";
                    var phoneCallerNumber = contact.Property.SingleOrDefault(cp => cp.Type == "CallerNumber");
                    exportContact.Phone = phoneCallerNumber != null ? phoneCallerNumber.Value : "";
                }
                else if (contact.LeadType == "Email")
                {
                    exportContact.ContactType = "E-post";
                    var emailFrom = contact.Property.SingleOrDefault(cp => cp.Type == "FromEmail");
                    exportContact.Email = emailFrom != null ? emailFrom.Value : "";
                }
                else if (contact.LeadType == "Chat")
                {
                    exportContact.ContactType = "Chat";
                    var chatPhone = contact.Property.SingleOrDefault(cp => cp.Type == "Phone");
                    if (chatPhone != null)
                    {
                        exportContact.Phone = chatPhone.Value;
                    }

                    var chatEmail = contact.Property.SingleOrDefault(cp => cp.Type == "Email");
                    if (chatEmail != null)
                    {
                        exportContact.Email = chatEmail.Value;

                    }
                }

                var contactProduct = contact.Property.SingleOrDefault(cp => cp.Type == "Product");
                if (contactProduct != null)
                {
                    exportContact.Product = contactProduct.Value;
                }

                var contactCampain = contact.Property.SingleOrDefault(cp => cp.Type == "Campaign");
                if (contactCampain != null)
                {
                    exportContact.Campaign = contactCampain.Value;
                }
                var contactsearchPhrase = contact.Property.SingleOrDefault(cp => cp.Type == "SearchPhrase");
                if (contactsearchPhrase != null)
                {
                    exportContact.SearchPhrase = contactsearchPhrase.Value;
                }

                if (contact.Interaction.Any(ci => ci.Type == "RatingScore"))
                {
                    exportContact.Rating = int.Parse(contact.Interaction.Single(ci => ci.Type == "RatingScore").Value);
                }

                contactsToExport.Add(exportContact);
            }

            string result = ConvertListToCsv(contactsToExport);
            var filename = "exportera("+DateTime.Now.ToString("yy-MM-dd")+").csv";
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
            Response.ContentType = "application/csv";
            Response.Charset = "iso-8859-1";
            Response.ContentEncoding = Encoding.GetEncoding("iso-8859-1");
            Response.Write(result);
            Response.End();

            return Content(String.Empty);
        }

        private string ConvertListToCsv<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            Type t = typeof(T);

            object obj = Activator.CreateInstance(t);
            PropertyInfo[] props = obj.GetType().GetProperties();
            byte[] carriageReturnBytes = System.Text.Encoding.UTF8.GetBytes("\r");

            string text;
            using (MemoryStream ms = new MemoryStream())
            using (StreamReader sr = new StreamReader(ms))
            {
                foreach (PropertyInfo pi in props)
                {
                    var attribute = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault()  as DisplayNameAttribute;
                    if (attribute != null)
                    {
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(attribute.DisplayName + ";");
                        ms.Write(data, 0, data.Length);
                    }
                }

                ms.Write(carriageReturnBytes, 0, carriageReturnBytes.Length);

                foreach (T item in list)
                {
                    foreach (PropertyInfo pi in props)
                    {
                        var attribute = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault()  as DisplayNameAttribute;
                        if (attribute != null)
                        {
                            string write ="\""+
                                Convert.ToString(item.GetType().GetProperty(pi.Name).GetValue(item, null))
                                    .Replace(";", "") + "\"" +';';

                            byte[] data = System.Text.Encoding.UTF8.GetBytes(write);
                            ms.Write(data, 0, data.Length);
                        }
                    }

                    byte[] writeNewLine = System.Text.Encoding.UTF8.GetBytes(Environment.NewLine);
                    ms.Write(writeNewLine, 0, writeNewLine.Length);
                }

                ms.Position = 0;
                text = sr.ReadToEnd();
                return text;
            }
        }

        public class ExportModel
        {
            
            public DateTime Date { get; set; }
            [DisplayName("Typ")]
            public string ContactType { get; set; }
            [DisplayName("Datum")]
            public string DateString {
                get { return this.Date.ToString("yy-MM-dd HH:mm"); } }
            [DisplayName("Helloy Product")]
            public string Product { get; set; }
            [DisplayName("Telefon")]
            public string Phone { get; set; }
            [DisplayName("E-Post")]
            public string Email { get; set; }
            [DisplayName("Betyg")]
            public int? Rating { get; set; }
            [DisplayName("Kampanj")]
            public string Campaign { get; set; }
            [DisplayName("Sökord")]
            public string SearchPhrase { get; set; }
            [DisplayName("Helloy Id")]
            public int Id { get; set; }
        }
    }
}