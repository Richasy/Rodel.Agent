// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace RodelAgent.Models.Common;

/// <summary>
/// Web数据对象.
/// </summary>
public sealed class WebDataObject
{
    /// <summary>
    /// 类型.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// 内容.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
}