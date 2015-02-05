namespace Task.SmsNotification.SmsNotificationSenderToUser
{
    public interface ISmsSender
    {
        string SendSms(string phoneNumber, string message);
    }
}
