
using Mandrill;

namespace Task.Email.Sender
{
    public interface IEmailSender
    {
        void Send(EmailMessage emailMessage);
    }
}
