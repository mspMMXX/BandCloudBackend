using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BandCloudBackend.Models
{
    /// <summary>
    /// Data Transfer Object (DTO) für Datei-Uploads über die API.
    /// Repräsentiert ein hochgeladenes File und einen optionalen Kommentar.
    /// </summary>
    public class FileUploadDto
    {
        /// <summary>
        /// Die hochgeladene Datei.
        /// </summary>
        [FromForm(Name = "file")]
        public IFormFile File { get; set; } = null!;

        /// <summary>
        /// Benutzerkommentar zur Datei (optional).
        /// </summary>
        [FromForm(Name = "comment")]
        public string Comment { get; set; } = string.Empty;
    }
}
