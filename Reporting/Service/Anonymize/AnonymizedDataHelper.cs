using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsideReporting.Service.Anonymize
{
    public class AnonymizedDataHelper : IAnonymizedDataHelper
    {
        private Queue<string> names;
        private Random randomGenerator;
        private Queue<string> phoneNumbers;
        private Queue<string> searchPhrases;
        private Queue<string> searchSource;
        private Queue<string> emailAddresses;
        private Queue<string> emailMessages;
        private Queue<string> chatDescriptions;

        public AnonymizedDataHelper()
        {
            randomGenerator = new Random();

            names = new Queue<string>();

            names.Enqueue("Alice Andersson");
            names.Enqueue("Maja Eriksson");
            names.Enqueue("Elias Gustafsson");
            names.Enqueue("Elsa Johansson");
            names.Enqueue("Julia Karlsson");
            names.Enqueue("William Larsson");
            names.Enqueue("Ella Nilsson");
            names.Enqueue("Oscar Olsson");
            names.Enqueue("Lucas Persson");
            names.Enqueue("Hugo Svensson");

            phoneNumbers = new Queue<string>();
            phoneNumbers.Enqueue("+46881266181");
            phoneNumbers.Enqueue("+46850137733");
            phoneNumbers.Enqueue("+46863741003");
            phoneNumbers.Enqueue("+46859248104");
            phoneNumbers.Enqueue("+463098804256");
            phoneNumbers.Enqueue("+464161970688");
            phoneNumbers.Enqueue("+46835455279");
            phoneNumbers.Enqueue("+463036214669");
            phoneNumbers.Enqueue("+46895970895");
            phoneNumbers.Enqueue("+46895851625");
            phoneNumbers.Enqueue("+463085821760");
            phoneNumbers.Enqueue("+464182724695");
            phoneNumbers.Enqueue("+46850314861");
            phoneNumbers.Enqueue("+46868978358");
            phoneNumbers.Enqueue("+46899501492");
            phoneNumbers.Enqueue("+46851261170");
            phoneNumbers.Enqueue("+46855951243");
            phoneNumbers.Enqueue("+463058057005");
            phoneNumbers.Enqueue("+46823573240");
            phoneNumbers.Enqueue("+46812368088");

            searchPhrases = new Queue<string>();
            
            searchPhrases.Enqueue("bra produkt");
            searchPhrases.Enqueue("företaget");
            searchPhrases.Enqueue("billig produkt");
            searchPhrases.Enqueue("köpa produkt");
            searchPhrases.Enqueue("företaget");
            searchPhrases.Enqueue("bra produkt");
            searchPhrases.Enqueue("köpa produkten");
            searchPhrases.Enqueue("företaget");

            searchSource = new Queue<string>();
            searchSource.Enqueue("Google Adwords");
            searchSource.Enqueue("Google Sök");
            searchSource.Enqueue("Google Adwords");
            searchSource.Enqueue("Google Adwords");
            searchSource.Enqueue("Google Sök");
            
            
            emailAddresses = new Queue<string>();
            
            emailAddresses.Enqueue("alice.andersson@epost.se");
            emailAddresses.Enqueue("maja.eriksson@epost.se");
            emailAddresses.Enqueue("elias.gustafsson@epost.se");
            emailAddresses.Enqueue("elsa.johansson@epost.se");
            emailAddresses.Enqueue("julia.karlsson@epost.se");
            emailAddresses.Enqueue("william.larsson@epost.se");
            emailAddresses.Enqueue("ella.nilsson@epost.se");
            emailAddresses.Enqueue("oscar.olsson@epost.se");
            emailAddresses.Enqueue("lucas.persson@epost.se");
            emailAddresses.Enqueue("hugo.svensson@epost.se");

            emailMessages = new Queue<string>();

            emailMessages.Enqueue("Hej, Jag undrar om ni har produkten inne. Hör gärna av er på 08123456.");
            emailMessages.Enqueue("Hej, Jag undrar om det går att boka tid. Hör gärna av er på 08123456.");
            emailMessages.Enqueue("Hej, Finns det möjlighet att komma in nästa måndag för att kolla på produkten. Hör gärna av er på 08123456.");


            chatDescriptions = new Queue<string>();

            chatDescriptions.Enqueue("Kunden undrar om produkten. Söker svar på leverans tid.");
            chatDescriptions.Enqueue("Kunden undrar om produkten. Söker svar på kompabilitet med andra produkten.");
            chatDescriptions.Enqueue("Kunden undrar om leverans tid.");
        }

        public bool NextRandomBool(double chance = 0.5)
        {
            return randomGenerator.NextDouble() < chance;
        }

        public string NextRandomName()
        {
            return GetNextFromQueueAndRequeue(names);
        }

        public DateTime NextRandomDateInIntevall(DateTime startDate, DateTime endDate)
        {
            int range = (int)(endDate - startDate).TotalSeconds;

            DateTime nextRandomDateInIntevall;
            do
            {
                nextRandomDateInIntevall = startDate.AddSeconds(randomGenerator.Next(range));

            } while (nextRandomDateInIntevall.Hour < 8 || nextRandomDateInIntevall.Hour > 20);

            return nextRandomDateInIntevall;
        }

        public string NextRandomPhoneNumber()
        {
            var areaNumbers = new List<string>() { "+468", "+468", "+4670", "+4670", "+468", "+468", "+4670"};

            var firstPart = areaNumbers[randomGenerator.Next(0, areaNumbers.Count - 1)];
            var secondPart = randomGenerator.Next(1000000, 9999999);

            return firstPart + secondPart.ToString();
        }

        public string NextRandomSearchPhrase()
        {
            return GetNextFromQueueAndRequeue(searchPhrases);
        }

        public string NextRandomChatDescription()
        {
            return GetNextFromQueueAndRequeue(chatDescriptions);
        }

        public string NextRandomSearchSource()
        {
            return GetNextFromQueueAndRequeue(searchSource);
        }

        private T GetNextFromQueueAndRequeue<T>(Queue<T> queue)
        {
            var value = queue.Dequeue();
            queue.Enqueue(value);

            return value;
        }

        public TimeSpan NextRandomTimeSpan(int maxSeconds)
        {
            var seconds = randomGenerator.Next(maxSeconds);
            return new TimeSpan(0, 0, 0, seconds);
        }

        public string NextEmailAdress()
        {
            return GetNextFromQueueAndRequeue(emailAddresses);
        }

        public string NextEmailContent()
        {
            return GetNextFromQueueAndRequeue(emailMessages);
        }
    }
}