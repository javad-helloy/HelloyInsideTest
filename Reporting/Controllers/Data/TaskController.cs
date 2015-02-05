using System.Net;
using System.Web.Mvc;
using System.Web.SessionState;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using InsideReporting.Helpers;
using Newtonsoft.Json;
using Task.ContactProduct.AnalyticDataTaskCreator;
using Task.TaskCreator;
using Task.UpdatePhonecalls.UpdatePhonecallsTaskCreator;

namespace InsideReporting.Controllers.Data
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class TaskController : Controller
    {
        private readonly IRepository<Client> clientRepository;
        private readonly ITaskQueueStorage taskQueueStorage;
        private readonly IRepository<InsideModel.Models.Task> taskRepository;
        private readonly IServerTime serverTime;
        private readonly ITaskManager taskManager;

        public TaskController(
            IRepository<Client> clientRepository,
            ITaskQueueStorage taskQueueStorage,
            IRepository<InsideModel.Models.Task> taskRepository,
            IServerTime serverTime,
            ITaskManager taskManager)
        {
            this.clientRepository = clientRepository;
            this.taskQueueStorage = taskQueueStorage;
            this.taskRepository = taskRepository;
            this.serverTime = serverTime;
            this.taskManager = taskManager;
        }

        [HttpPost]
        [BasicAuthorize]
        public ActionResult Create(string type)
        {

            if (type == "CreateAnalyticDataTasksForClients")
            {
                var endDate = serverTime.RequestStarted;
                var startDate = endDate.AddDays(-2);
                var createTaskForProductAnalyticTaskMessage = new TaskMessageWithStartDateAndEndDate()
                {
                    StartDate = startDate.Date,
                    EndDate = endDate.Date
                };
                var messageToAdd = JsonConvert.SerializeObject(createTaskForProductAnalyticTaskMessage);

                var dayOfExecutionLowerBound = serverTime.RequestStarted.Date;
                var dayOfExecutionUpperBound = dayOfExecutionLowerBound.AddDays(1);

                if (!taskManager.HasTaskInRepository(messageToAdd, TaskType.CreateAnalyticDataTasksForClients,
                    dayOfExecutionLowerBound, dayOfExecutionUpperBound))
                {
                    var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.CreateAnalyticDataTasksForClients, serverTime.RequestStarted);
                    taskQueueStorage.Add(taskToAdd);
                    return
                        Content("Successfully Created task: CreateAnalyticDataTasksForClients");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest,"Task Already in Database: " + messageToAdd);
                }
            }
            else if (type == "ImportCustomEvents")
            {
                var dayOfExecutionLowerBound = serverTime.RequestStarted.Date;
                var dayOfExecutionUpperBound = dayOfExecutionLowerBound.AddDays(1);
                var messageToAdd = "";

                if (!taskManager.HasTaskInRepository(messageToAdd, TaskType.ImportCustomEventsTaskCreator,
                    dayOfExecutionLowerBound, dayOfExecutionUpperBound))
                {
                    var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.ImportCustomEventsTaskCreator, serverTime.RequestStarted);
                    taskQueueStorage.Add(taskToAdd);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest,"Task Already in Database" );
                }
                return
                    Content("Successfully Created task: ImportCustomEventsTaskCreator");
            }
            else if (type == "ImportSeoData")
            {
                var dayOfExecutionLowerBound = serverTime.RequestStarted.Date;
                var dayOfExecutionUpperBound = dayOfExecutionLowerBound.AddDays(1);
                var messageToAdd = "";

                if (!taskManager.HasTaskInRepository(messageToAdd, TaskType.ImportSeoData,
                    dayOfExecutionLowerBound, dayOfExecutionUpperBound))
                {
                    var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.ImportSeoData, serverTime.RequestStarted);
                    taskQueueStorage.Add(taskToAdd);
                    
                    return
                        Content("Successfully Created task: ImportSeoData");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest,"Task Already in Database" );
                }
            }
           else if (type == "RemoveExpiredTokens")
            {
                var dayOfExecutionLowerBound = serverTime.RequestStarted.Date;
                var dayOfExecutionUpperBound = dayOfExecutionLowerBound.AddDays(1);
                var messageToAdd = "";

                if (!taskManager.HasTaskInRepository(messageToAdd, TaskType.RemoveExpiredTokens,
                    dayOfExecutionLowerBound, dayOfExecutionUpperBound))
                {
                    var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.RemoveExpiredTokens, serverTime.RequestStarted);
                    taskQueueStorage.Add(taskToAdd);
                    
                    return
                        Content("Successfully Created task: RemoveExpiredTokens");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Task Already in Database");
                }
            }
            else if (type == "UpdatePhoneCalls")
            {
                var endDate = serverTime.RequestStarted;
                var startDate = endDate.AddDays(-10);
                var createTaskForUpdatePhoneCallsTaskMessage = new UpdatePhonecallsTaskMesage()
                {
                    StartDate = startDate.Date,
                    EndDate = endDate.Date
                };
                var messageToAdd = JsonConvert.SerializeObject(createTaskForUpdatePhoneCallsTaskMessage);

                var dayOfExecutionLowerBound = serverTime.RequestStarted.Date;
                var dayOfExecutionUpperBound = dayOfExecutionLowerBound.AddDays(1);

                if (!taskManager.HasTaskInRepository(messageToAdd, TaskType.CreateUpdatePhonecallsTasksForClients,
                    dayOfExecutionLowerBound, dayOfExecutionUpperBound))
                {
                    var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.CreateUpdatePhonecallsTasksForClients, serverTime.RequestStarted);
                    taskQueueStorage.Add(taskToAdd);
                   
                    return
                        Content("Successfully Created task: CreateUpdatePhonecallsTasksForClients");
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Task Already in Database: " + messageToAdd);
                }
            }
            else
            {
                HttpContext.Response.StatusDescription = "BadRequest";
                HttpContext.Response.StatusCode = 400;
                return Content("No Task Created");
            }
        }
    }
}
