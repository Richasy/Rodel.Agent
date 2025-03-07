// Copyright (c) Richasy. All rights reserved.

using SqlSugar;

namespace RodelAgent.Models.Common;

/// <summary>
/// 对话元数据.
/// </summary>
[SugarTable("Conversations")]
public class ChatConversationMeta : Metadata;

/// <summary>
/// 群组元数据.
/// </summary>
[SugarTable("Groups")]
public class ChatGroupMeta : Metadata;