// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;

internal static partial class PredefinedModels
{
    internal static List<ChatModel> ZhipuModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "GLM-4",
            Id = "glm-4",
            IsSupportTool = true,
            Tokens = 128_000,
        },
        new ChatModel
        {
            DisplayName = "GLM-4 Vision",
            Id = "glm-4v",
            IsSupportVision = true,
            IsSupportBase64Image = true,
            Tokens = 128_000,
        },
        new ChatModel
        {
            DisplayName = "GLM-3 Turbo",
            Id = "glm-3-turbo",
            IsSupportTool = true,
            Tokens = 128_000,
        },
    };
}
