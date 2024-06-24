using System.Text.Json.Serialization;

internal sealed class EmojiMetadata
{
    [JsonPropertyName("cldr")]
    public string? CommonName { get; set; }

    [JsonPropertyName("fromVersion")]
    public string? FromVersion { get; set; }

    [JsonPropertyName("glyph")]
    public string? Glyph { get; set; }

    [JsonPropertyName("glyphAsUtfInEmoticons")]
    public string[]? GlyphAsUtfInEmoticons { get; set; }

    [JsonPropertyName("group")]
    public string? Group { get; set; }

    [JsonPropertyName("keywords")]
    public string[]? Keywords { get; set; }

    [JsonPropertyName("mappedToEmoticons")]
    public string[]? MappedToEmoticons { get; set; }

    [JsonPropertyName("tts")]
    public string? TTS { get; set; }

    [JsonPropertyName("unicode")]
    public string? Unicode { get; set; }

    public string? FolderName { get; set; }
}

