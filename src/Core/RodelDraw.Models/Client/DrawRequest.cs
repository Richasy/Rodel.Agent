// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace RodelDraw.Models.Client;

/// <summary>
/// 绘图请求.
/// </summary>
public sealed class DrawRequest
{
    /// <summary>
    /// 提示词.
    /// </summary>
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }

    /// <summary>
    /// 负面提示词.
    /// </summary>
    [JsonPropertyName("negative_prompt")]
    public string? NegativePrompt { get; set; }

    /// <summary>
    /// 尺寸.
    /// </summary>
    [JsonPropertyName("size")]
    public string? Size { get; set; }
}
