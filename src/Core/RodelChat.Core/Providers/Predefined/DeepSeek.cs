// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Chat;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> DeepSeekModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "DeepSeek Chat",
            Id = "deepseek-chat",
            MaxTokens = 32_000,
        },
        new ChatModel
        {
            DisplayName = "DeepSeek Coder",
            Id = "deepseek-coder",
            MaxTokens = 16_000,
        },
    };
}
