// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> HunYuanModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "HunYuan Pro",
            Id = "hunyuan-pro",
            ContextLength = 32_000,
            MaxOutput = 4000,
        },
        new ChatModel
        {
            DisplayName = "HunYuan Standard",
            Id = "hunyuan-standard",
            ContextLength = 32_000,
            MaxOutput = 2000,
        },
        new ChatModel
        {
            DisplayName = "HunYuan Lite",
            Id = "hunyuan-lite",
            ContextLength = 4000,
            MaxOutput = 1000,
        },
    };
}
