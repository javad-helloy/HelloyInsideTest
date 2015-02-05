namespace Task.SmsNotification.SmsNotificationSenderToUser
{
    public class UserSpecificSmsNotificationTaskMessage
    {
        public int ContactId { get; set; }
        public string UserId { get; set; }
    }
}
