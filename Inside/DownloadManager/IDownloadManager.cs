namespace Inside.DownloadManager
{
    public interface IDownloadManager
    {
        string FetchUrl(string url);
        string FetchUrl(string url, int numRetries, int retryTimeout);
    }
}
