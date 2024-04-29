﻿// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;

internal static partial class PredefinedModels
{
    internal static List<ChatModel> GeminiModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "Gemini 1.5 Pro",
            Id = "gemini-1.5-pro-latest",
            Tokens = 1_048_576,
            MaxOutput = 8192,
        },
        new ChatModel
        {
            DisplayName = "Gemini Pro",
            Id = "gemini-pro",
            Tokens = 30_720,
            MaxOutput = 2048,
        },
        new ChatModel
        {
            DisplayName = "Gemini 1.0 Pro Vision",
            Id = "gemini-pro-vision",
            IsSupportVision = true,
            Tokens = 12_288,
            MaxOutput = 4096,
        },
    };
}
