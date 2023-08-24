using Azure.Storage.Blobs;

namespace AlohaMaui.Api
{
    public interface IBlobServiceClientProvider
    {
        BlobServiceClient GetBlobClient();
    }

    public class BlobServiceClientProvider : IBlobServiceClientProvider
    {
        private readonly string _connectionString;

        public BlobServiceClientProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BlobServiceClient GetBlobClient()
        {
            return new BlobServiceClient(_connectionString);
        }
    }
}
