// Copyright (c) Rodel. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Migration.V1.Models;

namespace Migration.V1.Context;

/// <summary>
/// 绘图数据库上下文.
/// </summary>
internal sealed class DrawDbContext : DbContext
{
    private readonly string _dbPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawDbContext"/> class.
    /// </summary>
    public DrawDbContext(string dbPath) => _dbPath = dbPath;

    /// <summary>
    /// 图片列表.
    /// </summary>
    public DbSet<AiImage> Images { get; set; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
}
