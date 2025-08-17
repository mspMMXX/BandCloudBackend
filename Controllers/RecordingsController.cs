using BandCloudBackend.Data;
using BandCloudBackend.Models;
using BandCloudBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BandCloudBackend.Controllers
{
    [ApiController]
    [Route("files")]
    public class RecordingsController : ControllerBase
    {
        private readonly BlobStorageService _blob;

        public RecordingsController(BlobStorageService blob) => _blob = blob;

        [HttpGet] // GET /files
        public async Task<ActionResult<IEnumerable<RecordingMetadata>>> ListAsync([FromServices] AppDbContext db)
        {
            var list = await db.Recordings.ToListAsync();
            return Ok(list);
        }

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
