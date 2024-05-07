// Copyright (c) Rodel. All rights reserved.

using RodelChat.Interfaces.Client;
using RodelChat.Models.Chat;
using RodelChat.Models.Constants;

namespace RodelChat.Core;

/// <summary>
/// 聊天客户端.
/// </summary>
public sealed partial class ChatClient : IChatClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClient"/> class.
    /// </summary>
    public ChatClient(IChatProviderFactory providerFactory, List<object> plugins = null)
    {
        Sessions = new List<ChatSession>();
        _providerFactory = providerFactory;
        _plugins = plugins;
    }

    /// <inheritdoc/>
    public void LoadSessions(List<ChatSession> sessions)
    {
        Sessions.Clear();
        Sessions.AddRange(sessions);
    }

    /// <inheritdoc/>
    public void SetCustomModels(ProviderType type, List<ChatModel> models)
    {
        var provider = GetProvider(type);
        provider.SetCustomModels(models);
    }

    /// <inheritdoc/>
    public ChatSession CreateSession(ProviderType providerType, ChatParameters parameters = null, string? modelId = null)
    {
        var id = Guid.NewGuid().ToString("N");
        var session = ChatSession.CreateSession(id, parameters ?? ChatParameters.Create());
        session.Provider = providerType;
        if (!string.IsNullOrEmpty(modelId))
        {
            var model = FindModelInProvider(providerType, modelId);
            session.Model = model?.Id
                ?? throw new ArgumentException("Model not found.");
        }

        Sessions.Add(session);
        return session;
    }

    /// <inheritdoc/>
    public List<ChatModel> GetModels(ProviderType type)
        => GetProvider(type).GetModelList();

    /// <inheritdoc/>
    public async Task<ChatMessage> SendMessageAsync(
        string sessionId,
        ChatMessage? message = null,
        string? modelId = null,
        Action<string> streamingAction = default,
        CancellationToken cancellationToken = default)
    {
        var session = Sessions.FirstOrDefault(s => s.Id == sessionId)
            ?? throw new ArgumentException("Session not found.");

        ChatMessage response = default;

        try
        {
            ResetSessionModel(session, modelId);
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
