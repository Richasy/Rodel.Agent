// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.Logging;
using RodelTranslate.Interfaces.Client;

namespace RodelTranslate.Core;

/// <summary>
/// 翻译客户端.
/// </summary>
public sealed partial class TranslateClient
{
    private readonly ITranslateProviderFactory _providerFactory;
    private readonly ITranslateParametersFactory _parameterFactory;
    private readonly ILogger<TranslateClient> _logger;

    private bool _disposedValue;
}
