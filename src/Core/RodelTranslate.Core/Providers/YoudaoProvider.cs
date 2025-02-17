// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;

namespace RodelTranslate.Core.Providers;

/// <summary>
/// 有道翻译服务提供商.
/// </summary>
public sealed class YoudaoProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YoudaoProvider"/> class.
    /// </summary>
    public YoudaoProvider(YoudaoClientConfig config)
        : base(config.Key)
    {
        AppId = config.AppId;
        Languages = PredefinedLanguages.YoudaoLanguages;
    }

    private string AppId { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel()
    {
        Kernel ??= Kernel.CreateBuilder()
                .AddYoudaoTextTranslation(AppId, AccessKey)
                .Build();
        return Kernel;
    }

    /// <inheritdoc/>
    public override TranslateExecutionSettings ConvertExecutionSettings(TranslateSession sessionData)
        => new YoudaoTranslateExecutionSettings
        {
            From = sessionData.SourceLanguage?.Id,
            To = sessionData.TargetLanguage?.Id,
        };
}
