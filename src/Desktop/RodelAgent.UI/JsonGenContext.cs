// Copyright (c) Richasy. All rights reserved.

using Richasy.AgentKernel;
using RodelAgent.Models.Common;
using RodelAgent.Models.Feature;
using System.Text.Json.Serialization;

namespace RodelAgent.UI;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ChatClientConfiguration))]
[JsonSerializable(typeof(AudioClientConfiguration))]
[JsonSerializable(typeof(DrawClientConfiguration))]
[JsonSerializable(typeof(TranslateClientConfiguration))]
[JsonSerializable(typeof(List<SecretMeta>))]
[JsonSerializable(typeof(List<DrawMeta>))]
[JsonSerializable(typeof(List<AudioMeta>))]
[JsonSerializable(typeof(DrawRecord))]
[JsonSerializable(typeof(AudioRecord))]
[JsonSerializable(typeof(List<ChatInteropMessage>))]
[JsonSerializable(typeof(List<ChatWebInteropMessage>))]
[JsonSerializable(typeof(ChatInteropResources))]
[JsonSerializable(typeof(WebDataObject))]
[JsonSerializable(typeof(EditedInteropMessage))]
[JsonSerializable(typeof(List<ChatConversation>))]
internal sealed partial class JsonGenContext : JsonSerializerContext
{
}
