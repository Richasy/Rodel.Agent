// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;

namespace RodelAgent.Tools;

/// <summary>
/// Core tools.
/// </summary>
public static class CoreTools
{
    /// <summary>
    /// Tools that can be used.
    /// </summary>
    public static Dictionary<CoreToolType, List<AIFunction>> Tools { get; } = new Dictionary<CoreToolType, List<AIFunction>>
    {
        { CoreToolType.Weather, [AIFunctionFactory.Create(WeatherTool.GetWeather)] }
    };
}
