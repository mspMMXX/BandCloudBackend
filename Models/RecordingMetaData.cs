namespace BandCloudBackend.Models
{
    /// <summary>
    /// Repräsentiert die Metadaten einer hochgeladenen Aufnahme.
    /// Wird in der Datenbank über Entity Framework Core gespeichert.
    /// </summary>
    public class RecordingMetadata
    {
        /// <summary>
        /// Primärschlüssel der Aufnahme (Datenbank-ID).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dateiname des gespeicherten Blobs.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Benutzerkommentar zur Aufnahme.
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Dateigröße in Bytes.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Zeitpunkt des Uploads (UTC).
        /// </summary>
        public DateTime UploadedAt { get; set; }
    }
}
