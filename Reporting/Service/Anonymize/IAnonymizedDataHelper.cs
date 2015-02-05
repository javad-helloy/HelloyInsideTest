using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsideReporting.Service.Anonymize
{
    public interface IAnonymizedDataHelper
    {
        bool NextRandomBool(double chance = 0.5);
        string NextRandomName();
        DateTime NextRandomDateInIntevall(DateTime startDate, DateTime endDate);
        string NextRandomPhoneNumber();
        
        string NextRandomSearchPhrase();
        string NextRandomSearchSource();
        string NextRandomChatDescription();
        
        TimeSpan NextRandomTimeSpan(int maxSeconds);
        string NextEmailAdress();
        string NextEmailContent();
    }
}
