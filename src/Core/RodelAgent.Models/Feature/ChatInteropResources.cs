// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace RodelAgent.Models.Feature;

/// <summary>
/// Chat interop resources.
/// </summary>
public sealed class ChatInteropResources
{
    /// <summary>
    /// 保存.
    /// </summary>
    [JsonPropertyName("save")]
    public string Save { get; set; }

    /// <summary>
    /// 取消.
    /// </summary>
    [JsonPropertyName("discard")]
    public string Discard { get; set; }

    /// <summary>
    /// 复制.
    /// </summary>
    [JsonPropertyName("copy")]
    public string Copy { get; set; }

    /// <summary>
    /// 编辑.
    /// </summary>
    [JsonPropertyName("edit")]
    public string Edit { get; set; }

    /// <summary>
    /// 删除.
    /// </summary>
    [JsonPropertyName("delete")]
    public string Delete { get; set; }

    /// <summary>
    /// 思考过程.
    /// </summary>
    [JsonPropertyName("thoughtProcess")]
    public string ThoughtProcess { get; set; }

    /// <summary>
    /// 思考中.
    /// </summary>
    [JsonPropertyName("thinking")]
    public string Thinking { get; set; }
}
