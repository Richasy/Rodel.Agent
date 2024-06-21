// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> MoonshotModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Moonshot V1 8K",
            Id = "moonshot-v1-8k",
            ContextLength = 8192,
        },
        new ChatModel
        {
            DisplayName = "Moonshot V1 32K",
            Id = "moonshot-v1-32k",
            ContextLength = 32_768,
        },
        new ChatModel
        {
            DisplayName = "Moonshot V1 128K",
            Id = "moonshot-v1-128k",
            ContextLength = 128_000,
        },
    };
}
