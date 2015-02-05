using System.Linq;
using InsideModel.Models;

namespace Inside.AutoRating
{
    public class ContactAutoRating: IContactAutoRating
    {
        public void SetAutoRating(Contact contact)
        {
            switch (contact.LeadType)
            {
                case "Phone":
                    contact.AutoRatingScore= GetPhoneAutoRating(contact);
                    break;
                case "Email":
                    contact.AutoRatingScore= GetEmailAutoRating(contact);
                    break;
                case "Chat":
                    contact.AutoRatingScore=  GetChatAutoRating(contact);
                    break;
                case "Event":
                    contact.AutoRatingScore= GetEventAutoRating(contact);
                    break;
            }

            
        }

        private int GetPhoneAutoRating(Contact contact)
        {
            int autoRating = 2;
            if (contact.HasProperty("Status") && contact.GetProperty("Status").Value != "completed")
            {
                autoRating = 1;
            }
            
            if (contact.HasProperty("Duration"))
            {
                var duration = int.Parse(contact.Property.Single(p => p.Type == "Duration").Value);

                if (duration >= 0 && duration < 30)
                {
                    autoRating = 1;
                }
                else if (duration >= 30 && duration < 90)
                {
                    autoRating = 2;
                }
                else if (duration >= 90 && duration < 180)
                {
                    autoRating = 3;
                }
                else if (duration >= 180 && duration < 300)
                {
                    autoRating = 4;
                }
                else if (duration >= 300)
                {
                    autoRating = 5;
                }
            }
            

            return autoRating;
        }

        private int GetEmailAutoRating(Contact contact)
        {
            return 3;
        }

        private int GetChatAutoRating(Contact contact)
        {
            return 5;
        }

        private int GetEventAutoRating(Contact contact)
        {
            return 3;
        }

    }
}
