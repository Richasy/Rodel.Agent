// Copyright (c) Richasy. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace RodelChat.Models.Constants;

/// <summary>
/// 图像解析参数.
/// </summary>
[JsonConverter(typeof(ImageDetailTypeConverter))]
public enum ImageDetailType
{
    /// <summary>
    /// 自动.
    /// </summary>
    Auto,

    /// <summary>
    /// 低保真.
    /// </summary>
    Low,

    /// <summary>
    /// 高保真.
    /// </summary>
    High,
}

internal sealed class ImageDetailTypeConverter : JsonConverter<ImageDetailType>
{
    public override ImageDetailType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value?.ToLowerInvariant() switch
        {
            "auto" => ImageDetailType.Auto,
            "low" => ImageDetailType.Low,
            "high" => ImageDetailType.High,
            _ => throw new JsonException(),
        };
    }

    public override void Write(Utf8JsonWriter writer, ImageDetailType value, JsonSerializerOptions options)
    {
        var text = value switch
        {
            ImageDetailType.Auto => "auto",
            ImageDetailType.Low => "low",
            ImageDetailType.High => "high",
            _ => throw new JsonException(),
        };

        writer.WriteStringValue(text);
    }
}
