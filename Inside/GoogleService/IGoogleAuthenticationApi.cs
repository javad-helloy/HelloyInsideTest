using InsideModel.Models;

namespace Inside.GoogleService
{
    public interface IGoogleAuthentication
    {


        ExternalToken GetAccessToken(bool forceToGetNew = false);
        ExternalToken GetRefreshToken(string authCode, string redirectUri);
        string GetAuthorizationCodeUrl(string redirectUri);
    }

    public class GoogleApiTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
    }
}
