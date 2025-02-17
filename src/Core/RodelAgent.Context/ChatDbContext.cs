// Copyright (c) Richasy. All rights reserved.

using Microsoft.EntityFrameworkCore;
using RodelAgent.Models.Common;

namespace RodelAgent.Context;

/// <summary>
/// 对话数据库上下文.
/// </summary>
public sealed class ChatDbContext : DbContext
{
    private readonly string _dbPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatDbContext"/> class.
    /// </summary>
    public ChatDbContext() => _dbPath = "Assets/chat.db";

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatDbContext"/> class.
    /// </summary>
    public ChatDbContext(string dbPath) => _dbPath = dbPath;

    /// <summary>
    /// 会话列表.
    /// </summary>
    public DbSet<Metadata> Sessions { get; set; }

    /// <summary>
    /// 群组会话列表.
    /// </summary>
    public DbSet<Metadata> Groups { get; set; }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
}
