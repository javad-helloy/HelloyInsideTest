namespace Task.PhoneNotification
{
    public interface IPhoneNotificationTextBuilder
    {
        string GetDefinition(int contactId);
    }
}