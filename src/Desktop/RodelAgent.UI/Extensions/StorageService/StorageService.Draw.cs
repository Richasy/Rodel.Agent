// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
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
}
