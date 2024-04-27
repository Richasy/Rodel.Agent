// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Constants;

namespace RodelChat.Core.Models.Chat;

/// <summary>
/// 聊天会话.
/// </summary>
public sealed class ChatSession
{
    /// <summary>
    /// 会话标识符.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 获取或设置会话标题.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 会话参数.
    /// </summary>
    public ChatParameters Parameters { get; set; }

    /// <summary>
    /// 最大会话轮次.
    /// </summary>
    /// <remarks>
    /// <para>用户提问，AI 回答，一问一答称为一轮.</para>
    /// <para>默认为 <c>0</c>，表示不限轮次，直到达到预设的最大上下文窗口.</para>
    /// <para>当超过最大轮次后，之前的记录虽然保留，但不会作为上下文发送，将重新开始新一轮对话.</para>
    /// </remarks>
    public int MaxRounds { get; set; }

    /// <summary>
    /// 是否使用流输出.
    /// </summary>
    /// <remarks>
    /// <para>即通过 SSE（Server-Sent Events）实时接收服务器回传的数据.</para>
    /// <para>理论上响应速度更快，反应在 UI 上就会有打字机的效果.</para>
    /// </remarks>
    public bool UseStreamOutput { get; set; } = true;

    /// <summary>
    /// 服务商. 如果为 <see langword="null"/>，则使用默认服务商.
    /// </summary>
    public ProviderType? Provider { get; set; }

    /// <summary>
    /// 指定的模型，如果为 <see langword="null"/>，则使用默认模型.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 获取或设置系统指令.
    /// </summary>
    /// <remarks>
    /// <para>系统指令是特殊消息，它会引导 AI 的行为。它不受轮次限制，将始终和上下文一起发送给模型.</para>
    /// </remarks>
    public string? SystemInstruction { get; set; }

    /// <summary>
    /// 获取或设置历史记录.
    /// </summary>
    public List<ChatMessage> History { get; set; }

    /// <summary>
    /// 创建会话.
    /// </summary>
    /// <param name="id">标识符.</param>
    /// <param name="parameters">参数.</param>
    /// <returns>会话信息.</returns>
    public static ChatSession CreateSession(string id, ChatParameters parameters)
    {
        return new ChatSession
        {
            Id = id,
            Parameters = parameters,
            History = new List<ChatMessage>(),
        };
    }
}
