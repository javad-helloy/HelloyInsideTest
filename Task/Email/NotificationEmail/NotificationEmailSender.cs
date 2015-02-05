using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Newtonsoft.Json;
using Task.TaskCreator;
using Task.TaskPerformer.Performer;

namespace Task.Email.NotificationEmail
{
    public interface INotificationEmailSender : ITaskPerformer { }

    public class NotificationEmailSender : INotificationEmailSender
    {
        private readonly IRepository<Contact> contactRepository;
        private readonly ITaskManager taskManager;
        private readonly IServerTime serverTime;
        private readonly IRepository<InsideModel.Models.Task> taskRepository;
        private readonly ITaskQueueStorage taskQueueStorage;


        public NotificationEmailSender(
            
            IRepository<Contact> contactRepository,
            ITaskManager taskManager,
            IServerTime serverTime,
            IRepository<InsideModel.Models.Task> taskRepository,
            ITaskQueueStorage taskQueueStorage
            )
        {
            this.contactRepository = contactRepository;
            this.taskManager = taskManager;
            this.serverTime = serverTime;
            this.taskRepository = taskRepository;
            this.taskQueueStorage = taskQueueStorage;
        }

        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.CreateTaskForUsersToSendNewContactEmailNotification;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var sendEmailTask = JsonConvert.DeserializeObject<NotificationTaskMessage>(taskMessage.Message);
            if (!IsContactValidToSend(sendEmailTask.ContactId))
            {
                return;
            }
            var users =
                contactRepository.First(c => c.Id == sendEmailTask.ContactId)
                    .Client.InsideUserSets.Where(u => u.ReceiveEmail!= null && (bool)u.ReceiveEmail).ToList();

            if (!users.Any())
            {
                return;
            }

            var dayOfExecution = serverTime.RequestStarted.Date;
            var dayAfterExecution = serverTime.RequestStarted.AddDays(1).Date;
            var addedTask = new List<InsideModel.Models.Task>();

            foreach (var insideUser in users)
            {
                var createTaskForUserSpecificNotification = new UserSpecificNotificationTaskMessage()
                {
                    ContactId = sendEmailTask.ContactId,
                    UserId = insideUser.Id
                };
                var messageToAdd = JsonConvert.SerializeObject(createTaskForUserSpecificNotification);

                if (!taskManager.HasTaskInRepository(messageToAdd, TaskType.SendNewContactEmailNotificationToUser, dayOfExecution,
                    dayAfterExecution))
                {
                    var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.SendNewContactEmailNotificationToUser, serverTime.RequestStarted);
                    taskQueueStorage.Add(taskToAdd);
                }

            }
        }

        private bool IsContactValidToSend(int contactId)
        {
            var contactNotificationToSend = contactRepository.Where(c => c.Id == contactId)
                .Include(l => l.Client);

            if (!contactNotificationToSend.Any()) return false;
            var client = contactNotificationToSend.First().Client;
            if (!client.IsActive) return false;
            if (!client.Labels.Where(l => l.Name == "Helloypaket").Any()) return false;
            
            return true;
        }

        
    }
}
