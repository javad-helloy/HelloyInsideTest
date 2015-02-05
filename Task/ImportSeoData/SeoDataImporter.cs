using System;
using System.Collections.Generic;
using System.Linq;
using Inside.DownloadManager;
using InsideModel.Models;
using InsideModel.repositories;
using Newtonsoft.Json;

namespace Task.ImportSeoData
{
   public class SeoDataImporter : ISeoDataImporter
    {
        private readonly IDownloadManager downloadManager;
        private readonly IRepository<SerpRanking> serpRankingRepository;
        private readonly ISeoDataImportMapper seoDataMapper;


        public SeoDataImporter(IDownloadManager downloadManager,
                                       IRepository<SerpRanking> serpRankingRepository, 
                                       ISeoDataImportMapper seoDataMapper)
        {
            this.downloadManager = downloadManager;
            this.seoDataMapper = seoDataMapper;
            this.serpRankingRepository = serpRankingRepository;
        }
        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.ImportSeoData;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var importDate = DateTime.UtcNow;
            var beginingOfImportDate = importDate.Date;
            var cutOffDate = beginingOfImportDate.AddDays(1);
            Console.WriteLine("Begin import seo data");

            var seoJson =
                downloadManager.FetchUrl(
                    "http://serpbook.com/serp/api/?viewkey=63xc118&auth=aba2dfa3bdea8475e07d35a24a2b4bc6");

            seoJson = seoJson.Replace("\"n\\/a\"", "\"null\"");

            var seoPositions = JsonConvert.DeserializeObject(seoJson, typeof(IEnumerable<SearchEngineRank>)) as IEnumerable<SearchEngineRank>;

            if (!serpRankingRepository.Any(s => s.Date >= beginingOfImportDate && s.Date < cutOffDate))
            {

                foreach (var searchPosition in seoPositions)
                {
                    var serpRanking = serpRankingRepository.Create();
                    serpRanking.Date = importDate;
                    seoDataMapper.Map(searchPosition, serpRanking);
                    serpRankingRepository.Add(serpRanking);
                    
                }

                serpRankingRepository.SaveChanges();
                Console.WriteLine("Completed import seo data -  Added " + seoPositions.Count() + " keywords",
                    "Information");
                
            }
            else
            {
                throw new Exception("Seo Data for " + importDate.ToString("yyyy-MM-dd") + " already exists");
                
            }
        }

        
    }
}
