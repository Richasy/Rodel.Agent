// Copyright (c) Rodel. All rights reserved.

using Microsoft.EntityFrameworkCore;

namespace RodelAgent.Context;

/// <summary>
/// 数据库迁移工具.
/// </summary>
internal static class MigrationUtils
{
    /// <summary>
    /// 获取密钥数据库.
    /// </summary>
    /// <param name="workDir">工作目录.</param>
    /// <returns><see cref="SecretDbContext"/>.</returns>
    public static async Task<SecretDbContext> GetSecretDbAsync(string workDir)
    {
        await CheckDatabaseExistInternalAsync("secret.db", workDir);
        return new SecretDbContext(Path.Combine(workDir, "secret.db"));
    }

    /// <summary>
    /// 获取聊天数据库.
    /// </summary>
    /// <param name="workDir">工作目录.</param>
    /// <returns><see cref="ChatDbContext"/>.</returns>
    public static async Task<ChatDbContext> GetChatDbAsync(string workDir)
    {
        await CheckDatabaseExistInternalAsync("chat.db", workDir);
        var chatDbContext = new ChatDbContext(Path.Combine(workDir, "chat.db"));
        await chatDbContext.Database.MigrateAsync();
        return chatDbContext;
    }

    /// <summary>
    /// 获取翻译数据库.
    /// </summary>
    /// <param name="workDir">工作目录.</param>
    /// <returns><see cref="TranslateDbContext"/>.</returns>
    public static async Task<TranslateDbContext> GetTranslateDbAsync(string workDir)
    {
        await CheckDatabaseExistInternalAsync("trans.db", workDir);
        return new TranslateDbContext(Path.Combine(workDir, "trans.db"));
    }

    /// <summary>
    /// 获取绘图数据库.
    /// </summary>
    /// <param name="workDir">工作目录.</param>
    /// <returns><see cref="DrawDbContext"/>.</returns>
    public static async Task<DrawDbContext> GetDrawDbAsync(string workDir)
    {
        await CheckDatabaseExistInternalAsync("draw.db", workDir);
        return new DrawDbContext(Path.Combine(workDir, "draw.db"));
    }

    /// <summary>
    /// 获取音频数据库.
    /// </summary>
    /// <param name="workDir">工作目录.</param>
    /// <returns><see cref="AudioDbContext"/>.</returns>
    public static async Task<AudioDbContext> GetAudioDbAsync(string workDir)
    {
        await CheckDatabaseExistInternalAsync("audio.db", workDir);
        return new AudioDbContext(Path.Combine(workDir, "audio.db"));
    }

    private static async Task CheckDatabaseExistInternalAsync(string dbName, string workDir)
    {
        var targetDbPath = Path.Combine(workDir, dbName);
        if (!File.Exists(targetDbPath))
        {
            var emptyDb = Path.Combine(AppContext.BaseDirectory, "Assets", dbName);
            await Task.Run(() => File.Copy(emptyDb, targetDbPath));
        }
    }
}
