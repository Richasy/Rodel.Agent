// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.AgentKernel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RodelChat.Models.Client;

/// <summary>
/// 对话模型预设.
/// </summary>
[JsonConverter(typeof(ChatSessionPresetConverter))]
public class ChatSessionPreset
{
    /// <summary>
    /// 会话标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// 预设名称.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonRequired]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 获取或设置会话选项.
    /// </summary>
    [JsonPropertyName("options")]
    public ChatOptions? Options { get; set; }

    /// <summary>
    /// 最大会话轮次.
    /// </summary>
    /// <remarks>
    /// <para>用户提问，AI 回答，一问一答称为一轮.</para>
    /// <para>默认为 <c>0</c>，表示不限轮次，直到达到预设的最大上下文窗口.</para>
    /// <para>当超过最大轮次后，之前的记录虽然保留，但不会作为上下文发送，将重新开始新一轮对话.</para>
    /// </remarks>
    [JsonPropertyName("max_rounds")]
    public int MaxRounds { get; set; }

    /// <summary>
    /// 是否使用流输出.
    /// </summary>
    /// <remarks>
    /// <para>即通过 SSE（Server-Sent Events）实时接收服务器回传的数据.</para>
    /// <para>理论上响应速度更快，反应在 UI 上就会有打字机的效果.</para>
    /// </remarks>
    [JsonPropertyName("stream")]
    public bool UseStreamOutput { get; set; } = true;

    /// <summary>
    /// 服务商.
    /// </summary>
    [JsonPropertyName("provider")]
    public ChatProviderType Provider { get; set; }

    /// <summary>
    /// 指定的模型.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    /// 获取或设置系统指令.
    /// </summary>
    /// <remarks>
    /// <para>系统指令是特殊消息，它会引导 AI 的行为。它不受轮次限制，将始终和上下文一起发送给模型.</para>
    /// </remarks>
    [JsonPropertyName("system")]
    public string? SystemInstruction { get; set; }

    /// <summary>
    /// 获取或设置历史记录.
    /// </summary>
    [JsonPropertyName("history")]
    public List<Microsoft.Extensions.AI.ChatMessage>? History { get; set; }

    /// <summary>
    /// 需要过滤掉的字符.
    /// </summary>
    [JsonPropertyName("filter_chars")]
    public IList<string>? FilterCharacters { get; set; }

    /// <summary>
    /// 支持的插件.
    /// </summary>
    [JsonPropertyName("plugins")]
    public IList<string>? Plugins { get; set; }

    /// <summary>
    /// 表情头像.
    /// </summary>
    [JsonPropertyName("emoji")]
    public string? Emoji { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatSessionPreset preset && Id == preset.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}

internal sealed class ChatSessionPresetConverter : JsonConverter<ChatSessionPreset>
{
    public override ChatSessionPreset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var preset = new ChatSessionPreset();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return preset;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "id":
                        preset.Id = reader.GetString()!;
                        break;
                    case "name":
                        preset.Name = reader.GetString()!;
                        break;
                    case "options":
                        preset.Options = DeserializeOptions(ref reader, preset.Provider, options);
                        break;
                    case "max_rounds":
                        preset.MaxRounds = reader.GetInt32();
                        break;
                    case "stream":
                        preset.UseStreamOutput = reader.GetBoolean();
                        break;
                    case "provider":
                        preset.Provider = JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.ChatProviderType);
                        break;
                    case "model":
                        preset.Model = reader.GetString();
                        break;
                    case "system":
                        preset.SystemInstruction = reader.GetString();
                        break;
                    case "history":
                        preset.History = JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.ListChatMessage);
                        break;
                    case "filter_chars":
                        preset.FilterCharacters = JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.IListString);
                        break;
                    case "plugins":
                        preset.Plugins = JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.IListString);
                        break;
                    case "emoji":
                        preset.Emoji = reader.GetString();
                        break;
                    default:
                        throw new JsonException($"Unknown property: {propertyName}");
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ChatSessionPreset value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("id", value.Id);
        writer.WriteString("name", value.Name);

        if (value.Options != null)
        {
            writer.WritePropertyName("options");
            SerializeOptions(writer, value.Options, value.Provider, options);
        }

        writer.WriteNumber("max_rounds", value.MaxRounds);
        writer.WriteBoolean("stream", value.UseStreamOutput);
        writer.WritePropertyName("provider");
        JsonSerializer.Serialize(writer, value.Provider, JsonGenContext.Default.ChatProviderType);

        if (value.Model != null)
        {
            writer.WriteString("model", value.Model);
        }

        if (value.SystemInstruction != null)
        {
            writer.WriteString("system", value.SystemInstruction);
        }

        if (value.History != null)
        {
            writer.WritePropertyName("history");
            JsonSerializer.Serialize(writer, value.History, JsonGenContext.Default.ListChatMessage);
        }

        if (value.FilterCharacters != null)
        {
            writer.WritePropertyName("filter_chars");
            JsonSerializer.Serialize(writer, value.FilterCharacters, JsonGenContext.Default.IListString);
        }

        if (value.Plugins != null)
        {
            writer.WritePropertyName("plugins");
            JsonSerializer.Serialize(writer, value.Plugins, JsonGenContext.Default.IListString);
        }

        if (value.Emoji != null)
        {
            writer.WriteString("emoji", value.Emoji);
        }

        writer.WriteEndObject();
    }

    private static ChatOptions? DeserializeOptions(ref Utf8JsonReader reader, ChatProviderType provider, JsonSerializerOptions options)
    {
        return provider switch
        {
            ChatProviderType.OpenAI => JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.OpenAIChatOptions),
            ChatProviderType.Qwen => JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.QwenChatOptions),
            ChatProviderType.Ernie => JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.ErnieChatOptions),
            ChatProviderType.Groq => JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.GroqChatOptions),
            ChatProviderType.Hunyuan => JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.HunyuanChatOptions),
            ChatProviderType.ZhiPu => JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.ZhiPuChatOptions),
            _ => JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.ChatOptions),
        };
    }

    private static void SerializeOptions(Utf8JsonWriter writer, ChatOptions options, ChatProviderType provider, JsonSerializerOptions jsonOptions)
    {
        switch (provider)
        {
            case ChatProviderType.OpenAI:
                JsonSerializer.Serialize(writer, options, JsonGenContext.Default.OpenAIChatOptions);
                break;
            case ChatProviderType.Qwen:
                JsonSerializer.Serialize(writer, options, JsonGenContext.Default.QwenChatOptions);
                break;
            case ChatProviderType.Ernie:
                JsonSerializer.Serialize(writer, options, JsonGenContext.Default.ErnieChatOptions);
                break;
            case ChatProviderType.Groq:
                JsonSerializer.Serialize(writer, options, JsonGenContext.Default.GroqChatOptions);
                break;
            case ChatProviderType.Hunyuan:
                JsonSerializer.Serialize(writer, options, JsonGenContext.Default.HunyuanChatOptions);
                break;
            case ChatProviderType.ZhiPu:
                JsonSerializer.Serialize(writer, options, JsonGenContext.Default.ZhiPuChatOptions);
                break;
            default:
                JsonSerializer.Serialize(writer, options, JsonGenContext.Default.ChatOptions);
                break;
        }
    }
}
