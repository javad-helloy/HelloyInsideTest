using System;
using System.Linq;
using System.Net;
using Google;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Task.ContactProduct.AnalyticData;
using Task.ContactProduct.AnalyticDataTaskCreator;
using Task.Email.NotificationEmail;
using Task.ImportCustomEvents;
using Task.ImportSeoData;
using Task.PhoneNotification;
using Task.RemoveExpiredTokens;
using Task.SmsNotification.SmsNotificationSenderToUser;
using Task.SmsNotification.SmsNotificationUserTaskCreator;
using Task.UpdatePhonecalls.UpdatePhoneCalls;
using Task.UpdatePhonecalls.UpdatePhonecallsTaskCreator;

namespace Task.TaskPerformer.Delegate
{
    public class TaskDelegate : ITaskDelegate
    {
        private readonly ITaskQueueStorage taskQueueStorage;
        private readonly IRepository<InsideModel.Models.Task> taskRepository;
        private readonly INotificationEmailSender _notificationEmailSender;
        private readonly IPhoneNotificationTaskPerformer phoneNotificationTaskPerformer;
        private readonly IAddAnalyticProductDataForClient addAnalyticProductDataForClient;
        private readonly ICreateAnalyticDataTasksForClients createAnalyticDataTasksForClients;
        private readonly ICustomEventsImporter customEventsImporter;
        private readonly IServerTime serverTime;
        private readonly ISeoDataImporter seoDataImporter;
        private readonly IUserNotificationEmailSender userEmailSender;
        private readonly IImportCustomEventsTaskCreator importCustomEventsTaskCreator;
        private readonly IRemoveExpiredTokens removeExpiredTokens;
        private readonly ICreateUpdatePhonecallsTasksForClients updatePhonecallsTaskCreator;
        private readonly IUpdatePhonecalls updatePhonecalls;
        private readonly ISmsNotificationTaskPerformer smsNotificationTaskCreator;
        private readonly IUserSmsNotificationTaskPerformer userSmsNotificationTaskPerformer;


        public TaskDelegate(
            ITaskQueueStorage taskQueueStorage,
            IRepository<InsideModel.Models.Task> taskRepository,
            IServerTime serverTime,
            INotificationEmailSender _notificationEmailSender,
            IPhoneNotificationTaskPerformer phoneNotificationTaskPerformer,
            IAddAnalyticProductDataForClient addAnalyticProductDataForClient,
            ICreateAnalyticDataTasksForClients createAnalyticDataTasksForClients,
            ICustomEventsImporter customEventsImporter,
            ISeoDataImporter seoDataImporter,
            IUserNotificationEmailSender userEmailSender,
            IImportCustomEventsTaskCreator importCustomEventsTaskCreator,
            IRemoveExpiredTokens removeExpiredTokens,
            ICreateUpdatePhonecallsTasksForClients updatePhonecallsTaskCreator,
            IUpdatePhonecalls updatePhonecalls,
            ISmsNotificationTaskPerformer smsNotificationTaskCreator,
            IUserSmsNotificationTaskPerformer userSmsNotificationTaskPerformer)
        {
            this.taskQueueStorage = taskQueueStorage;
            this.taskRepository = taskRepository;
            this.serverTime = serverTime;
            this._notificationEmailSender = _notificationEmailSender;
            this.phoneNotificationTaskPerformer = phoneNotificationTaskPerformer;
            this.addAnalyticProductDataForClient = addAnalyticProductDataForClient;
            this.createAnalyticDataTasksForClients = createAnalyticDataTasksForClients;
            this.customEventsImporter = customEventsImporter;
            this.seoDataImporter = seoDataImporter;
            this.userEmailSender = userEmailSender;
            this.importCustomEventsTaskCreator = importCustomEventsTaskCreator;
            this.removeExpiredTokens = removeExpiredTokens;
            this.updatePhonecallsTaskCreator = updatePhonecallsTaskCreator;
            this.updatePhonecalls = updatePhonecalls;
            this.smsNotificationTaskCreator = smsNotificationTaskCreator;
            this.userSmsNotificationTaskPerformer = userSmsNotificationTaskPerformer;
        }

