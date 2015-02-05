using InsideModel.Models;

namespace Inside.AcceptToken
{
    public interface IAccessTokenProvider
    {
        Token GetToken(string userId);
        string GenerateAccessUrl(string userId, string returnUrl);
        void RemoveExpired();
        bool Validate(int clientId, string accessToken);

    }
}
