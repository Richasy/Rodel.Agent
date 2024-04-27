// Copyright (c) Rodel. All rights reserved.

namespace RodelChat.Core.Models.Chat;

/// <summary>
/// 聊天参数.
/// </summary>
public sealed class ChatParameters
{
    /// <summary>
    /// 用于减少模型生成的重复性的参数.
    /// </summary>
    public double FrequencyPenalty { get; set; }

    /// <summary>
    /// 用于减少模型主题多样性的参数.
    /// </summary>
    public double PresencePenalty { get; set; }

    /// <summary>
    /// 生成文本的最大长度.
    /// </summary>
    public int MaxTokens { get; set; }

    /// <summary>
    /// 用于控制文本的多样性和创造性.
    /// </summary>
    public double Temperature { get; set; }

    /// <summary>
    /// 用于控制生成文本的顶部概率.
    /// </summary>
    public double TopP { get; set; }

    /// <summary>
    /// 创建聊天参数.
    /// </summary>
    /// <returns><see cref="ChatParameters"/>.</returns>
    public static ChatParameters Create(
        double frequencyPenalty = 0d,
        double presencePenalty = 0d,
        int maxTokens = 250,
        double temperature = 0.6,
        double topP = 1d)
    {
        return new ChatParameters
        {
            FrequencyPenalty = frequencyPenalty,
            PresencePenalty = presencePenalty,
            MaxTokens = maxTokens,
            Temperature = temperature,
            TopP = topP,
        };
    }
}
