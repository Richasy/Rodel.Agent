// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;

namespace RodelTranslate.Core.Providers;

/// <summary>
/// 腾讯翻译服务提供商.
/// </summary>
public sealed class VolcanoProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VolcanoProvider"/> class.
    /// </summary>
    public VolcanoProvider(VolcanoClientConfig config)
        : base(config.Key)
    {
        KeyId = config.KeyId;
        Languages = PredefinedLanguages.VolcanoLanguages;
    }

    private string KeyId { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel()
    {
        Kernel ??= Kernel.CreateBuilder()
                .AddVolcanoTextTranslation(KeyId, AccessKey)
                .Build();
        return Kernel;
    }

    /// <inheritdoc/>
    public override TranslateExecutionSettings ConvertExecutionSettings(TranslateSession sessionData)
        => new VolcanoTranslateExecutionSettings
        {
            From = sessionData.SourceLanguage?.Id,
            To = sessionData.TargetLanguage?.Id,
        };
}

