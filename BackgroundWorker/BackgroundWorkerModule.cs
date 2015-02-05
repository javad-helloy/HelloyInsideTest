using System;
using System.Diagnostics.CodeAnalysis;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using Inside;
using Inside.DownloadManager;
using Inside.Event;
using Inside.googleDocs;
using Inside.Lead;
using Inside.LeadService;
using Inside.LimeJuice;
using Inside.membership;
using Inside.PhoneCall;
using Inside.ReleaseNotes;
using Inside.Seo;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;
using Ninject.Syntax;
using Task.Email;
using Task.Email.Sender;
using Task.TaskPerformer.Delegate;
using Task.TaskPerformer.Storage;

namespace BackgroundWorker
{
    [ExcludeFromCodeCoverage]
    internal class BackgroundWorkerModule : NinjectModule
    {
        public override void Load()
        {
            // inside bindings
            Bind<IServerTime>().To<ServerTime>().InCallScope();
            Bind<IRepository<InsideModel.Models.Task>>().ToMethod(c => new Repository<InsideModel.Models.Task>(context => context.Task)).InCallScope();
            Bind<IRepository<Client>>().ToMethod(c => new Repository<Client>(context => context.Clients)).InCallScope();
            Bind<IRepository<Admin>>().ToMethod(c => new Repository<Admin>(context => context.Admin)).InCallScope();
            Bind<IRepository<InsideUser>>().ToMethod(c => new Repository<InsideUser>(context => context.InsideUser)).InCallScope();
            Bind<IRepository<InsideModel.Models.PhoneCall>>().ToMethod(c => new Repository<PhoneCall>(context => context.PhoneCall)).InCallScope();
            Bind<IRepository<PhoneStatistic>>().ToMethod(c => new Repository<PhoneStatistic>(context => context.PhoneStatistic)).InCallScope();
            Bind<IRepository<SerpRanking>>().ToMethod(c => new Repository<SerpRanking>(context => context.SerpRanking)).InCallScope();
            Bind<IRepository<Lead>>().ToMethod(c => new Repository<Lead>(context => context.Lead)).InCallScope();

            Bind<IMembershipProvider>().To<AspMembershipAdapter>().InCallScope();
            
            Bind<IDownloadManager>().To<DownloadManager>().InCallScope();
            Bind<IGoogleSpreadSheetService>().To<GoogleSpreadsheetService>().InCallScope();

            Bind<IEventService>().To<EventService>().InCallScope();
            Bind<ILimeJuiceData>().To<LimeJuiceData>().InCallScope();
            Bind<IJsonConverter>().To<JsonUtcConverter>().InCallScope();
            Bind<ISeoService>().To<SeoService>().InCallScope();
            Bind<IReleaseNotes>().To<ReleaseNotesFromSpreadSheet>().InCallScope();
            Bind<IPhoneCallService>().To<PhoneCallService>().InCallScope();
            Bind<ILeadService>().To<LeadService>().InCallScope();
            

            // Task bindings
            Bind<IEmailSender>().To<MandrilEmailSender>().InCallScope();
            Bind<ITaskDelegate>().To<TaskDelegate>().InTransientScope();
            Bind<IWeeklyReportEmailSender>().To<WeeklyReportEmailSender>().InCallScope();
            Bind<ITaskQueueStorage>().To<AzureQueueTaskQueue>().InCallScope();
            Bind<IWeeklyReportDefinitionBuilder>().To<WeeklyReportDefinitionBuilder>().InCallScope();

            // External services
            Bind<IService>().ToMethod(c =>
            {
                var ss = new SpreadsheetsService("Helloy inside");
                ss.Credentials = new GDataCredentials("helloy.inside@gmail.com", "GuT9ThUn");
                return ss;
            }).InCallScope();
        }
    }
}
