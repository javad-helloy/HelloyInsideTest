using InsideModel.Models;
using InsideModel.repositories;
using Newtonsoft.Json;

namespace Task.SmsNotification.SmsNotificationSenderToUser
{
    public class UserSmsNotificationTaskPerformer : IUserSmsNotificationTaskPerformer
    {
        private readonly ISmsSender smsSender;
        private readonly ISmsNotificationTextBuilder smsDefinitionBuilder;
        private readonly IRepository<InsideUser> userRepository;

        public UserSmsNotificationTaskPerformer(
            ISmsSender smsSender,
            ISmsNotificationTextBuilder smsDefinitionBuilder,
            IRepository<InsideUser> userRepository)
        {
            this.smsSender = smsSender;
            this.smsDefinitionBuilder = smsDefinitionBuilder;
            this.userRepository = userRepository;
        }

        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.SendNewContactSmsNotificationToUser;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var sendSmsToUserTask = JsonConvert.DeserializeObject<UserSpecificSmsNotificationTaskMessage>(taskMessage.Message);
            var user = userRepository.Single(u => u.Id == sendSmsToUserTask.UserId);
            var defintion = smsDefinitionBuilder.GetDefinition(sendSmsToUserTask.ContactId, sendSmsToUserTask.UserId);
            if (defintion != null)
            {
                smsSender.SendSms(user.Phone, defintion);
            }
        }
    }
}
