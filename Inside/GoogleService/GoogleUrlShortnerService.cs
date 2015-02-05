using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace Inside.GoogleService
{
    public class GoogleUrlShortnerService : IGoogleUrlShortnerService
    {
        private readonly IJsonConverter jsonCoverter;
        private readonly string googleApiKey;

        public GoogleUrlShortnerService(IJsonConverter jsonCoverter)
        {
            this.jsonCoverter = jsonCoverter;
            googleApiKey = ConfigurationManager.AppSettings["GoogleApiKey"];
        }

        public string GetShortUrl(string longUrl)
        {
            WebRequest request = WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key="+googleApiKey);
            request.Method = "POST";
            request.ContentType = "application/json";
            var jsonData = jsonCoverter.Serilize(new GoogleUrlShortnerMessage { longUrl = longUrl });
            string requestData = jsonData;
            byte[] requestRawData = Encoding.ASCII.GetBytes(requestData);
            request.ContentLength = requestRawData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestRawData, 0, requestRawData.Length);
            requestStream.Close();

            WebResponse response = request.GetResponse();
            StreamReader responseReader = new StreamReader(response.GetResponseStream());
            string responseData = responseReader.ReadToEnd();
            responseReader.Close();

            var shortnerResult = jsonCoverter.Deserilize<GoogleUrlShortnerResponse>(responseData);
            return shortnerResult.id;
        }
    }
    internal class GoogleUrlShortnerResponse
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string longUrl { get; set; }
    }
    internal class GoogleUrlShortnerMessage
    {
        public string longUrl { get; set; }
    }
}
