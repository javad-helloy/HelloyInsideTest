using System;
using Inside.AcceptToken;
using InsideModel.Models;

namespace Task.RemoveExpiredTokens
{
    public class RemoveExpiredTokens : IRemoveExpiredTokens
    {
        private readonly IAccessTokenProvider accessTokenProvider;

        public RemoveExpiredTokens(IAccessTokenProvider accessTokenProvider)
        {
            this.accessTokenProvider = accessTokenProvider;
        }

        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.RemoveExpiredTokens;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            accessTokenProvider.RemoveExpired();
            Console.WriteLine("Finished Cleaning");
        }
    }
}
