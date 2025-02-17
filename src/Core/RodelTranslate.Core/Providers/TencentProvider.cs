// Copyright (c) Richasy. All rights reserved.

using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;
using System.Text.Json.Serialization;

namespace RodelTranslate.Core.Providers;

/// <summary>
/// 腾讯翻译服务提供商.
/// </summary>
public sealed class TencentProvider : ProviderBase, IProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TencentProvider"/> class.
    /// </summary>
    public TencentProvider(TencentClientConfig config)
        : base(config.Key)
    {
        SecretId = config.SecretId;
        Languages = PredefinedLanguages.TencentLanguages;
    }

    private string SecretId { get; }

    /// <inheritdoc/>
    public Kernel? GetOrCreateKernel()
    {
        Kernel ??= Kernel.CreateBuilder()
                .AddTencentTextTranslation(SecretId, AccessKey)
                .Build();
        return Kernel;
    }

    /// <inheritdoc/>
    public override TranslateExecutionSettings ConvertExecutionSettings(TranslateSession sessionData)
        => new TencentTranslateExecutionSettings
        {
            From = sessionData.SourceLanguage?.Id,
            To = sessionData.TargetLanguage?.Id,
            UntranslatedText = sessionData.Parameters.GetValueOrDefault<string>(nameof(TencentTranslateParameters.UntranslatedText)),
        };

    /// <summary>
    /// 腾讯翻译参数.
    /// </summary>
    [JsonConverter(typeof(BaseFieldParametersConverter))]
    public sealed class TencentTranslateParameters : BaseFieldParameters
    {
        /// <summary>
        /// 不需要翻译的文本内容.
        /// </summary>
        [JsonPropertyName("untranslated_text")]
        [TextField]
        public string? UntranslatedText { get; set; }
    }
}
