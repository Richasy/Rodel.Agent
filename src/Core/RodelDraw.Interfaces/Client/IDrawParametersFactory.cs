// Copyright (c) Rodel. All rights reserved.

using RodelAgent.Models.Abstractions;
using RodelDraw.Models.Constants;

namespace RodelDraw.Interfaces.Client;

/// <summary>
/// 绘图参数工厂.
/// </summary>
public interface IDrawParametersFactory
{
    /// <summary>
    /// 创建绘图参数.
    /// </summary>
    /// <param name="provider">绘图供应商.</param>
    /// <returns>参数.</returns>
    BaseFieldParameters CreateDrawParameters(ProviderType provider);
}
