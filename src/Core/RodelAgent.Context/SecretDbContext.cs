// Copyright (c) Richasy. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// 密钥数据库上下文.
/// </summary>
public sealed class SecretDbContext : DbContext
{
    private readonly string _dbPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecretDbContext"/> class.
    /// </summary>
    public SecretDbContext() => _dbPath = "Assets/secret.db";

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
