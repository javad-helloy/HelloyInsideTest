using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inside.AcceptToken;
using Inside.Analytics;
using InsideModel.Models;
using InsideModel.repositories;
using Ninject;
using Ninject.Extensions.NamedScope;
using Ninject.Modules;

namespace DeleteExpiredTokens
{
    class Program
    {
        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel();

            kernel.Load(new BackgroundModule());

            var wroker = kernel.Get<AccessTokenProvider>();
            try
            {
                wroker.RemoveExpired();
            }
            catch (Exception e)
            {
                throw e;
            }
            Console.WriteLine("Finished Cleaning");
        }


    }
    internal class BackgroundModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAccessTokenProvider>().To<AccessTokenProvider>().InCallScope();
            Bind<IRepository<Token>>().ToMethod(c => new Repository<Token>(context => context.Token)).InCallScope();
            Bind<IRepository<InsideUser>>().ToMethod(c => new Repository<InsideUser>(context => context.InsideUser)).InCallScope();
        }
    }
}
