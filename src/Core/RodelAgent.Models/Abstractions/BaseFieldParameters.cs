// Copyright (c) Rodel. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using RodelAgent.Models.Constants;

namespace RodelAgent.Models.Abstractions;

/// <summary>
/// 字段参数基类.
/// </summary>
[JsonConverter(typeof(BaseFieldParametersConverter))]
public class BaseFieldParameters
{
    /// <summary>
    /// 字段键值对.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, object> Fields { get; private set; }

    /// <summary>
    /// 将参数内的属性转换为字典.
    /// </summary>
    /// <returns>属性键值对.</returns>
    public virtual Dictionary<string, object> ToDictionary()
    {
        var type = GetType();
        var properties = type.GetProperties();
        var dictionary = new Dictionary<string, object>();

        foreach (var property in properties)
        {
            if (Attribute.GetCustomAttribute(property, typeof(BaseFieldAttribute)) is BaseFieldAttribute attribute)
            {
                var ft = attribute.FieldType;
                var v = ft switch
                {
                    ParameterFieldType.Boolean => property.GetValue(this),
                    ParameterFieldType.Selection => GetSelectionValue(attribute, property),
                    ParameterFieldType.Keywords => GetKeywords(attribute, property),
                    ParameterFieldType.RangeFloat => GetRangeFloatValue(attribute, property),
                    ParameterFieldType.RangeInt => GetRangeIntValue(attribute, property),
                    ParameterFieldType.RangeLong => GetRangeLongValue(attribute, property),
                    ParameterFieldType.Text => property.GetValue(this),
                    _ => throw new ArgumentOutOfRangeException(nameof(ft), ft, null)
                };

                if (v is null)
                {
                    continue;
                }

                dictionary[GetPropertyName(property)] = v;
            }
        }

        Fields = dictionary;
        return Fields;
    }

