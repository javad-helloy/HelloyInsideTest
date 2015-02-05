namespace Inside.ContactService
{
    public interface IContactService
    {
        void NotifyClientsForNewContactWithEmail(int contactId);
        void NotifyClientsForNewContactWithPhoneNotification(int contactId);
        void NotifyClientsForNewContactWithSmsNotification(int contactId);

    }
}
