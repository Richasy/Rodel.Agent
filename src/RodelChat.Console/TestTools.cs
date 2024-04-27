// Copyright (c) Rodel. All rights reserved.

namespace RodelChat.Console;

internal static class TestTools
{
    public static string GetWeather(string city)
    {
        return """
            {
                "weather": "Sunny",
                "temperature": "25°C"
            }
            """;
    }

    public static string GetBiliBiliHotSearch()
    {
        return """
            1. 阿狸
            2. 王者荣耀
            3. 三国杀
            4. 武学
            5. 三国演义
            """;
    }
}
