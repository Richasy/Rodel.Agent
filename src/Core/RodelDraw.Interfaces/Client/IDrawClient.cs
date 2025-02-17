// Copyright (c) Richasy. All rights reserved.

using RodelDraw.Models.Client;
using RodelDraw.Models.Constants;

namespace RodelDraw.Interfaces.Client;

/// <summary>
/// 聊天客户端的接口定义.
/// </summary>
public interface IDrawClient : IDisposable
{
    /// <summary>
    /// 获取预定义模型.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>预定义模型列表.</returns>
    List<DrawModel> GetPredefinedModels(ProviderType type);

    /// <summary>
    /// 获取模型列表.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>模型列表.</returns>
    List<DrawModel> GetModels(ProviderType type);

    /// <summary>
    /// 请求绘图.
    /// </summary>
    /// <param name="session">绘图请求.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>BASE64 字符串.</returns>
    Task<string> DrawAsync(
        DrawSession session,
        CancellationToken cancellationToken = default);
}
