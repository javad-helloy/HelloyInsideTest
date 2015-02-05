using System.Diagnostics.CodeAnalysis;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using Inside;
using Inside.AcceptToken;
using Inside.AutoRating;
using Inside.ContactProductService;
using Inside.ContactService;
using Inside.DownloadManager;
using Inside.ExternalData;
using Inside.GoogleService;
using Inside.HelloyIndex;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using RestSharp;
using Task.ContactProduct.AnalyticData;
using Task.ContactProduct.AnalyticDataTaskCreator;
using Task.Email.NotificationEmail;
using Task.Email.Sender;
using Task.ImportCustomEvents;
using Task.ImportSeoData;
using Task.PhoneNotification;
using Task.RemoveExpiredTokens;
using Task.SmsNotification.SmsNotificationSenderToUser;
using Task.SmsNotification.SmsNotificationUserTaskCreator;
using Task.TaskCreator;
using Task.TaskPerformer.Delegate;
using Task.TaskPerformer.Storage;
using Task.UpdatePhonecalls.UpdatePhoneCalls;
using Task.UpdatePhonecalls.UpdatePhonecallsTaskCreator;

namespace TaskExecutor
{
    [ExcludeFromCodeCoverage]
    internal class TaskExecutorModule : NinjectModule
    {
        public override void Load()
        {
            // inside bindings
            Bind<IServerTime>().To<ServerTime>().InCallScope();
            Bind<IRepository<InsideModel.Models.Task>>().ToMethod(c => new Repository<InsideModel.Models.Task>(context => context.Task)).InCallScope();
            Bind<IRepository<Client>>().ToMethod(c => new Repository<Client>(context => context.Clients)).InCallScope();
            Bind<IRepository<InsideUser>>().ToMethod(c => new Repository<InsideUser>(context => context.InsideUser)).InCallScope();
            Bind<IRepository<SerpRanking>>().ToMethod(c => new Repository<SerpRanking>(context => context.SerpRanking)).InCallScope();
            Bind<IRepository<Contact>>().ToMethod(c => new Repository<Contact>(context => context.Lead)).InCallScope();
            Bind<IRepository<Token>>().ToMethod(c => new Repository<Token>(context => context.Token)).InCallScope();
            Bind<IRepository<ExternalToken>>().ToMethod(c => new Repository<ExternalToken>(context => context.ExternalToken)).InCallScope();

            Bind<IDownloadManager>().To<DownloadManager>().InCallScope();
            
            Bind<IJsonConverter>().To<JsonUtcConverter>().InCallScope();
            Bind<IContactService>().To<ContactService>().InCallScope();
            Bind<IProductService>().To<ProductService>().InCallScope();
            Bind<IRestClient>().To<RestClient>().InCallScope();
            Bind<IGoogleAnalyticsApi>().To<GoogleAnalyticsApi>().InCallScope();
            Bind<IGoogleAuthentication>().To<Inside.GoogleService.GoogleAuthentication>().InCallScope();
            
            // Task bindings
            Bind<IEmailSender>().To<MandrilEmailSender>().InCallScope();
            Bind<ITaskDelegate>().To<TaskDelegate>().InTransientScope();
            Bind<ITaskQueueStorage>().To<AzureQueueTaskQueue>().InCallScope();
            Bind<INotificationEmailDefenitionBuilder>().To<NotificationEmailDefenitionBuilder>().InCallScope();
            Bind<INotificationEmailSender>().To<NotificationEmailSender>().InCallScope();
            Bind<IAccessTokenProvider>().To<AccessTokenProvider>().InCallScope();
            Bind<IPhoneNotificationTaskPerformer>().To<PhoneNotificationTaskPerformer>().InCallScope();
            Bind<IPhoneNotificationTextBuilder>().To<PhoneNotificationTextBuilder>().InCallScope();
            Bind<INotificationSender>().To<NotificationSender>().InCallScope();
            Bind<IAddAnalyticProductDataForClient>().To<AddAnalyticProductDataForClient>().InCallScope();
            Bind<IAnalyticDataPropertyExtractor>().To<AnalyticDataPropertyExtractor>().InCallScope();
            Bind<ICreateAnalyticDataTasksForClients>().To<CreateAnalyticDataTasksForClients>().InCallScope();
            Bind<ICustomEventsImporter>().To<CustomEventsImporter>().InCallScope();
            Bind<ICustomEventsExtractor>().To<CustomEventsExtractor>().InCallScope();
            Bind<ISeoDataImporter>().To<SeoDataImporter>().InCallScope();
            Bind<ISeoDataImportMapper>().To<SeoDataImportMapper>().InCallScope();
            Bind<IUserNotificationEmailSender>().To<UserNotificationEmailSender>().InCallScope();
            Bind<ITaskManager>().To<TaskManager>().InCallScope();
            Bind<IImportCustomEventsTaskCreator>().To<ImportCustomEventsTaskCreator>().InCallScope();
            Bind<IRemoveExpiredTokens>().To<RemoveExpiredTokens>().InCallScope();
            Bind<IUpdatePhonecalls>().To<UpdatePhonecalls>().InCallScope();
            Bind<ICreateUpdatePhonecallsTasksForClients>().To<CreateUpdatePhonecallsTasksForClients>().InCallScope();
            Bind<IExternalDataProvider>().To<ExternalDataProvider>().InCallScope();
            Bind<ISmsNotificationTextBuilder>().To<SmsNotificationTextBuilder>().InCallScope();
            Bind<ISmsSender>().To<SmsSender>().InCallScope();
            Bind<IUserSmsNotificationTaskPerformer>().To<UserSmsNotificationTaskPerformer>().InCallScope();
            Bind<ISmsNotificationTaskPerformer>().To<SmsNotificationTaskPerformer>().InCallScope();
            Bind<IContactAutoRating>().To<ContactAutoRating>().InCallScope();
            Bind<IContactIndexCalculator>().To<ContactIndexCalculator>().InCallScope();
            // External services
            Bind<IService>().ToMethod(c =>
            {
                var ss = new SpreadsheetsService("Helloy inside");
                ss.Credentials = new GDataCredentials("helloy.inside@gmail.com", "GuT9ThUn");
                return ss;
            }).InCallScope();

            Bind<IGoogleUrlShortnerService>().To<GoogleUrlShortnerService>().InCallScope();
        }
    }
}
