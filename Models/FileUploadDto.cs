using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BandCloudBackend.Models
{
    public class FileUploadDto
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; } = null!;

        [FromForm(Name = "comment")]
        public string Comment { get; set; } = string.Empty;
    }
}
