namespace Task.PhoneNotification
{
    public interface INotificationSender
    {
        string SendNotification(int clientId, string content);
    }
}
