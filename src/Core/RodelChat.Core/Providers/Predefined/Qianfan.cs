// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Chat;

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
            MaxTokens = 5000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-3.5-8K",
            Id = "completions",
            MaxTokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Speed-8K",
            Id = "ernie_speed",
            MaxTokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Lite-8K",
            Id = "eb-instant",
            MaxTokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Tiny-8K",
            Id = "ernie-tiny-8k",
            MaxTokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "Gemma-7B-it",
            Id = "gemma_7b_it",
            MaxTokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "Mixtral-8x7B-Instruct",
            Id = "mixtral_8x7b_instruct",
            MaxTokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "Meta-Llama-3-8B-Instruct",
            Id = "llama_3_8b",
            MaxTokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "Meta-Llama-3-70B-Instruct",
            Id = "llama_3_70b",
            MaxTokens = 4000,
        },
    };
}
