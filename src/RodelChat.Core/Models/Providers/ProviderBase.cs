// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;

namespace RodelChat.Core.Models.Providers;

/// <summary>
/// 服务商基类.
/// </summary>
public abstract class ProviderBase
{
    /// <summary>
    /// 标识符.
    /// </summary>
    public string Id { get; internal set; }

    /// <summary>
    /// 基础 URL.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// 访问密钥.
    /// </summary>
    public string? AccessKey { get; set; }

    /// <summary>
    /// 自定义的模型列表.
    /// </summary>
    public List<ChatModel>? CustomModels { get; set; }

    /// <summary>
    /// 服务端模型列表.
    /// </summary>
    public List<ChatModel>? ServerModels { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ProviderBase @base && Id == @base.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
