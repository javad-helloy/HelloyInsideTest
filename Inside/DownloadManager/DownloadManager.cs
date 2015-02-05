using System;
using System.Net;
using System.Threading;

namespace Inside.DownloadManager
{
    public class DownloadManager : IDownloadManager
    {
        public string FetchUrl(string url)
        {
            return FetchUrl(url, 3, 5000);
        }

        public string FetchUrl(string url, int numRetries, int retryTimeout)
        {
            WebClient webClient = new WebClient();

            do
            {
                try
                {
                    var result = "";
                    webClient.Encoding = System.Text.Encoding.UTF8;
                    result += webClient.DownloadString(url);
                    return result; }
                catch
                {
                    if (numRetries <= 0) throw;
                    else Thread.Sleep(retryTimeout);
                }
            } 
            while (numRetries-- > 0);

            throw new Exception("Reached unreachable point - should not arrive here.");
        }
    }
}