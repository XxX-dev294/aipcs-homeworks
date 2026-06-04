using Microsoft.EntityFrameworkCore;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Набор авиакомпаний
    /// </summary>
    public DbSet<Airline> Airlines { get; set; }

    /// <summary>
    /// Набор рейсов
    /// </summary>
    public DbSet<Flight> Flights { get; set; }

    /// <summary>
    /// Настройка подключения к SQLite
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=airlines.db");
}
