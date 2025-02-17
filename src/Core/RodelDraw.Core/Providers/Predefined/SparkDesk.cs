// Copyright (c) Richasy. All rights reserved.

using RodelDraw.Models.Client;

/// <summary>
/// 预定义模型.
/// </summary>
internal static partial class PredefinedModels
{
    internal static List<DrawModel> SparkDeskModels { get; } = new List<DrawModel>
    {
        new DrawModel
        {
            Id = "v2.1",
            DisplayName = "讯飞文生图 2.1",
            IsNegativeSupport = false,
            SupportSizes = [
                "512x512",
                "640x360",
                "640x480",
                "640x640",
                "680x512",
                "512x680",
                "768x768",
                "720x1280",
                "1280x720",
                "1024x1024"
              ],
        },
    };
}
