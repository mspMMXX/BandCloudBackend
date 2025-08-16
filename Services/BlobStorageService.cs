using Azure.Identity;
using Azure.Storage.Blobs;

namespace BandCloudBackend.Services
{
    public class BlobStorageService
    {
        //Verbindung zum Azure Blob Storage
        private readonly BlobServiceClient _blobServiceClient;
        //Speichert in welchem Container gearbeitet wird
        private readonly string _containerName;

        //Aus IConfiguration (appsettings.json) die notwendigen Daten lesen
        public BlobStorageService(IConfiguration config)
        {
            // Namen & Container werden aus der IConfiguration gelesen
            var accountName = config["STORAGE_ACCOUNT_NAME"]
                              ?? throw new InvalidOperationException("STORAGE_ACCOUNT_NAME missing");
            _containerName = config["STORAGE_CONTAINER"]
                              ?? throw new InvalidOperationException("STORAGE_CONTAINER missing");

            // Service-URI bauen (Standard-Domain für Azure Blob)
            var serviceUri = new Uri($"https://{accountName}.blob.core.windows.net");

            // Meldet sich mit ueber die Service-URI und dem DefaultAzureCredential (Login aus AZ Login, Lokal) an ohne Benutzername/Passwort.
            _blobServiceClient = new BlobServiceClient(serviceUri, new DefaultAzureCredential());
        }

        // Gibt eine Liste aller Blob-Namen aus dem Container zurück
        public async Task<List<string>> ListFilesAsync()
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            var result = new List<string>();

            await foreach (var blob in container.GetBlobsAsync())
                result.Add(blob.Name);

            return result;
        }

        // Datei hochladen
        public async Task UploadAsync(string fileName, Stream content)
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blob = container.GetBlobClient(fileName);

            await blob.UploadAsync(content, overwrite: true);
        }
    }
}
