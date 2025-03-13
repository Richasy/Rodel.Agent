﻿// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.AgentKernel;
using System.Text.Json.Serialization;

namespace RodelAgent.Models.Feature;

/// <summary>
/// 对话助理.
/// </summary>
public sealed class ChatAgent
{
    /// <summary>
    /// 标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 获取或设置选项.
    /// </summary>
    [JsonPropertyName("options")]
    [JsonConverter(typeof(ChatOptionsJsonConverter))]
    public ChatOptions? Options { get; set; }

    /// <summary>
    /// 最大会话轮次.
    /// </summary>
    /// <remarks>
    /// <para>用户提问，AI 回答，一问一答称为一轮.</para>
    /// <para>默认为 <c>0</c>，表示不限轮次，直到达到预设的最大上下文窗口.</para>
    /// <para>当超过最大轮次后，之前的记录虽然保留，但不会作为上下文发送，将重新开始新一轮对话.</para>
    /// </remarks>
    [JsonPropertyName("max_rounds")]
    public int MaxRounds { get; set; }

    /// <summary>
    /// 是否使用流输出.
    /// </summary>
    /// <remarks>
    /// <para>即通过 SSE（Server-Sent Events）实时接收服务器回传的数据.</para>
    /// <para>理论上响应速度更快，反应在 UI 上就会有打字机的效果.</para>
    /// </remarks>
    [JsonPropertyName("stream")]
    public bool? UseStreamOutput { get; set; }

    /// <summary>
    /// 服务商.
    /// </summary>
    [JsonPropertyName("provider")]
    public ChatProviderType? Provider { get; set; }

    /// <summary>
    /// 指定的模型.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    /// 获取或设置系统指令.
    /// </summary>
    /// <remarks>
    /// <para>系统指令是特殊消息，它会引导 AI 的行为。它不受轮次限制，将始终和上下文一起发送给模型.</para>
    /// </remarks>
    [JsonPropertyName("system")]
    public string? SystemInstruction { get; set; }

    /// <summary>
    /// 获取或设置历史记录.
    /// </summary>
    [JsonPropertyName("history")]
    public List<ChatInteropMessage>? History { get; set; }

    /// <summary>
    /// 需要过滤掉的字符.
    /// </summary>
    [JsonPropertyName("filter_chars")]
    public List<string>? FilterCharacters { get; set; }

    /// <summary>
    /// 表情头像.
    /// </summary>
    [JsonPropertyName("emoji")]
    public string? Emoji { get; set; }

    /// <summary>
    /// 工具列表.
    /// </summary>
    [JsonPropertyName("tools")]
    public List<string>? Tools { get; set; }

    /// <summary>
    /// Clone the agent.
    /// </summary>
    /// <returns>Agent with new id.</returns>
    public ChatAgent Clone()
    {
        return new ChatAgent
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = Name,
            Options = Options?.Clone(),
            MaxRounds = MaxRounds,
            UseStreamOutput = UseStreamOutput,
            Provider = Provider,
            Model = Model,
            SystemInstruction = SystemInstruction,
            History = History?.Select(p => p.Clone()).ToList(),
            FilterCharacters = FilterCharacters?.ToList(),
            Emoji = Emoji,
        };
    }
}
