// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;

internal static partial class PredefinedModels
{
    internal static List<ChatModel> DashScopeModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Qwen Turbo",
            Id = "qwen-turbo",
            Tokens = 6000,
        },
        new ChatModel
        {
            DisplayName = "Qwen Plus",
            Id = "qwen-plus",
            Tokens = 30_000,
        },
        new ChatModel
        {
            DisplayName = "Qwen Max",
            Id = "qwen-max",
            Tokens = 6000,
        },
        new ChatModel
        {
            DisplayName = "Qwen Max Long Context",
            Id = "qwen-max-longcontext",
            Tokens = 28_000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 1.5 72B Chat",
            Id = "qwen1.5-72b-chat",
            Tokens = 30_000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 1.5 32B Chat",
            Id = "qwen1.5-32b-chat",
            Tokens = 30_000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 1.5 14B Chat",
            Id = "qwen1.5-14b-chat",
            Tokens = 6000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 1.5 7B Chat",
            Id = "qwen1.5-7b-chat",
            Tokens = 6000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 72B Chat",
            Id = "qwen-72b-chat",
            Tokens = 30_000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 14B Chat",
            Id = "qwen-14b-chat",
            Tokens = 6000,
        },
        new ChatModel
        {
            DisplayName = "Qwen 7B Chat",
            Id = "qwen-7b-chat",
            Tokens = 6000,
        },
    };
}
