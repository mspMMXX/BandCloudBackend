using BandCloudBackend.Data;
using BandCloudBackend.Models;
using BandCloudBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace BandCloudBackend.Controllers
{
    [ApiController]
    [Route("files")]
    public class RecordingsController : ControllerBase
    {
        private readonly BlobStorageService _blob;

        public RecordingsController(BlobStorageService blob) => _blob = blob;

        [HttpGet] // GET /files
        public async Task<IActionResult> ListAsync()
        {
            var names = await _blob.ListFilesAsync();
            return Ok(names);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] FileUploadDto dto, [FromServices] AppDbContext db)
        {
            var file = dto.File;

            if (file == null || file.Length == 0)
                return BadRequest("Keine Datei hochgeladen.");

            using var stream = file.OpenReadStream();
            await _blob.UploadAsync(file.FileName, stream);

            var metadata = new RecordingMetadata
            {
                FileName = file.FileName,
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            db.Recordings.Add(metadata);
            await db.SaveChangesAsync();

            return Ok(new { message = $"Datei '{file.FileName}' hochgeladen und Metadaten gespeichert." });
        }



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
