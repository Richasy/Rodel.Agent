// Copyright (c) Rodel. All rights reserved.

using RodelChat.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<ChatModel> ZhiPuModels { get; } = new List<ChatModel>
    {
        new ChatModel
        {
            DisplayName = "GLM-4-0520",
            Id = "glm-4-0520",
            IsSupportTool = true,
            ContextLength = 128_000,
        },
        new ChatModel
        {
            DisplayName = "GLM-4",
            Id = "glm-4",
            IsSupportTool = true,
            ContextLength = 128_000,
        },
        new ChatModel
        {
            DisplayName = "GLM-4 Vision",
            Id = "glm-4v",
            IsSupportVision = true,
            IsSupportBase64Image = true,
            ContextLength = 128_000,
        },
        new ChatModel
        {
            DisplayName = "GLM-3 Turbo",
            Id = "glm-3-turbo",
            IsSupportTool = true,
            ContextLength = 128_000,
        },
    };
}
