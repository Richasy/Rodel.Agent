// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace RodelAgent.Models.Feature;

/// <summary>
/// Edited interop message.
/// </summary>
public sealed class EditedInteropMessage
{
    /// <summary>
    /// 消息.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// 索引.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
}
