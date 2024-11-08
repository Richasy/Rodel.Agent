// Copyright (c) Rodel. All rights reserved.

namespace RodelDraw.Models.Client;

/// <summary>
/// 绘图参数.
/// </summary>
public sealed class DrawParameters
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawParameters"/> class.
    /// </summary>
    public DrawParameters(string modelId, int width, int height, string? negativePrompt = default)
    {
        ModelId = modelId;
        Width = width;
        Height = height;
        NegativePrompt = negativePrompt;
    }

    /// <summary>
    /// 模型标识.
    /// </summary>
    public string ModelId { get; set; }

    /// <summary>
    /// 图像宽度.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 图像高度.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 负面提示词.
    /// </summary>
    public string? NegativePrompt { get; set; }
}
