// Copyright (c) Rodel. All rights reserved.

using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Localization;
using RodelAgent.Interfaces;

namespace RodelAudio.Console;

/// <summary>
/// 字符串资源工具箱.
/// </summary>
public sealed class StringResourceToolkit : IStringResourceToolkit
{
    private readonly IStringLocalizer<AudioService> _localizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringResourceToolkit"/> class.
    /// </summary>
    public StringResourceToolkit(IStringLocalizer<AudioService> localizer) => _localizer = localizer;

    /// <inheritdoc/>
    public string GetString(string key)
    {
        var str = _localizer.GetString(key);
        if (str.ResourceNotFound)
        {
            Debug.WriteLine($"Resource not found: {key}: {str.SearchedLocation}");
        }

        return str.ResourceNotFound ? string.Empty : str.Value;
    }
}
