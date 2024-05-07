// Copyright (c) Rodel. All rights reserved.

using System.Collections.Generic;
using RodelChat.Models.Chat;

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
            MaxTokens = 8192,
        },
        new ChatModel
        {
            DisplayName = "Moonshot V1 32K",
            Id = "moonshot-v1-32k",
            MaxTokens = 32_768,
        },
        new ChatModel
        {
            DisplayName = "Moonshot V1 128K",
            Id = "moonshot-v1-128k",
            MaxTokens = 128_000,
        },
    };
}
