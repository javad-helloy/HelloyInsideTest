using System;

namespace Inside.ContactProductService
{
    public class ProductService : IProductService
    {
        public bool IsSearch(string campaign, string medium)
        {

            if ((medium.ToLower() == "cpc") && !campaign.ToLower().Contains("display") &&
                !campaign.ToLower().Contains("remarketing") && !campaign.ToLower().Contains("retargeting"))
            {
                return true;
            }
            return false;
        }

        public bool IsOrganic( string medium)
        {

            if ((medium.ToLower() == "organic"))
            {
                return true;
            }
            return false;
        }

        public bool IsRetargeting(string campaign, string medium)
        {
            if (medium.ToLower() == "retargeting" || medium.ToLower() == "remarketing" ||
                campaign.ToLower().Contains("retargeting") || campaign.ToLower().Contains("remarketing"))
            {
                return true;
            }
            return false;
        }

       
        public bool IsDisplay(string campaign, string medium)
        {

            if (medium.ToLower() == "display" ||
                (medium.ToLower() == "cpc" && campaign.ToLower().Contains("display") &&
                 !campaign.ToLower().Contains("retargeting") && !campaign.ToLower().Contains("remarketing")))
            {
                return true;
            }

            return false;
        }

        public bool IsValidProduct(string contactCampaign, string contactMedium)
        {
            return IsDisplay(contactCampaign, contactMedium) || IsOrganic(contactMedium) ||
                   IsRetargeting(contactCampaign, contactMedium) || IsSearch(contactCampaign, contactMedium);
        }

        public string GetProduct(string contactCampaign, string contactMedium)
        {
            if (IsRetargeting(contactCampaign, contactMedium))
            {
                return "Retargeting";
            }
            else if (IsDisplay(contactCampaign, contactMedium))
            {
                return "Display";
            }
            else if (IsSearch(contactCampaign, contactMedium))
            {
                return "Search";
            }
            else if (IsOrganic(contactMedium))
            {
                return "Organic";
            }
            else
            {
                throw new ArgumentException("No product matching for provided campaign and medium.");    
            }
            
        }

    }
}
