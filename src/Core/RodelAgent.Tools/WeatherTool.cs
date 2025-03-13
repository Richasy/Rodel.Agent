using System.ComponentModel;

namespace RodelAgent.Tools;

internal sealed class WeatherTool
{
    [Description("Get the weather of a city.")]
    public static string GetWeather([Description("city code")]string city)
    {
        return $"The weather in {city} is sunny.";
    }
}
