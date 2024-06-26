﻿// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> MistralAIModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Mistral 7B",
            Id = "open-mistral-7b",
            ContextLength = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Mixtral 8x7b",
            Id = "open-mixtral-8x7b",
            ContextLength = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Mixtral 8x22B",
            Id = "open-mixtral-8x22b",
            ContextLength = 65_536,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Mistral Small",
            Id = "mistral-small-latest",
            ContextLength = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "Mistral Large",
            Id = "mistral-large-latest",
            ContextLength = 32_768,
            IsSupportTool = true,
        },
    };
}
