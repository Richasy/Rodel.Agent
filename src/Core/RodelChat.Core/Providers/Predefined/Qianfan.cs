// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> QianFanModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "ERNIE-4.0-8K",
            Id = "completions_pro",
            ContextLength = 5000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-3.5-8K",
            Id = "completions",
            ContextLength = 4000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Speed-8K",
            Id = "ernie_speed",
            ContextLength = 4000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Lite-8K",
            Id = "eb-instant",
            ContextLength = 4000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Tiny-8K",
            Id = "ernie-tiny-8k",
            ContextLength = 4000,
        },
    };
}
