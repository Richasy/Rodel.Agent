// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;

internal static partial class PredefinedModels
{
    internal static List<ChatModel> OpenAIModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "GPT-3.5 Turbo",
            Id = "gpt-3.5-turbo",
            IsSupportTool = true,
            Tokens = 16_385,
        },
        new ChatModel
        {
            DisplayName = "GPT-3.5 Turbo (0125)",
            Id = "gpt-3.5-turbo-0125",
            IsSupportTool = true,
            Tokens = 16_385,
        },
        new ChatModel
        {
            DisplayName = "GPT-3.5 Turbo (1106)",
            Id = "gpt-3.5-turbo-1106",
            IsSupportTool = true,
            Tokens = 16_385,
        },
        new ChatModel
        {
            DisplayName = "GPT-3.5 Turbo Instruct",
            Id = "gpt-3.5-turbo-instruct",
            Tokens = 4096,
        },
        new ChatModel
        {
            DisplayName = "GPT-3.5 Turbo 16K",
            Id = "gpt-3.5-turbo-16k",
            Tokens = 16_385,
        },
        new ChatModel
        {
            DisplayName = "GPT-3.5 Turbo (0613)",
            Id = "gpt-3.5-turbo-0613",
            Tokens = 4096,
            IsDeprecated = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-3.5 Turbo 16K (0613)",
            Id = "gpt-3.5-turbo-16k-0613",
            Tokens = 4096,
            IsDeprecated = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Turbo Preview",
            Id = "gpt-4-turbo-preview",
            IsSupportTool = true,
            Tokens = 128_000,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Turbo Preview (0125)",
            Id = "gpt-4-0125-preview",
            Tokens = 128_000,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Turbo Vision Preview",
            Id = "gpt-4-vision-preview",
            Tokens = 128_000,
            IsSupportVision = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Turbo Vision Preview (1106)",
            Id = "gpt-4-1106-vision-preview",
            Tokens = 128_000,
            IsSupportVision = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Turbo Preview (1106)",
            Id = "gpt-4-1106-preview",
            Tokens = 128_000,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4",
            Id = "gpt-4",
            Tokens = 8192,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 (0613)",
            Id = "gpt-4-0613",
            Tokens = 8192,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 32K",
            Id = "gpt-4-32k",
            Tokens = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 32K (0613)",
            Id = "gpt-4-32k-0613",
            Tokens = 32_768,
            IsSupportTool = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 32K (1106)",
            Id = "gpt-4-turbo",
            Tokens = 128_000,
            IsSupportTool = true,
            IsSupportVision = true,
        },
        new ChatModel
        {
            DisplayName = "GPT-4 Turbo Vision (240409)",
            Id = "gpt-4-turbo-2024-04-09",
            Tokens = 128_000,
            IsSupportTool = true,
            IsSupportVision = true,
        },
    };
}
