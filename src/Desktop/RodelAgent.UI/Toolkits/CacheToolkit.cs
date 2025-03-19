// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel.Core.Mcp.Shared;
using System.Text.Json;

namespace RodelAgent.UI.Toolkits;

public static class CacheToolkit
{
    public static async Task<McpServerDefinitionCollection?> GetMcpServersAsync()
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

        return JsonSerializer.Deserialize(json, JsonGenContext.Default.McpServerDefinitionCollection);
    }

    public static async Task SaveMcpServersAsync(McpServerDefinitionCollection collection)
    {
        var filePath = Path.Combine(GetLibraryPath(), "McpServers.json");
        var json = JsonSerializer.Serialize(collection, JsonGenContext.Default.McpServerDefinitionCollection);
        await File.WriteAllTextAsync(filePath, json);
    }

    private static string GetLibraryPath()
        => SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.WorkingDirectory, string.Empty);
}
