// Copyright (c) Rodel. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// 音频数据库上下文.
/// </summary>
public sealed class AudioDbContext : DbContext
{
    private readonly string _dbPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioDbContext"/> class.
    /// </summary>
    public AudioDbContext() => _dbPath = "Assets/audio.db";

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioDbContext"/> class.
    /// </summary>
    public AudioDbContext(string dbPath) => _dbPath = dbPath;

    /// <summary>
    /// 会话列表.
    /// </summary>
    public DbSet<Metadata> Sessions { get; set; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
}
