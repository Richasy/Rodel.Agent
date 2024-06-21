// Copyright (c) Rodel. All rights reserved.

namespace RodelTranslate.Models.Constants;

/// <summary>
/// 翻译服务提供商类型.
/// </summary>
public enum ProviderType
{
    /// <summary>
    /// 谷歌翻译.
    /// </summary>
    /// <remarks>
    /// 暂时没钱买谷歌翻译API，所以暂时不支持谷歌翻译.
    /// </remarks>
    Google,

    /// <summary>
    /// 阿里云翻译.
    /// </summary>
    Ali,

    /// <summary>
    /// 百度翻译.
    /// </summary>
    Baidu,

    /// <summary>
    /// 有道翻译.
    /// </summary>
    Youdao,

    /// <summary>
    /// Azure翻译.
    /// </summary>
    Azure,

    /// <summary>
    /// 火山翻译.
    /// </summary>
    Volcano,

    /// <summary>
    /// 腾讯翻译.
    /// </summary>
    Tencent,
}
