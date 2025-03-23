// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace RodelAgent.Models.Feature;

/// <summary>
/// Chat interop message.
/// </summary>
public class ChatInteropMessage
{
    /// <summary>
    /// 消息.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// 助理ID.
    /// </summary>
    [JsonPropertyName("agent_id")]
    public string? AgentId { get; set; }

    /// <summary>
    /// 角色.
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; }

    /// <summary>
    /// 消息ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 时间.
    /// </summary>
    [JsonPropertyName("time")]
    public long Time { get; set; }

    /// <summary>
    /// 工具服务ID.
    /// </summary>
    [JsonPropertyName("tool_client_id")]
    public string? ToolClientId { get; set; }

    /// <summary>
    /// 工具方法名称.
    /// </summary>
    [JsonPropertyName("tool_method")]
    public string? ToolMethod { get; set; }

    /// <summary>
    /// 工具数据.
    /// </summary>
    [JsonPropertyName("tool_data")]
    public string? ToolData { get; set; }

    /// <summary>
    /// Clone the message.
    /// </summary>
    /// <returns>Message with new id.</returns>
    public ChatInteropMessage Clone()
    {
        return new ChatInteropMessage
        {
            Id = Guid.NewGuid().ToString("N"),
            Message = Message,
            AgentId = AgentId,
            Role = Role,
            Time = Time,
        };
    }
}

/// <summary>
/// 用于 Web 交互的聊天消息.
/// </summary>
public sealed class ChatWebInteropMessage : ChatInteropMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatWebInteropMessage"/> class.
    /// </summary>
    public ChatWebInteropMessage()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatWebInteropMessage"/> class.
    /// </summary>
    public ChatWebInteropMessage(ChatInteropMessage source, string? author = null)
    {
        Id = source.Id;
        Message = source.Message;
        AgentId = source.AgentId;
        Role = source.Role;
        Time = source.Time;
        ToolClientId = source.ToolClientId;
        ToolMethod = source.ToolMethod;
        ToolData = source.ToolData;
        Author = author;
    }

    /// <summary>
    /// 作者.
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>
    /// 表情.
    /// </summary>
    [JsonPropertyName("emoji")]
    public string? Emoji { get; set; }

    /// <summary>
    /// 头像.
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 显示头像.
    /// </summary>
    [JsonPropertyName("showLogo")]
    public bool ShowLogo { get; set; }
}