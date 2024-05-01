﻿// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;

internal static partial class PredefinedModels
{
    internal static List<ChatModel> AnthropicModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Claude 3 Opus",
            Id = "claude-3-opus-20240229",
            Tokens = 200_000,
            MaxOutput = 4096,
        },
        new ChatModel
        {
            DisplayName = "Claude 3 Sonnet",
            Id = "claude-3-sonnet-20240229",
            Tokens = 200_000,
            MaxOutput = 4096,
        },
        new ChatModel
        {
            DisplayName = "Claude 3 Haiku",
            Id = "claude-3-haiku-20240307",
            Tokens = 200_000,
            MaxOutput = 4096,
        },
    };
}
