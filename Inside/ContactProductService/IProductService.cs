namespace Inside.ContactProductService
{
    public interface IProductService
    {
        bool IsSearch(string campaign, string medium);
        bool IsDisplay(string campaign, string medium);
        bool IsRetargeting(string campaign, string medium);
        bool IsOrganic(string medium);

        string GetProduct(string campaign, string medium);
        bool IsValidProduct(string contactCampaign, string contactMedium);
    }
}
