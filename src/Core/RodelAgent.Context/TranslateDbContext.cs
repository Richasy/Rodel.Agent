// Copyright (c) Rodel. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// 翻译数据库上下文.
/// </summary>
public sealed class TranslateDbContext : DbContext
{
    private readonly string _dbPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateDbContext"/> class.
    /// </summary>
    public TranslateDbContext() => _dbPath = "Assets/trans.db";

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateDbContext"/> class.
    /// </summary>
    public TranslateDbContext(string dbPath) => _dbPath = dbPath;

    /// <summary>
    /// 会话列表.
    /// </summary>
    public DbSet<Metadata> Sessions { get; set; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
}
