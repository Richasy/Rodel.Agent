﻿// Copyright (c) Richasy. All rights reserved.

using RodelDraw.Core.Providers;
using RodelDraw.Interfaces.Client;
using RodelDraw.Models.Constants;

namespace RodelAgent.UI.Extensions;

/// <summary>
/// 绘图参数工厂.
/// </summary>
public sealed class DrawParametersFactory : IDrawParametersFactory
{
    /// <inheritdoc/>
    public BaseFieldParameters CreateDrawParameters(ProviderType provider)
    {
        return provider switch
        {
            ProviderType.OpenAI => new OpenAIProvider.OpenAIDrawParameters(),
            ProviderType.AzureOpenAI => new AzureOpenAIProvider.AzureOpenAIDrawParameters(),
            _ => default,
        };
    }
}
