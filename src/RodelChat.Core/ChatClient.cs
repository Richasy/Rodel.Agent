// Copyright (c) Rodel. All rights reserved.

using RodelChat.Core.Models.Chat;
using RodelChat.Core.Models.Constants;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端.
/// </summary>
public sealed partial class ChatClient : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClient"/> class.
    /// </summary>
    public ChatClient(List<object> plugins = null)
    {
        Sessions = new List<ChatSession>();
        _plugins = plugins;
    }

    /// <summary>
    /// 加载会话列表.
    /// </summary>
    /// <param name="sessions">会话列表.</param>
    public void LoadSessions(List<ChatSession> sessions)
    {
        Sessions.Clear();
        Sessions.AddRange(sessions);
    }

    /// <summary>
    /// 设置默认服务商.
    /// </summary>
    /// <param name="provider">服务商.</param>
    public void SetDefaultProvider(ProviderType provider)
        => _defaultProvider = provider;

    /// <summary>
    /// 设置服务商的自定义模型列表.
    /// </summary>
    /// <param name="type">提供商类型.</param>
    /// <param name="models">模型列表.</param>
    /// <exception cref="ArgumentException">服务商尚未初始化.</exception>
    public void SetCustomModels(ProviderType type, List<ChatModel> models)
    {
        var provider = GetProvider(type)
            ?? throw new ArgumentException("Provider not initialized.");
        provider.CustomModels = models;
    }

    /// <summary>
    /// 创建新会话.
    /// </summary>
    /// <param name="parameters">会话参数.</param>
    /// <returns><see cref="ChatSession"/>.</returns>
    public ChatSession CreateSession(ChatParameters parameters = null, ProviderType? providerType = null, string? modelId = null)
    {
        var id = Guid.NewGuid().ToString("N");
        var session = ChatSession.CreateSession(id, parameters ?? ChatParameters.Create());
        session.Provider = providerType ?? _defaultProvider;
        if (providerType.HasValue && !string.IsNullOrEmpty(modelId))
        {
            var model = FindModelInProvider(providerType.Value, modelId);
            if (model != null)
            {
                session.Model = model.Id;
            }
        }

        Sessions.Add(session);
        return session;
    }

    /// <summary>
    /// 获取服务端模型列表.
    /// </summary>
    /// <param name="type">供应商类型.</param>
    /// <returns>模型列表.</returns>
    public List<ChatModel> GetServerModels(ProviderType type)
    {
        var provider = GetProvider(type);
        return provider?.ServerModels ?? new List<ChatModel>();
    }

    /// <summary>
    /// 发送消息.
    /// </summary>
    /// <param name="sessionId">会话标识符.</param>
    /// <param name="message">用户输入.</param>
    /// <param name="streamingAction">流式输出的处理.</param>
    /// <param name="cancellationToken">终止令牌.</param>
    /// <returns><see cref="ChatMessage"/>.</returns>
    /// <exception cref="ArgumentException">会话不存在.</exception>
    public async Task<ChatMessage> SendMessageAsync(
        string sessionId,
        ChatMessage? message = null,
        Action<string> streamingAction = default,
        CancellationToken cancellationToken = default)
    {
        var session = Sessions.FirstOrDefault(s => s.Id == sessionId)
            ?? throw new ArgumentException("Session not found.");

        ChatMessage response = default;

        try
        {
            var model = FindModelInProvider(session.Provider!.Value, session.Model);
            if (message.Content.Any(p => p.Type == ChatContentType.ImageUrl) && !model.IsSupportVision)
            {
                return ChatMessage.CreateClientMessage(ClientMessageType.ModelNotSupportImage, string.Empty);
            }

            var kernel = FindKernelProvider(session.Provider!.Value, session.Model);
            if (kernel == null)
            {
                return ChatMessage.CreateClientMessage(ClientMessageType.ProviderNotSupported, string.Empty);
            }

            response = await KernelSendMessageAsync(kernel, session, message, streamingAction, cancellationToken);
            response.Time = DateTimeOffset.Now;
            session.History.Add(response);

            return response;
        }
        catch (TaskCanceledException)
        {
            return ChatMessage.CreateClientMessage(ClientMessageType.GenerateCancelled, string.Empty);
        }
        catch (Exception ex)
        {
            return ChatMessage.CreateClientMessage(ClientMessageType.GeneralFailed, ex.Message);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
