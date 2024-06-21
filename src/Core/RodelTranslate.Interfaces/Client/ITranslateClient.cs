// Copyright (c) Rodel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;

namespace RodelTranslate.Interfaces.Client;

/// <summary>
/// 翻译客户端的接口定义.
/// </summary>
public interface ITranslateClient : IDisposable
{
    /// <summary>
    /// 获取语言列表.
    /// </summary>
    /// <param name="type">服务类型.</param>
    /// <returns>语言列表.</returns>
    List<Language> GetLanguageList(ProviderType type);

    /// <summary>
    /// 检查文本是否超过长度限制.
    /// </summary>
    /// <param name="text">文本.</param>
    /// <param name="type">服务商.</param>
    /// <returns>是否超出.</returns>
    bool IsTextExceedLimit(string text, ProviderType type);

    /// <summary>
    /// 获取最大文本长度.
    /// </summary>
    /// <param name="type">服务商.</param>
    /// <returns>文本长度.</returns>
    long GetMaxTextLength(ProviderType type);

    /// <summary>
    /// 翻译指定文本.
    /// </summary>
    /// <param name="sessionData">会话数据.</param>
    /// <param name="input">需要翻译的文本.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns>文本翻译结果.</returns>
    Task<TranslateTextContent> TranslateTextAsync(
        TranslateSession sessionData,
        string input,
        CancellationToken cancellationToken = default);
}
