// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using System.Text.Json.Serialization;

namespace Migration.V1;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ChatClientConfiguration))]
[JsonSerializable(typeof(AudioClientConfiguration))]
[JsonSerializable(typeof(DrawClientConfiguration))]
[JsonSerializable(typeof(TranslateClientConfiguration))]
internal sealed partial class JsonGenContext : JsonSerializerContext
{
}