    /// <summary>
    /// 设置属性值.
    /// </summary>
    /// <param name="dictionary">字典.</param>
    public void SetDictionary(Dictionary<string, object> dictionary)
    {
        Fields = dictionary;
        var type = GetType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var jsonAttr = Attribute.GetCustomAttribute(property, typeof(JsonPropertyNameAttribute));
            var name = jsonAttr != null ? ((JsonPropertyNameAttribute)jsonAttr).Name : property.Name;
            if (dictionary.ContainsKey(name))
            {
                if (Attribute.GetCustomAttribute(property, typeof(BaseFieldAttribute)) is BaseFieldAttribute attribute)
                {
                    var ft = attribute.FieldType;
                    var value = dictionary[name];
                    switch (ft)
                    {
                        case ParameterFieldType.RangeFloat:
                            SetRangeFloatValue(attribute, property, value);
                            break;
                        case ParameterFieldType.RangeInt:
                            SetRangeIntValue(attribute, property, value);
                            break;
                        case ParameterFieldType.RangeLong:
                            SetRangeLongValue(attribute, property, value);
                            break;
                        case ParameterFieldType.Text:
                            property.SetValue(this, value);
                            break;
                        case ParameterFieldType.Keywords:
                            SetKeywords(attribute, property, value);
                            break;
                        case ParameterFieldType.Selection:
                            SetSelectionValue(attribute, property, value);
                            break;
                        case ParameterFieldType.Boolean:
                            property.SetValue(this, value);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取属性值.
    /// </summary>
    /// <typeparam name="T">属性值类型.</typeparam>
    /// <param name="propertyName">属性名称.</param>
    /// <returns>值.</returns>
    public T? GetValueOrDefault<T>(string propertyName)
    {
        var type = GetType();
        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            var jsonAttr = Attribute.GetCustomAttribute(property, typeof(JsonPropertyNameAttribute));
            var jsonName = jsonAttr != null ? ((JsonPropertyNameAttribute)jsonAttr).Name : string.Empty;
            if (propertyName == jsonName || propertyName == property.Name)
            {
                var v = property.GetValue(this);
                return v is not null ? (T)property.GetValue(this) : default;
            }
        }

        throw new ArgumentException("The property does not exist.");
    }

    private static string GetPropertyName(PropertyInfo property)
    {
        var nameAttr = property.GetCustomAttribute<JsonPropertyNameAttribute>();
        return nameAttr?.Name ?? property.Name;
    }

    private void SetSelectionValue(BaseFieldAttribute attribute, PropertyInfo property, object value)
    {
        var selectionAttr = attribute as SelectionFieldAttribute
            ?? throw new ArgumentException("The property is not a selection field.");

        if (value is string str && selectionAttr.Options.Contains(str))
        {
            property.SetValue(this, str);
        }
        else
        {
            throw new ArgumentException("The value is not in the options.");
        }
    }

    private string? GetSelectionValue(BaseFieldAttribute attribute, PropertyInfo property)
    {
        var selectionAttr = attribute as SelectionFieldAttribute
            ?? throw new ArgumentException("The property is not a selection field.");

        var value = property.GetValue(this);
        return value is string str && !selectionAttr.Options.Contains(str)
            ? throw new ArgumentException("The value is not in the options.")
            : value?.ToString();
    }

    private void SetKeywords(BaseFieldAttribute attribute, PropertyInfo property, object value)
    {
        var keywordsAttr = attribute as KeywordsFieldAttribute
            ?? throw new ArgumentException("The property is not a keywords field.");

        if (value is string[] keywords)
        {
            property.SetValue(this, keywords);
        }
        else if (value is IEnumerable<string> enumerable)
        {
            property.SetValue(this, enumerable.Distinct().ToArray());
        }
        else
        {
            throw new ArgumentException("The value is not a string array or enumerable of strings.");
        }
    }

    private string[]? GetKeywords(BaseFieldAttribute attribute, PropertyInfo property)
    {
        _ = attribute as KeywordsFieldAttribute
            ?? throw new ArgumentException("The property is not a keywords field.");

        var value = property.GetValue(this);
        return value is string[] keywords
            ? keywords.Distinct().ToArray()
            : value is IEnumerable<string> enumerable
                ? enumerable.Distinct().ToArray()
                : default;
    }

    private void SetRangeFloatValue(BaseFieldAttribute attribute, PropertyInfo property, object value)
    {
        var rangeFloatAttr = attribute as RangeFloatFieldAttribute
            ?? throw new ArgumentException("The property is not a range float field.");

        if (value is double d && d >= rangeFloatAttr.Minimum && d <= rangeFloatAttr.Maximum)
        {
            property.SetValue(this, d);
        }
        else
        {
            throw new ArgumentException("The value is not in the range.");
        }
    }

    private double GetRangeFloatValue(BaseFieldAttribute attribute, PropertyInfo property)
    {
        var rangeFloatAttr = attribute as RangeFloatFieldAttribute
            ?? throw new ArgumentException("The property is not a range float field.");

        var value = property.GetValue(this);
        return value is double d && d >= rangeFloatAttr.Minimum && d <= rangeFloatAttr.Maximum
                ? d
                : default;
    }

    private void SetRangeIntValue(BaseFieldAttribute attribute, PropertyInfo property, object value)
    {
        var rangeIntAttr = attribute as RangeIntFieldAttribute
            ?? throw new ArgumentException("The property is not a range int field.");

        var i = Convert.ToInt32(value);
        if (i >= rangeIntAttr.Minimum && i <= rangeIntAttr.Maximum)
        {
            property.SetValue(this, i);
        }
        else
        {
            throw new ArgumentException("The value is not in the range.");
        }
    }

    private int GetRangeIntValue(BaseFieldAttribute attribute, PropertyInfo property)
    {
        var rangeIntAttr = attribute as RangeIntFieldAttribute
            ?? throw new ArgumentException("The property is not a range int field.");

        var value = property.GetValue(this);
        return value is int i && i >= rangeIntAttr.Minimum && i <= rangeIntAttr.Maximum
                ? i
                : default;
    }

    private void SetRangeLongValue(BaseFieldAttribute attribute, PropertyInfo property, object value)
    {
        var rangeLongAttr = attribute as RangeLongFieldAttribute
            ?? throw new ArgumentException("The property is not a range long field.");

        if (value is long i && i >= rangeLongAttr.Minimum && i <= rangeLongAttr.Maximum)
        {
            property.SetValue(this, i);
        }
        else
        {
            throw new ArgumentException("The value is not in the range.");
        }
    }

    private long GetRangeLongValue(BaseFieldAttribute attribute, PropertyInfo property)
    {
        var rangeLongAttr = attribute as RangeLongFieldAttribute
            ?? throw new ArgumentException("The property is not a range long field.");

        var value = property.GetValue(this);
        return value is long i && i >= rangeLongAttr.Minimum && i <= rangeLongAttr.Maximum
            ? i
            : default;
    }
}

/// <summary>
/// 字段参数转换器.
/// </summary>
public class BaseFieldParametersConverter : JsonConverter<BaseFieldParameters>
{
    /// <inheritdoc/>
    public override BaseFieldParameters? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var value = (BaseFieldParameters)Activator.CreateInstance(typeToConvert);
        var output = new Dictionary<string, object>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            var key = reader.GetString();
            reader.Read();
            output[key] = reader.TokenType switch
            {
                JsonTokenType.Number => reader.GetDouble(),
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.True or JsonTokenType.False => reader.GetBoolean(),
                JsonTokenType.StartArray => ParseStringArray(ref reader),
                _ => throw new JsonException(),
            };
        }

        value.SetDictionary(output);
        return value;

        string[] ParseStringArray(ref Utf8JsonReader reader)
        {
            var list = new List<string>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                list.Add(reader.GetString());
            }

            return [.. list];
        }
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, BaseFieldParameters value, JsonSerializerOptions options)
    {
        var dict = value.ToDictionary();
        JsonSerializer.Serialize(writer, dict, options);
    }
}
