// Copyright (c) Richasy. All rights reserved.

using Microsoft.EntityFrameworkCore;

namespace RodelAgent.Context;

/// <summary>
/// 数据库迁移工具.
/// </summary>
public static class MigrationUtils
{
    private static string _rootPath;

    /// <summary>
    /// 设置根目录.
    /// </summary>
    /// <param name="rootPath">应用安装目录.</param>
    public static void SetRootPath(string rootPath) => _rootPath = rootPath;

    /// <summary>
    /// 获取聊天数据库.
    /// </summary>
    /// <param name="workDir">工作目录.</param>
    /// <returns><see cref="ChatDbContext"/>.</returns>
    public static async Task<ChatDbContext> GetChatDbAsync(string workDir)
    {
        await CheckDatabaseExistInternalAsync("chat.db", workDir).ConfigureAwait(false);
        var chatDbContext = new ChatDbContext(Path.Combine(workDir, "chat.db"));
        await chatDbContext.Database.MigrateAsync().ConfigureAwait(false);
        return chatDbContext;
    }

    private static async Task CheckDatabaseExistInternalAsync(string dbName, string workDir)
    {
        var targetDbPath = Path.Combine(workDir, dbName);
        if (!File.Exists(targetDbPath))
        {
            var rootPath = string.IsNullOrEmpty(_rootPath) ? AppContext.BaseDirectory : _rootPath;
            var emptyDb = Path.Combine(rootPath, "Assets", dbName);
            await Task.Run(() => File.Copy(emptyDb, targetDbPath)).ConfigureAwait(false);
        }
    }
}
