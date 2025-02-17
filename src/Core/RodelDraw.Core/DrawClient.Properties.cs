// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.Logging;
using RodelDraw.Interfaces.Client;

namespace RodelDraw.Core;

/// <summary>
/// 绘图客户端的属性和字段.
/// </summary>
public sealed partial class DrawClient
{
    private readonly IDrawProviderFactory _providerFactory;
    private readonly IDrawParametersFactory _parameterFactory;
    private readonly ILogger<DrawClient> _logger;
    private bool _disposedValue;
}
