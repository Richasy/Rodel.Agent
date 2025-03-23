// Copyright (c) Richasy. All rights reserved.

using RodelAgent.UI.ViewModels.Items;
using System.Text.Json;

namespace RodelAgent.UI.Toolkits;

public static class CacheToolkit
{
    public static async Task<McpAgentConfigCollection?> GetMcpServersAsync()
    {
        var filePath = Path.Combine(GetLibraryPath(), "McpServers.json");
        if (!File.Exists(filePath))
        {
            return default;
        }

        var json = await File.ReadAllTextAsync(filePath);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize(json, JsonGenContext.Default.McpAgentConfigCollection);
    }

    public static async Task SaveMcpServersAsync(McpAgentConfigCollection collection)
    {
        var filePath = Path.Combine(GetLibraryPath(), "McpServers.json");
        var json = JsonSerializer.Serialize(collection, JsonGenContext.Default.McpAgentConfigCollection);
        await File.WriteAllTextAsync(filePath, json);
    }

    private static string GetLibraryPath()
        => SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
}
