// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.AI;
using Richasy.AgentKernel;
using Richasy.AgentKernel.Connectors.Ali.Models;
using Richasy.AgentKernel.Connectors.Baidu.Models;
using Richasy.AgentKernel.Connectors.Groq.Models;
using Richasy.AgentKernel.Connectors.OpenAI.Models;
using Richasy.AgentKernel.Connectors.Tencent.Models;
using Richasy.AgentKernel.Connectors.ZhiPu.Models;
using RodelChat.Models.Client;
using System.Text.Json.Serialization;

namespace RodelChat.Models;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(OpenAIChatOptions))]
[JsonSerializable(typeof(QwenChatOptions))]
[JsonSerializable(typeof(ErnieChatOptions))]
[JsonSerializable(typeof(GroqChatOptions))]
[JsonSerializable(typeof(HunyuanChatOptions))]
[JsonSerializable(typeof(ZhiPuChatOptions))]
[JsonSerializable(typeof(ChatOptions))]
[JsonSerializable(typeof(ChatSessionPreset))]
[JsonSerializable(typeof(List<Microsoft.Extensions.AI.ChatMessage>))]
[JsonSerializable(typeof(ChatProviderType))]
internal sealed partial class JsonGenContext : JsonSerializerContext
{
}
