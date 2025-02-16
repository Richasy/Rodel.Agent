// Copyright (c) Rodel. All rights reserved.

using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RodelAgent.Models.Abstractions;
using RodelChat.Interfaces.Client;
using RodelChat.Models.Client;
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
    public ChatClient(
        IChatProviderFactory providerFactory,
        IChatParametersFactory parameterFactory,
        ILogger<ChatClient> logger)
    {
        Sessions = new List<ChatSession>();
        Groups = new List<ChatGroup>();
        _logger = logger;
        _providerFactory = providerFactory;
        _parameterFactory = parameterFactory;
    }

    /// <inheritdoc/>
    public void LoadChatSessions(List<ChatSession> sessions)
    {
        Sessions.Clear();
        Sessions.AddRange(sessions);
    }

    /// <inheritdoc/>
    public void LoadGroupSessions(List<ChatGroup> groups)
    {
        Groups.Clear();
        Groups.AddRange(groups);
    }

    /// <inheritdoc/>
    public List<ChatModel> GetPredefinedModels(ProviderType type)
    {
        var preType = typeof(PredefinedModels);
        var properties = preType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        foreach (var prop in properties)
        {
            if (prop.Name.StartsWith(type.ToString()))
            {
                return prop.GetValue(default) as List<ChatModel>
                    ?? throw new ArgumentException("Predefined models not found.");
            }
        }

        return new List<ChatModel>();
    }

    /// <inheritdoc/>
    public ChatSession CreateSession(ProviderType providerType, BaseFieldParameters parameters = null, string? modelId = null)
    {
        var id = Guid.NewGuid().ToString("N");
        parameters = GetChatParameters(providerType, parameters);
        var session = ChatSession.CreateSession(id, parameters);
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

    /// <summary>
    /// 创建会话.
    /// </summary>
    /// <param name="preset">预设会话.</param>
    /// <returns>会话对象.</returns>
    /// <exception cref="ArgumentException">预设不合规.</exception>
    public ChatSession CreateSession(ChatSessionPresetOld preset)
    {
        var id = Guid.NewGuid().ToString("N");

        // 需要重新序列化以创建新的对象.
        var presetJson = JsonSerializer.Serialize(preset);
        var newPreset = JsonSerializer.Deserialize<ChatSessionPresetOld>(presetJson);
        newPreset.Parameters = GetChatParameters(preset.Provider, preset.Parameters);
        var session = ChatSession.CreateSession(id, newPreset);

        session.Parameters = GetChatParameters(preset.Provider, preset.Parameters);
        var model = string.IsNullOrEmpty(preset.Model)
            ? GetModels(preset.Provider).FirstOrDefault()
            : FindModelInProvider(preset.Provider, preset.Model);

        session.Model = model != null
            ? model.Id
            : throw new ArgumentException("Model not found.");

        Sessions.Add(session);
        return session;
    }

    /// <inheritdoc/>
    public ChatGroup CreateSession(ChatGroupPreset preset)
    {
        var id = Guid.NewGuid().ToString("N");

        var presetJson = JsonSerializer.Serialize(preset);
        var newPreset = JsonSerializer.Deserialize<ChatGroupPreset>(presetJson);
        var session = ChatGroup.CreateGroup(id, newPreset);
        Groups.Add(session);
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
        List<KernelPlugin>? plugins = default,
        CancellationToken cancellationToken = default)
    {
        var session = Sessions.FirstOrDefault(s => s.Id == sessionId)
            ?? throw new ArgumentException("Session not found.");

        ChatMessage response = default;
        ResetSessionModel(session, modelId);
        var model = FindModelInProvider(session.Provider, session.Model);
        if (message.Content.Any(p => p.Type == ChatContentType.ImageUrl) && !model.IsSupportVision)
        {
            return ChatMessage.CreateClientMessage(ClientMessageType.ModelNotSupportImage, string.Empty);
        }

        var kernel = FindKernelProvider(session.Provider, session.Model);
        if (kernel == null)
        {
            return ChatMessage.CreateClientMessage(ClientMessageType.ProviderNotSupported, string.Empty);
        }

        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        try
        {
            kernel.Plugins.Clear();
            if (plugins != null && plugins.Count > 0 && model.IsSupportTool)
            {
                kernel.Plugins.AddRange(plugins);
            }

            response = await KernelSendMessageAsync(kernel, session, message, streamingAction, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                return ChatMessage.CreateClientMessage(ClientMessageType.GenerateCancelled, string.Empty);
            }

            response.Time = DateTimeOffset.Now;
            session.Messages.Add(response);

            return response;
        }
        catch (TaskCanceledException)
        {
            return ChatMessage.CreateClientMessage(ClientMessageType.GenerateCancelled, string.Empty);
        }
        catch
        {
            throw;
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }
    }

    /// <inheritdoc/>
    public async Task SendGroupMessageAsync(string groupId, ChatMessage message, Action<ChatMessage> messageAction = null, List<ChatSessionPresetOld> agents = null, CancellationToken cancellationToken = default)
    {
        var group = Groups.FirstOrDefault(g => g.Id == groupId)
            ?? throw new ArgumentException("Group not found.");
        var chatAgents = new List<ChatCompletionAgent>();
        foreach (var agentId in group.Agents)
        {
            var agent = agents.FirstOrDefault(p => p.Id == agentId)
                ?? throw new ArgumentException("Agent not found.");
            var provider = _providerFactory.GetOrCreateProvider(agent.Provider);
            var kernel = FindKernelProvider(agent.Provider, agent.Model)
                ?? throw new KernelException($"Parse {agent.Name} failed, because provider config invalid.");
            var chatAgent = new ChatCompletionAgent
            {
                Instructions = agent.SystemInstruction,
                Arguments = new KernelArguments([provider.ConvertExecutionSettings(agent)]),
                Id = agent.Id,
                Kernel = kernel,
                Name = EncodeName(agent.Name),
            };

            chatAgents.Add(chatAgent);
        }

        var groupChat = new AgentGroupChat(chatAgents.ToArray())
        {
            ExecutionSettings =
                    new()
                    {
                        TerminationStrategy = new CustomTerminationStrategy(group.MaxRounds, group.TerminateText),
                    },
        };

        if (message.Role == MessageRole.User)
        {
            group.Messages.Add(message);
        }

        foreach (var item in group.Messages)
        {
            groupChat.AddChatMessage(ConvertToKernelMessage(item));
        }

        await foreach (var content in groupChat.InvokeAsync(cancellationToken))
        {
            var assistantName = DecodeName(content.AuthorName);
            var agent = agents.FirstOrDefault(p => p.Name == assistantName);
            var msg = ChatMessage.CreateAssistantMessage(content.Content);
            msg.Time = DateTimeOffset.Now;
            msg.Author = assistantName;
            msg.AuthorId = agent?.Id ?? string.Empty;
            group.Messages.Add(msg);
            messageAction?.Invoke(msg);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async Task<List<KernelPlugin>> RetrievePluginsFromDllAsync(string dllPath)
    {
        var result = new List<KernelPlugin>();
        if (!_dllPaths.Contains(dllPath))
        {
            _dllPaths.Add(dllPath);
        }

        _preferDllPath = dllPath;
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

        try
        {
            await Task.Run(() =>
            {
                var assembly = Assembly.LoadFile(dllPath);
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Name.EndsWith("Plugin", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            var instance = Activator.CreateInstance(type);
                            var displayName = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name.Replace("Plugin", string.Empty);
                            var kernelPlugin = KernelPluginFactory.CreateFromObject(instance, displayName)
                                ?? throw new Exception("Plugin not created.");

                            kernelPlugin.ExternalId = type.Name;
                            result.Add(kernelPlugin);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to create plugin from type {Type}.", type);
                            _logger.LogDebug(ex.Message);
                            continue;
                        }
                    }
                }
            }).ConfigureAwait(false);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
            _preferDllPath = string.Empty;
        }

        return result;
    }

    /// <inheritdoc/>
    public IChatCompletionService? GetKernelChatCompletionService(ProviderType type)
    {
        var provider = GetProvider(type);
        var kernel = provider.GetCurrentKernel();
        return kernel?.GetRequiredService<IChatCompletionService>();
    }

    /// <inheritdoc/>
    public async Task<string?> InvokeFunctionAsync(ProviderType type, string model, KernelFunction function, KernelArguments arguments)
    {
        var kernel = FindKernelProvider(type, model);
        var result = await function.InvokeAsync(kernel, arguments).ConfigureAwait(false);
        return result.GetValue<string>();
    }

    private Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var assemblyPath = string.Empty;
        if (!string.IsNullOrEmpty(_preferDllPath))
        {
            var pluginFolder = Path.GetDirectoryName(_preferDllPath);
            assemblyPath = Path.Combine(pluginFolder, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath))
            {
                assemblyPath = string.Empty;
            }
        }
        else if (_dllPaths.Count > 0)
        {
            foreach (var dllPath in _dllPaths)
            {
                var pluginFolder = Path.GetDirectoryName(dllPath);
                assemblyPath = Path.Combine(pluginFolder, new AssemblyName(args.Name).Name + ".dll");
                if (!File.Exists(assemblyPath))
                {
                    assemblyPath = string.Empty;
                }
                else
                {
                    break;
                }
            }
        }

        return !string.IsNullOrEmpty(assemblyPath) && File.Exists(assemblyPath) ? Assembly.LoadFile(assemblyPath) : default;
    }

    private sealed class CustomTerminationStrategy : TerminationStrategy
    {
        private readonly IList<string>? _terminateText;

        public CustomTerminationStrategy(int maxRounds, IList<string>? terminateText = default)
        {
            MaximumIterations = maxRounds;
            _terminateText = terminateText;
        }

        protected override Task<bool> ShouldAgentTerminateAsync(Agent agent, IReadOnlyList<Microsoft.SemanticKernel.ChatMessageContent> history, CancellationToken cancellationToken)
        {
            return _terminateText is null || _terminateText.Count == 0
                ? Task.FromResult(false)
                : Task.FromResult(_terminateText.Any(p => history[history.Count - 1].Content?.Contains(p, StringComparison.InvariantCultureIgnoreCase) ?? false));
        }
    }
}
