using BandCloudBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BandCloudBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<RecordingMetadata> Recordings { get; set; }
    }
}
