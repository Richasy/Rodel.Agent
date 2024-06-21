// Copyright (c) Rodel. All rights reserved.

using Microsoft.Extensions.Logging;
using RodelAudio.Interfaces.Client;

namespace RodelAudio.Core;

/// <summary>
/// 音频客户端的属性和字段.
/// </summary>
public sealed partial class AudioClient
{
    private readonly IAudioProviderFactory _providerFactory;
    private readonly IAudioParametersFactory _parameterFactory;
    private readonly ILogger<AudioClient> _logger;
    private bool _disposedValue;
}

