// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;

internal static partial class PredefinedModels
{
    internal static List<ChatModel> MoonshotModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Moonshot V1 8K",
            Id = "moonshot-v1-8k",
            Tokens = 8192,
        },
        new ChatModel
        {
            DisplayName = "Moonshot V1 32K",
            Id = "moonshot-v1-32k",
            Tokens = 32_768,
        },
        new ChatModel
        {
            DisplayName = "Moonshot V1 128K",
            Id = "moonshot-v1-128k",
            Tokens = 128_000,
        },
    };
}
