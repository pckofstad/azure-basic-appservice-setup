using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlazorApp.Shared;
using Microsoft.Extensions.Configuration;

namespace BlazorApp.Server.Infrastructure
{
    public class AzureStorageService
    {
        private const string AZURE_STORAGE_CONNECTION_STRING = "AzureWebJobsStorage";
        private const string _containerName = "blazorapp";
        private readonly string _azureStorageConnectionString;


        public AzureStorageService(IConfiguration configuration)
        {
            _azureStorageConnectionString = configuration.GetSection(AZURE_STORAGE_CONNECTION_STRING)?.Value
                ?? throw new Exception("Azure storage account connection string missing");
        }

        public AzureStorageService(string connectionString)
        {
            _azureStorageConnectionString = connectionString;
        }

        public async Task StoreFileAsync(Stream storeStream, string fileStorageName)
        {
            var container = await GetContainerAsync(_containerName);
            var fileBlob = container.GetBlobClient(fileStorageName);
            storeStream.Position = 0;
            await fileBlob.UploadAsync(storeStream);
        }

        public async Task<Stream> GetFileAsync(string fileStorageName, bool throwNotFound = true)
        {
            var container = await GetContainerAsync(_containerName);
            var blobClient = container.GetBlobClient(fileStorageName);

            if (!await blobClient.ExistsAsync())
            {
                if (throwNotFound)
                {
                    throw new FileNotFoundException();
                }
                return null;
            }

            BlobDownloadInfo download = await blobClient.DownloadAsync();

            var downloadFileStream = new MemoryStream();
            await download.Content.CopyToAsync(downloadFileStream);
            downloadFileStream.Position = 0;
            return downloadFileStream;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var container = await GetContainerAsync(_containerName);
            var fileBlob = container.GetBlobClient(fileName);

            return await fileBlob.DeleteIfExistsAsync();
        }

        public async Task<BlobContainerClient> GetContainerAsync(string containerName)
        {
            containerName = string.IsNullOrEmpty(containerName) ? _containerName : containerName;
            var container = new BlobContainerClient(_azureStorageConnectionString, containerName);
            return container;
        }

        public async Task<List<FileDto>> GetFileListAsync()
        {
            var container = await GetContainerAsync(_containerName);
            var fileList = new List<FileDto>();
            await foreach (BlobItem blob in container.GetBlobsAsync())
            {
                fileList.Add(new FileDto(blob.Name, blob.Properties.CreatedOn!.Value));
            }

            return fileList;
        }

        public void EnsureStorageContainers()
        {
            var masterFilesContainerClient = new BlobContainerClient(_azureStorageConnectionString, _containerName);
            masterFilesContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        }
    }
}
