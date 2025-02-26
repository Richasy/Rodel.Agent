// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using System.Text.Json.Serialization;

namespace RodelAgent.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ChatResponseFormat))]
[JsonSerializable(typeof(List<string>))]
internal sealed partial class JsonGenContext : JsonSerializerContext
{
}
