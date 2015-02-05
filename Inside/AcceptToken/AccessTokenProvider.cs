using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using InsideModel.Models;
using InsideModel.repositories;

namespace Inside.AcceptToken
{
    public class AccessTokenProvider:IAccessTokenProvider
    {
        private IRepository<Token> tokenRepository;
        private IRepository<InsideUser> userRepository;
        public AccessTokenProvider(IRepository<Token> tokenRepository,
                                   IRepository<InsideUser> userRepository)
        {
            this.tokenRepository = tokenRepository;
            this.userRepository = userRepository;
        }
        public Token GetToken(string userId)
        {
            var isUserValid = userRepository.Where(u => u.Id == userId).Any();
            if (!isUserValid)
            {
                return null;
            }
            var userIdString = userId;
            var today = DateTime.Now;
            var acceptToken = GenerateToken();
            var newToken = new Token
            {
                UserId = userIdString,
                AccessToken = acceptToken,
                ExpirationDate = today.AddDays(30)
            };

            tokenRepository.Add(newToken);
            tokenRepository.SaveChanges();
            return newToken;
        }
        
        public void RemoveExpired()
        {
            var today = DateTime.Now;
            foreach (var expiredToken in tokenRepository.Where(t=>t.ExpirationDate<today).ToList())
            {
               tokenRepository.Delete(expiredToken); 
            }
            tokenRepository.SaveChanges();
        }

        public bool Validate(int clientId, string accessToken)
        {
            var today = DateTime.Now;
            var tokenToValidate = tokenRepository.Where(t => t.AccessToken == accessToken && t.InsideUser.ClientId == clientId).SingleOrDefault();
            return tokenToValidate != null && tokenToValidate.ExpirationDate >= today;
        }

        public string GenerateAccessUrl(string userId, string returnUrl)
        {
            var token = GetToken(userId);
            var generatedUrl = "http://inside.helloy.se/Account/AuthenticateToken?token=" + token.AccessToken + "&returnUrl=" + HttpUtility.UrlEncode(returnUrl);
            return generatedUrl;

        }

        /// length:
        /// 128-bit	28	23	22	20
        /// 256-bit	55	45	43  40
        private string GenerateToken()
        {
            var length = 43;
            var complexity = 3;
            var csp = new RNGCryptoServiceProvider();
            // Define the possible character classes where complexity defines the number
            // of classes to include in the final output.
            char[][] classes =
                {
                @"abcdefghijklmnopqrstuvwxyz".ToCharArray(),
                @"ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
                @"0123456789".ToCharArray()
                //@" !""#$%&'()*+,./:;<>?@[\]^_{|}~".ToCharArray(),---> uncomment for special chars
                };

            complexity = Math.Max(1, Math.Min(classes.Length, complexity));
            if (length < complexity)
                throw new ArgumentOutOfRangeException("length");

            var allchars = classes.Take(complexity).SelectMany(c => c).ToArray();
            var bytes = new byte[allchars.Length];
            csp.GetBytes(bytes);
            for (int i = 0; i < allchars.Length; i++)
            {
                char tmp = allchars[i];
                allchars[i] = allchars[bytes[i] % allchars.Length];
                allchars[bytes[i] % allchars.Length] = tmp;
            }

            // Create the random values to select the characters
            Array.Resize(ref bytes, length);
            var result = new char[length];

            while (true)
            {
                csp.GetBytes(bytes);
                // Obtain the character of the class for each random byte
                for (int i = 0; i < length; i++)
                    result[i] = allchars[bytes[i] % allchars.Length];

                // Verify that it does not start or end with whitespace
                if (Char.IsWhiteSpace(result[0]) || Char.IsWhiteSpace(result[(length - 1) % length]))
                    continue;

                var testResult = new string(result);
                // Verify that all character classes are represented
                if (0 != classes.Take(complexity).Count(c => testResult.IndexOfAny(c) < 0))
                    continue;

                return testResult;
            }
        }

    }
}
