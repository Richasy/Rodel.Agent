// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Common;
using System.Text.Json.Serialization;

namespace RodelAgent.UI;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ChatClientConfiguration))]
[JsonSerializable(typeof(AudioClientConfiguration))]
[JsonSerializable(typeof(DrawClientConfiguration))]
[JsonSerializable(typeof(TranslateClientConfiguration))]
[JsonSerializable(typeof(List<SecretMeta>))]
[JsonSerializable(typeof(List<DrawMeta>))]
[JsonSerializable(typeof(DrawRecord))]
internal sealed partial class JsonGenContext : JsonSerializerContext
{
}
