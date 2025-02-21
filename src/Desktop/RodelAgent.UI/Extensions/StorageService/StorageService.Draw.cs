// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Common;
using RodelAgent.UI.Toolkits;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;

namespace RodelAgent.UI.Extensions;

internal sealed partial class StorageService
{
    public async Task<T?> GetDrawConfigAsync<T>(DrawProviderType type, JsonTypeInfo<T> typeInfo) where T : class
    {
        var json = await _dbService.GetSecretAsync("Draw_" + type.ToString());

        if (!string.IsNullOrEmpty(json) && Regex.IsMatch(json, @"\b\d{1,4}x\d{1,4}\b"))
        {
            json = ConvertDrawSizesInJson(json);
        }

        return typeof(T).Equals(typeof(string))
            ? json as T
            : json is null
                ? default
                : JsonSerializer.Deserialize(json, typeInfo);
    }

    public async Task SetDrawConfigAsync<T>(DrawProviderType type, T config, JsonTypeInfo<T> typeInfo) where T : class
    {
        var json = typeof(T).Equals(typeof(string)) ? config as string : JsonSerializer.Serialize(config, typeInfo);
        await _dbService.SetSecretAsync("Draw_" + type.ToString(), json!);
    }

    public async Task<List<DrawRecord>?> GetDrawSessionsAsync()
    {
        var sessionJsonList = await _dbService.GetAllDrawSessionsAsync();
        if (sessionJsonList is null || sessionJsonList.Count == 0)
        {
            return [];
        }

        var sessionList = new List<DrawRecord>();
        foreach (var sessionJson in sessionJsonList)
        {
            var sjson = sessionJson;
            if (string.IsNullOrEmpty(sjson))
            {
                continue;
            }

            if (sjson.Contains("\"request\":", StringComparison.OrdinalIgnoreCase))
            {
                sjson = ConvertDrawRecordInJson(sjson);
            }

            var session = JsonSerializer.Deserialize(sjson, JsonGenContext.Default.DrawRecord);
            if (session != null)
            {
                sessionList.Add(session);
            }
        }

        return sessionList;
    }

    public async Task AddOrUpdateDrawSessionAsync(DrawRecord session, byte[]? imageData)
    {
        var json = JsonSerializer.Serialize(session, JsonGenContext.Default.DrawRecord);
        await _dbService.AddOrUpdateDrawDataAsync(session.Id, json);

        if (imageData == null || imageData.Length == 0)
        {
            return;
        }

        var imagePath = AppToolkit.GetDrawPicturePath(session.Id);
        if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);
        }

        await File.WriteAllBytesAsync(imagePath, imageData);
    }

    public async Task RemoveDrawSessionAsync(string sessionId)
    {
        await _dbService.RemoveDrawDataAsync(sessionId);
        var imagePath = AppToolkit.GetDrawPicturePath(sessionId);
        if (File.Exists(imagePath))
        {
            await Task.Run(() => File.Delete(imagePath));
        }
    }

    private static string ConvertDrawSizesInJson(string json)
    {
        // 解析 JSON 字符串为 JsonNode
        var jsonNode = JsonNode.Parse(json);

        // 获取 models 数组
        if (jsonNode!["models"] is JsonArray modelsArray)
        {
            foreach (var model in modelsArray)
            {
                // 获取 sizes 数组
                if (model!["sizes"] is JsonArray sizesArray)
                {
                    var newSizesArray = new JsonArray();
                    foreach (var size in sizesArray)
                    {
                        if (size != null)
                        {
                            // 分割字符串获取宽度和高度
                            var parts = size.ToString().Split('x');
                            if (parts.Length == 2 && int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height))
                            {
                                var newSizeNode = JsonNode.Parse($"{{\"width\":{width},\"height\":{height}}}");
                                newSizesArray.Add(newSizeNode);
                            }
                        }
                    }
                    // 替换原有的 sizes 数组
                    model["sizes"] = newSizesArray;
                }
            }
        }

        // 将修改后的 JsonNode 转换回 JSON 字符串
        return jsonNode.ToJsonString();
    }

    private static string ConvertDrawRecordInJson(string json)
    {
        // 解析 JSON 字符串为 JsonNode
        var jsonNode = JsonNode.Parse(json);
        if (jsonNode!["request"] is JsonObject jobj)
        {
            jobj.TryGetPropertyValue("negative_prompt", out var negativePrompt);
            jobj.TryGetPropertyValue("prompt", out var prompt);
            jobj.TryGetPropertyValue("size", out var size);
            var negativePromptStr = negativePrompt?.ToString();
            var promptStr = prompt?.ToString();
            var sizeStr = size?.ToString();
            if (!string.IsNullOrEmpty(negativePromptStr))
            {
                jsonNode["negative_prompt"] = negativePromptStr.Replace("\n", "\\n", StringComparison.OrdinalIgnoreCase);
            }

            if (!string.IsNullOrEmpty(promptStr))
            {
                jsonNode["prompt"] = promptStr.Replace("\n", "\\n", StringComparison.OrdinalIgnoreCase);
            }

            if (!string.IsNullOrEmpty(sizeStr))
            {
                var parts = sizeStr.Split('x');
                if (parts.Length == 2 && int.TryParse(parts[0], out var width) && int.TryParse(parts[1], out var height))
                {
                    jsonNode["size"] = JsonNode.Parse($"{{\"width\":{width},\"height\":{height}}}");
                }
            }
        }

        var providerValue = jsonNode["provider"]?.GetValue<int>();
        if (providerValue.HasValue)
        {
            var providerStr = JsonSerializer.Serialize((DrawProviderType)providerValue.Value, JsonGenContext.Default.DrawProviderType);
            jsonNode["provider"] = providerStr.Replace("\"", string.Empty, StringComparison.Ordinal);
        }

        return jsonNode.ToJsonString();
    }
}
