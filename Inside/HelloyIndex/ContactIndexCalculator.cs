using System.Collections.Generic;
using System.Linq;

namespace Inside.HelloyIndex
{
    public class ContactIndexCalculator:IContactIndexCalculator
    {
        private const decimal positivUserRaitingfactor = 60;
        private const decimal autoRaitingfactor = 15;
        private const decimal visitorFactor = 1;

        public void SetIndexValues(IEnumerable<ContactCollection> contactBins)
        {
            // setting the unscaled values
            foreach (var contactCollection in contactBins)
            {
                var indexValue = 
                    positivUserRaitingfactor*GetPositivUserRaitingValue(contactCollection) +
                    autoRaitingfactor*GetAutoRaitingValues(contactCollection) +
                    visitorFactor*contactCollection.Visitors;

                contactCollection.IndexValue = contactCollection.Cost==0m? 0m: indexValue/contactCollection.Cost;
            }

            // Scalling the values to intervall
            decimal maxValue = contactBins.Max(cb => cb.IndexValue);
            decimal minValue = contactBins.Min(cb => cb.IndexValue);

            decimal targetMax = 1;
            decimal targetMin = 0.2m - 1/(contactBins.Count()+1);

            foreach (var contactCollection in contactBins)
            {
                var isAtUpperLimit = contactCollection.IndexValue >= maxValue;
                var isAtLowerLimit = contactCollection.IndexValue <= minValue;

                if (isAtUpperLimit)
                {
                    contactCollection.IndexValue = targetMax;
                }
                else if (isAtLowerLimit)
                {
                    contactCollection.IndexValue = targetMin;
                }
                else
                {
                    var indexValue = contactCollection.IndexValue;
                    var scaledIndexValue = (indexValue - minValue) / (maxValue - minValue) * (targetMax - targetMin);
                    contactCollection.IndexValue = scaledIndexValue;
                }
            }
        }

        private decimal GetAutoRaitingValues(ContactCollection contactCollection)
        {
            var starRatings = 0m;

            foreach (var contact in contactCollection.Contacts)
            {
                var hasAutoRaiting = contact.AutoRatingScore.HasValue;

                if (hasAutoRaiting)
                {
                    var raitingValue = contact.AutoRatingScore;

                    // 1 has a neutral contribution
                    if (raitingValue == 2)
                    {
                        starRatings += 1;
                    }
                    else if (raitingValue == 3)
                    {
                        starRatings += 3;
                    }
                    else if (raitingValue == 4)
                    {
                        starRatings += 9;
                    }
                    else if (raitingValue == 5)
                    {
                        starRatings += 27;
                    }
                }
                else // fix for missing auto ratings on first release
                {
                    starRatings += 3;
                }
            }

            return starRatings;
        }

        private decimal GetPositivUserRaitingValue(ContactCollection contactCollection)
        {
            var starRatings = 0m;

            foreach (var contact in contactCollection.Contacts)
            {
                var hasRaiting = contact.RatingScore.HasValue;
                if (hasRaiting)
                {
                    var raitingValue = contact.RatingScore;
                    
                    // 1 has a neutral contribution
                    if (raitingValue == 3)
                    {
                        starRatings += 3;
                    }
                    else if (raitingValue == 4)
                    {
                        starRatings += 9;
                    }
                    else if (raitingValue == 5)
                    {
                        starRatings += 27;
                    }
                }
            }

            return starRatings;
        }
    }
}
