// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using System.Text.Json.Serialization;

namespace RodelCommit;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ChatClientConfiguration))]
[JsonSerializable(typeof(CommitConfiguration))]
[JsonSerializable(typeof(ChatProviderType))]
internal sealed partial class JsonGenContext : JsonSerializerContext
{
}
