using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Newtonsoft.Json;
using Task.SmsNotification.SmsNotificationSenderToUser;
using Task.TaskCreator;

namespace Task.SmsNotification.SmsNotificationUserTaskCreator
{
    public class SmsNotificationTaskPerformer : ISmsNotificationTaskPerformer
    {

        private readonly IRepository<Contact> contactRepository;
        private readonly ITaskManager taskManager;
        private readonly IServerTime serverTime;
        private readonly IRepository<InsideModel.Models.Task> taskRepository;
        private readonly ITaskQueueStorage taskQueueStorage;



        public SmsNotificationTaskPerformer(IRepository<Contact> contactRepository,
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
            return taskType == TaskType.CreateTaskForUsersToSendNewContactSmsNotification;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var sendSmsTask = JsonConvert.DeserializeObject<SmsNotificationTaskMessage>(taskMessage.Message);
            if (!IsContactValidToSend(sendSmsTask.ContactId))
            {
                return;
            }
            var users =
                contactRepository.First(c => c.Id == sendSmsTask.ContactId)
                    .Client.InsideUserSets.Where(u => u.ReceiveSms!=null && (bool)u.ReceiveSms && !string.IsNullOrEmpty(u.Phone)).ToList();

            if (!users.Any())
            {
                return;
            }

            var dayOfExecution = serverTime.RequestStarted.Date;
            var dayAfterExecution = serverTime.RequestStarted.AddDays(1).Date;
            var addedTask = new List<InsideModel.Models.Task>();

            foreach (var insideUser in users)
            {
                var createTaskForUserSpecificNotification = new UserSpecificSmsNotificationTaskMessage()
                {
                    ContactId = sendSmsTask.ContactId,
                    UserId = insideUser.Id
                };
                var messageToAdd = JsonConvert.SerializeObject(createTaskForUserSpecificNotification);

                if (!taskManager.HasTaskInRepository(messageToAdd, TaskType.SendNewContactSmsNotificationToUser, dayOfExecution,
                    dayAfterExecution))
                {
                    var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.SendNewContactSmsNotificationToUser, serverTime.RequestStarted);
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