// Copyright (c) Richasy. All rights reserved.

namespace RodelAgent.Interfaces;

/// <summary>
/// 字符串资源工具箱.
/// </summary>
public interface IStringResourceToolkit
{
    /// <summary>
    /// 获取字符串.
    /// </summary>
    /// <param name="key">资源名称.</param>
    /// <returns>文本资源.</returns>
    string GetLocalizedString(string key);
}
