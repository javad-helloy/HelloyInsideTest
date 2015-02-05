namespace Task.Email.NotificationEmail
{
    public interface INotificationEmailDefenitionBuilder
    {
        NotificationEmailDefintion GetDefinition(int contactId, string userId);
    }

    public class NotificationEmailDefintion
    {
        public string ConsultantName { get; set; }
        public string ConsultantEmail { get; set; }
        public string ConsultantPhone { get; set; }
        public string ConsultantImage { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public string Type { get; set; }
        
        public string From { get; set; }
        public string Date { get; set; }
        public string InsideUrl { get; set; }
        public string SubjectOrDuration { get; set; }
    }

    public class ContactDetails
    {
        public string From { get; set; }
        public string Date { get; set; }
        public string InsideUrl { get; set; }
        public string SubjectOrDuration { get; set; }
    }

}
