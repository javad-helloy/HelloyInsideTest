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
using Inside.membership;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.Identity;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using InsideReporting.Service.Anonymize;
using InsideReporting.Service.DemoWrapper;
using InsideReporting.WebRepositories.DemoWrapper;
using Microsoft.AspNet.Identity;
using Ninject.Modules;
using RestSharp;
using Ninject.Web.Common;
using Task.Email.Sender;
using Task.TaskCreator;
using Task.TaskPerformer.Storage;
using GoogleAuthentication = Inside.GoogleService.GoogleAuthentication;

namespace InsideReporting
{
    public class ReportingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<InsideContext>().ToMethod(c => new InsideContext()).InRequestScope();

            Bind<IRestClient>().ToMethod(c => new RestClient()).InRequestScope();

            // security

            Bind<IAnonymizedDataHelper>().To<AnonymizedDataHelper>().InRequestScope();
            
            // nassa model access
            Bind<IRepository<InsideUser>>().ToMethod(c => new Repository<InsideUser>(context => context.InsideUser))
                .InRequestScope();

            // Anonymised repositories
            Bind<IRepository<Client>>().To<AnonymisedClientRepository>().InRequestScope();
            Bind<ISerpRankingRepository>().To<AnonymisedSerpRepository>().InRequestScope();
            Bind<IRepository<SerpRanking>>().ToMethod(c => new Repository<SerpRanking>(context => context.SerpRanking)).InRequestScope();

           /* Bind<IRepository<Admin>>().ToMethod(c => new Repository<Admin>(context => context.Admin)).InRequestScope();*/
            Bind<IRepository<InsideModel.Models.Task>>().ToMethod(c => new Repository<InsideModel.Models.Task>(context => context.Task)).InRequestScope();
            Bind<IRepository<Contact>>().To<AnonymisedContactRepositorySirius>().InRequestScope();
            Bind<IRepository<ContactInteraction>>().ToMethod(c => new Repository<ContactInteraction>(context => context.LeadInteraction)).InRequestScope();
            Bind<IRepository<ContactProperty>>().ToMethod(c => new Repository<ContactProperty>(context => context.LeadProperty)).InRequestScope();
            Bind<IRepository<Budget>>().ToMethod(c => new Repository<Budget>(context => context.Budget)).InRequestScope();
            Bind<IRepository<Label>>().ToMethod(c => new Repository<Label>(context => context.Label)).InRequestScope();
            Bind<IRepository<Token>>().ToMethod(c => new Repository<Token>(context => context.Token)).InRequestScope();
            Bind<IRepository<Tag>>().ToMethod(c => new Repository<Tag>(context => context.Tag)).InRequestScope();
            Bind<IRepository<ExternalToken>>().ToMethod(c => new Repository<ExternalToken>(context => context.ExternalToken)).InRequestScope();
            Bind<ITaskManager>().To<TaskManager>().InRequestScope();

            Bind<IRepository<InsideRole>>().ToMethod(c => new Repository<InsideRole>(context => context.Role)).InRequestScope();
            Bind<IRepository<UserLogins>>().ToMethod(c => new Repository<UserLogins>(context => context.UserLogins)).InRequestScope();
            Bind<IUserStore<InsideUser>>().To<InsideModel.Models.Identity.UserStore<InsideUser>>().InRequestScope();
            Bind<IRoleStore<InsideRole>>().To<InsideModel.Models.Identity.RoleStore<InsideRole>>().InRequestScope();
            Bind<InsideUserManager>().To<InsideUserManager>().InRequestScope();
            Bind<IIdentityMembershipProvider>().To<AspIdentityMembership>().InRequestScope();
            

            // External data
            Bind<IExternalDataProvider>().To<ExternalDataProvider>().InRequestScope();

            // Data Services
            Bind<IDownloadManager>().To<DownloadManager>().InRequestScope();
            Bind<IServerTime>().To<ServerTime>().InRequestScope();
            Bind<IGoogleAuthentication>().To<GoogleAuthentication>().InRequestScope();
            Bind<IGoogleAnalyticsApi>().To<GoogleAnalyticsApi>().InRequestScope();
            Bind<IContactService>().To<ContactService>().InRequestScope();
            Bind<IJsonConverter>().To<JsonUtcConverter>().InRequestScope();
            Bind<IAccessTokenProvider>().To<AccessTokenProvider>().InRequestScope();
            Bind<IProductService>().To<ProductService>().InRequestScope();
            Bind<IEmailSender>().To<MandrilEmailSender>();
            Bind<IContactAutoRating>().To<ContactAutoRating>().InRequestScope();
            Bind<IContactIndexCalculator>().To<ContactIndexCalculator>().InRequestScope();

            // task binding
            Bind<ITaskQueueStorage>().To<AzureQueueTaskQueue>().InRequestScope();

            Bind<IService>().ToMethod(c =>
            {
                var ss = new SpreadsheetsService("Helloy inside");
                ss.Credentials = new GDataCredentials("helloy.inside@gmail.com", "GuT9ThUn");
                return ss;
            }).InRequestScope();

        }
    }
}