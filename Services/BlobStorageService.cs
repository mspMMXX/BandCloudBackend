using Azure.Identity;
using Azure.Storage.Blobs;

namespace BandCloudBackend.Services
{
    /// <summary>
    /// Kapselt den Zugriff auf Azure Blob Storage für Listen, Upload, Download und Streaming.
    /// Authentifiziert über <see cref="DefaultAzureCredential"/> und liest Account/Container aus <see cref="IConfiguration"/>.
    /// </summary>
    public class BlobStorageService
    {
        //Verbindung zum Azure Blob Storage
        private readonly BlobServiceClient _blobServiceClient;
        //Speichert in welchem Container gearbeitet wird
        private readonly string _containerName;

        /// <summary>
        /// Erstellt den Service und initialisiert den <see cref="BlobServiceClient"/>.
        /// Erwartet in der Konfiguration: <c>STORAGE_ACCOUNT_NAME</c> und <c>STORAGE_CONTAINER</c>.
        /// </summary>
        /// <param name="config">Konfiguration (z. B. appsettings, App Settings in Azure).</param>
        /// <exception cref="InvalidOperationException">
        /// Wenn <c>STORAGE_ACCOUNT_NAME</c> oder <c>STORAGE_CONTAINER</c> fehlen.
        /// </exception>
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

        /// <summary>
        /// Listet alle Blob-Namen im konfigurierten Container.
        /// </summary>
        /// <returns>Liste der Blob-Namen.</returns>
        public async Task<List<string>> ListFilesAsync()
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            var result = new List<string>();

            await foreach (var blob in container.GetBlobsAsync())
                result.Add(blob.Name);

            return result;
        }

        /// <summary>
        /// Lädt eine Datei in den Container hoch (überschreibt vorhandene Datei gleichen Namens).
        /// </summary>
        /// <param name="fileName">Ziel-Dateiname (Blob-Name).</param>
        /// <param name="content">Quelldaten als Stream.</param>
        public async Task UploadAsync(string fileName, Stream content)
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blob = container.GetBlobClient(fileName);

            await blob.UploadAsync(content, overwrite: true);
        }

        /// <summary>
        /// Lädt eine Datei aus dem Container herunter.
        /// </summary>
        /// <param name="fileName">Dateiname (Blob-Name).</param>
        /// <returns>
        /// Datenstream der Datei oder <c>null</c>, falls der Blob nicht existiert.
        /// </returns>
        public async Task<Stream> DownloadAsync(string fileName)
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blob = container.GetBlobClient(fileName);

            if (await blob.ExistsAsync())
            {
                var response = await blob.DownloadAsync();
                return response.Value.Content;
            }
            return null;
        }

        /// <summary>
        /// Öffnet einen Read-Stream für eine Datei und liefert den Content-Type anhand der Dateiendung.
        /// </summary>
        /// <param name="fileName">Dateiname (Blob-Name).</param>
        /// <returns>
        /// Tupel aus Stream und Content-Type oder <c>null</c>, wenn der Blob nicht existiert.
        /// </returns>
        public async Task<(Stream Stream, string ContentType)?> GetFileStreamAsync(string fileName)
        {
            var container = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blob = container.GetBlobClient(fileName);

            if (!await blob.ExistsAsync())
                return null;

            // Blob-Stream öffnen
            var stream = await blob.OpenReadAsync();

            // ContentType anhand Dateiendung ermitteln
            var contentType = "application/octet-stream";
            if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                contentType = "audio/mpeg";
            else if (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                     fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                contentType = "image/jpeg";
            else if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                contentType = "image/png";
            else if (fileName.EndsWith(".HEIC", StringComparison.OrdinalIgnoreCase) ||
                     fileName.EndsWith(".heic", StringComparison.OrdinalIgnoreCase))
                contentType = "image/heic";

            return (stream, contentType);
        }

    }
}
