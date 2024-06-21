// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

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
            ContextLength = 32_000,
            MaxOutput = 4096,
        },
        new ChatModel
        {
            DisplayName = "DeepSeek Coder",
            Id = "deepseek-coder",
            ContextLength = 32_000,
            MaxOutput = 4096,
        },
    };
}
