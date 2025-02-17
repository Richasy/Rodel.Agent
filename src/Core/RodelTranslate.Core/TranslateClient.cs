// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.Logging;
using RodelTranslate.Interfaces.Client;
using RodelTranslate.Models.Client;
using RodelTranslate.Models.Constants;
using System.Reflection;

namespace RodelTranslate.Core;

/// <summary>
/// 翻译客户端.
/// </summary>
public sealed partial class TranslateClient : ITranslateClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslateClient"/> class.
    /// </summary>
    public TranslateClient(
        ITranslateProviderFactory providerFactory,
        ITranslateParametersFactory parameterFactory,
        ILogger<TranslateClient> logger)
    {
        _logger = logger;
        _providerFactory = providerFactory;
        _parameterFactory = parameterFactory;
    }

    /// <inheritdoc/>
    public List<Language> GetLanguageList(ProviderType type)
    {
        var preType = typeof(PredefinedLanguages);
        var properties = preType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        foreach (var prop in properties)
        {
            if (prop.Name.StartsWith(type.ToString()))
            {
                return prop.GetValue(default) as List<Language>
                    ?? throw new ArgumentException("Predefined languages not found.");
            }
        }

        return new List<Language>();
    }

    /// <inheritdoc/>
    public async Task<TranslateTextContent> TranslateTextAsync(
        TranslateSession sessionData,
        string input,
        CancellationToken cancellationToken = default)
    {
        if (IsTextExceedLimit(input, sessionData.Provider))
        {
            throw new ArgumentException("Text length exceeds the maximum limit.");
        }

        var kernel = FindKernelProvider(sessionData.Provider)
            ?? throw new ArgumentException("Provider not found.");
        var textService = kernel.GetRequiredService<ITextTranslateService>();
        var settings = GetExecutionSettings(sessionData);
        sessionData.InputText = input;
        var response = await textService.GetTextTranslateResultAsync(input, settings, cancellationToken: cancellationToken).ConfigureAwait(false);
        sessionData.OutputText = response.FirstOrDefault()?.Text ?? string.Empty;
        sessionData.Time = DateTimeOffset.Now;
        return cancellationToken.IsCancellationRequested ? default : response.FirstOrDefault();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public bool IsTextExceedLimit(string text, ProviderType type)
        => text.Length >= GetProvider(type).GetMaxTextLength();

    /// <inheritdoc/>
    public long GetMaxTextLength(ProviderType type)
        => GetProvider(type).GetMaxTextLength();
}
