using BandCloudBackend.Data;
using BandCloudBackend.Models;
using BandCloudBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BandCloudBackend.Controllers
{
    /// <summary>
    /// API-Controller für Upload, Auflistung und Abruf von Dateien (Recordings).
    /// </summary>
    [ApiController]
    [Route("files")]
    public class RecordingsController : ControllerBase
    {
        private readonly BlobStorageService _blob;

        /// <summary>
        /// Erstellt den Controller mit BlobStorageService.
        /// </summary>
        /// <param name="blob">BlobStorageService zum Speichern/Abrufen von Dateien.</param>
        public RecordingsController(BlobStorageService blob) => _blob = blob;

        /// <summary>
        /// Liefert alle gespeicherten Metadaten von Aufnahmen.
        /// </summary>
        /// <param name="db">AppDbContext (per Dependency Injection).</param>
        /// <returns>Liste aller Aufnahmen aus der Datenbank.</returns>
        [HttpGet] // GET /files
        public async Task<ActionResult<IEnumerable<RecordingMetadata>>> ListAsync([FromServices] AppDbContext db)
        {
            var list = await db.Recordings.ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Lädt eine Datei hoch und speichert dazugehörige Metadaten.
        /// </summary>
        /// <param name="dto">Upload-DTO (Datei und Kommentar).</param>
        /// <param name="db">AppDbContext (per Dependency Injection).</param>
        /// <returns>Die gespeicherten Metadaten.</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<RecordingMetadata>> Upload([FromForm] FileUploadDto dto, [FromServices] AppDbContext db)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Keine Datei hochgeladen.");

            using var stream = dto.File.OpenReadStream();
            await _blob.UploadAsync(dto.File.FileName, stream);

            var metadata = new RecordingMetadata
            {
                FileName = dto.File.FileName,
                Comment = dto.Comment ?? string.Empty,
                FileSize = dto.File.Length,
                UploadedAt = DateTime.UtcNow
            };

            db.Recordings.Add(metadata);
            await db.SaveChangesAsync();

            return Ok(metadata);
        }

        /// <summary>
        /// Lädt eine Datei herunter.
        /// </summary>
        /// <param name="fileName">Name der Datei.</param>
        /// <returns>Dateistream als Download oder 404, wenn nicht gefunden.</returns>
        [HttpGet("{fileName}")] // GET /files/{fileName}
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _blob.DownloadAsync(fileName);

            if (stream == null)
            {
                return NotFound($"Datei '{fileName}' nicht gefunden.");
            }

            return File(stream, "application/octet-stream", fileName);
        }

        /// <summary>
        /// Streamt eine Datei direkt zurück (mit passendem Content-Type).
        /// </summary>
        /// <param name="fileName">Name der Datei.</param>
        /// <returns>Stream mit Content-Type oder 404, wenn nicht gefunden.</returns>
        [HttpGet("{fileName}/stream")]
        public async Task<IActionResult> StreamFile(string fileName)
        {
            var file = await _blob.GetFileStreamAsync(fileName);

            if (file == null)
                return NotFound();

            return File(file.Value.Stream, file.Value.ContentType);
        }
    }
}