        public void PerformNextTask(string messageOfTaskToPerform)
        {
            var idOfTaskToPerform = int.Parse(messageOfTaskToPerform);

            IQueryable<InsideModel.Models.Task> matchedTask = taskRepository.Where(t => t.Id == idOfTaskToPerform);
            if (matchedTask.Count() == 0)
            {
                return;
            }

            var taskToPerform = matchedTask.First();

            if (taskToPerform.Status == TaskStatus.Queued)
            {
                taskToPerform.UpdateDate = serverTime.Now;
                if (!IsTaskToTry(taskToPerform))
                {
                    taskToPerform.Status = TaskStatus.Failed;
                }
                else
                {
                    if (taskToPerform.EarliestExecution < serverTime.RequestStarted)
                    {
                        taskToPerform.NumTries += 1;
                        
                        try
                        {
                            if (!taskToPerform.StartDate.HasValue)
                            {
                                taskToPerform.StartDate = serverTime.Now;
                            }
                            PerformTask(taskToPerform);
                            taskToPerform.Status = TaskStatus.Completed;
                        }
                        catch (GoogleApiException googleException)
                        {
                            if (googleException.HttpStatusCode == HttpStatusCode.Forbidden)
                            {
                                taskQueueStorage.ReQueue(taskToPerform);
                            }
                        }
                        catch (Exception exceotion)
                        {
                            Console.WriteLine(exceotion.Message);
                            taskToPerform.Status = TaskStatus.Failed;
                        }

                        
                    }
                    else
                    {
                        taskQueueStorage.ReQueue(taskToPerform);
                    }
                }

               
                taskRepository.SaveChanges();
            }
            else if (taskToPerform.Status == TaskStatus.Completed || taskToPerform.Status == TaskStatus.Failed)
            {
                return;
            }
            return;
        }

        private bool IsTaskToTry(InsideModel.Models.Task taskToCheck)
        {
            if (taskToCheck.NumTries <= 0) return true;
            if (taskToCheck.Type == TaskType.AddProductAnalyticData)
            {
                if (taskToCheck.NumTries >= 3)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private void PerformTask(InsideModel.Models.Task taskToPerform)
        {
            if (taskToPerform.Type == TaskType.CreateTaskForUsersToSendNewContactEmailNotification)
            {
                _notificationEmailSender.PerformTask(taskToPerform);

            }
            else if (taskToPerform.Type == TaskType.SendNewContactPhoneNotification)
            {
                phoneNotificationTaskPerformer.PerformTask(taskToPerform);

            }
            else if (taskToPerform.Type == TaskType.AddProductAnalyticData)
            {
                addAnalyticProductDataForClient.PerformTask(taskToPerform);

            }
            else if (taskToPerform.Type == TaskType.CreateAnalyticDataTasksForClients)
            {
                createAnalyticDataTasksForClients.PerformTask(taskToPerform);

            }
            else if (taskToPerform.Type == TaskType.ImportCustomEvents)
            {
                customEventsImporter.PerformTask(taskToPerform);

            }
            else if (taskToPerform.Type == TaskType.ImportSeoData)
            {
                seoDataImporter.PerformTask(taskToPerform);
            }
            else if (taskToPerform.Type == TaskType.SendNewContactEmailNotificationToUser)
            {
                userEmailSender.PerformTask(taskToPerform);
            }
            else if (taskToPerform.Type == TaskType.ImportCustomEventsTaskCreator)
            {
                importCustomEventsTaskCreator.PerformTask(taskToPerform);
            }
            else if (taskToPerform.Type == TaskType.RemoveExpiredTokens)
            {
                removeExpiredTokens.PerformTask(taskToPerform);
            }
            else if (taskToPerform.Type == TaskType.CreateUpdatePhonecallsTasksForClients)
            {
                updatePhonecallsTaskCreator.PerformTask(taskToPerform);
            }
            else if (taskToPerform.Type == TaskType.UpdatePhonecalls)
            {
                updatePhonecalls.PerformTask(taskToPerform);
            }
            else if (taskToPerform.Type == TaskType.CreateTaskForUsersToSendNewContactSmsNotification)
            {
                smsNotificationTaskCreator.PerformTask(taskToPerform);
            }
            else if (taskToPerform.Type == TaskType.SendNewContactSmsNotificationToUser)
            {
                userSmsNotificationTaskPerformer.PerformTask(taskToPerform);
            }
            else
            {
                throw new Exception("Unknown task type: " + taskToPerform.Type);
            }

        }

        
    }
}
