using BandCloudBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace BandCloudBackend.Data
{
    /// <summary>
    /// Entity Framework Core DbContext für die BandCloud-Anwendung.
    /// Verwaltet die Datenbankverbindung und enthält DbSets für Entitäten.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Erstellt einen neuen DbContext mit den angegebenen Optionen.
        /// </summary>
        /// <param name="options">Konfigurationsoptionen für den DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Tabelle für Metadaten von Aufnahmen.
        /// </summary>
        public DbSet<RecordingMetadata> Recordings { get; set; }
    }
}
