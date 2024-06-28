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
            DisplayName = "ERNIE-4.0-Turbo-8K",
            Id = "ernie-4.0-turbo-8k",
            ContextLength = 8124,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-4.0-8K",
            Id = "completions_pro",
            ContextLength = 8124,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-3.5-8K",
            Id = "completions",
            ContextLength = 8124,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Speed-8K",
            Id = "ernie_speed",
            ContextLength = 8124,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Lite-8K",
            Id = "eb-instant",
            ContextLength = 8124,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Tiny-8K",
            Id = "ernie-tiny-8k",
            ContextLength = 8124,
        },
    };
}
