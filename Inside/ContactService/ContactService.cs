using System.Collections.Generic;
using System.Linq;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Newtonsoft.Json;

namespace Inside.ContactService
{
    public class ContactService : IContactService
    {
        private IRepository<Contact> contactRepository;
        private ITaskQueueStorage taskQueueStorage;
        private IRepository<InsideModel.Models.Task> taskRepository;
        private IServerTime serverTime;

        public ContactService(IRepository<Contact> contactRepository,
            ITaskQueueStorage taskQueueStorage,
            IRepository<InsideModel.Models.Task> taskRepository,
            IServerTime serverTime)
        {
            this.contactRepository = contactRepository;
            this.taskQueueStorage = taskQueueStorage;
            this.taskRepository = taskRepository;
            this.serverTime = serverTime;
        }

       

        public void NotifyClientsForNewContactWithEmail(int contactId)
        {
            var addedTask = new List<InsideModel.Models.Task>();
            var messageToAdd =
                JsonConvert.SerializeObject(new NewContactTaskMessage() { ContactId = contactId });

            var dayOfExecution = serverTime.RequestStarted;
            var hasTaskInRepository = taskRepository.Where(tr => tr.Message == messageToAdd
                                            && dayOfExecution == tr.EarliestExecution
                                            && tr.Type == TaskType.CreateTaskForUsersToSendNewContactEmailNotification).Any();
            if (!hasTaskInRepository)
            {
                var taskToAdd = new InsideModel.Models.Task(messageToAdd,
                    TaskType.CreateTaskForUsersToSendNewContactEmailNotification, serverTime.RequestStarted);
                taskQueueStorage.Add(taskToAdd);
            }
        }

        public void NotifyClientsForNewContactWithPhoneNotification(int contactId)
        {
            var addedTask = new List<InsideModel.Models.Task>();
            var messageToAdd =
                JsonConvert.SerializeObject(new NewContactTaskMessage() { ContactId = contactId });

            var dayOfExecution = serverTime.RequestStarted;
            var hasTaskInRepository = taskRepository.Where(tr => tr.Message == messageToAdd
                                            && dayOfExecution == tr.EarliestExecution
                                            && tr.Type == TaskType.SendNewContactPhoneNotification).Any();
            if (!hasTaskInRepository)
            {
                var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.SendNewContactPhoneNotification,
                    serverTime.RequestStarted);
                taskQueueStorage.Add(taskToAdd);
             
            }
        }

        public void NotifyClientsForNewContactWithSmsNotification(int contactId)
        {
            var addedTask = new List<InsideModel.Models.Task>();
            var messageToAdd =
                JsonConvert.SerializeObject(new NewContactTaskMessage() { ContactId = contactId });

            var dayOfExecution = serverTime.RequestStarted;
            var hasTaskInRepository = taskRepository.Where(tr => tr.Message == messageToAdd
                                            && dayOfExecution == tr.EarliestExecution
                                            && tr.Type == TaskType.CreateTaskForUsersToSendNewContactSmsNotification).Any();
            if (!hasTaskInRepository)
            {
                var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.CreateTaskForUsersToSendNewContactSmsNotification,
                    serverTime.RequestStarted);

                taskQueueStorage.Add(taskToAdd);
            }
        }
    }

    public class NewContactTaskMessage
    {
        public int ContactId { get; set; }
    }
}
