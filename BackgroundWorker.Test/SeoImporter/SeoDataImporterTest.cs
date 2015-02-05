using System;
using System.Linq;
using Helpers.test;
using Inside.DownloadManager;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.ImportSeoData;

namespace BackgroundWorker.Test.SeoImporter
{
    /// <summary>
    /// Summary description for SeoDataImporterTest
    /// </summary>
    [TestClass]
    public class SeoDataImporterTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var downloadManager= new Mock<IDownloadManager>();
            var seoDataMapper = new Mock<ISeoDataImportMapper>();
            var serpRepository = new Mock<IRepository<SerpRanking>>();
            var task = new SeoDataImporter(downloadManager.Object,serpRepository.Object,seoDataMapper.Object);
        }

        [TestMethod]
        public void CanPerform()
        {
            var downloadManager = new Mock<IDownloadManager>();
            var seoDataMapper = new Mock<ISeoDataImportMapper>();
            var serpRepository = new Mock<IRepository<SerpRanking>>();

            var task = new SeoDataImporter(downloadManager.Object, serpRepository.Object, seoDataMapper.Object);

            var result = task.CanPerformTask("ImportSeoData");
            Assert.IsTrue(result);

            var resultFalse = task.CanPerformTask("This is not a Task");
            Assert.IsFalse(resultFalse); 
        }

        [TestMethod]
        public void PerformTask()
        {
            var downloadManager = new Mock<IDownloadManager>();
            var seoDataMapper = new Mock<ISeoDataImportMapper>();
            var serpRepository = new LocalRepository<SerpRanking>();

            var serpRankingInDb = new SerpRanking
            {
                Date = DateTime.Now.AddDays(-1),
                Keyword = "Keyword",
                Url = "url.com"
            };
            
            serpRepository.Add(serpRankingInDb);
            
            var seoPosion = "[{\"pr\":\"0\",\"url\":\"url.se\",\"kw\":\"key word\",\"region\":\"google.se\",\"language\":\"sv\",\"start\":\"0\"}]";
            downloadManager.Setup(dm => dm.FetchUrl(It.IsAny<string>())).Returns(seoPosion);
            
            var taskMessage = "";

            var task = new SeoDataImporter(downloadManager.Object, serpRepository, seoDataMapper.Object);

            task.PerformTask(new InsideModel.Models.Task() { Message = taskMessage });

            seoDataMapper.Verify(di=>di.Map(It.IsAny<SearchEngineRank>(),It.IsAny<SerpRanking>()),Times.Once);
            
            Assert.AreEqual(2, serpRepository.All().Count());



        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ExistingDateForSeoDataInDbNotCreateNewDataThrowException()
        {
            var downloadManager = new Mock<IDownloadManager>();
            var seoDataMapper = new Mock<ISeoDataImportMapper>();
            var serpRepository = new LocalRepository<SerpRanking>();

            var serpRankingInDb = new SerpRanking
            {
                Date = DateTime.Now,
                Keyword = "Keyword",
                Url = "url.se"
            };

            serpRepository.Add(serpRankingInDb);
            
            var seoPosion = "[{\"pr\":\"3\",\"url\":\"url.se\",\"kw\":\"key word\",\"region\":\"google.se\",\"language\":\"sv\",\"start\":\"0\"}]";
            downloadManager.Setup(dm => dm.FetchUrl(It.IsAny<string>())).Returns(seoPosion);

            var taskMessage = "";

            var task = new SeoDataImporter(downloadManager.Object, serpRepository, seoDataMapper.Object);

            task.PerformTask(new InsideModel.Models.Task() { Message = taskMessage });

            seoDataMapper.Verify(di => di.Map(It.IsAny<SearchEngineRank>(), It.IsAny<SerpRanking>()), Times.Never);
            Assert.AreEqual(1, serpRepository.All().Count());
        }

        [TestMethod]
        public void CanAddSeoDataForDatesBetweenExistingDatesInRepository()
        {
            var downloadManager = new Mock<IDownloadManager>();
            var seoDataMapper = new SeoDataImportMapper();
            var serpRepository = new LocalRepository<SerpRanking>();

            var serpRankingInDb = new SerpRanking
            {
                Date = DateTime.Now.AddDays(-1),
                Keyword = "Keyword",
                Url = "url.com"
            };

            var serpRankingInDb2 = new SerpRanking
            {
                Date = DateTime.Now.AddDays(1),
                Keyword = "Keyword",
                Url = "url.com"
            };

            serpRepository.Add(serpRankingInDb);
            serpRepository.Add(serpRankingInDb2);

            var seoPosion = "[{\"pr\":\"0\",\"url\":\"url.se\",\"kw\":\"key word\",\"region\":\"google.se\",\"language\":\"sv\",\"start\":\"0\"}]";
            downloadManager.Setup(dm => dm.FetchUrl(It.IsAny<string>())).Returns(seoPosion);

            var taskMessage = "";

            var task = new SeoDataImporter(downloadManager.Object, serpRepository, seoDataMapper);

            task.PerformTask(new InsideModel.Models.Task() { Message = taskMessage });

            Assert.AreEqual(3, serpRepository.All().Count());

            var importedSerpRankings = serpRepository.All().Last();

            Assert.AreEqual("url.se", importedSerpRankings.Url);
            Assert.AreEqual("key word", importedSerpRankings.Keyword);
            Assert.AreEqual(0, importedSerpRankings.PageRank);
        }
    }
}
