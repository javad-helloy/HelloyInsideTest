namespace Task.SmsNotification.SmsNotificationSenderToUser
{
    public interface ISmsNotificationTextBuilder
    {
        string GetDefinition(int contactId, string userId);
    }
}