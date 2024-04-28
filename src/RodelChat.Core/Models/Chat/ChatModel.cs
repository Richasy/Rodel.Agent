// Copyright (c) Rodel. All rights reserved.

namespace RodelChat.Core.Models.Chat;

/// <summary>
/// 模型信息.
/// </summary>
public sealed class ChatModel
{
    /// <summary>
    /// 获取或设置模型 ID.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 获取或设置模型的显示名称.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// 获取或设置模型是否支持上传文件.
    /// </summary>
    public bool IsSupportFileUpload { get; set; }

    /// <summary>
    /// 获取或设置模型是否支持工具.
    /// </summary>
    public bool IsSupportTool { get; set; }

    /// <summary>
    /// 获取或设置模型是否支持视觉功能.
    /// </summary>
    public bool IsSupportVision { get; set; }

    /// <summary>
    /// 获取或设置模型是否支持 Base64 图片.
    /// </summary>
    public bool? IsSupportBase64Image { get; set; }

    /// <summary>
    /// 获取或设置该模型是否为自定义模型.
    /// </summary>
    public bool IsCustomModel { get; set; }

    /// <summary>
    /// 该模型是否已弃用或不推荐使用.
    /// </summary>
    public bool IsDeprecated { get; set; }

    /// <summary>
    /// 模型支持的最大输出长度.
    /// </summary>
    public long MaxOutput { get; set; }

    /// <summary>
    /// 模型的令牌数.
    /// </summary>
    public long Tokens { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatModel model && Id == model.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    /// <inheritdoc/>
    public override string ToString() => DisplayName ?? Id;
}
