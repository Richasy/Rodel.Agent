// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;

internal static partial class PredefinedModels
{
    internal static List<ChatModel> QianFanModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "ERNIE-4.0-8K",
            Id = "completions_pro",
            Tokens = 5000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-3.5-8K",
            Id = "completions",
            Tokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Speed-8K",
            Id = "ernie_speed",
            Tokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Lite-8K",
            Id = "eb-instant",
            Tokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "ERNIE-Tiny-8K",
            Id = "ernie-tiny-8k",
            Tokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "Gemma-7B-it",
            Id = "gemma_7b_it",
            Tokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "Mixtral-8x7B-Instruct",
            Id = "mixtral_8x7b_instruct",
            Tokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "Meta-Llama-3-8B-Instruct",
            Id = "llama_3_8b",
            Tokens = 4000,
        },
        new ChatModel
        {
            DisplayName = "Meta-Llama-3-70B-Instruct",
            Id = "llama_3_70b",
            Tokens = 4000,
        },
    };
}
