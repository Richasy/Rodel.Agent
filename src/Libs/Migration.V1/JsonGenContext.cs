// Copyright (c) Rodel. All rights reserved.

using System.Text.Json.Serialization;
using Richasy.AgentKernel;

namespace Migration.V1;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ChatClientConfiguration))]
[JsonSerializable(typeof(AudioClientConfiguration))]
[JsonSerializable(typeof(DrawClientConfiguration))]
[JsonSerializable(typeof(TranslateClientConfiguration))]
internal sealed partial class JsonGenContext : JsonSerializerContext
{
}
