// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.Context;

internal static class DbTool
{
    public static async Task InitializeAsync(string dbName, string packageDir, string workingDir, Func<string, Task> assignDbAction)
    {
        var targetDbPath = Path.Combine(workingDir, dbName);
        var defaultDbPath = Path.Combine(packageDir, "Assets", "Database", dbName);
        if (!File.Exists(targetDbPath))
        {
            if (!File.Exists(defaultDbPath))
            {
                throw new FileNotFoundException("Default database file not found.", defaultDbPath);
            }

            await Task.Run(() => File.Copy(defaultDbPath, targetDbPath)).ConfigureAwait(false);
        }

        await Task.Run(async () => await assignDbAction(targetDbPath).ConfigureAwait(false)).ConfigureAwait(false);
    }
}
