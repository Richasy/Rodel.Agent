// Copyright (c) Richasy. All rights reserved.

using Microsoft.SemanticKernel;
using System.ComponentModel;

#pragma warning disable CA1822 // 将成员标记为 static

namespace RodelAgent.Samples.Plugin;

/// <summary>
/// A sample plugin.
/// </summary>
[DisplayName("天气插件")]
[Description("该插件可以获取天气相关的信息")]
public sealed class WeatherPlugin
{
    /// <summary>
    /// Get the weather of a city.
    /// </summary>
    /// <param name="city">City name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Weather info.</returns>
    [KernelFunction]
    [Description("Get the weather of a city.")]
    public async Task<string> GetWeatherAsync(
        [Description("City name")] string city,
        CancellationToken cancellationToken = default)
    {
        // Mock data.
        await Task.Delay(1000, cancellationToken);
        var testText = "The weather of " + city + " is sunny. 26℃.";
        return testText;
    }
}
