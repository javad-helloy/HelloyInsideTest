using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Helpers.test;
using Inside.DownloadManager;
using Inside.Seo;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.Data;
using InsideReporting.Service.Seo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace InsideReporting.Tests.Controllers.Data
{
    [TestClass]
    public class SeoControllerTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var serpRankingRepository = new Mock<ISerpRankingRepository>();
            var seoService = new Mock<ISeoService>();
            var downloadManager =  new Mock<IDownloadManager>();
            var seoDataMapper =new Mock<ISeoDataImportMapper>();

            var seoController = new SeoController(clientRepository.Object, serpRankingRepository.Object,
                                                  seoService.Object, downloadManager.Object, seoDataMapper.Object);
        }

        [TestMethod]
        public void CanGetPositionsForClientInRepositoryWithOkDomain()
        {
            var clientRepository = new LocalRepository<Client>(); ;
            var serpRankingRepository = new Mock<ISerpRankingRepository>();
            var seoService = new Mock<ISeoService>();
            var downloadManager = new Mock<IDownloadManager>();
            var seoDataMapper = new Mock<ISeoDataImportMapper>();

            var client = ModelHelper.TestClient1AllDataNoReferences;
            clientRepository.Add(client);
            var endDate = new DateTime(2013, 09, 01);
            serpRankingRepository.Setup(sr => sr.GetRankings(client.Domain, endDate))
                                 .Returns(new List<SerpRanking>() {new SerpRanking() {Url = client.Domain}});

            var seoController = new SeoController(clientRepository, serpRankingRepository.Object,
                                                  seoService.Object, downloadManager.Object, seoDataMapper.Object);

           var result = seoController.GetPositions(client.Id, endDate) as ContentResult;
            var resultObject = JsonConvert.DeserializeObject<List<SerpRanking>>(result.Content);
            
            Assert.IsTrue(resultObject.Any(ro=>ro.Url==client.Domain));
        }

        [TestMethod]
        public void CanGetPositionsWithHistoryForClientInRepositoryWithOkDomain()
        {
            var clientRepository = new LocalRepository<Client>(); ;
            var serpRankingRepository = new Mock<ISerpRankingRepository>();
            var seoService = new Mock<ISeoService>();
            var downloadManager = new Mock<IDownloadManager>();
            var seoDataMapper = new Mock<ISeoDataImportMapper>();

            var client = ModelHelper.TestClient1AllDataNoReferences;
            clientRepository.Add(client);
            var startDate = new DateTime(2013, 06, 01);
            var endDate = new DateTime(2013, 09, 01);
            serpRankingRepository.Setup(sr => sr.GetRankingsWithHistory(client.Domain, startDate, endDate))
                                 .Returns(new List<SerpRankingWithHistory>()
                                     {
                                         new SerpRankingWithHistory() {RankingUrl = client.Domain}
                                     });

            var seoController = new SeoController(clientRepository, serpRankingRepository.Object,
                                                  seoService.Object, downloadManager.Object, seoDataMapper.Object);

            var result = seoController.GetPositionsWithHistory(client.Id, startDate, endDate) as ContentResult;
            var resultObject = JsonConvert.DeserializeObject<List<SerpRankingWithHistory>>(result.Content);

            Assert.IsTrue(resultObject.Any(ro => ro.RankingUrl == client.Domain));
        }

    }
}
