using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using RestSharp;

namespace Inside.GoogleService
{
    public class GoogleAuthentication : IGoogleAuthentication
    {
        private readonly IRestClient restClient;
        private readonly IRepository<ExternalToken> externalTokenRepository;
        private readonly IServerTime serverTime;
        private readonly IJsonConverter jsonConverter;
        public string ClientId;
        public string ClientSecret;
        
        
        public GoogleAuthentication(IRestClient restClient, IRepository<ExternalToken> externalTokenRepository, IServerTime serverTime,
            IJsonConverter jsonConverter)
        {
            this.restClient = restClient;
            this.externalTokenRepository = externalTokenRepository;
            this.serverTime = serverTime;
            this.jsonConverter = jsonConverter;
            ClientId = ConfigurationManager.AppSettings["GoogleClientId"]; 
            ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"]; 
            
        }

       
        public ExternalToken GetRefreshToken(string authCode, string redirectUri)
        {
            var tokenFromGoogle = jsonConverter.Deserilize<GoogleApiTokenResponse>(this.GetRestRefreshToken(authCode, redirectUri));
            if (!string.IsNullOrEmpty(tokenFromGoogle.refresh_token))
            {
                var refreshToken = new ExternalToken
                {
                    AccessToken = tokenFromGoogle.refresh_token,
                    ExpirationDate = serverTime.RequestStarted.AddYears(100),
                    Type = "Google_Refresh_Token"
                };
                externalTokenRepository.Add(refreshToken);
            }

            var accessToken = new ExternalToken
            {
                AccessToken = tokenFromGoogle.access_token,
                Type = "Google_Access_Token",
                ExpirationDate = serverTime.RequestStarted.AddSeconds(tokenFromGoogle.expires_in)
            };

            externalTokenRepository.Add(accessToken);
            externalTokenRepository.SaveChanges();

            return externalTokenRepository.First(et => et.Type == "Google_Refresh_Token");
        }

        public ExternalToken GetAccessToken(bool forceToGetNew=false)
        {
            ExternalToken authToken = null;
            var requestTimePlusThreeMins = serverTime.RequestStarted.AddMinutes(3);
            
            var hasValidTokenInDb = externalTokenRepository.Any(t => t.Type == "Google_Access_Token" && t.ExpirationDate > requestTimePlusThreeMins);
            if (hasValidTokenInDb && !forceToGetNew)
            {
                authToken =
                    externalTokenRepository.First(
                        t => t.Type == "Google_Access_Token" && t.ExpirationDate > requestTimePlusThreeMins);
            }
            else
            {
                var refreshToken = externalTokenRepository.First(et => et.Type == "Google_Refresh_Token").AccessToken;
                if (string.IsNullOrEmpty(refreshToken))
                {
                    throw new Exception("Refresh Token Not Available");
                }
                var tokenFromGoogle = jsonConverter.Deserilize<GoogleApiTokenResponse>(this.GetRestAccessToken(refreshToken));
                

                var hasTokenInDb = externalTokenRepository.Any(t => t.Type == "Google_Access_Token" && t.AccessToken == tokenFromGoogle.access_token);

                if (!hasTokenInDb)
                {
                    authToken = new ExternalToken
                    {
                        AccessToken = tokenFromGoogle.access_token,
                        ExpirationDate = serverTime.RequestStarted.AddSeconds(tokenFromGoogle.expires_in),
                        Type = "Google_Access_Token"
                    };
                    externalTokenRepository.Add(authToken);
                    externalTokenRepository.SaveChanges();
                }
                else
                {
                    authToken = externalTokenRepository.First(t => t.Type == "Google_Access_Token" && t.AccessToken == tokenFromGoogle.access_token);
                }
            }

            return authToken;
        }

        public string GetAuthorizationCodeUrl(string redirectUri)
        {
            var url = "https://accounts.google.com/o/oauth2/auth";
            url += "?scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fanalytics.readonly&";
            url += "redirect_uri=" +HttpUtility.UrlEncode(redirectUri) + "&";
            url += "response_type=code&";
            url += "client_id="+ClientId+"&";
            url += "access_type=offline";
            return url;
        }

        private string GetRestAccessToken(string refreshToken)
        {
            this.restClient.BaseUrl = new Uri("https://accounts.google.com");
            var request = new RestRequest("o/oauth2/token", Method.POST);

            request.AddParameter("client_id", this.ClientId);
            request.AddParameter("client_secret", this.ClientSecret);
            request.AddParameter("refresh_token", refreshToken);
            request.AddParameter("grant_type", "refresh_token");
            var response = restClient.Execute(request);
            return response.Content;
        }

        private string GetRestRefreshToken(string authCode, string redirectUri)
        {

            this.restClient.BaseUrl = new Uri("https://accounts.google.com");

            var request = new RestRequest("o/oauth2/token", Method.POST);

            request.AddParameter("code", authCode);
            request.AddParameter("client_id", this.ClientId);
            request.AddParameter("client_secret", this.ClientSecret);
            request.AddParameter("redirect_uri", redirectUri);
            request.AddParameter("grant_type", "authorization_code");
            var response = restClient.Execute(request);
            return response.Content;
        }

    }
}
