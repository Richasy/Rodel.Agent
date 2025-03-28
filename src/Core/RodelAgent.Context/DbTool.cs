// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.Context;

internal static class DbTool
{
    public static Func<string, string, Task>? CustomCopyMethod { get; set; }

    public static Func<Exception, Task>? CustomErrorMethod { get; set; }

    public static async Task InitializeAsync(string dbName, string packageDir, string workingDir, Func<string, Task> assignDbAction)
    {
        var targetDbPath = Path.Combine(workingDir, dbName);
        var defaultDbPath = Path.Combine(packageDir, "Assets", "Database", dbName);
        try
        {
            if (!File.Exists(targetDbPath))
            {
                if (!File.Exists(defaultDbPath))
                {
                    throw new FileNotFoundException("Default database file not found.", defaultDbPath);
                }

                if (CustomCopyMethod is not null)
                {
                    await CustomCopyMethod(defaultDbPath, targetDbPath).ConfigureAwait(false);
                }
                else
                {
                    await Task.Run(() => File.Copy(defaultDbPath, targetDbPath)).ConfigureAwait(false);
                }
            }

            await Task.Run(async () => await assignDbAction(targetDbPath).ConfigureAwait(false)).ConfigureAwait(false);
        }
        catch (Exception ex) when (CustomErrorMethod is not null)
        {
            await CustomErrorMethod(ex).ConfigureAwait(false);
        }
    }
}
