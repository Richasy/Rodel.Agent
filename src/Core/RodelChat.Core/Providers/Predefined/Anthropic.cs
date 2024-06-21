// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> AnthropicModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Claude 3.5 Sonnet",
            Id = "claude-3-5-sonnet-20240620",
            ContextLength = 200_000,
            MaxOutput = 4096,
        },
        new ChatModel
        {
            DisplayName = "Claude 3 Opus",
            Id = "claude-3-opus-20240229",
            ContextLength = 200_000,
            MaxOutput = 4096,
        },
        new ChatModel
        {
            DisplayName = "Claude 3 Sonnet",
            Id = "claude-3-sonnet-20240229",
            ContextLength = 200_000,
            MaxOutput = 4096,
        },
        new ChatModel
        {
            DisplayName = "Claude 3 Haiku",
            Id = "claude-3-haiku-20240307",
            ContextLength = 200_000,
            MaxOutput = 4096,
        },
    };
}
