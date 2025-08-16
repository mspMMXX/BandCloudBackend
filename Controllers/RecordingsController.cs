using Microsoft.AspNetCore.Mvc;
using BandCloudBackend.Services;
using BandCloudBackend.Models;

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

        [HttpPost] // POST /files
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] FileUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Keine Datei hochgeladen.");

            using var stream = dto.File.OpenReadStream();
            await _blob.UploadAsync(dto.File.FileName, stream);

            return Ok(new { message = $"Datei '{dto.File.FileName}' hochgeladen." });
        }
    }
}
