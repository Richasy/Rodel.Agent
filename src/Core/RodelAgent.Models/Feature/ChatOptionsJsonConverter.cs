// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RodelAgent.Models.Feature;

/// <summary>
/// 聊天选项转换器.
/// </summary>
public sealed class ChatOptionsJsonConverter : JsonConverter<ChatOptions>
{
    /// <inheritdoc/>
    public override ChatOptions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var chatOptions = new ChatOptions();
        var additionalProperties = new Dictionary<string, object?>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                chatOptions.AdditionalProperties = new AdditionalPropertiesDictionary(additionalProperties);
                return chatOptions;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString() ?? throw new JsonException();

            reader.Read();

            switch (propertyName)
            {
                case nameof(ChatOptions.Temperature):
                    chatOptions.Temperature = reader.TokenType == JsonTokenType.Null ? null : reader.GetSingle();
                    break;
                case nameof(ChatOptions.MaxOutputTokens):
                    chatOptions.MaxOutputTokens = reader.TokenType == JsonTokenType.Null ? null : reader.GetInt32();
                    break;
                case nameof(ChatOptions.TopP):
                    chatOptions.TopP = reader.TokenType == JsonTokenType.Null ? null : reader.GetSingle();
                    break;
                case nameof(ChatOptions.TopK):
                    chatOptions.TopK = reader.TokenType == JsonTokenType.Null ? null : reader.GetInt32();
                    break;
                case nameof(ChatOptions.FrequencyPenalty):
                    chatOptions.FrequencyPenalty = reader.TokenType == JsonTokenType.Null ? null : reader.GetSingle();
                    break;
                case nameof(ChatOptions.PresencePenalty):
                    chatOptions.PresencePenalty = reader.TokenType == JsonTokenType.Null ? null : reader.GetSingle();
                    break;
                case nameof(ChatOptions.Seed):
                    chatOptions.Seed = reader.TokenType == JsonTokenType.Null ? null : reader.GetInt64();
                    break;
                case nameof(ChatOptions.ResponseFormat):
                    chatOptions.ResponseFormat = JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.ChatResponseFormat);
                    break;
                case nameof(ChatOptions.ModelId):
                    chatOptions.ModelId = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                    break;
                case nameof(ChatOptions.StopSequences):
                    chatOptions.StopSequences = JsonSerializer.Deserialize(ref reader, JsonGenContext.Default.ListString);
                    break;
                default:
                    // Handle additional properties
                    if (reader.TokenType == JsonTokenType.Null)
                    {
                        additionalProperties[propertyName] = null;
                    }
                    else
                    {
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.Number:
                                if (reader.TryGetInt32(out var intValue))
                                {
                                    additionalProperties[propertyName] = intValue;
                                }
                                else if (reader.TryGetInt64(out var longValue))
                                {
                                    additionalProperties[propertyName] = longValue;
                                }
                                else if (reader.TryGetDouble(out var doubleValue))
                                {
                                    additionalProperties[propertyName] = doubleValue;
                                }
                                else
                                {
                                    throw new JsonException($"Unsupported number type for property {propertyName}");
                                }

                                break;
                            case JsonTokenType.String:
                                additionalProperties[propertyName] = reader.GetString();
                                break;
                            case JsonTokenType.True:
                            case JsonTokenType.False:
                                additionalProperties[propertyName] = reader.GetBoolean();
                                break;
                            default:
                                throw new JsonException($"Unsupported token type {reader.TokenType} for property {propertyName}");
                        }
                    }

                    break;
            }
        }

        throw new JsonException();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ChatOptions value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.Temperature.HasValue)
        {
            writer.WriteNumber(nameof(ChatOptions.Temperature), value.Temperature.Value);
        }
        else
        {
            writer.WriteNull(nameof(ChatOptions.Temperature));
        }

        if (value.MaxOutputTokens.HasValue)
        {
            writer.WriteNumber(nameof(ChatOptions.MaxOutputTokens), value.MaxOutputTokens.Value);
        }
        else
        {
            writer.WriteNull(nameof(ChatOptions.MaxOutputTokens));
        }

        if (value.TopP.HasValue)
        {
            writer.WriteNumber(nameof(ChatOptions.TopP), value.TopP.Value);
        }
        else
        {
            writer.WriteNull(nameof(ChatOptions.TopP));
        }

        if (value.TopK.HasValue)
        {
            writer.WriteNumber(nameof(ChatOptions.TopK), value.TopK.Value);
        }
        else
        {
            writer.WriteNull(nameof(ChatOptions.TopK));
        }

        if (value.FrequencyPenalty.HasValue)
        {
            writer.WriteNumber(nameof(ChatOptions.FrequencyPenalty), value.FrequencyPenalty.Value);
        }
        else
        {
            writer.WriteNull(nameof(ChatOptions.FrequencyPenalty));
        }

        if (value.PresencePenalty.HasValue)
        {
            writer.WriteNumber(nameof(ChatOptions.PresencePenalty), value.PresencePenalty.Value);
        }
        else
        {
            writer.WriteNull(nameof(ChatOptions.PresencePenalty));
        }

        if (value.Seed.HasValue)
        {
            writer.WriteNumber(nameof(ChatOptions.Seed), value.Seed.Value);
        }
        else
        {
            writer.WriteNull(nameof(ChatOptions.Seed));
        }

        if (value.ResponseFormat != null)
        {
            writer.WritePropertyName(nameof(ChatOptions.ResponseFormat));
            JsonSerializer.Serialize(writer, value.ResponseFormat, JsonGenContext.Default.ChatResponseFormat);
        }
        else
        {
            writer.WriteNull(nameof(ChatOptions.ResponseFormat));
        }

        if (value.ModelId != null)
        {
            writer.WriteString(nameof(ChatOptions.ModelId), value.ModelId);
        }
        else
        {
            writer.WriteNull(nameof(ChatOptions.ModelId));
        }

        if (value.StopSequences != null)
        {
            writer.WritePropertyName(nameof(ChatOptions.StopSequences));
            JsonSerializer.Serialize(writer, value.StopSequences, JsonGenContext.Default.ListString);
        }
        else
        {
            writer.WriteNull(nameof(ChatOptions.StopSequences));
        }

        if (value.AdditionalProperties != null)
        {
            foreach (var kvp in value.AdditionalProperties)
            {
                writer.WritePropertyName(kvp.Key);
                if (kvp.Value == null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    switch (kvp.Value)
                    {
                        case int intValue:
                            writer.WriteNumberValue(intValue);
                            break;
                        case long longValue:
                            writer.WriteNumberValue(longValue);
                            break;
                        case double doubleValue:
                            writer.WriteNumberValue(doubleValue);
                            break;
                        case string stringValue:
                            writer.WriteStringValue(stringValue);
                            break;
                        case bool boolValue:
                            writer.WriteBooleanValue(boolValue);
                            break;
                        default:
                            throw new JsonException($"Unsupported type {kvp.Value.GetType()} for additional property {kvp.Key}");
                    }
                }
            }
        }

        writer.WriteEndObject();
    }
}
