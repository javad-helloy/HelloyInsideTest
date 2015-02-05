using System.Data.Entity;
using System.Linq;
using InsideModel.Models;
using InsideModel.repositories;
using Newtonsoft.Json;
using Task.Email.NotificationEmail;

namespace Task.PhoneNotification
{
    public class PhoneNotificationTaskPerformer : IPhoneNotificationTaskPerformer
    {
        private readonly INotificationSender phoneNotificationSender;
        private readonly IPhoneNotificationTextBuilder notificationPhoneTextBuilder;
        private readonly IRepository<Contact> contactRepository;


        public PhoneNotificationTaskPerformer(
            INotificationSender phoneNotificationSender,
            IPhoneNotificationTextBuilder notificationPhoneTextBuilder,
            IRepository<Contact> contactRepository
            )
        {
            this.phoneNotificationSender = phoneNotificationSender;
            this.contactRepository = contactRepository;
            this.notificationPhoneTextBuilder = notificationPhoneTextBuilder;
        }
        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.SendNewContactPhoneNotification;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var sendEmailTask = JsonConvert.DeserializeObject<NotificationTaskMessage>(taskMessage.Message);

            var contactId = sendEmailTask.ContactId;
            var contactNotificationToSend = contactRepository.Where(c => c.Id == contactId)
                .Include(l => l.Client);
            if (!contactNotificationToSend.Any())return;
            var client = contactNotificationToSend.First().Client;
            if (!client.IsActive) return;
            if (!client.Labels.Where(l => l.Name == "Helloypaket").Any()) return;
            
            var notificationText = notificationPhoneTextBuilder.GetDefinition(contactId);
            
            phoneNotificationSender.SendNotification(client.Id, notificationText);
        }

    }
}