using Microsoft.AspNetCore.Mvc;
using BandCloudBackend.Services;

namespace BandCloudBackend.Controllers
{
    // Attribute um den Controller als API-Controller zu kennzeichnen
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
    }
}
