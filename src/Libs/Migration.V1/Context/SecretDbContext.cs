using Microsoft.EntityFrameworkCore;
using Migration.V1.Models;

namespace Migration.V1.Context;

internal sealed class SecretDbContext : DbContext
{
    private readonly string _dbPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretDbContext"/> class.
    /// </summary>
    public SecretDbContext(string dbPath) => _dbPath = dbPath;

    /// <summary>
    /// 元数据列表.
    /// </summary>
    public DbSet<Metadata> Metadata { get; set; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
}
