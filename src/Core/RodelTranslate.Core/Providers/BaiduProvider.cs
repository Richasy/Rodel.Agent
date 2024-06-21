// Copyright (c) Rodel. All rights reserved.

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Translators.Baidu;
using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;

namespace RodelTranslate.Core.Providers;

/// <summary>
/// 百度翻译服务提供商.
/// </summary>
public sealed class BaiduProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaiduProvider"/> class.
    /// </summary>
    public BaiduProvider(BaiduClientConfig config)
        : base(config.Key)
    {
        AppId = config.AppId;
        Languages = PredefinedLanguages.BaiduLanguages;
    }

    private string AppId { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel()
    {
        Kernel ??= Kernel.CreateBuilder()
                .AddBaiduTextTranslation(AppId, AccessKey)
                .Build();
        return Kernel;
    }

    /// <inheritdoc/>
    public override TranslateExecutionSettings ConvertExecutionSettings(TranslateSession sessionData)
        => new BaiduTranslateExecutionSettings
        {
            From = sessionData.SourceLanguage?.Id,
            To = sessionData.TargetLanguage?.Id,
        };

    /// <inheritdoc/>
    public override long GetMaxTextLength() => 6000;
}
